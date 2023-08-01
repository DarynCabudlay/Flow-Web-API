using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBReasons
    {
        public static void ReasonsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reason>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.Name, e.ReasonTypeId }, "IX_ReasonNameAndReasonType").IsUnique();

                entity.Property(e => e.Id)
                    .UseIdentityColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ReasonTypeId)
                    .IsRequired();

                entity.Property(e => e.Published)
                    .IsRequired();

                entity.Property(e => e.CreatedByPK)
                    .HasMaxLength(128)
                    .IsRequired();

                entity.Property(e => e.CreatedDate)
                    .IsRequired();

                entity.Property(e => e.ModifiedByPK)
                    .HasMaxLength(128);

                entity.Property(e => e.ModifiedDate)
                    .IsRequired(false);

                entity.HasOne(d => d.ReasonType)
                   .WithMany(p => p.Reasons)
                   .HasForeignKey(d => d.ReasonTypeId);

            });
        }
    }
}
