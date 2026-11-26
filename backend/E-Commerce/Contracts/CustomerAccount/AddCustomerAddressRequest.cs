namespace E_Commerce.Contracts.CustomerAccount
{
    public sealed record AddCustomerAddressRequest(
    string RecipientName,
    string ContactNumber,
    string City,
    string Neighborhood,
    string Street,
    string BuildingInfo,
    string? AdditionalAddressInfo
);
}
