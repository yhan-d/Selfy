using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Selfy.Core.Entities;
using Selfy.Data.Identity;

namespace Selfy.Data.EntityFramework
{
    public sealed class MyContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {

        public MyContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(x => x.Name).HasMaxLength(50).IsRequired(false);
                entity.Property(x => x.Surname).HasMaxLength(50).IsRequired(false);
                entity.Property(x => x.RegisterDate).HasColumnType("datetime");
            });

            /* builder.Entity<ApplicationRole>(entity =>
             {
                 entity.Property(x => x.Description).HasMaxLength(120).IsRequired(false);
             }); */

            builder.Entity<Product>(entity =>
            {
                entity.Property(x => x.Name).IsRequired().HasMaxLength(50);
                entity.HasMany(x => x.Requests)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId);
            });

            builder.Entity<Service>(entity =>
            {
                entity.Property(x => x.Name).IsRequired().HasMaxLength(150);
                entity.Property(x => x.UnitPrice).IsRequired();
            });

            builder.Entity<Request>(entity =>
            {
                entity.HasOne<ApplicationUser>().WithMany()
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(x => x.Name);
                entity.Property(x => x.Surname);
                entity.Property(x => x.Address);
                entity.Property(x => x.Email);
                entity.Property(x => x.TextOfRequest);
                entity.Property(x => x.Status);

                entity.HasOne(x => x.Product)
              .WithMany(x => x.Requests)
              .HasForeignKey(x => x.ProductId);
                entity.HasOne(x => x.Operation)
                    .WithOne(y => y.Request)
                    .HasForeignKey<Operation>(x => x.RequestId);
            });

            builder.Entity<Operation>(entity =>
            {
                entity.Property(x => x.UserId);
                entity.HasOne(x => x.Request)
                    .WithOne(x => x.Operation)
                    .HasForeignKey<Operation>(x => x.RequestId);
            });


        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Operation> Operations { get; set; }


    }

}

