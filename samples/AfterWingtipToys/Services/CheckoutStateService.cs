using WingtipToys.Models;

namespace WingtipToys.Services
{
    public class CheckoutStateService
    {
        private Order? _currentOrder;
        private string? _paymentTransactionId;
        private bool _isConfirmed;

        public async Task CreateOrderFromCartAsync(CartStateService cart, string username)
        {
            var cartItems = await cart.GetCartItemsAsync();
            var total = await cart.GetTotalAsync();

            _currentOrder = new Order
            {
                OrderDate = DateTime.UtcNow,
                Username = username,
                Total = total,
                FirstName = "Demo",
                LastName = "User",
                Address = "123 Main St",
                City = "Redmond",
                State = "WA",
                PostalCode = "98052",
                Country = "US",
                Phone = "555-0100",
                Email = "demo@wingtiptoys.com",
                OrderDetails = cartItems.Select(ci => new OrderDetail
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product?.UnitPrice ?? 0,
                    Product = ci.Product
                }).ToList()
            };

            _isConfirmed = false;
            _paymentTransactionId = null;
        }

        public Order? GetCurrentOrder() => _currentOrder;

        public void SetPaymentConfirmed(string transactionId)
        {
            _paymentTransactionId = transactionId;
            _isConfirmed = true;
            if (_currentOrder != null)
            {
                _currentOrder.PaymentTransactionId = transactionId;
            }
        }

        public string? GetTransactionId() => _paymentTransactionId;

        public bool IsConfirmed => _isConfirmed;

        public void ClearCheckoutState()
        {
            _currentOrder = null;
            _paymentTransactionId = null;
            _isConfirmed = false;
        }
    }
}
