using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBOrganizationEntities
    {
        public static void OrganizationEntitiesEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrganizationEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.Name }, "IX_OrganizationEntityName").IsUnique();

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Hierarchy)
                    .IsRequired();

                entity.Property(e => e.WebClass)
                    .IsRequired(false)
                    .HasMaxLength(20);

                entity.Property(e => e.Color)
                    .IsRequired(false)
                    .HasMaxLength(20);

                entity.Property(e => e.Published)
                    .IsRequired();

                entity.Property(e => e.CreatedByPK)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedByPK)
                    .IsRequired(false)
                    .HasMaxLength(128);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .IsRequired(false);

            });
        }
    }
}
