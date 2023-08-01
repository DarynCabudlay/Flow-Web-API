using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBRequestStepConditions
    {
        public static void RequestStepConditionsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestStepCondition>(entity =>
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

                entity.Property(e => e.Condition)
                   .IsRequired()
                   .HasMaxLength(1000);

                entity.Property(e => e.ConditionInText)
                  .IsRequired()
                  .HasMaxLength(1000);

                entity.HasOne(d => d.RequestStep)
                  .WithMany(p => p.Conditions)
                  .HasForeignKey(d => new { d.RequestTypeId, d.Version, d.StepId })
                  .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
