using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.ResetCustomerPassword
{
    public sealed class ResetPasswordCommand
    {
        public string Email { get; init; } = string.Empty;

        public string Token { get; init; } = string.Empty;

        public string NewPassword { get; init; } = string.Empty;
    }
}
