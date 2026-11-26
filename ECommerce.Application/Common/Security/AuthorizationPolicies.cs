using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Common.Security
{
    public static class AuthorizationPolicies
    {
        public const string CustomerOnly = "CustomerOnly";
        public const string StoreOwnerOnly = "StoreOwnerOnly";
    }
}
