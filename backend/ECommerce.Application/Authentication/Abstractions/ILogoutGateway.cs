using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Authentication.Abstractions
{
    public interface ILogoutGateway
    {
        Task LogoutAsync();
    }
}
