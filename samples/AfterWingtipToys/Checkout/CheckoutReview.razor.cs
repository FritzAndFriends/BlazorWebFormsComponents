using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;
using WingtipToys.Services;

namespace WingtipToys.Checkout
{
    public partial class CheckoutReview : ComponentBase
    {
        [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

        private Order? _order;

        protected override void OnInitialized()
        {
            _order = CheckoutState.GetCurrentOrder();
        }

        private async Task CompleteOrder_Click()
        {
            if (_order == null) return;

            var (_, token) = await PayPalService.SetExpressCheckoutAsync(_order);
            var (_, payerId) = await PayPalService.GetExpressCheckoutDetailsAsync(token);
            var transactionId = await PayPalService.DoExpressCheckoutPaymentAsync(token, payerId, _order);

            CheckoutState.SetPaymentConfirmed(transactionId);

            using var context = DbFactory.CreateDbContext();
            var orderToSave = new Order
            {
                OrderDate = _order.OrderDate,
                Username = _order.Username,
                Total = _order.Total,
                FirstName = _order.FirstName,
                LastName = _order.LastName,
                Address = _order.Address,
                City = _order.City,
                State = _order.State,
                PostalCode = _order.PostalCode,
                Country = _order.Country,
                Phone = _order.Phone,
                Email = _order.Email,
                PaymentTransactionId = transactionId,
                OrderDetails = _order.OrderDetails.Select(d => new OrderDetail
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice
                }).ToList()
            };

            context.Orders.Add(orderToSave);
            await context.SaveChangesAsync();

            await CartService.EmptyCartAsync();

            Navigation.NavigateTo("/CheckoutComplete");
        }
    }
}
