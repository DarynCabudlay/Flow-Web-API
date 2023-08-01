using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBRequestStepConditionDetails
    {
        public static void RequestStepConditionDetailsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestStepConditionDetail>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.RequestStepConditionId)
                    .IsRequired();

                entity.Property(e => e.Source)
                    .IsRequired();

                entity.Property(e => e.ReferenceDetail)
                    .IsRequired();

                entity.Property(e => e.ComparisonOperatorId)
                   .IsRequired();

                entity.Property(e => e.LogicalOperator)
                   .IsRequired(false)
                   .HasMaxLength(10);

                entity.Property(e => e.Value)
                   .IsRequired();

                entity.Property(e => e.Value2)
                   .IsRequired(false);

                entity.Property(e => e.Sequence)
                  .IsRequired();

                entity.Property(e => e.Published)
                .IsRequired();

                entity.HasOne(d => d.ConditionHeader)
                  .WithMany(p => p.ConditionDetails)
                  .HasForeignKey(d => new { d.RequestStepConditionId })
                  .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ComparisonOperator)
                .WithMany(p => p.ConditionDetails)
                .HasForeignKey(d => new { d.ComparisonOperatorId })
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.SourceInfo)
                 .WithMany(p => p.ConditionSource)
                 .HasForeignKey(d => d.Source)
                 .OnDelete(DeleteBehavior.Restrict);

            });
        }
    }
}
