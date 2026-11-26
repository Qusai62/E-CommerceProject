// Local, dependency-free OpenAPI -> typed RTK Query generator for this Swagger subset.
// The official @rtk-query/codegen-openapi configuration is also supplied.
import fs from 'node:fs';
import { execFileSync } from 'node:child_process';
execFileSync(process.execPath, ['scripts/prepare-openapi.mjs'], { stdio: 'inherit' });
const openapi = JSON.parse(fs.readFileSync('openapi/swagger.prepared.json', 'utf8'));
const lines = [];
const append = (...parts) => lines.push(...parts);
const tsType = (schema) => {
  if (!schema) return 'unknown';
  if (Array.isArray(schema.enum))
    return schema.enum.map((value) => JSON.stringify(value)).join(' | ');
  if (schema.$ref) return schema.$ref.split('/').at(-1);
  if (schema.type === 'array')
    return `Array<${tsType(schema.items)}>` + (schema.nullable ? ' | null' : '');
  if (schema.type === 'object') return `Record<string, unknown>`;
  const t =
    { integer: 'number', number: 'number', string: 'string', boolean: 'boolean' }[schema.type] ??
    'unknown';
  return schema.nullable ? `${t} | null` : t;
};
const title = (value) => value.slice(0, 1).toUpperCase() + value.slice(1);
append(
  '/* AUTO-GENERATED from openapi/swagger.original.json. Do not edit manually. */',
  '/* Run `npm run api:generate` whenever the ASP.NET Core OpenAPI contract changes. */',
  'import { baseApi } from "../baseApi";',
  '',
);
for (const [name, schema] of Object.entries(openapi.components.schemas)) {
  if (Array.isArray(schema.enum)) {
    append(`export type ${name} = ${tsType(schema)};`, '');
    continue;
  }
  if (schema.type !== 'object') continue;
  append(`export type ${name} = {`);
  for (const [property, definition] of Object.entries(schema.properties ?? {})) {
    const optional = (schema.required ?? []).includes(property) ? '' : '?';
    append(`  ${JSON.stringify(property)}${optional}: ${tsType(definition)};`);
  }
  if (schema.additionalProperties && schema.additionalProperties !== false)
    append('  [key: string]: unknown;');
  append('};', '');
}
const operations = [];
for (const [path, verbs] of Object.entries(openapi.paths)) {
  for (const [verb, o] of Object.entries(verbs)) {
    const name = o.operationId;
    const params = o.parameters ?? [];
    const json = o.requestBody?.content?.['application/json']?.schema;
    const form = o.requestBody?.content?.['multipart/form-data']?.schema;
    const body = json ? tsType(json) : form ? 'FormData' : undefined;
    const fields = params.map((p) => ({
      name: p.name,
      type: tsType(p.schema),
      optional: !p.required,
    }));
    if (body) fields.push({ name: 'body', type: body, optional: false });
    const responseEntry = Object.entries(o.responses ?? {}).find(([code]) => code.startsWith('2'));
    const schema = responseEntry?.[1]?.content?.['application/json']?.schema;
    const result = schema
      ? tsType(schema)
      : responseEntry?.[0]?.startsWith('2')
        ? 'void'
        : 'unknown';
    const argName = title(name) + 'ApiArg';
    const resultName = title(name) + 'ApiResponse';
    append(
      `export type ${argName} = ${fields.length ? '{ ' + fields.map((f) => `${JSON.stringify(f.name)}${f.optional ? '?' : ''}: ${f.type}`).join('; ') + ' }' : 'void'};`,
      `export type ${resultName} = ${result};`,
      '',
    );
    operations.push({ path, verb, name, fields, params, body, result, argName, resultName });
  }
}
append('export const generatedApi = baseApi.injectEndpoints({', '  endpoints: (build) => ({');
// Tags express cache dependencies, not a one-to-one mapping from URL to component.
const tagName = (path) => {
  if (path.startsWith('/api/checkout')) return 'Checkout';
  if (path.startsWith('/api/orders') || path.startsWith('/api/store/orders')) return 'Orders';
  if (path.startsWith('/api/auth')) return 'Auth';
  if (path.startsWith('/api/store/categories')) return 'Products';
  if (path.startsWith('/api/owner/') || path.startsWith('/api/variants/')) return 'Inventory';
  if (path.startsWith('/api/cart')) return 'Cart';
  if (path.startsWith('/api/customer/addresses')) return 'Addresses';
  if (path.startsWith('/api/customers')) return 'Customer';
  return 'Products';
};
const extraInvalidations = {
  addCartItem: ['Checkout', 'Inventory'],
  updateCartItemQuantity: ['Checkout', 'Inventory'],
  removeCartItem: ['Checkout'],
  clearCart: ['Checkout'],
  placeOrder: ['Cart', 'Checkout', 'Inventory', 'Orders'],
  addAddress: ['Checkout'],
  updateAddress: ['Checkout'],
  deleteAddress: ['Checkout'],
  setDefaultAddress: ['Checkout'],
  login: ['Auth', 'Customer', 'Cart', 'Checkout', 'Addresses', 'Orders'],
  logout: ['Auth', 'Customer', 'Cart', 'Checkout', 'Addresses', 'Orders'],
  registerCustomer: ['Auth', 'Cart', 'Addresses'],
  createCategory: ['Products'],
  createStockReservation: ['Inventory', 'Checkout'],
  createVariant: ['Inventory', 'Products'],
  updateVariant: ['Inventory', 'Products'],
  increaseStock: ['Products'],
  decreaseStock: ['Products'],
  deactivateProduct: ['Products', 'Inventory'],
  reactivateProduct: ['Products', 'Inventory'],
};
for (const o of operations) {
  const query = o.verb === 'get';
  const tag = tagName(o.path);
  let route =
    '`' +
    o.path.replace(/\{([^}]+)\}/g, (_, p) => '${encodeURIComponent(String(arg.' + p + '))}') +
    '`';
  const hasArgs = o.fields.length > 0;
  const pathExpr = o.path.includes('{') ? route : JSON.stringify(o.path);
  const searchParams = o.params.filter((p) => p.in === 'query');
  const queryPart = searchParams.length
    ? `, params: { ${searchParams.map((p) => `${JSON.stringify(p.name)}: arg.${p.name}`).join(', ')} }`
    : '';
  const bodyPart = o.body ? ', body: arg.body' : '';
  const namePart = hasArgs ? 'arg' : '';
  const tags = new Set([tag]);
  if (!query) for (const extra of extraInvalidations[o.name] ?? []) tags.add(extra);
  append(
    `    ${o.name}: build.${query ? 'query' : 'mutation'}<${o.resultName}, ${o.argName}>({`,
    `      query: (${namePart}) => ({ url: ${pathExpr}, method: "${o.verb.toUpperCase()}"${queryPart}${bodyPart} }),`,
    `      ${query ? 'providesTags' : 'invalidatesTags'}: ${JSON.stringify([...tags])},`,
    '    }),',
  );
}
append('  }),', '  overrideExisting: false,', '});', '');
append('export const {');
for (const o of operations)
  append(`  use${title(o.name)}${o.verb === 'get' ? 'Query' : 'Mutation'},`);
append('} = generatedApi;', '');
fs.mkdirSync('src/shared/api/generated', { recursive: true });
fs.writeFileSync('src/shared/api/generated/api.generated.ts', lines.join('\n'));
console.log(
  `Generated ${operations.length} RTK Query endpoints and ${Object.keys(openapi.components.schemas).length} models`,
);
