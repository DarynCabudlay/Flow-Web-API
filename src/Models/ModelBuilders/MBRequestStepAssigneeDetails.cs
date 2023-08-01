using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBRequestStepAssigneeDetails
    {
        public static void RequestStepAssigneeDetailsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestStepAssigneeDetail>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.RequestStepAssigneeId, e.Assignee }, "IX_RequestStepAssignee").IsUnique();

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.RequestStepAssigneeId)
                    .IsRequired();

                entity.Property(e => e.Assignee)
                   .IsRequired()
                   .HasMaxLength(128);

                entity.HasOne(d => d.RequestStepAssignee)
                  .WithMany(p => p.RequestStepAssigneeDetails)
                  .HasForeignKey(d => d.RequestStepAssigneeId)
                  .OnDelete(DeleteBehavior.Cascade);

            });
        }
    }
}
