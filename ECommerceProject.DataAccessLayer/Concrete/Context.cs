using ECommerceProject.EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DataAccessLayer.Concrete
{
    public class Context : IdentityDbContext<AppUser,AppRole,int>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;initial catalog = ECommerceProjectDb;User Id=sa; Password=sa1234SA; MultipleActiveResultSets=true; Trust Server Certificate=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder)
                ;
            /** Alttaki Sale İle İlgili İşlemler Yapılmazsa Migration İşleminde Problem Çıkıyor **/

            // AppUser ile Sales arasındaki ilişki için Cascade Delete kaldırılıyor
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.AppUser)
                .WithMany(u => u.Sales)
                .HasForeignKey(s => s.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Adress ile Sales arasındaki ilişki için Cascade Delete kaldırılıyor
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Adress)
                .WithMany(a => a.Sales)
                .HasForeignKey(s => s.AdressId)
                .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<Adress> Adresses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
    }
}
