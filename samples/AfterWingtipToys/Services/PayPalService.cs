using WingtipToys.Models;

namespace WingtipToys.Services
{
    public interface IPayPalService
    {
        Task<(string RedirectUrl, string Token)> SetExpressCheckoutAsync(Order order);
        Task<(string PayerEmail, string PayerId)> GetExpressCheckoutDetailsAsync(string token);
        Task<string> DoExpressCheckoutPaymentAsync(string token, string payerId, Order order);
    }

    public class MockPayPalService : IPayPalService
    {
        public Task<(string RedirectUrl, string Token)> SetExpressCheckoutAsync(Order order)
        {
            var token = "EC-" + Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
            var redirectUrl = $"/CheckoutReview?token={token}";
            return Task.FromResult((redirectUrl, token));
        }

        public Task<(string PayerEmail, string PayerId)> GetExpressCheckoutDetailsAsync(string token)
        {
            return Task.FromResult(("demo@wingtiptoys.com", "PAYER-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()));
        }

        public Task<string> DoExpressCheckoutPaymentAsync(string token, string payerId, Order order)
        {
            var transactionId = "FAKE-TXN-" + Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
            return Task.FromResult(transactionId);
        }
    }
}
