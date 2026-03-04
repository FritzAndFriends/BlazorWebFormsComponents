using System.ComponentModel.DataAnnotations;

namespace WingtipToys.Models;

public class Order
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public string? Username { get; set; }

    [Required, StringLength(160)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(160)]
    public string LastName { get; set; } = string.Empty;

    [Required, StringLength(70)]
    public string Address { get; set; } = string.Empty;

    [Required, StringLength(40)]
    public string City { get; set; } = string.Empty;

    [Required, StringLength(40)]
    public string State { get; set; } = string.Empty;

    [Required, StringLength(10)]
    public string PostalCode { get; set; } = string.Empty;

    [Required, StringLength(40)]
    public string Country { get; set; } = string.Empty;

    [StringLength(24)]
    public string? Phone { get; set; }

    [Required, StringLength(160), DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [ScaffoldColumn(false)]
    public decimal Total { get; set; }

    [ScaffoldColumn(false)]
    public string? PaymentTransactionId { get; set; }

    [ScaffoldColumn(false)]
    public bool HasBeenShipped { get; set; }

    public List<OrderDetail> OrderDetails { get; set; } = new();
}
