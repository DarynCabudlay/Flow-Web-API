﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace workflow.Models
{
    public class OrganizationalStructure
    {
        public OrganizationalStructure()
        {
            UserOrganizationalStructures = new HashSet<UserOrganizationalStructure>();
        }

        public int Id { get; set; }
        public int ParentId { get; set; }
        public int OrganizationEntityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Published { get; set; }
        public string CreatedByPK { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByPK { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public virtual OrganizationEntity OrganizationEntity { get; set; }
        public virtual ICollection<UserOrganizationalStructure> UserOrganizationalStructures { get; set; }
    }
}
