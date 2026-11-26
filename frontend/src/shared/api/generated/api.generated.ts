/* AUTO-GENERATED from openapi/swagger.original.json. Do not edit manually. */
/* Run `npm run api:generate` whenever the ASP.NET Core OpenAPI contract changes. */
import { baseApi } from "../baseApi";

export type AddCartItemRequest = {
  "productVariantId"?: number;
  "quantity"?: number;
};

export type AddCartItemResponse = {
  "cartItemId"?: number;
  "productVariantId"?: number;
  "quantity"?: number;
};

export type AddCustomerAddressRequest = {
  "recipientName"?: string | null;
  "contactNumber"?: string | null;
  "city"?: string | null;
  "neighborhood"?: string | null;
  "street"?: string | null;
  "buildingInfo"?: string | null;
  "additionalAddressInfo"?: string | null;
};

export type AddCustomerAddressResponse = {
  "savedAddressId"?: number;
  "recipientName"?: string | null;
  "contactNumber"?: string | null;
  "city"?: string | null;
  "neighborhood"?: string | null;
  "street"?: string | null;
  "buildingInfo"?: string | null;
  "additionalAddressInfo"?: string | null;
  "isDefault"?: boolean;
};

export type AddProductImageResponse = {
  "productImageId"?: number;
  "productId"?: number;
  "storageKey"?: string | null;
  "altText"?: string | null;
  "sortOrder"?: number;
  "isPrimary"?: boolean;
  "createdAtUtc"?: string;
};

export type AddTechnicalSpecificationRequest = {
  "name"?: string | null;
  "value"?: string | null;
  "sortOrder"?: number;
};

export type AddTechnicalSpecificationResponse = {
  "technicalSpecificationId"?: number;
  "productId"?: number;
  "name"?: string | null;
  "value"?: string | null;
  "sortOrder"?: number;
  "createdAtUtc"?: string;
};

export type ChangeCustomerPasswordRequest = {
  "currentPassword"?: string | null;
  "newPassword"?: string | null;
};

export type CheckoutItemResponse = {
  "cartItemId"?: number;
  "productVariantId"?: number;
  "productName"?: string | null;
  "sku"?: string | null;
  "quantity"?: number;
  "unitPrice"?: number;
  "subtotal"?: number;
};

export type CheckoutSummaryResponse = {
  "shoppingCartId"?: number;
  "items"?: Array<CheckoutItemResponse> | null;
  "productsTotal"?: number;
};

export type CreateCategoryCommand = {
  "name"?: string | null;
  "description"?: string | null;
  "parentCategoryId"?: number | null;
  "sortOrder"?: number;
};

export type CreateCategoryResponse = {
  "categoryId"?: number;
  "name"?: string | null;
  "slug"?: string | null;
  "parentCategoryId"?: number | null;
  "isActive"?: boolean;
  "createdAtUtc"?: string;
};

export type CategoryListItemResponse = {
  "categoryId"?: number;
  "name"?: string | null;
  "slug"?: string | null;
  "parentCategoryId"?: number | null;
  "isActive"?: boolean;
};

export type CreateProductCommand = {
  "name"?: string | null;
  "description"?: string | null;
  "brandId"?: number | null;
  "categoryId"?: number | null;
};

export type CreateProductVariantRequest = {
  "sku"?: string | null;
  "price"?: number;
  "attributes"?: Array<VariantAttributeRequest> | null;
};

export type CreateProductVariantResponse = {
  "productVariantId"?: number;
  "productId"?: number;
  "sku"?: string | null;
  "price"?: number;
  "isActive"?: boolean;
  "onHandQuantity"?: number;
  "createdAtUtc"?: string;
};

export type CreateStockReservationResponse = {
  "paymentAttemptId"?: number;
  "expiresAtUtc"?: string;
};

export type CurrentUserResponse = {
  "userId"?: string;
  "email"?: string | null;
  "roles"?: Array<string> | null;
};

export type CustomerAddressResponse = {
  "savedAddressId"?: number;
  "recipientName"?: string | null;
  "contactNumber"?: string | null;
  "city"?: string | null;
  "neighborhood"?: string | null;
  "street"?: string | null;
  "buildingInfo"?: string | null;
  "additionalAddressInfo"?: string | null;
  "isDefault"?: boolean;
};

export type CustomerOrderDetailsResponse = {
  "orderId"?: number;
  "orderNumber"?: string | null;
  "placedAtUtc"?: string;
  "fulfillmentStatus"?: OrderFulfillmentStatus;
  "paymentMethod"?: PaymentMethod;
  "paymentStatus"?: PaymentStatus;
  "deliveryRecipientName"?: string | null;
  "deliveryContactNumber"?: string | null;
  "deliveryCity"?: string | null;
  "deliveryNeighborhood"?: string | null;
  "deliveryStreet"?: string | null;
  "deliveryBuildingInfo"?: string | null;
  "deliveryAdditionalInfo"?: string | null;
  "shippingCharge"?: number;
  "totalAmount"?: number;
  "items"?: Array<CustomerOrderItemDto> | null;
  "subtotal"?: number;
};

export type CustomerOrderItemDto = {
  "orderItemId"?: number;
  "productVariantId"?: number;
  "productName"?: string | null;
  "sku"?: string | null;
  "variantAttributes"?: string | null;
  "quantity"?: number;
  "unitPrice"?: number;
  "subtotal"?: number;
};

export type DecreaseStockRequest = {
  "quantity"?: number;
};

export type DecreaseStockResponse = {
  "productVariantId"?: number;
  "quantityDecreased"?: number;
};

export type DetermineShippingChargeResponse = {
  "shippingMode"?: ShippingMode;
  "shippingCharge"?: number;
};

export type GetCustomerProfileResponse = {
  "customerId"?: number;
  "firstName"?: string | null;
  "lastName"?: string | null;
  "email"?: string | null;
  "phoneNumber"?: string | null;
  "isEmailVerified"?: boolean;
  "createdAtUtc"?: string;
  "updatedAtUtc"?: string;
};

export type GetVariantAvailabilityResponse = {
  "productVariantId"?: number;
  "availableQuantity"?: number;
  "isInStock"?: boolean;
};

export type IncreaseStockRequest = {
  "quantity"?: number;
};

export type IncreaseStockResponse = {
  "productVariantId"?: number;
  "quantityAdded"?: number;
};

export type LoginRequest = {
  "email"?: string | null;
  "password"?: string | null;
};

export type LoginResponse = {
  "userId"?: string;
  "email"?: string | null;
  "roles"?: Array<string> | null;
  "isEmailVerified"?: boolean;
};

export type OrderFulfillmentStatus = 1 | 2 | 3 | 4;

export type OutOfStockProductResponse = {
  "productId"?: number;
  "productName"?: string | null;
  "productIsActive"?: boolean;
};

export type OutOfStockProductResponsePagedResult = {
  "items"?: Array<OutOfStockProductResponse> | null;
  "pageNumber"?: number;
  "pageSize"?: number;
  "totalCount"?: number;
  "totalPages"?: number;
  "hasPreviousPage"?: boolean;
  "hasNextPage"?: boolean;
};

export type PaymentMethod = 1 | 2;

export type PaymentStatus = 1 | 2;

export type PlaceOrderRequest = {
  "checkoutSubmissionId"?: string;
  "savedAddressId"?: number;
  "paymentMethod"?: PaymentMethod;
};

export type PlaceOrderResponse = {
  "orderId"?: number;
  "orderNumber"?: string | null;
  "totalAmount"?: number;
  "paymentMethod"?: PaymentMethod;
  "paymentStatus"?: PaymentStatus;
};

export type ProblemDetails = {
  "type"?: string | null;
  "title"?: string | null;
  "status"?: number | null;
  "detail"?: string | null;
  "instance"?: string | null;
  [key: string]: unknown;
};

export type PublicCategoryResponse = {
  "categoryId"?: number;
  "name"?: string | null;
  "slug"?: string | null;
};

export type ProductListItemResponse = {
  "productId"?: number;
  "name"?: string | null;
  "slug"?: string | null;
  "brandId"?: number | null;
  "categoryId"?: number | null;
  "priceFrom"?: number | null;
  "primaryImageStorageKey"?: string | null;
};

export type ProductListItemResponsePagedResult = {
  "items"?: Array<ProductListItemResponse> | null;
  "pageNumber"?: number;
  "pageSize"?: number;
  "totalCount"?: number;
  "totalPages"?: number;
  "hasPreviousPage"?: boolean;
  "hasNextPage"?: boolean;
};

export type RegisterCustomerRequest = {
  "firstName"?: string | null;
  "lastName"?: string | null;
  "email"?: string | null;
  "phoneNumber"?: string | null;
  "password"?: string | null;
};

export type RegisterCustomerResponse = {
  "customerId"?: number;
  "email"?: string | null;
  "isEmailVerified"?: boolean;
  "createdAtUtc"?: string;
};

export type ReviewCheckoutRequest = {
  "savedAddressId"?: number;
  "paymentMethod"?: PaymentMethod;
};

export type ReviewCheckoutResponse = {
  "shoppingCartId"?: number;
  "items"?: Array<CheckoutItemResponse> | null;
  "deliveryAddress"?: SelectCheckoutAddressResponse;
  "paymentMethod"?: PaymentMethod;
  "productsTotal"?: number;
  "shippingCharge"?: number;
  "finalTotal"?: number;
};

export type SelectCheckoutAddressResponse = {
  "savedAddressId"?: number;
  "recipientName"?: string | null;
  "contactNumber"?: string | null;
  "city"?: string | null;
  "neighborhood"?: string | null;
  "street"?: string | null;
  "buildingInfo"?: string | null;
  "additionalAddressInfo"?: string | null;
};

export type SetDefaultCustomerAddressResponse = {
  "savedAddressId"?: number;
  "isDefault"?: boolean;
};

export type ShippingMode = 1 | 2;

export type ShoppingCartItemResponse = {
  "cartItemId"?: number;
  "productVariantId"?: number;
  "productName"?: string | null;
  "sku"?: string | null;
  "quantity"?: number;
  "unitPrice"?: number;
  "subtotal"?: number;
};

export type ShoppingCartResponse = {
  "shoppingCartId"?: number;
  "items"?: Array<ShoppingCartItemResponse> | null;
  "totalQuantity"?: number;
  "totalAmount"?: number;
};

export type StoreOwnerStockResponse = {
  "productVariantId"?: number;
  "productId"?: number;
  "productName"?: string | null;
  "sku"?: string | null;
  "productIsActive"?: boolean;
  "variantIsActive"?: boolean;
  "onHandQuantity"?: number;
  "reservedQuantity"?: number;
  "securedCODQuantity"?: number;
  "availableQuantity"?: number;
  "isInStock"?: boolean;
};

export type StoreOwnerStockResponsePagedResult = {
  "items"?: Array<StoreOwnerStockResponse> | null;
  "pageNumber"?: number;
  "pageSize"?: number;
  "totalCount"?: number;
  "totalPages"?: number;
  "hasPreviousPage"?: boolean;
  "hasNextPage"?: boolean;
};

export type StoreOrderResponse = {
  "orderId"?: number;
  "orderNumber"?: string | null;
  "customerId"?: number;
  "totalAmount"?: number;
  "paymentMethod"?: PaymentMethod;
  "paymentStatus"?: number;
  "fulfillmentStatus"?: number;
  "placedAtUtc"?: string | null;
};

export type StoreOrderResponsePagedResult = {
  "items"?: Array<StoreOrderResponse> | null;
  "pageNumber"?: number;
  "pageSize"?: number;
  "totalCount"?: number;
  "totalPages"?: number;
  "hasPreviousPage"?: boolean;
  "hasNextPage"?: boolean;
};

export type StoreOrderDetailsResponse = {
  "orderId"?: number;
  "orderNumber"?: string | null;
  "customerId"?: number;
  "placedAtUtc"?: string | null;
  "fulfillmentStatus"?: OrderFulfillmentStatus;
  "paymentMethod"?: PaymentMethod;
  "paymentStatus"?: PaymentStatus;
  "paidAtUtc"?: string | null;
  "delivery"?: StoreOrderDeliveryResponse;
  "shippingCharge"?: number;
  "totalAmount"?: number;
  "items"?: Array<CustomerOrderItemDto> | null;
  "subtotal"?: number;
};

export type UpdateOrderFulfillmentStatusRequest = {
  "newStatus": OrderFulfillmentStatus;
};

export type UpdateOrderFulfillmentStatusResponse = {
  "orderId"?: number;
  "fulfillmentStatus"?: OrderFulfillmentStatus;
};

export type CancelOrderRequest = {
  "reason": string;
};

export type CancelOrderResponse = {
  "orderId"?: number;
  "orderNumber"?: string;
  "fulfillmentStatus"?: OrderFulfillmentStatus;
  "cancelledAtUtc"?: string;
  "cancellationReason"?: string;
};

export type GetOrderFulfillmentHistoryResponse = {
  "orderId"?: number;
  "orderNumber"?: string;
  "currentStatus"?: OrderFulfillmentStatus;
  "history"?: Array<OrderFulfillmentHistoryResponse>;
};

export type OrderFulfillmentHistoryResponse = {
  "orderFulfillmentStatusHistoryId"?: number;
  "status"?: OrderFulfillmentStatus;
  "changedAtUtc"?: string;
  "changedByApplicationUserId"?: string | null;
  "reason"?: string | null;
};

export type StoreOrderDeliveryResponse = {
  "recipientName"?: string | null;
  "contactNumber"?: string | null;
  "city"?: string | null;
  "neighborhood"?: string | null;
  "street"?: string | null;
  "buildingInfo"?: string | null;
  "additionalInfo"?: string | null;
  "deliveryStatus"?: number | null;
  "trackingReference"?: string | null;
  "shippedAtUtc"?: string | null;
  "deliveredAtUtc"?: string | null;
};

export type ReactivateCategoryResponse = {
  "categoryId"?: number;
  "isActive"?: boolean;
};

export type DeactivateCategoryResponse = {
  "categoryId"?: number;
  "isActive"?: boolean;
};

export type UpdateBrandRequest = {
  "name"?: string | null;
};

export type UpdateBrandResponse = {
  "brandId"?: number;
  "name"?: string | null;
  "slug"?: string | null;
  "isActive"?: boolean;
  "updatedAtUtc"?: string;
};

export type BrandListItemResponse = {
  "brandId"?: number;
  "name"?: string | null;
  "slug"?: string | null;
  "isActive"?: boolean;
};

export type GetBrandsResponse = {
  "items"?: Array<BrandListItemResponse> | null;
  "pageNumber"?: number;
  "pageSize"?: number;
  "totalCount"?: number;
  "totalPages"?: number;
  "hasPreviousPage"?: boolean;
  "hasNextPage"?: boolean;
};

export type CreateBrandRequest = {
  "name"?: string | null;
};

export type CreateBrandResponse = {
  "brandId"?: number;
  "name"?: string | null;
  "slug"?: string | null;
  "createdAtUtc"?: string;
};

export type CategoryHierarchyResponse = {
  "categoryId"?: number;
  "name"?: string | null;
  "slug"?: string | null;
  "children"?: Array<CategoryHierarchyResponse> | null;
};

export type MoveCategoryRequest = {
  "parentCategoryId"?: number | null;
};

export type MoveCategoryResponse = {
  "categoryId"?: number;
  "parentCategoryId"?: number | null;
};

export type UpdateCategoryRequest = {
  "name"?: string | null;
  "parentCategoryId"?: number | null;
};

export type UpdateCategoryResponse = {
  "categoryId"?: number;
  "name"?: string | null;
  "slug"?: string | null;
  "parentCategoryId"?: number | null;
  "isActive"?: boolean;
  "createdAtUtc"?: string;
};

export type UpdateCartItemQuantityRequest = {
  "quantity"?: number;
};

export type UpdateCartItemQuantityResponse = {
  "cartItemId"?: number;
  "productVariantId"?: number;
  "quantity"?: number;
};

export type UpdateCustomerAddressRequest = {
  "recipientName"?: string | null;
  "contactNumber"?: string | null;
  "city"?: string | null;
  "neighborhood"?: string | null;
  "street"?: string | null;
  "buildingInfo"?: string | null;
  "additionalAddressInfo"?: string | null;
};

export type UpdateCustomerAddressResponse = {
  "savedAddressId"?: number;
  "recipientName"?: string | null;
  "contactNumber"?: string | null;
  "city"?: string | null;
  "neighborhood"?: string | null;
  "street"?: string | null;
  "buildingInfo"?: string | null;
  "additionalAddressInfo"?: string | null;
  "isDefault"?: boolean;
  "updatedAtUtc"?: string;
};

export type UpdateCustomerProfileRequest = {
  "firstName"?: string | null;
  "lastName"?: string | null;
  "email"?: string | null;
  "phoneNumber"?: string | null;
  "currentPassword"?: string | null;
};

export type UpdateCustomerProfileResponse = {
  "customerId"?: number;
  "firstName"?: string | null;
  "lastName"?: string | null;
  "email"?: string | null;
  "phoneNumber"?: string | null;
  "isEmailVerified"?: boolean;
  "updatedAtUtc"?: string;
};

export type UpdateProductImageRequest = {
  "altText"?: string | null;
  "sortOrder"?: number;
  "isPrimary"?: boolean;
};

export type UpdateProductImageResponse = {
  "productImageId"?: number;
  "productId"?: number;
  "storageKey"?: string | null;
  "altText"?: string | null;
  "sortOrder"?: number;
  "isPrimary"?: boolean;
  "updatedAtUtc"?: string;
};

export type UpdateProductRequest = {
  "name"?: string | null;
  "description"?: string | null;
  "brandId"?: number | null;
  "categoryId"?: number | null;
};

export type UpdateProductResponse = {
  "productId"?: number;
  "name"?: string | null;
  "slug"?: string | null;
  "description"?: string | null;
  "brandId"?: number | null;
  "categoryId"?: number | null;
  "updatedAtUtc"?: string;
};

export type UpdateProductVariantRequest = {
  "sku"?: string | null;
  "price"?: number;
  "isActive"?: boolean;
};

export type UpdateProductVariantResponse = {
  "productVariantId"?: number;
  "productId"?: number;
  "sku"?: string | null;
  "price"?: number;
  "isActive"?: boolean;
  "updatedAtUtc"?: string;
};

export type UpdateTechnicalSpecificationRequest = {
  "name"?: string | null;
  "value"?: string | null;
  "sortOrder"?: number;
};

export type UpdateTechnicalSpecificationResponse = {
  "technicalSpecificationId"?: number;
  "productId"?: number;
  "name"?: string | null;
  "value"?: string | null;
  "sortOrder"?: number;
  "updatedAtUtc"?: string;
};

export type VariantAttributeRequest = {
  "name"?: string | null;
  "value"?: string | null;
  "sortOrder"?: number;
};

export type GetBrandsApiArg = { "PageNumber"?: number; "PageSize"?: number; "Search"?: string; "IsActive"?: boolean };
export type GetBrandsApiResponse = GetBrandsResponse;

export type CreateBrandApiArg = { "body": CreateBrandRequest };
export type CreateBrandApiResponse = CreateBrandResponse;

export type UpdateBrandApiArg = { "brandId": number; "body": UpdateBrandRequest };
export type UpdateBrandApiResponse = UpdateBrandResponse;

export type GetPublicCategoriesApiArg = void;
export type GetPublicCategoriesApiResponse = Array<PublicCategoryResponse>;

export type LoginApiArg = { "body": LoginRequest };
export type LoginApiResponse = LoginResponse;

export type LogoutApiArg = void;
export type LogoutApiResponse = void;

export type GetCurrentUserApiArg = void;
export type GetCurrentUserApiResponse = CurrentUserResponse;

export type GetCheckoutApiArg = void;
export type GetCheckoutApiResponse = CheckoutSummaryResponse;

export type SelectCheckoutAddressApiArg = { "savedAddressId": number };
export type SelectCheckoutAddressApiResponse = SelectCheckoutAddressResponse;

export type GetShippingChargeApiArg = void;
export type GetShippingChargeApiResponse = DetermineShippingChargeResponse;

export type ReviewCheckoutApiArg = { "body": ReviewCheckoutRequest };
export type ReviewCheckoutApiResponse = ReviewCheckoutResponse;

export type CreateStockReservationApiArg = { "paymentAttemptId": number };
export type CreateStockReservationApiResponse = CreateStockReservationResponse;

export type AddAddressApiArg = { "body": AddCustomerAddressRequest };
export type AddAddressApiResponse = AddCustomerAddressResponse;

export type GetAddressesApiArg = void;
export type GetAddressesApiResponse = Array<CustomerAddressResponse>;

export type UpdateAddressApiArg = { "addressId": number; "body": UpdateCustomerAddressRequest };
export type UpdateAddressApiResponse = UpdateCustomerAddressResponse;

export type DeleteAddressApiArg = { "addressId": number };
export type DeleteAddressApiResponse = void;

export type SetDefaultAddressApiArg = { "addressId": number };
export type SetDefaultAddressApiResponse = SetDefaultCustomerAddressResponse;

export type RegisterCustomerApiArg = { "body": RegisterCustomerRequest };
export type RegisterCustomerApiResponse = RegisterCustomerResponse;

export type GetCustomerProfileApiArg = void;
export type GetCustomerProfileApiResponse = GetCustomerProfileResponse;

export type UpdateCustomerProfileApiArg = { "body": UpdateCustomerProfileRequest };
export type UpdateCustomerProfileApiResponse = UpdateCustomerProfileResponse;

export type ChangeCustomerPasswordApiArg = { "X-CSRF-TOKEN"?: string; "body": ChangeCustomerPasswordRequest };
export type ChangeCustomerPasswordApiResponse = void;

export type PlaceOrderApiArg = { "body": PlaceOrderRequest };
export type PlaceOrderApiResponse = PlaceOrderResponse;

export type GetOrdersApiArg = { "pageNumber"?: number; "pageSize"?: number };
export type GetOrdersApiResponse = void;

export type GetOrderByIdApiArg = { "orderId": number };
export type GetOrderByIdApiResponse = CustomerOrderDetailsResponse;

export type AddProductImageApiArg = { "productId": number; "body": FormData };
export type AddProductImageApiResponse = AddProductImageResponse;

export type UpdateProductImageApiArg = { "productId": number; "imageId": number; "body": UpdateProductImageRequest };
export type UpdateProductImageApiResponse = UpdateProductImageResponse;

export type DeleteProductImageApiArg = { "productId": number; "imageId": number };
export type DeleteProductImageApiResponse = void;

export type GetProductByIdApiArg = { "id": number };
export type GetProductByIdApiResponse = void;

export type UpdateProductApiArg = { "id": number; "body": UpdateProductRequest };
export type UpdateProductApiResponse = UpdateProductResponse;

export type CreateProductApiArg = { "body": CreateProductCommand };
export type CreateProductApiResponse = void;

export type GetProductsApiArg = { "PageNumber"?: number; "PageSize"?: number; "Search"?: string; "BrandId"?: number; "CategoryId"?: number; "MinPrice"?: number; "MaxPrice"?: number; "SortBy"?: string; "SortDirection"?: string };
export type GetProductsApiResponse = ProductListItemResponsePagedResult;

export type DeactivateProductApiArg = { "productId": number };
export type DeactivateProductApiResponse = void;

export type ReactivateProductApiArg = { "productId": number };
export type ReactivateProductApiResponse = void;

export type CreateVariantApiArg = { "productId": number; "body": CreateProductVariantRequest };
export type CreateVariantApiResponse = CreateProductVariantResponse;

export type UpdateVariantApiArg = { "productId": number; "variantId": number; "body": UpdateProductVariantRequest };
export type UpdateVariantApiResponse = UpdateProductVariantResponse;

export type GetCartApiArg = void;
export type GetCartApiResponse = ShoppingCartResponse;

export type AddCartItemApiArg = { "body": AddCartItemRequest };
export type AddCartItemApiResponse = AddCartItemResponse;

export type ClearCartApiArg = void;
export type ClearCartApiResponse = void;

export type UpdateCartItemQuantityApiArg = { "cartItemId": number; "body": UpdateCartItemQuantityRequest };
export type UpdateCartItemQuantityApiResponse = UpdateCartItemQuantityResponse;

export type RemoveCartItemApiArg = { "cartItemId": number };
export type RemoveCartItemApiResponse = void;

export type GetStoreOrdersApiArg = { "PageNumber"?: number; "PageSize"?: number; "FulfillmentStatus"?: OrderFulfillmentStatus; "PaymentStatus"?: PaymentStatus; "StartDate"?: string; "EndDate"?: string };
export type GetStoreOrdersApiResponse = StoreOrderResponsePagedResult;

export type GetStoreOrderByIdApiArg = { "orderId": number };
export type GetStoreOrderByIdApiResponse = StoreOrderDetailsResponse;

export type UpdateOrderFulfillmentStatusApiArg = { "orderId": number; "body": UpdateOrderFulfillmentStatusRequest };
export type UpdateOrderFulfillmentStatusApiResponse = UpdateOrderFulfillmentStatusResponse;

export type CancelOrderApiArg = { "orderId": number; "body": CancelOrderRequest };
export type CancelOrderApiResponse = CancelOrderResponse;

export type GetOrderFulfillmentHistoryApiArg = { "orderId": number };
export type GetOrderFulfillmentHistoryApiResponse = GetOrderFulfillmentHistoryResponse;

export type UpdateOrderPaymentStatusApiArg = { "orderId": number };
export type UpdateOrderPaymentStatusApiResponse = void;

export type GetStoreOrderByNumberApiArg = { "orderNumber": string };
export type GetStoreOrderByNumberApiResponse = StoreOrderResponse;

export type GetStoreCategoriesApiArg = void;
export type GetStoreCategoriesApiResponse = Array<CategoryListItemResponse>;

export type CreateCategoryApiArg = { "body": CreateCategoryCommand };
export type CreateCategoryApiResponse = CreateCategoryResponse;

export type UpdateCategoryApiArg = { "categoryId": number; "body": UpdateCategoryRequest };
export type UpdateCategoryApiResponse = UpdateCategoryResponse;

export type MoveCategoryApiArg = { "categoryId": number; "body": MoveCategoryRequest };
export type MoveCategoryApiResponse = MoveCategoryResponse;

export type GetCategoryHierarchyApiArg = void;
export type GetCategoryHierarchyApiResponse = Array<CategoryHierarchyResponse>;

export type DeactivateCategoryApiArg = { "categoryId": number };
export type DeactivateCategoryApiResponse = DeactivateCategoryResponse;

export type ReactivateCategoryApiArg = { "categoryId": number };
export type ReactivateCategoryApiResponse = ReactivateCategoryResponse;

export type GetOwnerInventoryApiArg = { "pageNumber"?: number; "pageSize"?: number };
export type GetOwnerInventoryApiResponse = StoreOwnerStockResponsePagedResult;

export type IncreaseStockApiArg = { "productVariantId": number; "body": IncreaseStockRequest };
export type IncreaseStockApiResponse = IncreaseStockResponse;

export type DecreaseStockApiArg = { "productVariantId": number; "body": DecreaseStockRequest };
export type DecreaseStockApiResponse = DecreaseStockResponse;

export type GetOutOfStockVariantsApiArg = { "pageNumber"?: number; "pageSize"?: number };
export type GetOutOfStockVariantsApiResponse = StoreOwnerStockResponsePagedResult;

export type GetOutOfStockProductsApiArg = { "pageNumber"?: number; "pageSize"?: number };
export type GetOutOfStockProductsApiResponse = OutOfStockProductResponsePagedResult;

export type AddSpecificationApiArg = { "productId": number; "body": AddTechnicalSpecificationRequest };
export type AddSpecificationApiResponse = AddTechnicalSpecificationResponse;

export type UpdateSpecificationApiArg = { "productId": number; "specificationId": number; "body": UpdateTechnicalSpecificationRequest };
export type UpdateSpecificationApiResponse = UpdateTechnicalSpecificationResponse;

export type DeleteSpecificationApiArg = { "productId": number; "specificationId": number };
export type DeleteSpecificationApiResponse = void;

export type GetVariantAvailabilityApiArg = { "productVariantId": number };
export type GetVariantAvailabilityApiResponse = GetVariantAvailabilityResponse;

export const generatedApi = baseApi.injectEndpoints({
  endpoints: (build) => ({
    getBrands: build.query<GetBrandsApiResponse, GetBrandsApiArg>({
      query: (arg) => ({ url: "/api/brands", method: "GET", params: { "PageNumber": arg.PageNumber, "PageSize": arg.PageSize, "Search": arg.Search, "IsActive": arg.IsActive } }),
      providesTags: ["Products"],
    }),
    createBrand: build.mutation<CreateBrandApiResponse, CreateBrandApiArg>({
      query: (arg) => ({ url: "/api/brands", method: "POST", body: arg.body }),
      invalidatesTags: ["Products"],
    }),
    updateBrand: build.mutation<UpdateBrandApiResponse, UpdateBrandApiArg>({
      query: (arg) => ({ url: `/api/brands/${encodeURIComponent(String(arg.brandId))}`, method: "PUT", body: arg.body }),
      invalidatesTags: ["Products"],
    }),
    getPublicCategories: build.query<GetPublicCategoriesApiResponse, GetPublicCategoriesApiArg>({
      query: () => ({ url: "/api/categories", method: "GET" }),
      providesTags: ["Products"],
    }),
    login: build.mutation<LoginApiResponse, LoginApiArg>({
      query: (arg) => ({ url: "/api/auth/login", method: "POST", body: arg.body }),
      invalidatesTags: ["Auth","Customer","Cart","Checkout","Addresses","Orders"],
    }),
    logout: build.mutation<LogoutApiResponse, LogoutApiArg>({
      query: () => ({ url: "/api/auth/logout", method: "POST" }),
      invalidatesTags: ["Auth","Customer","Cart","Checkout","Addresses","Orders"],
    }),
    getCurrentUser: build.query<GetCurrentUserApiResponse, GetCurrentUserApiArg>({
      query: () => ({ url: "/api/auth/me", method: "GET" }),
      providesTags: ["Auth"],
    }),
    getCheckout: build.query<GetCheckoutApiResponse, GetCheckoutApiArg>({
      query: () => ({ url: "/api/checkout", method: "GET" }),
      providesTags: ["Checkout"],
    }),
    selectCheckoutAddress: build.query<SelectCheckoutAddressApiResponse, SelectCheckoutAddressApiArg>({
      query: (arg) => ({ url: `/api/checkout/addresses/${encodeURIComponent(String(arg.savedAddressId))}`, method: "GET" }),
      providesTags: ["Checkout"],
    }),
    getShippingCharge: build.query<GetShippingChargeApiResponse, GetShippingChargeApiArg>({
      query: () => ({ url: "/api/checkout/shipping", method: "GET" }),
      providesTags: ["Checkout"],
    }),
    reviewCheckout: build.mutation<ReviewCheckoutApiResponse, ReviewCheckoutApiArg>({
      query: (arg) => ({ url: "/api/checkout/review", method: "POST", body: arg.body }),
      invalidatesTags: ["Checkout"],
    }),
    createStockReservation: build.mutation<CreateStockReservationApiResponse, CreateStockReservationApiArg>({
      query: (arg) => ({ url: `/api/checkout/payment-attempts/${encodeURIComponent(String(arg.paymentAttemptId))}/stock-reservation`, method: "POST" }),
      invalidatesTags: ["Checkout","Inventory"],
    }),
    addAddress: build.mutation<AddAddressApiResponse, AddAddressApiArg>({
      query: (arg) => ({ url: "/api/customer/addresses", method: "POST", body: arg.body }),
      invalidatesTags: ["Addresses","Checkout"],
    }),
    getAddresses: build.query<GetAddressesApiResponse, GetAddressesApiArg>({
      query: () => ({ url: "/api/customer/addresses", method: "GET" }),
      providesTags: ["Addresses"],
    }),
    updateAddress: build.mutation<UpdateAddressApiResponse, UpdateAddressApiArg>({
      query: (arg) => ({ url: `/api/customer/addresses/${encodeURIComponent(String(arg.addressId))}`, method: "PUT", body: arg.body }),
      invalidatesTags: ["Addresses","Checkout"],
    }),
    deleteAddress: build.mutation<DeleteAddressApiResponse, DeleteAddressApiArg>({
      query: (arg) => ({ url: `/api/customer/addresses/${encodeURIComponent(String(arg.addressId))}`, method: "DELETE" }),
      invalidatesTags: ["Addresses","Checkout"],
    }),
    setDefaultAddress: build.mutation<SetDefaultAddressApiResponse, SetDefaultAddressApiArg>({
      query: (arg) => ({ url: `/api/customer/addresses/${encodeURIComponent(String(arg.addressId))}/default`, method: "PUT" }),
      invalidatesTags: ["Addresses","Checkout"],
    }),
    registerCustomer: build.mutation<RegisterCustomerApiResponse, RegisterCustomerApiArg>({
      query: (arg) => ({ url: "/api/customers/register", method: "POST", body: arg.body }),
      invalidatesTags: ["Customer","Auth","Cart","Addresses"],
    }),
    getCustomerProfile: build.query<GetCustomerProfileApiResponse, GetCustomerProfileApiArg>({
      query: () => ({ url: "/api/customers/me", method: "GET" }),
      providesTags: ["Customer"],
    }),
    updateCustomerProfile: build.mutation<UpdateCustomerProfileApiResponse, UpdateCustomerProfileApiArg>({
      query: (arg) => ({ url: "/api/customers/me", method: "PUT", body: arg.body }),
      invalidatesTags: ["Customer"],
    }),
    changeCustomerPassword: build.mutation<ChangeCustomerPasswordApiResponse, ChangeCustomerPasswordApiArg>({
      query: (arg) => ({ url: "/api/customers/me/change-password", method: "POST", body: arg.body }),
      invalidatesTags: ["Customer"],
    }),
    placeOrder: build.mutation<PlaceOrderApiResponse, PlaceOrderApiArg>({
      query: (arg) => ({ url: "/api/orders", method: "POST", body: arg.body }),
      invalidatesTags: ["Orders","Cart","Checkout","Inventory"],
    }),
    getOrders: build.query<GetOrdersApiResponse, GetOrdersApiArg>({
      query: (arg) => ({ url: "/api/orders", method: "GET", params: { "pageNumber": arg.pageNumber, "pageSize": arg.pageSize } }),
      providesTags: ["Orders"],
    }),
    getOrderById: build.query<GetOrderByIdApiResponse, GetOrderByIdApiArg>({
      query: (arg) => ({ url: `/api/orders/${encodeURIComponent(String(arg.orderId))}`, method: "GET" }),
      providesTags: ["Orders"],
    }),
    addProductImage: build.mutation<AddProductImageApiResponse, AddProductImageApiArg>({
      query: (arg) => ({ url: `/api/products/${encodeURIComponent(String(arg.productId))}/images`, method: "POST", body: arg.body }),
      invalidatesTags: ["Products"],
    }),
    updateProductImage: build.mutation<UpdateProductImageApiResponse, UpdateProductImageApiArg>({
      query: (arg) => ({ url: `/api/products/${encodeURIComponent(String(arg.productId))}/images/${encodeURIComponent(String(arg.imageId))}`, method: "PUT", body: arg.body }),
      invalidatesTags: ["Products"],
    }),
    deleteProductImage: build.mutation<DeleteProductImageApiResponse, DeleteProductImageApiArg>({
      query: (arg) => ({ url: `/api/products/${encodeURIComponent(String(arg.productId))}/images/${encodeURIComponent(String(arg.imageId))}`, method: "DELETE" }),
      invalidatesTags: ["Products"],
    }),
    getProductById: build.query<GetProductByIdApiResponse, GetProductByIdApiArg>({
      query: (arg) => ({ url: `/api/products/${encodeURIComponent(String(arg.id))}`, method: "GET" }),
      providesTags: ["Products"],
    }),
    updateProduct: build.mutation<UpdateProductApiResponse, UpdateProductApiArg>({
      query: (arg) => ({ url: `/api/products/${encodeURIComponent(String(arg.id))}`, method: "PUT", body: arg.body }),
      invalidatesTags: ["Products"],
    }),
    createProduct: build.mutation<CreateProductApiResponse, CreateProductApiArg>({
      query: (arg) => ({ url: "/api/products", method: "POST", body: arg.body }),
      invalidatesTags: ["Products"],
    }),
    getProducts: build.query<GetProductsApiResponse, GetProductsApiArg>({
      query: (arg) => ({ url: "/api/products", method: "GET", params: { "PageNumber": arg.PageNumber, "PageSize": arg.PageSize, "Search": arg.Search, "BrandId": arg.BrandId, "CategoryId": arg.CategoryId, "MinPrice": arg.MinPrice, "MaxPrice": arg.MaxPrice, "SortBy": arg.SortBy, "SortDirection": arg.SortDirection } }),
      providesTags: ["Products"],
    }),
    deactivateProduct: build.mutation<DeactivateProductApiResponse, DeactivateProductApiArg>({
      query: (arg) => ({ url: `/api/products/${encodeURIComponent(String(arg.productId))}/deactivate`, method: "PATCH" }),
      invalidatesTags: ["Products","Inventory"],
    }),
    reactivateProduct: build.mutation<ReactivateProductApiResponse, ReactivateProductApiArg>({
      query: (arg) => ({ url: `/api/products/${encodeURIComponent(String(arg.productId))}/reactivate`, method: "PATCH" }),
      invalidatesTags: ["Products","Inventory"],
    }),
    createVariant: build.mutation<CreateVariantApiResponse, CreateVariantApiArg>({
      query: (arg) => ({ url: `/api/products/${encodeURIComponent(String(arg.productId))}/variants`, method: "POST", body: arg.body }),
      invalidatesTags: ["Products","Inventory"],
    }),
    updateVariant: build.mutation<UpdateVariantApiResponse, UpdateVariantApiArg>({
      query: (arg) => ({ url: `/api/products/${encodeURIComponent(String(arg.productId))}/variants/${encodeURIComponent(String(arg.variantId))}`, method: "PUT", body: arg.body }),
      invalidatesTags: ["Products","Inventory"],
    }),
    getCart: build.query<GetCartApiResponse, GetCartApiArg>({
      query: () => ({ url: "/api/cart", method: "GET" }),
      providesTags: ["Cart"],
    }),
    addCartItem: build.mutation<AddCartItemApiResponse, AddCartItemApiArg>({
      query: (arg) => ({ url: "/api/cart/items", method: "POST", body: arg.body }),
      invalidatesTags: ["Cart","Checkout","Inventory"],
    }),
    clearCart: build.mutation<ClearCartApiResponse, ClearCartApiArg>({
      query: () => ({ url: "/api/cart/items", method: "DELETE" }),
      invalidatesTags: ["Cart","Checkout"],
    }),
    updateCartItemQuantity: build.mutation<UpdateCartItemQuantityApiResponse, UpdateCartItemQuantityApiArg>({
      query: (arg) => ({ url: `/api/cart/items/${encodeURIComponent(String(arg.cartItemId))}`, method: "PUT", body: arg.body }),
      invalidatesTags: ["Cart","Checkout","Inventory"],
    }),
    removeCartItem: build.mutation<RemoveCartItemApiResponse, RemoveCartItemApiArg>({
      query: (arg) => ({ url: `/api/cart/items/${encodeURIComponent(String(arg.cartItemId))}`, method: "DELETE" }),
      invalidatesTags: ["Cart","Checkout"],
    }),
    getStoreOrders: build.query<GetStoreOrdersApiResponse, GetStoreOrdersApiArg>({
      query: (arg) => ({ url: "/api/store/orders", method: "GET", params: { "PageNumber": arg.PageNumber, "PageSize": arg.PageSize, "FulfillmentStatus": arg.FulfillmentStatus, "PaymentStatus": arg.PaymentStatus, "StartDate": arg.StartDate, "EndDate": arg.EndDate } }),
      providesTags: ["Orders"],
    }),
    getStoreOrderById: build.query<GetStoreOrderByIdApiResponse, GetStoreOrderByIdApiArg>({
      query: (arg) => ({ url: `/api/store/orders/${encodeURIComponent(String(arg.orderId))}`, method: "GET" }),
      providesTags: ["Orders"],
    }),
    updateOrderFulfillmentStatus: build.mutation<UpdateOrderFulfillmentStatusApiResponse, UpdateOrderFulfillmentStatusApiArg>({
      query: (arg) => ({ url: `/api/store/orders/${encodeURIComponent(String(arg.orderId))}/fulfillment-status`, method: "PATCH", body: arg.body }),
      invalidatesTags: ["Orders"],
    }),
    cancelOrder: build.mutation<CancelOrderApiResponse, CancelOrderApiArg>({
      query: (arg) => ({ url: `/api/store/orders/${encodeURIComponent(String(arg.orderId))}/cancel`, method: "POST", body: arg.body }),
      invalidatesTags: ["Orders"],
    }),
    getOrderFulfillmentHistory: build.query<GetOrderFulfillmentHistoryApiResponse, GetOrderFulfillmentHistoryApiArg>({
      query: (arg) => ({ url: `/api/store/orders/${encodeURIComponent(String(arg.orderId))}/fulfillment-history`, method: "GET" }),
      providesTags: ["Orders"],
    }),
    updateOrderPaymentStatus: build.mutation<UpdateOrderPaymentStatusApiResponse, UpdateOrderPaymentStatusApiArg>({
      query: (arg) => ({ url: `/api/store/orders/${encodeURIComponent(String(arg.orderId))}/payment-status`, method: "PATCH" }),
      invalidatesTags: ["Orders"],
    }),
    getStoreOrderByNumber: build.query<GetStoreOrderByNumberApiResponse, GetStoreOrderByNumberApiArg>({
      query: (arg) => ({ url: `/api/store/orders/by-number/${encodeURIComponent(String(arg.orderNumber))}`, method: "GET" }),
      providesTags: ["Orders"],
    }),
    getStoreCategories: build.query<GetStoreCategoriesApiResponse, GetStoreCategoriesApiArg>({
      query: () => ({ url: "/api/store/categories", method: "GET" }),
      providesTags: ["Products"],
    }),
    createCategory: build.mutation<CreateCategoryApiResponse, CreateCategoryApiArg>({
      query: (arg) => ({ url: "/api/store/categories", method: "POST", body: arg.body }),
      invalidatesTags: ["Products"],
    }),
    updateCategory: build.mutation<UpdateCategoryApiResponse, UpdateCategoryApiArg>({
      query: (arg) => ({ url: `/api/store/categories/${encodeURIComponent(String(arg.categoryId))}`, method: "PUT", body: arg.body }),
      invalidatesTags: ["Products"],
    }),
    moveCategory: build.mutation<MoveCategoryApiResponse, MoveCategoryApiArg>({
      query: (arg) => ({ url: `/api/store/categories/${encodeURIComponent(String(arg.categoryId))}/parent`, method: "PUT", body: arg.body }),
      invalidatesTags: ["Products"],
    }),
    getCategoryHierarchy: build.query<GetCategoryHierarchyApiResponse, GetCategoryHierarchyApiArg>({
      query: () => ({ url: "/api/store/categories/hierarchy", method: "GET" }),
      providesTags: ["Products"],
    }),
    deactivateCategory: build.mutation<DeactivateCategoryApiResponse, DeactivateCategoryApiArg>({
      query: (arg) => ({ url: `/api/store/categories/${encodeURIComponent(String(arg.categoryId))}/deactivate`, method: "PATCH" }),
      invalidatesTags: ["Products"],
    }),
    reactivateCategory: build.mutation<ReactivateCategoryApiResponse, ReactivateCategoryApiArg>({
      query: (arg) => ({ url: `/api/store/categories/${encodeURIComponent(String(arg.categoryId))}/reactivate`, method: "PATCH" }),
      invalidatesTags: ["Products"],
    }),
    getOwnerInventory: build.query<GetOwnerInventoryApiResponse, GetOwnerInventoryApiArg>({
      query: (arg) => ({ url: "/api/owner/inventory", method: "GET", params: { "pageNumber": arg.pageNumber, "pageSize": arg.pageSize } }),
      providesTags: ["Inventory"],
    }),
    increaseStock: build.mutation<IncreaseStockApiResponse, IncreaseStockApiArg>({
      query: (arg) => ({ url: `/api/owner/inventory/variants/${encodeURIComponent(String(arg.productVariantId))}/increase`, method: "POST", body: arg.body }),
      invalidatesTags: ["Inventory","Products"],
    }),
    decreaseStock: build.mutation<DecreaseStockApiResponse, DecreaseStockApiArg>({
      query: (arg) => ({ url: `/api/owner/inventory/variants/${encodeURIComponent(String(arg.productVariantId))}/decrease`, method: "POST", body: arg.body }),
      invalidatesTags: ["Inventory","Products"],
    }),
    getOutOfStockVariants: build.query<GetOutOfStockVariantsApiResponse, GetOutOfStockVariantsApiArg>({
      query: (arg) => ({ url: "/api/owner/inventory/out-of-stock/variants", method: "GET", params: { "pageNumber": arg.pageNumber, "pageSize": arg.pageSize } }),
      providesTags: ["Inventory"],
    }),
    getOutOfStockProducts: build.query<GetOutOfStockProductsApiResponse, GetOutOfStockProductsApiArg>({
      query: (arg) => ({ url: "/api/owner/inventory/out-of-stock/products", method: "GET", params: { "pageNumber": arg.pageNumber, "pageSize": arg.pageSize } }),
      providesTags: ["Inventory"],
    }),
    addSpecification: build.mutation<AddSpecificationApiResponse, AddSpecificationApiArg>({
      query: (arg) => ({ url: `/api/products/${encodeURIComponent(String(arg.productId))}/specifications`, method: "POST", body: arg.body }),
      invalidatesTags: ["Products"],
    }),
    updateSpecification: build.mutation<UpdateSpecificationApiResponse, UpdateSpecificationApiArg>({
      query: (arg) => ({ url: `/api/products/${encodeURIComponent(String(arg.productId))}/specifications/${encodeURIComponent(String(arg.specificationId))}`, method: "PUT", body: arg.body }),
      invalidatesTags: ["Products"],
    }),
    deleteSpecification: build.mutation<DeleteSpecificationApiResponse, DeleteSpecificationApiArg>({
      query: (arg) => ({ url: `/api/products/${encodeURIComponent(String(arg.productId))}/specifications/${encodeURIComponent(String(arg.specificationId))}`, method: "DELETE" }),
      invalidatesTags: ["Products"],
    }),
    getVariantAvailability: build.query<GetVariantAvailabilityApiResponse, GetVariantAvailabilityApiArg>({
      query: (arg) => ({ url: `/api/variants/${encodeURIComponent(String(arg.productVariantId))}/availability`, method: "GET" }),
      providesTags: ["Inventory"],
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetBrandsQuery,
  useCreateBrandMutation,
  useUpdateBrandMutation,
  useGetPublicCategoriesQuery,
  useLoginMutation,
  useLogoutMutation,
  useGetCurrentUserQuery,
  useGetCheckoutQuery,
  useSelectCheckoutAddressQuery,
  useGetShippingChargeQuery,
  useReviewCheckoutMutation,
  useCreateStockReservationMutation,
  useAddAddressMutation,
  useGetAddressesQuery,
  useUpdateAddressMutation,
  useDeleteAddressMutation,
  useSetDefaultAddressMutation,
  useRegisterCustomerMutation,
  useGetCustomerProfileQuery,
  useUpdateCustomerProfileMutation,
  useChangeCustomerPasswordMutation,
  usePlaceOrderMutation,
  useGetOrdersQuery,
  useGetOrderByIdQuery,
  useAddProductImageMutation,
  useUpdateProductImageMutation,
  useDeleteProductImageMutation,
  useGetProductByIdQuery,
  useUpdateProductMutation,
  useCreateProductMutation,
  useGetProductsQuery,
  useDeactivateProductMutation,
  useReactivateProductMutation,
  useCreateVariantMutation,
  useUpdateVariantMutation,
  useGetCartQuery,
  useAddCartItemMutation,
  useClearCartMutation,
  useUpdateCartItemQuantityMutation,
  useRemoveCartItemMutation,
  useGetStoreOrdersQuery,
  useGetStoreOrderByIdQuery,
  useUpdateOrderFulfillmentStatusMutation,
  useCancelOrderMutation,
  useGetOrderFulfillmentHistoryQuery,
  useUpdateOrderPaymentStatusMutation,
  useGetStoreOrderByNumberQuery,
  useGetStoreCategoriesQuery,
  useCreateCategoryMutation,
  useUpdateCategoryMutation,
  useMoveCategoryMutation,
  useGetCategoryHierarchyQuery,
  useDeactivateCategoryMutation,
  useReactivateCategoryMutation,
  useGetOwnerInventoryQuery,
  useIncreaseStockMutation,
  useDecreaseStockMutation,
  useGetOutOfStockVariantsQuery,
  useGetOutOfStockProductsQuery,
  useAddSpecificationMutation,
  useUpdateSpecificationMutation,
  useDeleteSpecificationMutation,
  useGetVariantAvailabilityQuery,
} = generatedApi;
