using System.ComponentModel.DataAnnotations;

namespace WingtipToys.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Username { get; set; } = "";
        [DataType(DataType.Currency)]
        public decimal Total { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string State { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public string Country { get; set; } = "";
        public string Phone { get; set; } = "";
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = "";
        public string? PaymentTransactionId { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}
