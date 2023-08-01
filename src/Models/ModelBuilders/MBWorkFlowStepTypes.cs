using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBWorkFlowStepTypes
    {
        public static void WorkFlowStepTypesEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkFlowStepType>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.Name }, "IX_StepTypeName").IsUnique();

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Color)
                   .IsRequired()
                   .HasMaxLength(50);

                entity.Property(e => e.Published)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.ApplyApprovalPolicyForAssignees)
                    .IsRequired()
                    .HasDefaultValue(false);

            });
        }
    }
}
