using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBRequestStepAssignees
    {
        public static void RequestStepAssigneesEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestStepAssignee>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.RequestTypeId)
                    .IsRequired();

                entity.Property(e => e.Version)
                    .IsRequired();

                entity.Property(e => e.StepId)
                    .IsRequired();

                entity.Property(e => e.IsConditional)
                   .IsRequired()
                   .HasDefaultValue(false);

                entity.Property(e => e.AssigneeType)
                    .IsRequired();

                entity.Property(e => e.RequiredAssigneeToExecute)
                   .IsRequired();

                entity.Property(e => e.Field1)
                    .IsRequired(false);

                entity.Property(e => e.Value1)
                    .IsRequired(false)
                    .HasMaxLength(8000);

                entity.Property(e => e.Field2)
                    .IsRequired(false);

                entity.Property(e => e.Value2)
                    .IsRequired(false)
                    .HasMaxLength(8000);

                entity.Property(e => e.Field3)
                   .IsRequired(false);

                entity.Property(e => e.Value3)
                    .IsRequired(false)
                    .HasMaxLength(8000);

                entity.HasOne(d => d.RequestStep)
                  .WithMany(p => p.Assignees)
                  .HasForeignKey(d => new { d.RequestTypeId, d.Version, d.StepId })
                  .OnDelete(DeleteBehavior.Cascade);

            });
        }
    }
}
