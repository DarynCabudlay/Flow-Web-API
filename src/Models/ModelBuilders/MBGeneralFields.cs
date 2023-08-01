using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBGeneralFields
    {
        public static void GeneralFieldsEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GeneralField>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .UseIdentityColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Slug)
                   .IsRequired()
                   .HasMaxLength(100);

                entity.Property(e => e.FieldType)
                   .IsRequired();

                entity.Property(e => e.DataType)
                  .HasMaxLength(200)
                  .IsRequired(false);

                entity.Property(e => e.DecimalDigit)
                  .IsRequired(false);

                entity.Property(e => e.Description)
                  .HasMaxLength(250)
                  .IsRequired(false);

                entity.Property(e => e.MinLength)
                  .IsRequired(false);

                entity.Property(e => e.MaxLength)
                   .IsRequired(false);

                entity.Property(e => e.MinNumber)
                   .IsRequired(false);

                entity.Property(e => e.MaxNumber)
                   .IsRequired(false);

                entity.Property(e => e.CurrencySymbol)
                  .IsRequired(false)
                  .HasMaxLength(3);

                entity.Property(e => e.LOVs)
                   .IsRequired(false);

            });
        }
    }
}
