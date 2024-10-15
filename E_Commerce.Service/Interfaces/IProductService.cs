﻿using E_Commerce.Domain.Entities;
using E_Commerce.Service.DTOs.Product;

namespace E_Commerce.Service.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product> GetProductByIdAsync(long id);
    Task<Product> CreateProductAsync(ProductCreateDto productDto);
    Task<Product> UpdateProductAsync(ProductUpdateDto productDto);
    Task<bool> DeleteProductAsync(long id);
}
