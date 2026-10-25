using ECommerce.Application.Catalog.Abstractions;
using ECommerce.Application.Catalog.Common.Abstractions;
using ECommerce.Application.Catalog.Product.CreateProduct;
using ECommerce.Application.Catalog.Product.GetProductById;
using ECommerce.Application.Catalog.Product.UpdateProduct;
using ECommerce.Application.Catalog.ProductImage.AddProductImage;
using ECommerce.Application.Catalog.ProductImage.UpdateProductImage;
using ECommerce.Application.Catalog.ProductImageApplication.DeleteProductImage;
using ECommerce.Application.Catalog.ProductVariant.CreateProductVariant;
using ECommerce.Application.Catalog.ProductVariant.UpdateProductVariant;
using ECommerce.Application.Catalog.TechnicalSpecification.AddTechnicalSpecification;
using ECommerce.Application.Catalog.TechnicalSpecification.DeleteTechnicalSpecification;
using ECommerce.Application.Catalog.TechnicalSpecification.UpdateTechnicalSpecification;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.Persistence;
using ECommerce.Infrastructure.Persistence.Repositories;
using ECommerce.Infrastructure.Storage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(//This tells Entity Framework Core to use the SQL Server database provider.
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddDataProtection();
builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

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

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program { }