using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBRequestSteps
    {
        public static void RequestStepsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestStep>(entity =>
            {
                entity.HasKey(e => new { e.RequestTypeId, e.Version, e.StepId });

                entity.Property(e => e.IsConditional)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.IsScheduled)
                   .IsRequired()
                   .HasDefaultValue(false);

                entity.Property(e => e.TAT)
                    .IsRequired();

                entity.Property(e => e.EnableEmail)
                   .IsRequired()
                   .HasDefaultValue(true);

                entity.Property(e => e.EnableSMS)
                   .IsRequired()
                   .HasDefaultValue(false);

                entity.Property(e => e.Sequence)
                   .IsRequired();

                entity.HasOne(d => d.WorkFlowStep)
                  .WithMany(p => p.RequestSteps)
                  .HasForeignKey(d => d.StepId);

                entity.HasOne(d => d.RequestType)
                   .WithMany(p => p.RequestSteps)
                   .HasForeignKey(d => new { d.RequestTypeId, d.Version })
                   .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
