using AutoMapper;
using E_Commerce.Data.Repositories;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Service.DTOs.Product;
using E_Commerce.Service.Exceptions;
using E_Commerce.Service.Interfaces;
using Microsoft.AspNetCore.Http;

namespace E_Commerce.Service.Services;

public class ProductService : IProductService
{
    private readonly IGenericRepository<Product> _genericRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public ProductService(IGenericRepository<Product> genericRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _genericRepository = genericRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Product> CreateProductAsync(ProductCreateDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);

        var claims = _httpContextAccessor.HttpContext.User.Claims;

        var userIdClaim = claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        product.OwnerId = userIdClaim;

        if (productDto.ImageUrl != null)
        {
            var imagePath = Path.Combine("wwwroot/images", productDto.ImageUrl.FileName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await productDto.ImageUrl.CopyToAsync(stream);
            }

            product.ImageUrl = $"/images/{productDto.ImageUrl.FileName}";
        }

        await _genericRepository.CreateAsync(product);
        await _genericRepository.SaveChangesAsync(); // Awaiting the save changes call
        return product;
    }

    public async Task<bool> DeleteProductAsync(long id)
    {
        var product = await _genericRepository.GetAsync(x => x.Id == id);
        if (product == null)
            throw new CustomException("Product not found", 404);

        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            var imagePath = Path.Combine("wwwroot", product.ImageUrl.TrimStart('/'));
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }

        await _genericRepository.DeleteAsync(id);
        await _genericRepository.SaveChangesAsync(); // Awaiting the save changes call
        return true;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        var products = _genericRepository.GetAll(null!);
        return await Task.FromResult(products);
    }

    public async Task<int> GetNumberOfProduct(Category category)
    {
        var products = _genericRepository.GetAll(x => x.Category == category);
        return Task.FromResult(products.Count()).Result;
    }

    public async Task<Product> GetProductByIdAsync(long id)
    {
        var product = await _genericRepository.GetAsync(x => x.Id == id);
        if (product == null)
            throw new CustomException("Product not found", 404);
        return product;
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(Category category)
    {
        var products = _genericRepository.GetAll(x => x.Category == category);
        return await Task.FromResult(products);
    }

    public async Task<Product> UpdateProductAsync(ProductUpdateDto productDto)
    {
        var existingProduct = await _genericRepository.GetAsync(x => x.Id == productDto.Id);
        if (existingProduct == null)
            throw new CustomException("Product not found", 404);

        _mapper.Map(productDto, existingProduct);

        if (productDto.ImageUrl != null)
        {
            // Delete the old image if it exists
            if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
            {
                var oldImagePath = Path.Combine("wwwroot", existingProduct.ImageUrl.TrimStart('/'));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            // Save the new image
            var newImagePath = Path.Combine("wwwroot/images", productDto.ImageUrl.FileName);
            using (var stream = new FileStream(newImagePath, FileMode.Create))
            {
                await productDto.ImageUrl.CopyToAsync(stream);
            }

            existingProduct.ImageUrl = $"/images/{productDto.ImageUrl.FileName}";
        }



        _genericRepository.Update(existingProduct);
        await _genericRepository.SaveChangesAsync(); 
        return existingProduct;
    }

    public async Task<bool> ApplyPromoCodes(IEnumerable<Product> products, long promoId)
    {
        var productsIds = products.Select(x => x.Id).ToList();

        foreach (var product in products)
        {
            product.PromocodeId = promoId;
            _genericRepository.Update(product);
            await _genericRepository.SaveChangesAsync();
        }


        return true;
    }
}
