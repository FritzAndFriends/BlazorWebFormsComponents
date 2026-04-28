using System.Text.Json;
using Microsoft.AspNetCore.Http;
using WingtipToys.Models;

namespace WingtipToys.Services;

public sealed class CartService
{
    private const string CartSessionKey = "Wingtip.Cart";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CatalogService _catalogService;

    public CartService(IHttpContextAccessor httpContextAccessor, CatalogService catalogService)
    {
        _httpContextAccessor = httpContextAccessor;
        _catalogService = catalogService;
    }

    public IReadOnlyList<CartLine> GetItems()
    {
        return LoadRecords()
            .Select(record => new CartLine(_catalogService.GetProduct(record.ProductId), record.Quantity))
            .Where(line => line.Product is not null)
            .Select(line => new CartLine(line.Product!, line.Quantity))
            .ToList();
    }

    public int GetCount() => GetItems().Sum(item => item.Quantity);

    public decimal GetTotal()
    {
        return GetItems().Sum(item => (decimal)(item.Product.UnitPrice ?? 0) * item.Quantity);
    }

    public void AddToCart(int productId)
    {
        var records = LoadRecords();
        var existing = records.FirstOrDefault(record => record.ProductId == productId);
        if (existing is null)
        {
            records.Add(new CartRecord { ProductId = productId, Quantity = 1 });
        }
        else
        {
            existing.Quantity += 1;
        }

        SaveRecords(records);
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        if (quantity <= 0)
        {
            Remove(productId);
            return;
        }

        var records = LoadRecords();
        var existing = records.FirstOrDefault(record => record.ProductId == productId);
        if (existing is null)
        {
            return;
        }

        existing.Quantity = quantity;
        SaveRecords(records);
    }

    public void Remove(int productId)
    {
        var records = LoadRecords();
        records.RemoveAll(record => record.ProductId == productId);
        SaveRecords(records);
    }

    private List<CartRecord> LoadRecords()
    {
        var json = _httpContextAccessor.HttpContext?.Session.GetString(CartSessionKey);
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<CartRecord>>(json, JsonOptions) ?? [];
    }

    private void SaveRecords(List<CartRecord> records)
    {
        _httpContextAccessor.HttpContext?.Session.SetString(CartSessionKey, JsonSerializer.Serialize(records, JsonOptions));
    }

    public sealed record CartLine(Product Product, int Quantity);

    private sealed class CartRecord
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
