using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.CreateInStoreSale
{
  
        public interface ICreateInStoreSaleRepository
        {
            Task<InStoreSale?> GetBySubmissionIdAsync(
                Guid submissionId,
                CancellationToken cancellationToken = default);

            Task<bool> TryAddAsync(
                InStoreSale sale,
                CancellationToken cancellationToken = default);
        }
    
}
