import type { BaseQueryFn, FetchArgs, FetchBaseQueryError } from '@reduxjs/toolkit/query';

// Explicit local sandbox. None of the example names/prices/images come from the API.
const seedProducts = [
  ['Apex 14 Pro Laptop', 1199, 1, 1],
  ['Nova Wireless Headphones', 169, 2, 2],
  ['Orbit 5G Smartphone', 849, 3, 3],
  ['Vision 27-inch QHD Monitor', 349, 4, 4],
  ['Pulse Smartwatch', 229, 3, 5],
  ['Pixel Mirrorless Camera', 999, 4, 6],
  ['Flux Mechanical Keyboard', 129, 1, 7],
  ['Core Wireless Mouse', 69, 1, 8],
  ['Wave Bluetooth Speaker', 139, 2, 9],
  ['Prism Gaming Console', 499, 4, 10],
  ['Volt 65W USB-C Charger', 59, 1, 11],
  ['Nimbus Ultrabook 13', 1399, 1, 12],
] as const;
type DemoVariant = { productVariantId: number; sku: string; price: number; isActive: boolean };
type DemoProduct = {
  productId: number;
  name: string;
  priceFrom: number | null;
  brandId: number | null;
  categoryId: number | null;
  slug: string;
  primaryImageStorageKey: string | null;
  isActive: boolean;
  description: string;
  images: Array<{
    productImageId: number;
    storageKey: string;
    altText: string;
    sortOrder: number;
    isPrimary: boolean;
  }>;
  variants: DemoVariant[];
  technicalSpecifications: Array<{
    technicalSpecificationId?: number;
    name: string;
    value: string;
    sortOrder?: number;
  }>;
};
const products: DemoProduct[] = seedProducts.map(([name, priceFrom, brandId, productId]) => ({
  productId,
  name,
  priceFrom,
  brandId,
  categoryId: (productId % 4) + 1,
  slug: name.toLowerCase().replaceAll(' ', '-'),
  primaryImageStorageKey: null,
  isActive: true,
  description:
    'Explore modern performance, clean design, and practical technology for work, play, and everyday life.',
  images: [],
  variants: [
    {
      productVariantId: productId + 100,
      sku: `AST-${String(productId).padStart(3, '0')}`,
      price: priceFrom,
      isActive: true,
    },
  ],
  technicalSpecifications: [
    { name: 'Category', value: 'Electronics' },
    { name: 'Condition', value: 'New (demo sample)' },
  ],
}));
const addresses = [
  {
    savedAddressId: 1,
    recipientName: 'Alex Morgan',
    contactNumber: '0790000000',
    city: 'Amman',
    neighborhood: 'Abdoun',
    street: 'Garden Street',
    buildingInfo: '12B',
    additionalAddressInfo: '',
    isDefault: true,
  },
];
const stock = seedProducts.map(([name, , , id]) => ({
  productVariantId: id + 100,
  productId: id,
  productName: name,
  sku: `AST-${String(id).padStart(3, '0')}`,
  productIsActive: true,
  variantIsActive: true,
  onHandQuantity: id % 5 === 0 ? 0 : 20 + id * 2,
  reservedQuantity: id % 5 === 0 ? 0 : 2,
  securedCODQuantity: 0,
  availableQuantity: id % 5 === 0 ? 0 : 18 + id * 2,
  isInStock: id % 5 !== 0,
}));
const cartItems: Array<{
  cartItemId: number;
  productVariantId: number;
  productName: string;
  sku: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}> = [];
let loggedIn = false;
let demoRoles: string[] = [];
let cartId = 1;
const customer = {
  customerId: 1,
  firstName: 'Alex',
  lastName: 'Morgan',
  email: 'alex@example.com',
  phoneNumber: '0790000000',
  isEmailVerified: true,
  createdAtUtc: new Date().toISOString(),
  updatedAtUtc: new Date().toISOString(),
};
let nextProduct = 100;
let nextOrder = 1000;
const placedOrders = new Map<
  string,
  {
    orderId: number;
    orderNumber: string;
    totalAmount: number;
    paymentMethod: number;
    paymentStatus: number;
  }
>();
const demoShippingCharge = 5;
const orderHistory: Array<Record<string, unknown>> = [];
const fulfillmentHistory = new Map<number, Array<Record<string, unknown>>>();
const categories: Array<Record<string, unknown>> = [
  { categoryId: 1, name: 'Laptops', slug: 'laptops', parentCategoryId: null, isActive: true },
  { categoryId: 2, name: 'Audio', slug: 'audio', parentCategoryId: null, isActive: true },
  { categoryId: 3, name: 'Phones', slug: 'phones', parentCategoryId: null, isActive: true },
  {
    categoryId: 4,
    name: 'Accessories',
    slug: 'accessories',
    parentCategoryId: null,
    isActive: true,
  },
];
let nextCategoryId = 20;
let nextBrandId = 20;
let nextSupplierId = 100;
let nextSupplierInvoiceId = 1000;
let nextSupplierPaymentId = 1000;
const supplierInvoices: Array<Record<string, any>> = [];
const supplierPayments = new Map<string, Record<string, unknown>>();
const suppliers: Array<{
  supplierId: number;
  name: string;
  phoneNumber: string | null;
  email: string | null;
  createdAtUtc: string;
  updatedAtUtc: string;
}> = [];
const brands: Array<{ brandId: number; name: string; slug: string; isActive: boolean }> = [
  { brandId: 1, name: 'Aster', slug: 'aster', isActive: true },
  { brandId: 2, name: 'Nova', slug: 'nova', isActive: true },
  { brandId: 3, name: 'Orbit', slug: 'orbit', isActive: true },
  { brandId: 4, name: 'Vision', slug: 'vision', isActive: false },
];
const cartTotals = () => ({
  shoppingCartId: cartId,
  items: [...cartItems],
  totalQuantity: cartItems.reduce((total, item) => total + item.quantity, 0),
  totalAmount: cartItems.reduce((total, item) => total + item.subtotal, 0),
});
const toDelivery = (address: (typeof addresses)[number]) => {
  const { isDefault: _isDefault, ...delivery } = address;
  return delivery;
};

const ok = (data: unknown) => ({ data });
const fail = (status: number, detail: string): { error: FetchBaseQueryError } => ({
  error: { status, data: { title: 'Request unsuccessful', status, detail } },
});
const paged = <T>(items: T[], params: URLSearchParams) => {
  const pageNumber = Math.max(1, Number(params.get('pageNumber') ?? params.get('PageNumber') ?? 1));
  const pageSize = Math.max(1, Number(params.get('pageSize') ?? params.get('PageSize') ?? 9));
  const totalCount = items.length;
  const totalPages = Math.ceil(totalCount / pageSize);
  return {
    items: items.slice((pageNumber - 1) * pageSize, pageNumber * pageSize),
    pageNumber,
    pageSize,
    totalCount,
    totalPages,
    hasPreviousPage: pageNumber > 1,
    hasNextPage: pageNumber < totalPages,
  };
};

export const demoBaseQuery: BaseQueryFn<string | FetchArgs, unknown, FetchBaseQueryError> = async (
  args,
) => {
  await new Promise((resolve) => setTimeout(resolve, 180));
  const input = typeof args === 'string' ? { url: args } : args;
  const method = (input.method ?? 'GET').toUpperCase();
  const route = new URL(input.url, 'https://demo.invalid');
  const path = route.pathname;
  const params = new URLSearchParams(route.search);
  if (input.params)
    Object.entries(input.params as Record<string, unknown>).forEach(([key, val]) => {
      if (val != null && val !== '') params.set(key, String(val));
    });
  const body = (input.body ?? {}) as Record<string, any>;
  const numberInPath = (index: number) => Number(path.split('/')[index]);

  const isOwner = demoRoles.includes('StoreOwner');
  const productWrite =
    (path === '/api/products' && method === 'POST') ||
    (path.startsWith('/api/products/') && method !== 'GET');
  if (
    (path.startsWith('/api/owner/') ||
      path.startsWith('/api/store/categories') ||
      path === '/api/brands' ||
      path === '/api/suppliers' ||
      path === '/api/supplier-invoices' ||
      productWrite) &&
    !isOwner
  ) {
    return fail(loggedIn ? 403 : 401, 'StoreOwner role is required');
  }
  if (path === '/api/suppliers' && method === 'GET') {
    const search = params.get('Search')?.trim().toLowerCase();
    let results = search
      ? suppliers.filter((supplier) =>
          [supplier.name, supplier.phoneNumber, supplier.email]
            .filter(Boolean)
            .some((value) => value!.toLowerCase().includes(search)),
        )
      : [...suppliers];
    const descending = params.get('SortDescending') === 'true';
    const sortBy = params.get('SortBy')?.toLowerCase() ?? 'name';
    results.sort((a, b) => {
      const left = sortBy === 'createdatutc' ? a.createdAtUtc : a.name;
      const right = sortBy === 'createdatutc' ? b.createdAtUtc : b.name;
      return (descending ? -1 : 1) * left.localeCompare(right);
    });
    return ok(paged(results, params));
  }
  if (path === '/api/suppliers' && method === 'POST') {
    const name = typeof body.name === 'string' ? body.name.trim() : '';
    if (!name) return fail(400, 'Supplier name is required');
    const now = new Date().toISOString();
    const supplier = {
      supplierId: ++nextSupplierId,
      name,
      phoneNumber: typeof body.phoneNumber === 'string' ? body.phoneNumber : null,
      email: typeof body.email === 'string' ? body.email : null,
      createdAtUtc: now,
      updatedAtUtc: now,
    };
    suppliers.push(supplier);
    return ok(supplier);
  }
  const supplierMatch = path.match(/^\/api\/suppliers\/(\d+)$/);
  if (supplierMatch && method === 'GET') {
    const supplier = suppliers.find((item) => item.supplierId === Number(supplierMatch[1]));
    return supplier ? ok(supplier) : fail(404, 'Supplier not found');
  }
  if (supplierMatch && method === 'PUT') {
    const supplier = suppliers.find((item) => item.supplierId === Number(supplierMatch[1]));
    if (!supplier) return fail(404, 'Supplier not found');
    const name = typeof body.name === 'string' ? body.name.trim() : '';
    if (!name) return fail(400, 'Supplier name is required');
    supplier.name = name;
    supplier.phoneNumber = typeof body.phoneNumber === 'string' ? body.phoneNumber : null;
    supplier.email = typeof body.email === 'string' ? body.email : null;
    supplier.updatedAtUtc = new Date().toISOString();
    return ok(supplier);
  }
  if (path === '/api/supplier-invoices' && method === 'POST') {
    const supplierId = Number(body.supplierId);
    const invoiceNumber = typeof body.invoiceNumber === 'string' ? body.invoiceNumber.trim() : '';
    const items = Array.isArray(body.items) ? body.items : [];
    if (!suppliers.some((supplier) => supplier.supplierId === supplierId))
      return fail(404, 'Supplier not found');
    if (!invoiceNumber || !items.length)
      return fail(400, 'Invoice number and at least one item are required');
    const normalized = items.map((item) => ({
      productName: String(item.productName ?? '').trim(),
      quantity: Number(item.quantity),
      unitPrice: Number(item.unitPrice),
    }));
    if (
      normalized.some(
        (item) =>
          !item.productName ||
          !Number.isInteger(item.quantity) ||
          item.quantity < 1 ||
          !Number.isFinite(item.unitPrice) ||
          item.unitPrice < 0,
      )
    )
      return fail(400, 'Invoice items are invalid');
    const totalAmount = normalized.reduce(
      (total, item) => total + item.quantity * item.unitPrice,
      0,
    );
    const invoice = {
      supplierInvoiceId: ++nextSupplierInvoiceId,
      supplierId,
      invoiceNumber,
      invoiceDateUtc: body.invoiceDateUtc,
      totalAmount,
      paidAmount: 0,
      remainingAmount: totalAmount,
      paymentStatus: 'Unpaid',
      items: normalized.map((item, index) => ({
        ...item,
        supplierInvoiceItemId: index + 1,
        lineTotal: item.quantity * item.unitPrice,
      })),
      createdAtUtc: new Date().toISOString(),
      supplierName: suppliers.find((supplier) => supplier.supplierId === supplierId)?.name,
    };
    supplierInvoices.push(invoice);
    return ok(invoice);
  }
  if (path === '/api/supplier-invoices' && method === 'GET') {
    const supplierId = Number(params.get('SupplierId'));
    const search = params.get('Search')?.trim().toLowerCase();
    const paymentStatus = params.get('PaymentStatus')?.trim().toLowerCase();
    const sortBy = params.get('SortBy')?.toLowerCase() ?? 'invoicedate';
    const descending = params.get('SortDescending') !== 'false';
    let results = supplierInvoices.filter((invoice) => {
      if (supplierId && invoice.supplierId !== supplierId) return false;
      if (paymentStatus && invoice.paymentStatus.toLowerCase() !== paymentStatus) return false;
      return (
        !search ||
        [invoice.supplierName, invoice.invoiceNumber]
          .filter(Boolean)
          .some((value) => String(value).toLowerCase().includes(search))
      );
    });
    results.sort((left, right) => {
      const leftValue =
        sortBy === 'suppliername'
          ? left.supplierName
          : sortBy === 'invoicenumber'
            ? left.invoiceNumber
            : sortBy === 'totalamount'
              ? left.totalAmount
              : sortBy === 'paidamount'
                ? left.paidAmount
                : sortBy === 'remainingamount'
                  ? left.remainingAmount
                  : left.invoiceDateUtc;
      const rightValue =
        sortBy === 'suppliername'
          ? right.supplierName
          : sortBy === 'invoicenumber'
            ? right.invoiceNumber
            : sortBy === 'totalamount'
              ? right.totalAmount
              : sortBy === 'paidamount'
                ? right.paidAmount
                : sortBy === 'remainingamount'
                  ? right.remainingAmount
                  : right.invoiceDateUtc;
      const comparison =
        typeof leftValue === 'number' && typeof rightValue === 'number'
          ? leftValue - rightValue
          : String(leftValue).localeCompare(String(rightValue));
      return descending ? -comparison : comparison;
    });
    return ok(paged(results, params));
  }
  const supplierInvoicePaymentMatch = path.match(/^\/api\/supplier-invoices\/(\d+)\/payments$/);
  if (supplierInvoicePaymentMatch && method === 'POST') {
    const supplierInvoiceId = Number(supplierInvoicePaymentMatch[1]);
    const invoice = supplierInvoices.find((item) => item.supplierInvoiceId === supplierInvoiceId);
    if (!invoice) return fail(404, 'Supplier invoice not found');
    const submissionId = typeof body.submissionId === 'string' ? body.submissionId : '';
    const amount = Number(body.amount);
    const paidAtUtc = typeof body.paidAtUtc === 'string' ? body.paidAtUtc : '';
    if (
      !submissionId ||
      !Number.isFinite(amount) ||
      amount <= 0 ||
      Number.isNaN(Date.parse(paidAtUtc))
    )
      return fail(400, 'A submission ID, positive payment amount, and payment date are required');
    const duplicate = supplierPayments.get(submissionId);
    if (duplicate) return ok(duplicate);
    if (amount > invoice.remainingAmount)
      return fail(409, 'Payment amount exceeds the remaining balance');
    invoice.paidAmount += amount;
    invoice.remainingAmount -= amount;
    invoice.paymentStatus = invoice.remainingAmount === 0 ? 'Paid' : 'PartiallyPaid';
    const response = {
      supplierPaymentId: ++nextSupplierPaymentId,
      supplierInvoiceId,
      submissionId,
      paymentAmount: amount,
      paidAtUtc,
      recordedAtUtc: new Date().toISOString(),
      totalAmount: invoice.totalAmount,
      paidAmount: invoice.paidAmount,
      remainingAmount: invoice.remainingAmount,
      paymentStatus: invoice.paymentStatus,
    };
    supplierPayments.set(submissionId, response);
    return ok(response);
  }
  if (supplierInvoicePaymentMatch && method === 'GET') {
    const supplierInvoiceId = Number(supplierInvoicePaymentMatch[1]);
    const invoice = supplierInvoices.find((item) => item.supplierInvoiceId === supplierInvoiceId);
    if (!invoice) return fail(404, 'Supplier invoice not found');
    const payments = [...supplierPayments.values()]
      .filter((payment) => payment.supplierInvoiceId === supplierInvoiceId)
      .sort((left, right) => String(right.paidAtUtc).localeCompare(String(left.paidAtUtc)))
      .map((payment) => ({
        supplierPaymentId: payment.supplierPaymentId,
        submissionId: payment.submissionId,
        amount: payment.paymentAmount,
        paidAtUtc: payment.paidAtUtc,
        recordedAtUtc: payment.recordedAtUtc,
      }));
    return ok({
      supplierInvoiceId,
      invoiceNumber: invoice.invoiceNumber,
      totalAmount: invoice.totalAmount,
      paidAmount: invoice.paidAmount,
      remainingAmount: invoice.remainingAmount,
      paymentStatus: invoice.paymentStatus,
      payments,
    });
  }
  const supplierInvoiceMatch = path.match(/^\/api\/supplier-invoices\/(\d+)$/);
  if (supplierInvoiceMatch && method === 'GET') {
    const invoice = supplierInvoices.find(
      (item) => item.supplierInvoiceId === Number(supplierInvoiceMatch[1]),
    );
    return invoice ? ok(invoice) : fail(404, 'Supplier invoice not found');
  }

  if (path === '/api/products' && method === 'GET') {
    let list = [...products];
    const text = params.get('Search')?.trim().toLowerCase();
    if (text) list = list.filter((p) => p.name.toLowerCase().includes(text));
    const min = Number(params.get('MinPrice') || 0);
    const max = Number(params.get('MaxPrice') || Infinity);
    list = list.filter((p) => p.priceFrom >= min && p.priceFrom <= max);
    const brand = Number(params.get('BrandId') || 0);
    const cat = Number(params.get('CategoryId') || 0);
    if (brand) list = list.filter((p) => p.brandId === brand);
    if (cat) list = list.filter((p) => p.categoryId === cat);
    const desc = params.get('SortDirection')?.toLowerCase() === 'desc';
    switch (params.get('SortBy')?.toLowerCase()) {
      case 'price':
        list.sort((a, b) => a.priceFrom - b.priceFrom);
        break;
      case 'name':
        list.sort((a, b) => a.name.localeCompare(b.name));
        break;
      default:
        list.sort((a, b) => a.productId - b.productId);
    }
    if (desc) list.reverse();
    return ok(paged(list, params));
  }
  if (path === '/api/products' && method === 'POST') {
    const p = {
      productId: ++nextProduct,
      name: String(body.name || 'Untitled product'),
      description: body.description || '',
      priceFrom: null,
      brandId: body.brandId ?? null,
      categoryId: body.categoryId ?? null,
      slug: String(body.name || 'new-product')
        .toLowerCase()
        .replaceAll(' ', '-'),
      primaryImageStorageKey: null,
      isActive: false,
      variants: [],
      images: [],
      technicalSpecifications: [],
    };
    products.unshift(p as DemoProduct);
    return ok(p);
  }
  const productMatch = path.match(/^\/api\/products\/(\d+)$/);
  if (productMatch) {
    const product = products.find((p) => p.productId === Number(productMatch[1]));
    if (!product) return fail(404, 'Product not found');
    if (method === 'PUT') Object.assign(product, body);
    return ok(product);
  }
  const productStatus = path.match(/^\/api\/products\/(\d+)\/(deactivate|reactivate)$/);
  if (productStatus && method === 'PATCH') {
    const p = products.find((p) => p.productId === Number(productStatus[1]));
    if (!p) return fail(404, 'Product not found');
    p.isActive = productStatus[2] === 'reactivate';
    return ok(p);
  }
  const variantMatch = path.match(/^\/api\/products\/(\d+)\/variants$/);
  if (variantMatch && method === 'POST') {
    const p = products.find((p) => p.productId === Number(variantMatch[1]));
    if (!p) return fail(404, 'Product not found');
    const item = {
      productVariantId: Date.now(),
      productId: p.productId,
      sku: body.sku,
      price: body.price,
      isActive: true,
      onHandQuantity: 0,
      createdAtUtc: new Date().toISOString(),
    };
    p.variants.push(item);
    stock.unshift({
      productVariantId: item.productVariantId,
      productId: p.productId,
      productName: p.name,
      sku: String(item.sku),
      productIsActive: p.isActive,
      variantIsActive: true,
      onHandQuantity: 0,
      reservedQuantity: 0,
      securedCODQuantity: 0,
      availableQuantity: 0,
      isInStock: false,
    });
    if (p.priceFrom == null || Number(body.price) < p.priceFrom) p.priceFrom = Number(body.price);
    return ok(item);
  }
  const updateVariant = path.match(/^\/api\/products\/(\d+)\/variants\/(\d+)$/);
  if (updateVariant && method === 'PUT') {
    const p = products.find((p) => p.productId === Number(updateVariant[1]));
    const variant = p?.variants.find((v) => v.productVariantId === Number(updateVariant[2]));
    if (!variant) return fail(404, 'Variant not found');
    Object.assign(variant, body);
    return ok({ ...variant, productId: p?.productId, updatedAtUtc: new Date().toISOString() });
  }
  if (path === '/api/owner/inventory' && method === 'GET') return ok(paged(stock, params));
  if (path === '/api/owner/inventory/out-of-stock/variants')
    return ok(
      paged(
        stock.filter((i) => !i.isInStock),
        params,
      ),
    );
  if (path === '/api/owner/inventory/out-of-stock/products')
    return ok(
      paged(
        stock
          .filter((i) => !i.isInStock)
          .map((i) => ({
            productId: i.productId,
            productName: i.productName,
            productIsActive: i.productIsActive,
          })),
        params,
      ),
    );
  const adjustment = path.match(/^\/api\/owner\/inventory\/variants\/(\d+)\/(increase|decrease)$/);
  if (adjustment) {
    const item = stock.find((i) => i.productVariantId === Number(adjustment[1]));
    if (!item) return fail(404, 'Variant not found');
    const amount = Number(body.quantity);
    if (!Number.isInteger(amount) || amount <= 0)
      return fail(400, 'Quantity must be a positive whole number');
    if (adjustment[2] === 'decrease' && item.availableQuantity < amount)
      return fail(409, 'Insufficient available stock');
    item.onHandQuantity += adjustment[2] === 'increase' ? amount : -amount;
    item.availableQuantity += adjustment[2] === 'increase' ? amount : -amount;
    item.isInStock = item.availableQuantity > 0;
    return ok({
      productVariantId: item.productVariantId,
      [adjustment[2] === 'increase' ? 'quantityAdded' : 'quantityDecreased']: amount,
    });
  }
  const availability = path.match(/^\/api\/variants\/(\d+)\/availability$/);
  if (availability) {
    const row = stock.find((s) => s.productVariantId === Number(availability[1]));
    return row
      ? ok({
          productVariantId: row.productVariantId,
          availableQuantity: row.availableQuantity,
          isInStock: row.isInStock,
        })
      : fail(404, 'Not found');
  }
  if (path === '/api/auth/me' && method === 'GET') {
    return loggedIn
      ? ok({
          userId: '11111111-1111-4111-8111-111111111111',
          email: customer.email,
          roles: demoRoles,
        })
      : fail(401, 'No current session');
  }
  if (path === '/api/auth/login' && method === 'POST') {
    if (!body.email || !body.password) return fail(400, 'Enter an email and password');
    loggedIn = true;
    customer.email = String(body.email);
    demoRoles =
      String(body.email).toLowerCase() === 'owner@aster.test' ? ['StoreOwner'] : ['Customer'];
    return ok({
      userId: '11111111-1111-4111-8111-111111111111',
      email: customer.email,
      roles: demoRoles,
      isEmailVerified: true,
    });
  }
  if (path === '/api/auth/logout' && method === 'POST') {
    if (!loggedIn) return fail(401, 'No active session');
    loggedIn = false;
    demoRoles = [];
    cartItems.length = 0;
    return ok(undefined);
  }
  if (path === '/api/customers/register') {
    if (!body.email || !body.password) return fail(400, 'Email and password are required');
    loggedIn = false;
    demoRoles = [];
    Object.assign(customer, body);
    return ok(customer);
  }
  if (path === '/api/store/categories' && method === 'POST') {
    if (!String(body.name ?? '').trim()) return fail(400, 'Category name required');
    const result = {
      categoryId: ++nextCategoryId,
      name: String(body.name),
      slug: String(body.name).toLowerCase().trim().replaceAll(' ', '-'),
      parentCategoryId: body.parentCategoryId ?? null,
      isActive: true,
      createdAtUtc: new Date().toISOString(),
    };
    categories.push(result);
    return ok(result);
  }
  if (path === '/api/brands' && method === 'POST') {
    const name = String(body.name ?? '').trim();
    if (!name) return fail(400, 'Brand name required');
    const result = {
      brandId: ++nextBrandId,
      name,
      slug: name.toLowerCase().replaceAll(' ', '-'),
      isActive: true,
      createdAtUtc: new Date().toISOString(),
    };
    brands.unshift(result);
    return ok(result);
  }
  if (path === '/api/brands' && method === 'GET') {
    const search = params.get('Search')?.trim().toLowerCase();
    const active = params.get('IsActive');
    let list = [...brands];
    if (search) list = list.filter((brand) => brand.name.toLowerCase().includes(search));
    if (active != null) list = list.filter((brand) => brand.isActive === (active === 'true'));
    return ok(paged(list, params));
  }
  const brandMatch = path.match(/^\/api\/brands\/(\d+)$/);
  if (brandMatch && method === 'PUT') {
    const brand = brands.find((item) => item.brandId === Number(brandMatch[1]));
    if (!brand) return fail(404, 'Brand not found');
    const name = String(body.name ?? '').trim();
    if (!name) return fail(400, 'Brand name required');
    Object.assign(brand, { name, slug: name.toLowerCase().replaceAll(' ', '-') });
    return ok({ ...brand, updatedAtUtc: new Date().toISOString() });
  }
  if (path === '/api/store/categories' && method === 'GET') {
    return ok([...categories]);
  }
  if (path === '/api/store/categories/hierarchy' && method === 'GET') {
    const buildTree = (parentCategoryId: number | null): Array<Record<string, unknown>> =>
      categories
        .filter((category) => (category.parentCategoryId ?? null) === parentCategoryId)
        .map((category) => ({
          ...category,
          children: buildTree((category.categoryId as number) ?? null),
        }));
    return ok(buildTree(null));
  }
  const categoryMatch = path.match(/^\/api\/store\/categories\/(\d+)$/);
  if (categoryMatch && method === 'PUT') {
    const category = categories.find((item) => item.categoryId === Number(categoryMatch[1]));
    if (!category) return fail(404, 'Category not found');
    Object.assign(category, body, { updatedAtUtc: new Date().toISOString() });
    return ok(category);
  }
  const categoryParentMatch = path.match(/^\/api\/store\/categories\/(\d+)\/parent$/);
  if (categoryParentMatch && method === 'PUT') {
    const category = categories.find((item) => item.categoryId === Number(categoryParentMatch[1]));
    if (!category) return fail(404, 'Category not found');
    category.parentCategoryId = body.parentCategoryId ?? null;
    return ok({ categoryId: category.categoryId, parentCategoryId: category.parentCategoryId });
  }
  const categoryDeactivateMatch = path.match(/^\/api\/store\/categories\/(\d+)\/deactivate$/);
  if (categoryDeactivateMatch && method === 'PATCH') {
    const category = categories.find(
      (item) => item.categoryId === Number(categoryDeactivateMatch[1]),
    );
    if (!category) return fail(404, 'Category not found');
    category.isActive = false;
    return ok({ categoryId: category.categoryId, isActive: false });
  }
  const categoryReactivateMatch = path.match(/^\/api\/store\/categories\/(\d+)\/reactivate$/);
  if (categoryReactivateMatch && method === 'PATCH') {
    const category = categories.find(
      (item) => item.categoryId === Number(categoryReactivateMatch[1]),
    );
    if (!category) return fail(404, 'Category not found');
    category.isActive = true;
    return ok({ categoryId: category.categoryId, isActive: true });
  }
  if (path === '/api/categories' && method === 'GET') {
    return ok(categories.map(({ categoryId, name, slug }) => ({ categoryId, name, slug })));
  }
  if (path === '/api/customers/me') {
    if (!loggedIn) return fail(401, 'Sign in to continue');
    if (method === 'PUT') Object.assign(customer, body);
    return ok(customer);
  }
  if (path === '/api/customers/me/change-password')
    return loggedIn ? ok(undefined) : fail(401, 'Sign in to continue');
  if (path === '/api/customer/addresses') {
    if (!loggedIn) return fail(401, 'Sign in to manage addresses');
    if (method === 'GET') return ok([...addresses]);
    if (method === 'POST') {
      const a = { ...body, savedAddressId: Date.now(), isDefault: !addresses.length };
      addresses.push(a);
      return ok(a);
    }
  }
  const address = path.match(/^\/api\/customer\/addresses\/(\d+)(\/default)?$/);
  if (address) {
    if (!loggedIn) return fail(401, 'Sign in to continue');
    const item = addresses.find((a) => a.savedAddressId === Number(address[1]));
    if (!item) return fail(404, 'Address not found');
    if (address[2]) {
      addresses.forEach((a) => (a.isDefault = false));
      item.isDefault = true;
      return ok({ savedAddressId: item.savedAddressId, isDefault: true });
    }
    if (method === 'PUT') {
      Object.assign(item, body);
      return ok(item);
    }
    if (method === 'DELETE') {
      if (item.isDefault) return fail(409, 'Choose another default address first');
      addresses.splice(addresses.indexOf(item), 1);
      return ok(undefined);
    }
  }
  if (path === '/api/cart') {
    if (!loggedIn) return fail(401, 'Sign in to manage your cart');
    if (method === 'GET') return ok(cartTotals());
    if (method === 'DELETE') {
      cartItems.splice(0);
      return ok(undefined);
    }
  }
  if (path === '/api/cart/items' && method === 'DELETE') {
    if (!loggedIn) return fail(401, 'Sign in to manage your cart');
    cartItems.splice(0);
    return ok(undefined);
  }
  if (path === '/api/cart/items' && method === 'POST') {
    if (!loggedIn) return fail(401, 'Sign in to add to cart');
    const variantId = Number(body.productVariantId);
    const quantity = Number(body.quantity);
    const p = products.find((p) => p.variants.some((v) => v.productVariantId === variantId));
    const variant = p?.variants.find((v) => v.productVariantId === variantId);
    if (!p || !variant) return fail(404, 'Variant not found');
    if (!Number.isInteger(quantity) || quantity < 1) return fail(400, 'Invalid quantity');
    const available =
      stock.find((item) => item.productVariantId === variantId)?.availableQuantity ?? 0;
    const existing = cartItems.find((i) => i.productVariantId === variantId);
    if (quantity + (existing?.quantity ?? 0) > available)
      return fail(409, 'Requested quantity exceeds available stock');
    if (existing) {
      existing.quantity += quantity;
      existing.subtotal = existing.quantity * existing.unitPrice;
      return ok({
        cartItemId: existing.cartItemId,
        productVariantId: variantId,
        quantity: existing.quantity,
      });
    }
    const row = {
      cartItemId: ++cartId,
      productVariantId: variantId,
      productName: p.name,
      sku: variant.sku,
      quantity,
      unitPrice: variant.price,
      subtotal: quantity * variant.price,
    };
    cartItems.push(row);
    return ok({ cartItemId: row.cartItemId, productVariantId: variantId, quantity });
  }
  const updateCartItemMatch = path.match(/^\/api\/cart\/items\/(\d+)$/);
  if (updateCartItemMatch && method === 'DELETE') {
    if (!loggedIn) return fail(401, 'Sign in to remove items');
    const index = cartItems.findIndex((row) => row.cartItemId === Number(updateCartItemMatch[1]));
    if (index < 0) return fail(404, 'Cart item not found');
    cartItems.splice(index, 1);
    return ok(undefined);
  }
  if (updateCartItemMatch && method === 'PUT') {
    if (!loggedIn) return fail(401, 'Sign in to update your cart');
    const item = cartItems.find((row) => row.cartItemId === Number(updateCartItemMatch[1]));
    if (!item) return fail(404, 'Cart item not found');
    const quantity = body.quantity;
    if (
      typeof quantity !== 'number' ||
      !Number.isInteger(quantity) ||
      quantity < 1 ||
      quantity > 2147483647
    ) {
      return fail(400, 'Quantity must be a positive whole number');
    }
    const availability =
      stock.find((row) => row.productVariantId === item.productVariantId)?.availableQuantity ?? 0;
    if (quantity > availability)
      return fail(409, `Only ${availability} units are currently available`);
    item.quantity = quantity;
    item.subtotal = quantity * item.unitPrice;
    // Demo-only sample payload. UI deliberately does not rely on response fields.
    return ok({ cartItemId: item.cartItemId, productVariantId: item.productVariantId, quantity });
  }
  // End-to-end checkout demo. Live data always comes from the API instead.
  if (path === '/api/checkout') {
    if (!loggedIn) return fail(401, 'Sign in to checkout');
    return ok({
      shoppingCartId: cartId,
      items: [...cartItems],
      productsTotal: cartTotals().totalAmount,
    });
  }
  const checkoutAddress = path.match(/^\/api\/checkout\/addresses\/(\d+)$/);
  if (checkoutAddress && method === 'GET') {
    if (!loggedIn) return fail(401, 'Sign in to checkout');
    const address = addresses.find((row) => row.savedAddressId === Number(checkoutAddress[1]));
    return address ? ok(toDelivery(address)) : fail(404, 'Delivery address was not found');
  }
  if (path === '/api/checkout/shipping' && method === 'GET') {
    if (!loggedIn) return fail(401, 'Sign in to checkout');
    return ok({ shippingMode: 1, shippingCharge: demoShippingCharge });
  }
  const validateCheckout = () => {
    if (!loggedIn) return fail(401, 'Sign in to continue');
    if (!cartItems.length) return fail(409, 'Your cart is empty');
    if (![1, 2].includes(Number(body.paymentMethod)))
      return fail(400, 'Select a supported payment method');
    const address = addresses.find((a) => a.savedAddressId === Number(body.savedAddressId));
    if (!address) return fail(404, 'Delivery address not found');
    for (const item of cartItems) {
      const available =
        stock.find((s) => s.productVariantId === item.productVariantId)?.availableQuantity ?? 0;
      if (item.quantity > available) return fail(409, `Insufficient stock for ${item.productName}`);
    }
    return { address };
  };
  if (path === '/api/checkout/review' && method === 'POST') {
    const validation = validateCheckout();
    if ('error' in validation) return validation;
    return ok({
      shoppingCartId: cartId,
      items: [...cartItems],
      deliveryAddress: toDelivery(validation.address),
      paymentMethod: Number(body.paymentMethod),
      productsTotal: cartTotals().totalAmount,
      shippingCharge: demoShippingCharge,
      finalTotal: cartTotals().totalAmount + demoShippingCharge,
    });
  }
  if (path === '/api/orders' && method === 'GET') {
    if (!loggedIn) return fail(401, 'Sign in to view orders');
    return ok(paged([...orderHistory].reverse(), params));
  }
  const orderDetailMatch = path.match(/^\/api\/orders\/(\d+)$/);
  if (orderDetailMatch && method === 'GET') {
    if (!loggedIn) return fail(401, 'Sign in to view order details');
    const order = orderHistory.find((item) => item.orderId === Number(orderDetailMatch[1]));
    return order ? ok(order) : fail(404, 'Order not found');
  }
  const storeOrderFulfillmentMatch = path.match(
    /^\/api\/store\/orders\/(\d+)\/fulfillment-status$/,
  );
  if (storeOrderFulfillmentMatch && method === 'PATCH') {
    const order = orderHistory.find(
      (item) => item.orderId === Number(storeOrderFulfillmentMatch[1]),
    );
    if (!order) return fail(404, 'Order not found');
    const fulfillmentStatus = Number(body.newStatus);
    if (![1, 2, 3, 4].includes(fulfillmentStatus))
      return fail(400, 'A valid fulfillment status is required');
    order.fulfillmentStatus = fulfillmentStatus;
    const orderId = Number(order.orderId);
    const entries = fulfillmentHistory.get(orderId) ?? [];
    entries.push({
      orderFulfillmentStatusHistoryId: Date.now(),
      status: fulfillmentStatus,
      changedAtUtc: new Date().toISOString(),
      changedByApplicationUserId: null,
      reason: null,
    });
    fulfillmentHistory.set(orderId, entries);
    return ok({ orderId: order.orderId, fulfillmentStatus });
  }
  const cancelStoreOrderMatch = path.match(/^\/api\/store\/orders\/(\d+)\/cancel$/);
  if (cancelStoreOrderMatch && method === 'POST') {
    const order = orderHistory.find((item) => item.orderId === Number(cancelStoreOrderMatch[1]));
    if (!order) return fail(404, 'Order not found');
    const reason = typeof body.reason === 'string' ? body.reason.trim() : '';
    if (!reason) return fail(400, 'A cancellation reason is required');
    if (order.fulfillmentStatus === 4) return fail(409, 'Order is already cancelled');
    const cancelledAtUtc = new Date().toISOString();
    order.fulfillmentStatus = 4;
    const orderId = Number(order.orderId);
    const entries = fulfillmentHistory.get(orderId) ?? [];
    entries.push({
      orderFulfillmentStatusHistoryId: Date.now(),
      status: 4,
      changedAtUtc: cancelledAtUtc,
      changedByApplicationUserId: null,
      reason,
    });
    fulfillmentHistory.set(orderId, entries);
    return ok({
      orderId: order.orderId,
      orderNumber: order.orderNumber,
      fulfillmentStatus: 4,
      cancelledAtUtc,
      cancellationReason: reason,
    });
  }
  const storeOrderFulfillmentHistoryMatch = path.match(
    /^\/api\/store\/orders\/(\d+)\/fulfillment-history$/,
  );
  if (storeOrderFulfillmentHistoryMatch && method === 'GET') {
    const order = orderHistory.find(
      (item) => item.orderId === Number(storeOrderFulfillmentHistoryMatch[1]),
    );
    if (!order) return fail(404, 'Order not found');
    return ok({
      orderId: order.orderId,
      orderNumber: order.orderNumber,
      currentStatus: order.fulfillmentStatus,
      history: fulfillmentHistory.get(Number(order.orderId)) ?? [],
    });
  }
  const storeOrderPaymentStatusMatch = path.match(/^\/api\/store\/orders\/(\d+)\/payment-status$/);
  if (storeOrderPaymentStatusMatch && method === 'PATCH') {
    const order = orderHistory.find(
      (item) => item.orderId === Number(storeOrderPaymentStatusMatch[1]),
    );
    if (!order) return fail(404, 'Payment not found');
    if (order.paymentMethod !== 2)
      return fail(400, 'Only cash-on-delivery payments can be updated');
    if (order.paymentStatus !== 1) return fail(409, 'Payment is already paid');
    order.paymentStatus = 2;
    order.paidAtUtc = new Date().toISOString();
    return ok(undefined);
  }
  if (path === '/api/orders' && method === 'POST') {
    if (!loggedIn) return fail(401, 'Sign in to place an order');
    const submissionId = String(body.checkoutSubmissionId ?? '');
    if (
      !/^[0-9a-f]{8}-[0-9a-f]{4}-[1-8][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(
        submissionId,
      )
    )
      return fail(400, 'A valid checkoutSubmissionId is required');
    const existing = placedOrders.get(submissionId);
    if (existing) return ok(existing); // Demo models server-side idempotency.
    const validation = validateCheckout();
    if ('error' in validation) return validation;
    const orderId = ++nextOrder;
    const result = {
      orderId,
      orderNumber: `AST-${String(orderId).padStart(6, '0')}`,
      totalAmount: cartTotals().totalAmount + demoShippingCharge,
      paymentMethod: Number(body.paymentMethod),
      paymentStatus: 1,
    };
    placedOrders.set(submissionId, result);
    orderHistory.push({
      ...result,
      placedAtUtc: new Date().toISOString(),
      fulfillmentStatus: 1,
      deliveryRecipientName: validation.address.recipientName,
      deliveryContactNumber: validation.address.contactNumber,
      deliveryCity: validation.address.city,
      deliveryNeighborhood: validation.address.neighborhood,
      deliveryStreet: validation.address.street,
      deliveryBuildingInfo: validation.address.buildingInfo,
      shippingCharge: demoShippingCharge,
      subtotal: cartTotals().totalAmount,
      items: cartItems.map((item) => ({
        orderItemId: item.cartItemId,
        productVariantId: item.productVariantId,
        productName: item.productName,
        sku: item.sku,
        variantAttributes: '',
        quantity: item.quantity,
        unitPrice: item.unitPrice,
        subtotal: item.subtotal,
      })),
    });
    for (const item of cartItems) {
      const row = stock.find((s) => s.productVariantId === item.productVariantId);
      if (row) {
        row.onHandQuantity -= item.quantity;
        row.availableQuantity -= item.quantity;
        row.isInStock = row.availableQuantity > 0;
      }
    }
    cartItems.splice(0);
    return ok(result);
  }
  const specs = path.match(/^\/api\/products\/(\d+)\/specifications$/);
  if (specs && method === 'POST') {
    const p = products.find((p) => p.productId === Number(specs[1]));
    if (!p) return fail(404, 'Product not found');
    const spec = {
      technicalSpecificationId: Date.now(),
      name: String(body.name || ''),
      value: String(body.value || ''),
      sortOrder: Number(body.sortOrder || 0),
    };
    p.technicalSpecifications.push(spec);
    return ok({ ...spec, technicalSpecificationId: Date.now(), productId: p.productId });
  }

  const specification = path.match(/^\/api\/products\/(\d+)\/specifications\/(\d+)$/);
  if (specification) {
    const p = products.find((p) => p.productId === Number(specification[1]));
    const item = p?.technicalSpecifications.find(
      (s) => s.technicalSpecificationId === Number(specification[2]),
    );
    if (!p || !item) return fail(404, 'Specification not found');
    if (method === 'DELETE') {
      p.technicalSpecifications.splice(p.technicalSpecifications.indexOf(item), 1);
      return ok(undefined);
    }
    Object.assign(item, body);
    return ok({ ...item, productId: p.productId, updatedAtUtc: new Date().toISOString() });
  }
  const imageCollection = path.match(/^\/api\/products\/(\d+)\/images$/);
  if (imageCollection && method === 'POST') {
    const p = products.find((p) => p.productId === Number(imageCollection[1]));
    if (!p) return fail(404, 'Product not found');
    const form = input.body instanceof FormData ? input.body : null;
    const item = {
      productImageId: Date.now(),
      storageKey: `demo/${Date.now()}.jpg`,
      altText: String(form?.get('AltText') || ''),
      sortOrder: Number(form?.get('SortOrder') || 0),
      isPrimary: p.images.length === 0,
    };
    p.images.push(item);
    return ok({ ...item, productId: p.productId });
  }
  const imageDetail = path.match(/^\/api\/products\/(\d+)\/images\/(\d+)$/);
  if (imageDetail) {
    const p = products.find((p) => p.productId === Number(imageDetail[1]));
    const item = p?.images.find((i) => i.productImageId === Number(imageDetail[2]));
    if (!p || !item) return fail(404, 'Image not found');
    if (method === 'DELETE') {
      p.images.splice(p.images.indexOf(item), 1);
      return ok(undefined);
    }
    if (body.isPrimary) p.images.forEach((i) => (i.isPrimary = false));
    Object.assign(item, body);
    return ok({ ...item, productId: p.productId, updatedAtUtc: new Date().toISOString() });
  }
  if (path.includes('/stock-reservation'))
    return ok({
      ...body,
      paymentAttemptId: numberInPath(4),
      expiresAtUtc: new Date(Date.now() + 900000).toISOString(),
    });

  return fail(501, `Demo route not implemented: ${method} ${path}`);
};
