using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBWorkFlowSteps
    {
        public static void WorkFlowStepsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkFlowStep>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.Slug)
                   .IsRequired()
                   .HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Description)
                     .HasMaxLength(8000);

                entity.Property(e => e.StepTypeId)
                     .IsRequired();

                entity.HasOne(d => d.StepType)
                   .WithMany(p => p.Steps)
                   .HasForeignKey(d => d.StepTypeId)
                   .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
