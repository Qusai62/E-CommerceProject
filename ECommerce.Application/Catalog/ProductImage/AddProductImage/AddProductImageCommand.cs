using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.ProductImage.AddProductImage
{
    public class AddProductImageCommand
    {
        public long ProductId { get; set; }

        public Stream FileContent { get; set; } = Stream.Null;

        public string FileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public string? AltText { get; set; }

        public int SortOrder { get; set; }
    }
}
