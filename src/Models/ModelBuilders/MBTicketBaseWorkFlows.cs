using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBTicketBaseWorkFlows
    {
        public static void TicketBaseWorkFlowsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketBaseWorkFlow>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.TicketId, e.StepId }, "IX_TicketAndStepId").IsUnique();

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.StepId)
                   .IsRequired();

                entity.Property(e => e.StepTypeId)
                  .IsRequired();

                entity.Property(e => e.AssigneeId)
                  .IsRequired();

                entity.Property(e => e.AssigneeType)
                  .IsRequired();

                entity.Property(e => e.RequiredToExecute)
                  .IsRequired();

                entity.Property(e => e.TAT)
                  .IsRequired();

                entity.Property(e => e.ApplicableButtons)
                  .IsRequired();

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.TicketBaseWorkFlows)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);

            });
        }
    }
}
