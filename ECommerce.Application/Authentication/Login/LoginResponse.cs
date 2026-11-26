using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Authentication.Login
{
    public sealed class LoginResponse
    {
        public Guid UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public IReadOnlyList<string> Roles { get; set; }
            = Array.Empty<string>();

        public bool IsEmailVerified { get; set; }
    }
}
