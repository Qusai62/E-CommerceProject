namespace E_Commerce.Contracts.CustomerAccount
{
    public sealed class RegisterCustomerRequest
    {
        public string? FirstName { get; init; }

        public string? LastName { get; init; }

        public string? Email { get; init; }

        public string? PhoneNumber { get; init; }

        public string? Password { get; init; }
    }
}
