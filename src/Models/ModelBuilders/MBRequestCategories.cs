using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBRequestCategories
    {
        public static void RequestCategoriesEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestCategory>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.Name }, "IX_RequestCategoryName").IsUnique();

                entity.Property(e => e.Id)
                    .UseIdentityColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Published)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedByPK)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValue(DateTime.Now);

                entity.Property(e => e.ModifiedByPK)
                    .HasMaxLength(128)
                    .IsRequired(false);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .IsRequired(false);
            });
        }
    }
}
