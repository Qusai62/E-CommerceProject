using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.TechnicalSpecification.DeleteTechnicalSpecification
{
    public class DeleteTechnicalSpecificationCommand
    {
        public long ProductId { get; set; }

        public long SpecificationId { get; set; }
    }
}
