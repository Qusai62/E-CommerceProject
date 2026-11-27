import fs from 'node:fs';
import assert from 'node:assert/strict';
const raw = JSON.parse(fs.readFileSync('openapi/swagger.original.json', 'utf8'));
const prepared = JSON.parse(fs.readFileSync('openapi/swagger.prepared.json', 'utf8'));
const generated = fs.readFileSync('src/shared/api/generated/api.generated.ts', 'utf8');
const verbs = ['get', 'post', 'put', 'delete', 'patch'];
let count = 0;
for (const [path, methods] of Object.entries(raw.paths))
  for (const verb of verbs) {
    if (!methods[verb]) continue;
    count++;
    assert.equal(typeof prepared.paths[path][verb].operationId, 'string');
    const name = prepared.paths[path][verb].operationId;
    assert.ok(generated.includes(`${name}: build.`), `Generated API missing ${verb} ${path}`);
  }
const schemas = Object.keys(raw.components.schemas);
for (const schema of schemas)
  assert.ok(generated.includes(`export type ${schema} = `), `Missing model ${schema}`);
console.log(
  `PASS: ${count} operations and ${schemas.length} models have typed generation coverage`,
);
