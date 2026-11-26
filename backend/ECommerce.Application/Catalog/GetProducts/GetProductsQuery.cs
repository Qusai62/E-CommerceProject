using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.GetProducts
{
    public class GetProductsQuery
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public string? Search { get; set; }

        public long? BrandId { get; set; }
        public long? CategoryId { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string SortBy { get; set; } = "newest";
        public string SortDirection { get; set; } = "desc";
    }
}
