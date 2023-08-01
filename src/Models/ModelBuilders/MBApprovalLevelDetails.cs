using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBApprovalLevelDetails
    {
        public static void ApprovalLevelDetailsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApprovalLevelDetail>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.ApprovalLevelId, e.Sequence }, "IX_ApprovalLevel_Sequence").IsUnique();

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.ApprovalLevelId)
                    .IsRequired();

                entity.Property(e => e.Sequence)
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

                entity.HasOne(d => d.ApprovelLevel)
                    .WithMany(p => p.ApprovalLevelDetails)
                    .HasForeignKey(d => d.ApprovalLevelId)
                    .OnDelete(DeleteBehavior.Cascade);

            });
        }
    }
}
