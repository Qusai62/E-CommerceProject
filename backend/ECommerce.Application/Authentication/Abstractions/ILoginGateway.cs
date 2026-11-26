using ECommerce.Application.Authentication.Login;
using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Authentication.Abstractions
{
    public interface ILoginGateway
    {
        Task<Result<LoginResponse>> LoginAsync(
            LoginCommand command,
            CancellationToken cancellationToken = default);
    }
}
