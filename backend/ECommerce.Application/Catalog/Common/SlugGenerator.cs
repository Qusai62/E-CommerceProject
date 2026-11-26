using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.Common
{
    public static class SlugGenerator
    {
        public static string Generate(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var builder = new StringBuilder();
            var previousWasSeparator = false;

            foreach (var character in name.ToLowerInvariant())
            {
                if (character is >= 'a' and <= 'z'
                    or >= '0' and <= '9')
                {
                    builder.Append(character);
                    previousWasSeparator = false;
                }
                else if (!previousWasSeparator && builder.Length > 0)
                {
                    builder.Append('-');
                    previousWasSeparator = true;
                }
            }

            var slug = builder.ToString().TrimEnd('-');

            if (string.IsNullOrEmpty(slug))
                slug = "product";

            return slug.Length <= 200
                ? slug
                : slug[..200].TrimEnd('-');
        }
    }
}
