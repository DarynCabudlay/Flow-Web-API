using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using workflow.Models.DataTableViewModels;
using workflow.Models.ManageViewModels;

namespace workflow.Services
{
    public interface IOrganizationalStructure : IGenericMethod<OrganizationalStructureViewModel>
    {
        Task SaveOrganizationEntity(OrganizationEntityViewModel data);
        
        IQueryable<OrganizationEntityViewModel> GetOrgranizationEntities(PagingRequest paging = null, bool? published = null);
        
        PagingResponse<OrganizationEntityViewModel> OrganizationEntitiesPagingFeature(IQueryable<OrganizationEntityViewModel> query, PagingRequest paging);

        Task DeleteOrganizationEntity(int id);

        IQueryable<OrganizationalStructureViewModel> GetOrganizationalStructures(PagingRequest paging = null, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null);

        PagingResponse<OrganizationalStructureViewModel> PagingFeature(IQueryable<OrganizationalStructureViewModel> query, PagingRequest paging);

        Task<IEnumerable<WholeOrganizationalStructuresViewModel>> GetWholeOrganizationalStructures(bool structuresOnly = true, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null);
        Task<OrganizationalStructureTabularViewModel> GetWholeOrganizationalStructuresInTabularView(PagingRequest paging = null, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null);
        DataTablePagingResponse DataTablePagingFeature(DataTable query, PagingRequest paging);

        Task<IEnumerable<OrganizationalStructureViewModel>> GetOrganizationalStructureChildren(int parentId, List<OrganizationalStructureViewModel> organizationalStructures, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null);

        Task<IEnumerable<OrganizationalStructureViewModel>> GetOrganizationalStructureParents(int childId, List<OrganizationalStructureViewModel> organizationalStructures, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null);

        Task<IEnumerable<OrganizationalStructureViewModel>> GetOrganizationalStructurePathAsync(int childId, List<OrganizationalStructureViewModel> organizationalStructures, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null);

        IEnumerable<OrganizationalStructureViewModel> GetOrganizationalStructurePath(int childId, List<OrganizationalStructureViewModel> organizationalStructures, bool? published = null, bool? organizationEntityPublished = null, bool? usersPublished = null);
    }
}
