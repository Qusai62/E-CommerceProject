using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.ChangeCustomerPassword
{
    public sealed class ChangeCustomerPasswordCommand
    {
        public string CurrentPassword { get; init; } = string.Empty;

        public string NewPassword { get; init; } = string.Empty;
    }
}
