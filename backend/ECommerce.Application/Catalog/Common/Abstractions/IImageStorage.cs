using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.Common.Abstractions
{
    public interface IImageStorage
    {
        Task<string> SaveAsync(
        Stream content,
        string extension,
        CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string storageKey,
            CancellationToken cancellationToken = default);
    }
}
