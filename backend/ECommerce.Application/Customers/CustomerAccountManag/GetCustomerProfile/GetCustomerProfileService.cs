using ECommerce.Application.Common.Results;
using ECommerce.Application.Customers.CustomerAccountManag.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Customers.CustomerAccountManag.GetCustomerProfile
{
    public class GetCustomerProfileService
    {
        private readonly ICustomerProfileReadRepository _repository;

        public GetCustomerProfileService(
            ICustomerProfileReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<GetCustomerProfileResponse>> ExecuteAsync(
            Guid applicationUserId,
            CancellationToken cancellationToken = default)
        {
            var profile = await _repository.GetByApplicationUserIdAsync(
                applicationUserId,
                cancellationToken);

            if (profile is null)
            {
                return Result<GetCustomerProfileResponse>.Failure(
                    new List<Error>
                    {
                    new Error(
                        "Customer.ProfileNotFound",
                        "Customer profile was not found.",
                        string.Empty)
                    });
            }

            return Result<GetCustomerProfileResponse>.Success(profile);
        }
    }
}
