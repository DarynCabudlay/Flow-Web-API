using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBUserOrganizationalStructures
    {
        public static void UserOrganizationalStructuresEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserOrganizationalStructure>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.UserId, e.OrganizationalStructureId }, "IX_User_OrganizationalStructure").IsUnique();

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.OrganizationalStructureId)
                   .IsRequired();

                entity.Property(e => e.ApprovalLevelDetailId)
                    .IsRequired();

                entity.Property(e => e.ReportingTo)
                    .IsRequired();

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

                entity.HasOne(d => d.User)
                      .WithMany(p => p.UserOrganizationalStructures)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.HasOne(d => d.OrganizationalStructure)
                   .WithMany(p => p.UserOrganizationalStructures)
                   .HasForeignKey(d => d.OrganizationalStructureId)
                   .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
