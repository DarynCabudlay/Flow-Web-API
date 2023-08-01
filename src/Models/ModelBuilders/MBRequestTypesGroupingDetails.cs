using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBRequestTypesGroupingDetails
    {
        public static void RequestTypesGroupingDetailsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestTypesGroupingDetail>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.RequestTypesGroupingId, e.RequestTypeId, e.Version }, "IX_RequestTypesGroupingRequestTypeVersion").IsUnique();

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.RequestTypesGroupingId)
                    .IsRequired();

                entity.Property(e => e.RequestTypeId)
                   .IsRequired();

                entity.Property(e => e.Version)
                   .IsRequired();

                entity.Property(e => e.Published)
                  .IsRequired();

                entity.Property(e => e.CreatedByPK)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValue(DateTime.Now);

                entity.Property(e => e.ModifiedByPK)
                    .IsRequired(false)
                    .HasMaxLength(128);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.HasOne(d => d.RequestTypesGrouping)
                   .WithMany(p => p.RequestTypesGroupingDetails)
                   .HasForeignKey(d => d.RequestTypesGroupingId)
                   .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.RequestTypes)
                   .WithMany(p => p.RequestTypesGroupingDetails)
                   .HasForeignKey(d => new { d.RequestTypeId, d.Version })
                   .OnDelete(DeleteBehavior.Restrict);

            });
        }
    }
}
