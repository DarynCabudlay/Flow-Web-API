using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBOrganizationalStructures
    {
        public static void OrganizationalStructuresEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrganizationalStructure>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.ParentId)
                    .IsRequired();

                entity.Property(e => e.OrganizationEntityId)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .IsRequired(false)
                    .HasMaxLength(250);

                entity.Property(e => e.Published)
                    .IsRequired();

                entity.Property(e => e.Published)
                    .IsRequired();

                entity.Property(e => e.CreatedByPK)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedByPK)
                    .IsRequired(false)
                    .HasMaxLength(128);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .IsRequired(false);

                entity.HasOne(d => d.OrganizationEntity)
                   .WithMany(p => p.OrganizationalStructures)
                   .HasForeignKey(d => d.OrganizationEntityId)
                   .OnDelete(DeleteBehavior.Restrict);

            });
        }
    }
}
