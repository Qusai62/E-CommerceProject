using ECommerce.Application.Authentication.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Authentication.Logout
{
    public sealed class LogoutService
    {
        private readonly ILogoutGateway _gateway;

        public LogoutService(ILogoutGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task ExecuteAsync()
        {
            await _gateway.LogoutAsync();
        }
    }
}
