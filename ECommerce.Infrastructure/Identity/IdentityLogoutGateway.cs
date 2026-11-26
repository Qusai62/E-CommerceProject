using ECommerce.Application.Authentication.Abstractions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Identity
{
    public sealed class IdentityLogoutGateway : ILogoutGateway
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IdentityLogoutGateway(
            SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
