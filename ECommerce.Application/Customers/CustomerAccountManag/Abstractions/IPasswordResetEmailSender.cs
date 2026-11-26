using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.Abstractions
{
    public interface IPasswordResetEmailSender
    {
        Task SendResetLinkAsync(
            string recipientEmail,
            string resetLink,
            CancellationToken cancellationToken = default);
    }
}
