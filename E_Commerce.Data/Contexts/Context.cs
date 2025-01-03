﻿using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Data.Contexts;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options) { }
    public DbSet<Product> Produts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<PromoCode> PromoCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.PromoCode)
            .WithMany(pc => pc.Products)
            .HasForeignKey(p => p.PromocodeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
