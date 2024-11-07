using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Service.DTOs.Product;

namespace E_Commerce.Service.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product> GetProductByIdAsync(long id);
    Task<Product> CreateProductAsync(ProductCreateDto productDto);
    Task<Product> UpdateProductAsync(ProductUpdateDto productDto);
    Task<bool> DeleteProductAsync(long id);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(Category category);
    Task<bool> ApplyPromoCodes(IEnumerable<Product> products, long promoId);
    Task<int> GetNumberOfProduct(Category category);
}
