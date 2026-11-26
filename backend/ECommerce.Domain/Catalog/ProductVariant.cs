using ECommerce.Domain.Cart;
using ECommerce.Domain.InventoryManagement;
using ECommerce.Domain.Orders;

namespace ECommerce.Domain.Catalog
{
    public class ProductVariant
    {
        public long ProductVariantId { get; set; }

        public long ProductId { get; set; }

        public string Sku { get; set; } = null!;

        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public Product Product { get; set; } = null!;

        public Inventory Inventory { get; set; } = null!;
        public ICollection<VariantAttribute> Attributes { get; set; }= new List<VariantAttribute>();
        public ICollection<OrderItem> OrderItems { get; set; }
    = new List<OrderItem>();
        public ICollection<CartItem> CartItems { get; set; }= new List<CartItem>();
    }
}
