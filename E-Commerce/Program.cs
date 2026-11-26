using ECommerce.Application.Authentication.Abstractions;
using ECommerce.Application.Authentication.Login;
using ECommerce.Application.Authentication.Logout;
using ECommerce.Application.Brands.Abstractions;
using ECommerce.Application.Brands.CreateBrand;
using ECommerce.Application.Brands.DeactivateBrand;
using ECommerce.Application.Brands.GetBrands;
using ECommerce.Application.Brands.UpdateBrand;
using ECommerce.Application.Catalog.Abstractions;
using ECommerce.Application.Catalog.Common.Abstractions;
using ECommerce.Application.Catalog.Common_repository_contract;
using ECommerce.Application.Catalog.DeactivateProduct;
using ECommerce.Application.Catalog.GetProducts;
using ECommerce.Application.Catalog.Product.CreateProduct;
using ECommerce.Application.Catalog.Product.GetProductById;
using ECommerce.Application.Catalog.Product.UpdateProduct;
using ECommerce.Application.Catalog.ProductImage.AddProductImage;
using ECommerce.Application.Catalog.ProductImage.UpdateProductImage;
using ECommerce.Application.Catalog.ProductImageApplication.DeleteProductImage;
using ECommerce.Application.Catalog.ProductVariant.CreateProductVariant;
using ECommerce.Application.Catalog.ProductVariant.UpdateProductVariant;
using ECommerce.Application.Catalog.ReactivateProduct;
using ECommerce.Application.Catalog.TechnicalSpecification.AddTechnicalSpecification;
using ECommerce.Application.Catalog.TechnicalSpecification.DeleteTechnicalSpecification;
using ECommerce.Application.Catalog.TechnicalSpecification.UpdateTechnicalSpecification;
using ECommerce.Application.Categories.Abstractions;
using ECommerce.Application.Categories.CreateCategory;
using ECommerce.Application.Categories.DeactivateCategory;
using ECommerce.Application.Categories.ManageHierarchy;
using ECommerce.Application.Categories.PublicCategories;
using ECommerce.Application.Categories.ReactivateCategory;
using ECommerce.Application.Categories.UpdateCategory;
using ECommerce.Application.Categories.ViewCategories;
using ECommerce.Application.Categories.ViewHierarchy;
using ECommerce.Application.Checkout.DetermineShippingCharge;
using ECommerce.Application.Checkout.PlaceOrder;
using ECommerce.Application.Checkout.ReviewOrder;
using ECommerce.Application.Checkout.SelectPaymentMethod;
using ECommerce.Application.Checkout.SelectSavedAddress;
using ECommerce.Application.Checkout.StartCheckout;
using ECommerce.Application.Checkout.ValidateAvailability;
using ECommerce.Application.Common.Security;
using ECommerce.Application.Customers.Addresses.Abstractions;
using ECommerce.Application.Customers.Addresses.AddCustomerAddress;
using ECommerce.Application.Customers.Addresses.DeleteCustomerAddress;
using ECommerce.Application.Customers.Addresses.GetCustomerAddresses;
using ECommerce.Application.Customers.Addresses.SetDefaultCustomerAddress;
using ECommerce.Application.Customers.Addresses.UpdateCustomerAddress;
using ECommerce.Application.Customers.CustomerAccountManag.Abstractions;
using ECommerce.Application.Customers.CustomerAccountManag.ChangeCustomerPassword;
using ECommerce.Application.Customers.CustomerAccountManag.GetCustomerProfile;
using ECommerce.Application.Customers.CustomerAccountManag.RegisterCustomer;
using ECommerce.Application.Customers.CustomerAccountManag.UpdateCustomerProfile;
using ECommerce.Application.InStoreSales.CreateInStoreSale;
using ECommerce.Application.InStoreSales.GetInStoreSaleDetails;
using ECommerce.Application.InStoreSales.GetInStoreSales;
using ECommerce.Application.InStoreSales.UpdateInStoreSale;
using ECommerce.Application.InStoreSales.VoidInStoreSale;
using ECommerce.Application.Inventory.Abstractions;
using ECommerce.Application.Inventory.CreateStockReservation;
using ECommerce.Application.Inventory.DecreaseStock;
using ECommerce.Application.Inventory.GetVariantAvailability;
using ECommerce.Application.Inventory.IncreaseStock;
using ECommerce.Application.Inventory.StoreOwnerViewStock;
using ECommerce.Application.Inventory.ViewOutOfStock;
using ECommerce.Application.Orders.GetCustomerOrderDetails;
using ECommerce.Application.Orders.GetCustomerOrders;
using ECommerce.Application.Reports.OrdersByStatus;
using ECommerce.Application.Reports.RevenueTrends;
using ECommerce.Application.Reports.SalesSummary;
using ECommerce.Application.ShoppingCart.AddCartItem;
using ECommerce.Application.ShoppingCart.ClearCart;
using ECommerce.Application.ShoppingCart.GetShoppingCart;
using ECommerce.Application.ShoppingCart.RemoveCartItem;
using ECommerce.Application.ShoppingCart.UpdateCartItemQuantity;
using ECommerce.Application.StoreOrders.CancelOrder;
using ECommerce.Application.StoreOrders.SearchOrders;
using ECommerce.Application.StoreOrders.UpdateFulfillmentStatus;
using ECommerce.Application.StoreOrders.UpdatePaymentStatus;
using ECommerce.Application.StoreOrders.ViewFulfillmentHistory;
using ECommerce.Application.StoreOrders.ViewOrderDetails;
using ECommerce.Application.StoreOrders.ViewOrders;
using ECommerce.Application.SupplierInvoices.CreateSupplierInvoice;
using ECommerce.Application.SupplierInvoices.GetSupplierInvoiceById;
using ECommerce.Application.SupplierInvoices.GetSupplierPaymentHistory;
using ECommerce.Application.SupplierInvoices.ListSupplierInvoices;
using ECommerce.Application.SupplierInvoices.RecordSupplierPayment;
using ECommerce.Application.Suppliers.CreateSupplier;
using ECommerce.Application.Suppliers.GetSupplierById;
using ECommerce.Application.Suppliers.ListSuppliers;
using ECommerce.Application.Suppliers.UpdateSupplier;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.Persistence;
using ECommerce.Infrastructure.Persistence.Locking;
using ECommerce.Infrastructure.Persistence.Repositories;
using ECommerce.Infrastructure.Persistence.Repositories.Brand;
using ECommerce.Infrastructure.Persistence.Repositories.Catalog;
using ECommerce.Infrastructure.Persistence.Repositories.Checkout;
using ECommerce.Infrastructure.Persistence.Repositories.Customers;
using ECommerce.Infrastructure.Persistence.Repositories.InStoreSales;
using ECommerce.Infrastructure.Persistence.Repositories.Inventory;
using ECommerce.Infrastructure.Persistence.Repositories.Orders;
using ECommerce.Infrastructure.Persistence.Repositories.Reports;
using ECommerce.Infrastructure.Persistence.Repositories.Reports.SalesSummary;
using ECommerce.Infrastructure.Persistence.Repositories.ShoppingCart;
using ECommerce.Infrastructure.Persistence.Repositories.StoreOrders;
using ECommerce.Infrastructure.Persistence.Repositories.SupplierInvoices;
using ECommerce.Infrastructure.Persistence.Repositories.Suppliers;
using ECommerce.Infrastructure.Storage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services);
});
// Add services to the container.

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(
        new AutoValidateAntiforgeryTokenAttribute());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "CsrfToken",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "X-CSRF-TOKEN",
            Description = "Enter the CSRF request token"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "CsrfToken"
                    }
                },
                Array.Empty<string>()
            }
        });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(//This tells Entity Framework Core to use the SQL Server database provider.
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Data Protection is used to protect sensitive information cryptographically
builder.Services.AddDataProtection();
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";

    options.Cookie.Name = "__Host-ECommerce.Csrf";

    options.Cookie.HttpOnly = true;

    options.Cookie.SecurePolicy =
        CookieSecurePolicy.Always;

    options.Cookie.SameSite = SameSiteMode.Strict;

    options.Cookie.Path = "/";
});
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            IdentityConstants.ApplicationScheme;

        options.DefaultChallengeScheme =
            IdentityConstants.ApplicationScheme;

        options.DefaultSignInScheme =
            IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddDataProtection();
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        // BR-001: Customer email must be unique.
        options.User.RequireUniqueEmail = true;

        // BR-007: Proposed password policy.
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;

        // The current BRs do not require confirmation for login
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedAccount = false;

        // Proposed protection against repeated password guesses
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(
        AuthorizationPolicies.CustomerOnly,
        policy => policy
            .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
            .RequireAuthenticatedUser()
            .RequireRole(ApplicationRoles.Customer))
    .AddPolicy(
        AuthorizationPolicies.StoreOwnerOnly,
        policy => policy
            .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
            .RequireAuthenticatedUser()
            .RequireRole(ApplicationRoles.StoreOwner));
builder.Services.Configure<DataProtectionTokenProviderOptions>(
    options =>
    {
        options.TokenLifespan = TimeSpan.FromHours(1);
    });


builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "__Host-ECommerce.Auth";

    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Path = "/";

    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;

    // Make API requests receive HTTP statuses, not HTML redirects
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode =
            StatusCodes.Status401Unauthorized;

        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode =
            StatusCodes.Status403Forbidden;

        return Task.CompletedTask;
    };
});


builder.Services.AddScoped<IProductReadRepository,ProductReadRepository>();

builder.Services.AddScoped<GetProductByIdService>();

builder.Services.AddScoped<
    IProductWriteRepository,
    ProductWriteRepository>();
builder.Services.AddScoped<CreateProductService>();

builder.Services.AddScoped<
    IProductUpdateRepository,
    ProductUpdateRepository>();
builder.Services.AddScoped<UpdateProductService>();

builder.Services.AddScoped<
    IProductVariantWriteRepository,
    ProductVariantWriteRepository>();

builder.Services.AddScoped<CreateProductVariantService>();

builder.Services.AddScoped<ChangeCustomerPasswordService>();

builder.Services.AddScoped<
    ICustomerPasswordGateway,
    CustomerPasswordGateway>();

builder.Services.AddScoped<IImageStorage>(sp =>
{
    var environment = sp.GetRequiredService<IWebHostEnvironment>();

    var storageDirectory = Path.Combine(
        environment.ContentRootPath,
        "PrivateUploads",
        "ProductImages");

    return new LocalImageStorage(storageDirectory);
});

builder.Services.AddScoped<
    IImageValidator,
    ImageSharpImageValidator>();
builder.Services.AddScoped<
    IProductImageRepository,
    ProductImageRepository>();

builder.Services.AddScoped<AddProductImageService>();

builder.Services.AddScoped<
    ITechnicalSpecificationRepository,
    TechnicalSpecificationRepository>();

builder.Services.AddScoped<AddTechnicalSpecificationService>();

builder.Services.AddScoped<
    ITechnicalSpecificationUpdateRepository,
    TechnicalSpecificationUpdateRepository>();
builder.Services.AddScoped<UpdateTechnicalSpecificationService>();

builder.Services.AddScoped<
    ITechnicalSpecificationDeleteRepository,
    TechnicalSpecificationDeleteRepository>();

builder.Services.AddScoped<DeleteTechnicalSpecificationService>();

builder.Services.AddScoped<
    ILogoutGateway,
    IdentityLogoutGateway>();

builder.Services.AddScoped<LogoutService>();

builder.Services.AddScoped<
    IProductVariantUpdateRepository,
    ProductVariantUpdateRepository>();

builder.Services.AddScoped<UpdateProductVariantService>();

builder.Services.AddScoped<IProductImageUpdateRepository,
    ProductImageUpdateRepository>();

builder.Services.AddScoped<UpdateProductImageService>();

builder.Services.AddScoped<
    IProductImageDeleteRepository,
    ProductImageDeleteRepository>();

builder.Services.AddScoped<DeleteProductImageService>();

builder.Services.AddScoped<
    IProductListReadRepository,
    ProductListReadRepository>();
builder.Services.AddScoped<GetProductsService>();

builder.Services.AddScoped<DeactivateProductService>();
builder.Services.AddScoped<ReactivateProductService>();

builder.Services.AddScoped<
    IProductActivationRepository,
    ProductActivationRepository>();

builder.Services.AddScoped<
    IInventoryAvailabilityReadRepository,
    InventoryAvailabilityReadRepository>();

builder.Services.AddScoped<GetVariantAvailabilityService>();

builder.Services.AddScoped<
    IStoreOwnerInventoryReadRepository,
    StoreOwnerInventoryReadRepository>();
builder.Services.AddScoped<StoreOwnerViewStockService>();

builder.Services.AddScoped<
    IInventoryStockWriteRepository,
    InventoryStockWriteRepository>(); 
builder.Services.AddScoped<IncreaseStockService>();

builder.Services.AddScoped<DecreaseStockService>();

builder.Services.AddScoped<ViewOutOfStockService>();

builder.Services.AddScoped<
    IStockReservationWriteRepository,
    StockReservationWriteRepository>();

builder.Services.AddScoped<CreateStockReservationService>();


builder.Services
    .AddOptions<StockReservationOptions>()
    .Bind(builder.Configuration.GetSection(
        StockReservationOptions.SectionName))
    .Validate(
        options => options.DurationMinutes is >= 1 and <= 60,
        "Reservation duration must be between 1 and 60 minutes.")
    .ValidateOnStart();

builder.Services.AddScoped<
    ICustomerRegistrationGateway,
    CustomerRegistrationGateway>();
builder.Services.AddScoped<RegisterCustomerService>();




builder.Services.AddScoped<GetCustomerProfileService>();

builder.Services.AddScoped<
    ICustomerProfileReadRepository,
    CustomerProfileReadRepository>();

builder.Services.AddScoped<UpdateCustomerProfileService>();

builder.Services.AddScoped<
    ICustomerProfileUpdateGateway,
    CustomerProfileUpdateGateway>();

builder.Services.AddScoped<
    ICustomerAddressRepository,
    CustomerAddressRepository>();
builder.Services.AddScoped<AddCustomerAddressService>();
builder.Services.AddSingleton<TimeProvider>(
    TimeProvider.System);

builder.Services.AddScoped<GetCustomerAddressesService>();
builder.Services.AddScoped<UpdateCustomerAddressService>();
builder.Services.AddScoped<DeleteCustomerAddressService>();
builder.Services.AddScoped<SetDefaultCustomerAddressService>();

builder.Services.AddScoped<
    IShoppingCartReadRepository,
    ShoppingCartReadRepository>();
builder.Services.AddScoped<GetShoppingCartService>();

builder.Services.AddScoped<AddCartItemService>();
builder.Services.AddScoped<
    IAddCartItemRepository,
    AddCartItemRepository>();

builder.Services.AddScoped<
    ICartVariantAvailabilityReader,
    CartVariantAvailabilityReader>();

builder.Services.AddScoped<UpdateCartItemQuantityService>();
builder.Services.AddScoped<
    IUpdateCartItemQuantityRepository,
    UpdateCartItemQuantityRepository>();

builder.Services.AddScoped<RemoveCartItemService>();
builder.Services.AddScoped<
    IRemoveCartItemRepository,
    RemoveCartItemRepository>();

builder.Services.AddScoped<ClearCartService>();
builder.Services.AddScoped<
    IClearCartRepository,
    ClearCartRepository>();

builder.Services.AddScoped<
    ICheckoutCartReadRepository,
    CheckoutCartReadRepository>();
builder.Services.AddScoped<GetCheckoutSummaryService>();

builder.Services.AddScoped<
    ICheckoutAddressReadRepository,
    CheckoutAddressReadRepository>();
builder.Services.AddScoped<SelectCheckoutAddressService>();

builder.Services.AddScoped<
    IShippingConfigurationReadRepository,
    ShippingConfigurationReadRepository>();

builder.Services.AddScoped<DetermineShippingChargeService>();
builder.Services.AddSingleton<
    IPaymentMethodValidator,
    PaymentMethodValidator>();
builder.Services.AddScoped<CheckoutAvailabilityValidator>();

builder.Services.AddScoped<ValidateCheckoutAvailabilityService>();

builder.Services.AddScoped<ReviewCheckoutService>();

builder.Services.AddScoped<PlaceOrderService>();
builder.Services.AddScoped<
    IPlaceOrderRepository,
    PlaceOrderRepository>();
builder.Services.AddScoped<ShoppingCartLock>();


builder.Services.AddScoped<
    IOrderHistoryReadRepository,
    OrderHistoryReadRepository>();

builder.Services.AddScoped<GetCustomerOrdersService>();
builder.Services.AddScoped<GetCustomerOrderDetailsService>();

builder.Services.AddScoped<
    ILoginGateway,
    IdentityLoginGateway>();

builder.Services.AddScoped<LoginService>();

builder.Services.AddScoped<
    ICategoryRepository,
    CategoryRepository>();

builder.Services.AddScoped<CreateCategoryService>();


builder.Services.AddScoped<ViewCategoriesService>();
builder.Services.AddScoped<
  ICategoryReadRepository,
  CategoryReadRepository>();
builder.Services.AddScoped<UpdateCategoryService>();
builder.Services.AddScoped<GetPublicCategoriesService>();

builder.Services.AddScoped<MoveCategoryService>();
builder.Services.AddScoped<GetCategoryHierarchyService>();
builder.Services.AddScoped<DeactivateCategoryService>();
builder.Services.AddScoped<ReactivateCategoryService>();

builder.Services.AddScoped<CreateBrandService>();
builder.Services.AddScoped<
    IBrandRepository,
    BrandRepository>();

builder.Services.AddScoped<GetBrandsService>();
builder.Services.AddScoped<
    IBrandReadRepository,
    BrandReadRepository>();

builder.Services.AddScoped<UpdateBrandService>();

builder.Services.AddScoped<DeactivateBrandService>();

builder.Services.AddScoped<GetStoreOrdersService>();
builder.Services.AddScoped<
    IStoreOrderReadRepository,
    StoreOrderReadRepository>();

builder.Services.AddScoped<GetStoreOrderDetailsService>();
builder.Services.AddScoped<
    IStoreOrderDetailsReadRepository,
    StoreOrderDetailsReadRepository>();


builder.Services.AddScoped<SearchStoreOrdersService>();


builder.Services.AddScoped<
    IOrderFulfillmentRepository,
    OrderFulfillmentRepository>();

builder.Services.AddScoped<
    UpdateOrderFulfillmentStatusService>();

builder.Services.AddScoped<
    IOrderFulfillmentHistoryReadRepository,
    OrderFulfillmentHistoryReadRepository>();
builder.Services.AddScoped<
    GetOrderFulfillmentHistoryService>();
builder.Services.AddScoped<
    IUpdatePaymentStatusRepository,
    UpdatePaymentStatusRepository>();

builder.Services.AddScoped<UpdatePaymentStatusService>();

builder.Services.AddScoped<CancelOrderService>();

builder.Services.AddScoped<
    ICancelOrderRepository,
    CancelOrderRepository>();


builder.Services.AddScoped<
    ICreateInStoreSaleRepository,
    CreateInStoreSaleRepository>();
builder.Services.AddScoped<CreateInStoreSaleService>();

builder.Services.AddScoped<GetInStoreSalesService>();

builder.Services.AddScoped<
    IGetInStoreSalesRepository,
    GetInStoreSalesRepository>();

builder.Services.AddScoped<GetInStoreSaleDetailsService>();
builder.Services.AddScoped<
    IGetInStoreSaleDetailsRepository,
    GetInStoreSaleDetailsRepository>();


builder.Services.AddScoped<
    IVoidInStoreSaleRepository,
    VoidInStoreSaleRepository>();
builder.Services.AddScoped<VoidInStoreSaleService>();

builder.Services.AddScoped<GetSalesSummaryService>();

builder.Services.AddScoped<
    IGetSalesSummaryRepository,
    GetSalesSummaryRepository>();

builder.Services.AddScoped<GetRevenueTrendsService>();
builder.Services.AddScoped<
    IGetRevenueTrendsRepository,
    GetRevenueTrendsRepository>();



builder.Services.AddScoped<GetOrdersByStatusService>();

builder.Services.AddScoped<
    IGetOrdersByStatusRepository,
    GetOrdersByStatusRepository>();


builder.Services.AddScoped<
    IUpdateInStoreSaleRepository,
    UpdateInStoreSaleRepository>();

builder.Services.AddScoped<UpdateInStoreSaleService>();

builder.Services.AddScoped<CreateSupplierService>();
builder.Services.AddScoped<
    ICreateSupplierRepository,
    CreateSupplierRepository>();

builder.Services.AddScoped<GetSupplierByIdService>();

builder.Services.AddScoped<
    IGetSupplierByIdRepository,
    GetSupplierByIdRepository>();

builder.Services.AddScoped<ListSuppliersService>();

builder.Services.AddScoped<
    IListSuppliersRepository,
    ListSuppliersRepository>();

builder.Services.AddScoped<UpdateSupplierService>();

builder.Services.AddScoped<
    IUpdateSupplierRepository,
    UpdateSupplierRepository>();

builder.Services.AddScoped<CreateSupplierInvoiceService>();


builder.Services.AddScoped<
    ICreateSupplierInvoiceRepository,
    CreateSupplierInvoiceRepository>();

builder.Services.AddScoped<GetSupplierInvoiceByIdService>();

builder.Services.AddScoped<
    IGetSupplierInvoiceByIdRepository,
    GetSupplierInvoiceByIdRepository>();

builder.Services.AddScoped<ListSupplierInvoicesService>();

builder.Services.AddScoped<
    IListSupplierInvoicesRepository,
    ListSupplierInvoicesRepository>();

builder.Services.AddScoped<RecordSupplierPaymentService>();

builder.Services.AddScoped<
    IRecordSupplierPaymentRepository,
    RecordSupplierPaymentRepository>();
builder.Services.AddScoped<
    GetSupplierPaymentHistoryService>();

builder.Services.AddScoped<
    IGetSupplierPaymentHistoryRepository,
    GetSupplierPaymentHistoryRepository>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("web", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ...after var app = builder.Build();
// before app.MapControllers()

var app = builder.Build();
app.Logger.LogInformation(
    "E-Commerce application initialized successfully");
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<
        RoleManager<IdentityRole<Guid>>>();

    var userManager = services.GetRequiredService<
        UserManager<ApplicationUser>>();

    var dbContext = services.GetRequiredService<
        ApplicationDbContext>();

  

    // First create the required roles.
    await IdentityRoleSeeder.SeedAsync(roleManager);

    // Then provision the initial Store Owner.
    await StoreOwnerSeeder.SeedAsync(
        userManager,
        roleManager,
        dbContext,
        app.Configuration);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("web");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();


public partial class Program { }