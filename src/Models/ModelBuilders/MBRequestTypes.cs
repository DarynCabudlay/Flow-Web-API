using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBRequestTypes
    {
        public static void RequestTypesEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestType>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Version});

                entity.Property(e => e.RequestCategoryId)
                    .IsRequired();

                entity.Property(e => e.Version)
                    .IsRequired();

                entity.Property(e => e.Version2)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Id)
                  .IsRequired();


                entity.Property(e => e.Description)
                    .HasMaxLength(250);

                entity.Property(e => e.Status)
                      .IsRequired();

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

                entity.Property(e => e.DeletionReasonId)
                   .IsRequired(false);

                entity.Property(e => e.DeletionRemarks)
                   .IsRequired(false)
                   .HasMaxLength(8000);

                entity.HasOne(d => d.RequestCategory)
                   .WithMany(p => p.RequestTypes)
                   .HasForeignKey(d => d.RequestCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
