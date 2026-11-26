using ECommerce.Application.InStoreSales.CreateInStoreSale;
using ECommerce.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.InStoreSales
{
    public sealed class CreateInStoreSaleRepository
    : ICreateInStoreSaleRepository
    {
        private readonly ApplicationDbContext _context;

        public CreateInStoreSaleRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InStoreSale?> GetBySubmissionIdAsync(
            Guid submissionId,
            CancellationToken cancellationToken = default)
        {
            return await _context.InStoreSales
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    x => x.SubmissionId == submissionId,
                    cancellationToken);
        }

        public async Task<bool> TryAddAsync(
            InStoreSale sale,
            CancellationToken cancellationToken = default)
        {
            await _context.InStoreSales.AddAsync(
                sale,
                cancellationToken);

            try
            {
                await _context.SaveChangesAsync(
                    cancellationToken);

                return true;
            }
            catch (DbUpdateException ex)
                when (IsSubmissionIdConflict(ex))
            {
                // Stop tracking the failed insert graph.
                foreach (var entry in _context.ChangeTracker
                    .Entries()
                    .Where(e =>
                        ReferenceEquals(e.Entity, sale) ||
                        sale.Items.Any(i =>
                            ReferenceEquals(e.Entity, i)))
                    .ToList())
                {
                    entry.State = EntityState.Detached;
                }

                return false;
            }
        }

        private static bool IsSubmissionIdConflict(
            DbUpdateException exception)
        {
            if (exception.InnerException is not SqlException sql)
            {
                return false;
            }

            const string indexName =
                "IX_InStoreSales_SubmissionId";

            return sql.Errors
                .Cast<SqlError>()
                .Any(error =>
                    (error.Number == 2601 ||
                     error.Number == 2627) &&
                    error.Message.Contains(
                        indexName,
                        StringComparison.OrdinalIgnoreCase));
        }
    }
}
