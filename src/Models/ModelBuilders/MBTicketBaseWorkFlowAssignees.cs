using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models;

namespace workflow.Models.ModelBuilders
{
    public static class MBTicketBaseWorkFlowAssignees
    {
        public static void TicketBaseWorkFlowAssigneesEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketBaseWorkFlowAssignee>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseIdentityColumn();

                entity.Property(e => e.TicketBaseWorkFlowId)
                    .IsRequired();

                entity.Property(e => e.OrganizationalEntityId)
                    .IsRequired(false);

                entity.Property(e => e.OrganizationalEntity)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(e => e.OrganizationalEntityHierarchy)
                    .IsRequired(false);

                entity.Property(e => e.OrganizationalStructureId)
                    .IsRequired(false);

                entity.Property(e => e.OrganizationalStructure)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(e => e.ApprovalLevelDetailId)
                   .IsRequired(false);

                entity.Property(e => e.ApprovalLevelDetail)
                  .HasMaxLength(100)
                  .IsRequired(false);

                entity.Property(e => e.ApprovalLevelId)
                  .IsRequired(false);

                entity.Property(e => e.ApprovalLevel)
                  .HasMaxLength(50)
                  .IsRequired(false);

                entity.Property(e => e.UserId)
                  .HasMaxLength(128)
                  .IsRequired();

                entity.Property(e => e.SortOrder)
                  .IsRequired(false);

                entity.HasOne(d => d.TicketBaseWorkFlow)
                    .WithMany(p => p.TicketBaseWorkFlowAssignees)
                    .HasForeignKey(d => d.TicketBaseWorkFlowId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
