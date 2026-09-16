using SmartAutoCashSystem.Data.Repositories;
using SmartAutoCashSystem.Models;

namespace SmartAutoCashSystem.Services;

public class ProductService
{
    private readonly ProductRepository _repo;

    public ProductService(ProductRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyCollection<Product>> GetActiveAsync() => _repo.GetActiveAsync();

    public Task<Product?> GetByBarcodeAsync(string barcode) => _repo.GetByBarcodeAsync(barcode);
}

