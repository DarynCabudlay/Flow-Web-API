using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBHolidayDates
    {
        public static void HolidayDatesEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HolidayDate>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .UseIdentityColumn();

                entity.Property(e => e.HolidayId)
                    .IsRequired();

                entity.Property(e => e.HolidayTypeId)
                    .IsRequired();

                entity.Property(e => e.Date)
                   .IsRequired()
                   .HasColumnType("datetime");

                entity.Property(e => e.Published)
                   .IsRequired();

                entity.Property(e => e.CreatedByPK)
                   .IsRequired()
                   .HasMaxLength(128);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValue(DateTime.Now);

                entity.Property(e => e.ModifiedByPK)
                    .HasMaxLength(128)
                    .IsRequired(false);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.HasOne(d => d.Holiday)
                   .WithMany(p => p.HolidayDates)
                   .HasForeignKey(d => d.HolidayId);

                entity.HasOne(d => d.HolidayType)
                  .WithMany(p => p.HolidayDates)
                  .HasForeignKey(d => d.HolidayTypeId)
                  .OnDelete(DeleteBehavior.Restrict);

            });
        }
    }
}
