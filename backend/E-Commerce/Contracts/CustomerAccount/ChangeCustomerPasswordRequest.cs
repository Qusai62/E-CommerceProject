namespace E_Commerce.Contracts.CustomerAccount
{
    public sealed class ChangeCustomerPasswordRequest
    {
        public string? CurrentPassword { get; init; }

        public string? NewPassword { get; init; }
    }
}
