using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.CreateInStoreSale
{
    public static class CreateInStoreSaleFingerprint
    {
        public static string Compute(
            IReadOnlyList<InStoreSaleItem> items)
        {
            var builder = new StringBuilder();

            builder.Append("POS-V1|");
            builder.Append(items.Count).Append('|');

            foreach (var item in items)
            {
                var name = item.ItemName.Trim();

                builder.Append(name.Length)
                    .Append(':')
                    .Append(name);

                builder.Append('|')
                    .Append(item.UnitPrice.ToString(
                        "F2",
                        CultureInfo.InvariantCulture));

                builder.Append('|')
                    .Append(item.Quantity)
                    .Append('|');
            }

            byte[] bytes = Encoding.UTF8.GetBytes(
                builder.ToString());

            byte[] hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }
    }
}
