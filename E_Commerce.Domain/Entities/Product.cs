﻿using E_Commerce.Domain.Commons;
using E_Commerce.Domain.Enums;

namespace E_Commerce.Domain.Entities;

public class Product : Auditable
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = null;
    public Category Category { get; set; }
    public string OwnerId { get; set; }
    public long? PromocodeId { get; set; }
    public PromoCode PromoCode { get; set; }
}
