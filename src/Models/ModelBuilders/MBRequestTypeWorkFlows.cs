using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBRequestTypeWorkFlows
    {
        public static void RequestTypeWorkFlowsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestTypeWorkFlow>(entity =>
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

                entity.Property(e => e.NextStepRequestTypeId)
                    .IsRequired(false);

                entity.Property(e => e.NextStepVersion)
                    .IsRequired(false);

                entity.Property(e => e.NextStepId)
                    .IsRequired(false);

                entity.Property(e => e.Action)
                   .IsRequired();

                entity.Property(e => e.TypeWhenRejected)
                   .IsRequired(false);

                entity.HasOne(d => d.RequestStep)
                  .WithMany(p => p.WorkFlows)
                  .HasForeignKey(d => new { d.RequestTypeId, d.Version, d.StepId })
                  .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.NextRequestStep)
                  .WithMany(p => p.NextWorkFlows)
                  .HasForeignKey(d => new { d.NextStepRequestTypeId, d.NextStepVersion, d.NextStepId });
            });
        }
    }
}
