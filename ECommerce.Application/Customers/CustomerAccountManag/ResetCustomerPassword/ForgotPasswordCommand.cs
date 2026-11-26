using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.ResetCustomerPassword
{
    public sealed class ForgotPasswordCommand
    {
        public string Email { get; init; } = string.Empty;
    }
}
