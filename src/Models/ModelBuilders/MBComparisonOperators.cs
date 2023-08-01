using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBComparisonOperators
    {
        public static void ComparisonOperatorsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ComparisonOperator>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(100);

                entity.HasIndex(e => e.Name, "IX_ComparisonOperatorName").IsUnique();

                entity.Property(e => e.ApplicableDataTypes)
                    .IsRequired(false)
                    .HasMaxLength(1000);

                entity.Property(e => e.ApplicableFieldTypes)
                    .IsRequired(false)
                    .HasMaxLength(100);

                entity.Property(e => e.TechnicalEquivalent)
                    .IsRequired(true)
                    .HasMaxLength(100);

                entity.Property(e => e.TechnicalEquivalent2)
                   .IsRequired(false)
                   .HasMaxLength(100);

                entity.Property(e => e.UseInMultipleTypes)
                  .IsRequired(true);

            });
        }
    }
}
