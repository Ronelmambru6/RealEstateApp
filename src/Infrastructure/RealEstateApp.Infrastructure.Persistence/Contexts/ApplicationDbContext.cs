using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;
using System.Reflection.Emit;

namespace RealEstateApp.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Property> Properties { get; set; }
    public DbSet<PropertyType> PropertyTypes { get; set; }
    public DbSet<SaleType> SaleTypes { get; set; }
    public DbSet<Improvement> Improvements { get; set; }
    public DbSet<PropertyImage> PropertyImages { get; set; }
    public DbSet<PropertyImprovement> PropertyImprovements { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<FavoriteProperty> FavoriteProperties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Property
        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => p.Code).IsUnique();
            entity.Property(p => p.Code).HasMaxLength(6).IsRequired();
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(p => p.Description).IsRequired();
            entity.Property(p => p.AgentId).IsRequired();
        });

        // 2. PropertyType (Borrado en cascada hacia Properties según doc funcional)
        modelBuilder.Entity<PropertyType>(entity =>
        {
            entity.HasKey(pt => pt.Id);
            entity.HasIndex(pt => pt.Name).IsUnique();
            entity.Property(pt => pt.Name).HasMaxLength(100).IsRequired();
            entity.Property(pt => pt.Description).IsRequired();

            entity.HasMany(pt => pt.Properties)
                  .WithOne(p => p.PropertyType)
                  .HasForeignKey(p => p.PropertyTypeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 3. SaleType (Borrado en cascada hacia Properties según doc funcional)
        modelBuilder.Entity<SaleType>(entity =>
        {
            entity.HasKey(st => st.Id);
            entity.HasIndex(st => st.Name).IsUnique();
            entity.Property(st => st.Name).HasMaxLength(100).IsRequired();
            entity.Property(st => st.Description).IsRequired();

            entity.HasMany(st => st.Properties)
                  .WithOne(p => p.SaleType)
                  .HasForeignKey(p => p.SaleTypeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 4. Improvement
        modelBuilder.Entity<Improvement>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.HasIndex(i => i.Name).IsUnique();
            entity.Property(i => i.Name).HasMaxLength(100).IsRequired();
            entity.Property(i => i.Description).IsRequired();
        });

        // 5. PropertyImprovement (Muchos a Muchos: borrar Mejora solo borra la asociación)
        modelBuilder.Entity<PropertyImprovement>(entity =>
        {
            entity.HasKey(pi => new { pi.PropertyId, pi.ImprovementId });

            entity.HasOne(pi => pi.Property)
                  .WithMany(p => p.PropertyImprovements)
                  .HasForeignKey(pi => pi.PropertyId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pi => pi.Improvement)
                  .WithMany(i => i.PropertyImprovements)
                  .HasForeignKey(pi => pi.ImprovementId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 6. PropertyImage
        modelBuilder.Entity<PropertyImage>(entity =>
        {
            entity.HasKey(img => img.Id);
            entity.Property(img => img.ImageUrl).IsRequired();

            entity.HasOne(img => img.Property)
                  .WithMany(p => p.Images)
                  .HasForeignKey(img => img.PropertyId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 7. Offer
        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(o => o.ClientId).IsRequired();

            entity.HasOne(o => o.Property)
                  .WithMany(p => p.Offers)
                  .HasForeignKey(o => o.PropertyId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 8. ChatMessage
        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Content).IsRequired();
            entity.Property(m => m.ClientId).IsRequired();
            entity.Property(m => m.SenderId).IsRequired();

            entity.HasOne(m => m.Property)
                  .WithMany(p => p.ChatMessages)
                  .HasForeignKey(m => m.PropertyId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 9. FavoriteProperty (Un cliente no puede agregar la misma propiedad 2 veces)
        modelBuilder.Entity<FavoriteProperty>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.HasIndex(f => new { f.ClientId, f.PropertyId }).IsUnique();

            entity.HasOne(f => f.Property)
                  .WithMany(p => p.FavoriteProperties)
                  .HasForeignKey(f => f.PropertyId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}