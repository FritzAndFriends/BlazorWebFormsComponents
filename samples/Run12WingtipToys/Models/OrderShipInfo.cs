namespace WingtipToys.Models;

/// <summary>
/// Stub model for checkout order shipping info, used by the DetailsView in CheckoutReview.
/// </summary>
public class OrderShipInfo
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public decimal Total { get; set; }
}
