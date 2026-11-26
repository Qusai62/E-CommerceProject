namespace E_Commerce.Contracts.CustomerAccount
{
    public sealed class LoginCustomerRequest
    {
        public string? Email { get; init; }

        public string? Password { get; init; }
    }
}
