using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBApprovalLevels
    {
        public static void ApprovalLevelsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApprovalLevel>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.Name }, "IX_ApprovalLevelName").IsUnique();

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OrganizationEntityId)
                   .IsRequired();

                entity.HasOne(d => d.OrganizationEntity)
                    .WithOne(p => p.ApprovalLevel)
                    .HasForeignKey<ApprovalLevel>(d => d.OrganizationEntityId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_OrganizationEntityApprovalLevel");

            });
        }
    }
}
