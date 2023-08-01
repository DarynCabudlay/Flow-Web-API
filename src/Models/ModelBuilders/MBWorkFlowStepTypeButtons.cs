using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBWorkFlowStepTypeButtons
    {
        public static void WorkFlowStepTypeButtonsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkFlowStepTypeButton>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.StepTypeId)
                    .IsRequired();

                entity.Property(e => e.ActionButtonId)
                    .IsRequired();

                entity.Property(e => e.IsActionToNextStep)
                    .IsRequired();

                entity.HasOne(d => d.StepType)
                   .WithMany(p => p.StepsButtons)
                   .HasForeignKey(d => d.StepTypeId)
                   .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ActionButton)
                 .WithMany(p => p.ActionButtons)
                 .HasForeignKey(d => d.ActionButtonId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
