using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using workflow.Models.ManageViewModels;
using workflow.Models.ModelBuilders;

namespace workflow.Models
{
    public partial class FliDbContext : DbContext
    {
        public FliDbContext()
        {
        }

        public FliDbContext(DbContextOptions<FliDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUsersLoginHistory> AspNetUsersLoginHistory { get; set; }
        public virtual DbSet<AspNetUsersMenu> AspNetUsersMenu { get; set; }
        public virtual DbSet<AspNetUsersMenuControl> AspNetUsersMenuControl { get; set; }
        public virtual DbSet<AspNetUsersMenuPermission> AspNetUsersMenuPermission { get; set; }
        public virtual DbSet<AspNetUsersMenuPermissionControl> AspNetUsersMenuPermissionControl { get; set; }
        public virtual DbSet<AspNetUsersPageVisited> AspNetUsersPageVisited { get; set; }
        public virtual DbSet<AspNetUsersProfile> AspNetUsersProfile { get; set; }
        public virtual DbSet<ChangeLog> ChangeLogs { get; set; }
        public virtual DbSet<Setting> Setting { get; set; }
        public virtual DbSet<Options> Options { get; set; }
        public virtual DbSet<UserPasswordPolicy> UserPasswordPolicies { get; set; }
        public virtual DbSet<UserPreferences> UserPreferences { get; set; }
        public virtual DbSet<ControlViewModel> ControlViewModel { get; set; }
        public virtual DbSet<RequestCategory> RequestCategories { get; set; }
        public virtual DbSet<RequestType> RequestTypes { get; set; }
        public virtual DbSet<GeneralField> GeneralFields { get; set; }
        public virtual DbSet<RequestForm> RequestForms { get; set; }
        public virtual DbSet<RequestTypeProcessOwner> RequestTypeProcessOwners { get; set; }
        public virtual DbSet<LockedRequestType> LockedRequestTypes { get; set; }
        public virtual DbSet<Reason> Reasons { get; set; }
        public virtual DbSet<UserIPAddressPerSession> UserIPAddressPerSessions  { get; set; }
        public virtual DbSet<SqlSession> SqlSessions { get; set; }
        public virtual DbSet<AuditTrail> AuditTrails { get; set; }
        public virtual DbSet<AuditTrailDetail> AuditTrailDetails { get; set; }
        public virtual DbSet<WorkFlowStep> WorkFlowSteps { get; set; }
        public virtual DbSet<WorkFlowStepType> WorkFlowStepTypes { get; set; }
        public virtual DbSet<WorkFlowStepTypeButton> WorkFlowStepTypeButtons { get; set; }
        public virtual DbSet<RequestStep> RequestSteps { get; set; }
        public virtual DbSet<RequestStepAssignee> RequestStepAssignees { get; set; }
        public virtual DbSet<RequestStepAssigneeDetail> RequestStepAssigneeDetails { get; set; }
        public virtual DbSet<ComparisonOperator> ComparisonOperators { get; set; }
        public virtual DbSet<RequestStepCondition> RequestStepConditions { get; set; }
        public virtual DbSet<RequestStepConditionDetail> RequestStepConditionDetails { get; set; }
        public virtual DbSet<RequestTypeWorkFlow> RequestTypeWorkFlows { get; set; }
        public virtual DbSet<RequestTypeWorkFlowParallel> RequestTypeWorkFlowParallels { get; set; }
        public virtual DbSet<Holiday> Holidays { get; set; }
        public virtual DbSet<HolidayDate> HolidayDates { get; set; }
        public virtual DbSet<HolidayAffectedOffice> HolidayAffectedOffices { get; set; }
        public virtual DbSet<OrganizationEntity> OrganizationEntities { get; set; }
        public virtual DbSet<OrganizationalStructure> OrganizationalStructures { get; set; }
        public virtual DbSet<UserOrganizationalStructure> UserOrganizationalStructures { get; set; }
        public virtual DbSet<ApprovalLevel> ApprovalLevels { get; set; }
        public virtual DbSet<ApprovalLevelDetail> ApprovalLevelDetails { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<TicketRequest> TicketRequests { get; set; }
        public virtual DbSet<TicketRequestDetail> TicketRequestDetails { get; set; }
        public virtual DbSet<TicketBaseWorkFlow> TicketBaseWorkFlows { get; set; }
        public virtual DbSet<TicketLink> TicketLinks { get; set; }
        public virtual DbSet<TicketAttachment> TicketAttachments { get; set; }
        public virtual DbSet<LinkType> LinkTypes { get; set; }
        public virtual DbSet<UserNotAllowedLinkType> UserNotAllowedLinkTypes { get; set; }
        public virtual DbSet<RequestTypesGrouping> RequestTypesGroupings { get; set; }
        public virtual DbSet<RequestTypesGroupingDetail> RequestTypesGroupingDetails { get; set; }
        public virtual DbSet<UserRequestTypesGrouping> UserRequestTypesGroupings { get; set; }
        public virtual DbSet<FileType> FileTypes { get; set; }
        public virtual DbSet<TicketBaseWorkFlowAssignee> TicketBaseWorkFlowAssignees { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // default constring if settings is not properly configured
                optionsBuilder.UseSqlServer("Server=ServerName;User ID=YourUserID;Password=YourPassword;Database=DatabaseName;MultipleActiveResultSets=true;");
                // optionsBuilder.UseMySQL("server=ServerName;port=PORT;user=YourUserID;password=YourPassword;database=DatabaseName;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //modelBuilder.Ignore<ControlViewModel>();
            //this model is only for SP values retrieval and should not be saved in DB.
            modelBuilder.Entity<ControlViewModel>(entity =>
            {
                entity.HasNoKey();
            });

            //Get Session ID in SQL
            modelBuilder.Entity<SqlSession>(entity =>
            {
                entity.HasNoKey()
                      .Metadata.SetIsTableExcludedFromMigrations(true);
            });
                                 
            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.Id).HasMaxLength(128);

                entity.Property(e => e.RoleId).IsRequired().HasMaxLength(128);

                entity.Property(e => e.ClaimType).HasMaxLength(250).IsUnicode(false);

                entity.Property(e => e.ClaimValue).HasMaxLength(250).IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasMaxLength(128);

                entity.HasIndex(e => e.NormalizedName)
                    .HasDatabaseName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.IndexPage)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.NormalizedName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Published)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedByPK)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValue(DateTime.Now);

                entity.Property(e => e.ModifiedByPK)
                    .HasMaxLength(128);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValue(DateTime.Now);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired().HasMaxLength(128);

                entity.Property(e => e.ClaimType).HasMaxLength(250).IsUnicode(false);

                entity.Property(e => e.ClaimValue).HasMaxLength(250).IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.ProviderDisplayName).HasMaxLength(250).IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.UserId).HasMaxLength(128);

                entity.Property(e => e.RoleId).HasMaxLength(128);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.UserId).HasMaxLength(128);

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.NormalizedEmail)
                    .HasDatabaseName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasDatabaseName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Id).HasMaxLength(128);

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).HasMaxLength(250).IsUnicode(false);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(250).IsUnicode(false);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(250).IsUnicode(false);

                entity.Property(e => e.UserName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.BlockedAccount)
                    .IsRequired()
                    .HasDefaultValue(false);
            });

            modelBuilder.Entity<AspNetUsersProfile>(entity =>
            {

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("FirstName")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("LastName")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.MiddleName)
                    .HasColumnName("MiddleName")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Gender)
                    .HasColumnName("Gender")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Company)
                    .HasColumnName("Company")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Department)
                    .HasColumnName("Department")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Position)
                    .HasColumnName("Position")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Rank)
                    .HasColumnName("Rank")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasColumnName("Phone")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasColumnName("Mobile")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("Country")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Photo)
                    .HasColumnName("Photo");

                entity.Property(e => e.CreatedByPK)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValue(DateTime.Now);

                entity.Property(e => e.ModifiedByPK)
                    .HasMaxLength(128);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValue(DateTime.Now);

                entity.Property(e => e.OfficeId)
                    .IsRequired(false);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.AspNetUsersProfile)
                    .HasForeignKey<AspNetUsersProfile>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AspNetUsersProfile_AspNetUsers");

                entity.HasOne(d => d.Office)
                   .WithMany(p => p.UserOffices)
                   .HasForeignKey(d => d.OfficeId)
                   .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<AspNetUsersLoginHistory>(entity =>
            {
                entity.HasKey(e => e.VUlhid);

                entity.Property(e => e.VUlhid)
                    .HasColumnName("vULHId")
                    .HasMaxLength(128);

                entity.Property(e => e.DLogIn)
                    .HasColumnName("dLogIn")
                    .HasColumnType("datetime");

                entity.Property(e => e.DLogOut)
                    .HasColumnName("dLogOut")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.NvIpaddress)
                    .IsRequired()
                    .HasColumnName("nvIPAddress")
                    .HasMaxLength(100);

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.AspNetUsersLoginHistory)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AspNetUsersLoginHistory_AspNetUsers");
            });

            modelBuilder.Entity<AspNetUsersMenu>(entity =>
            {
                entity.HasKey(e => e.VMenuId);

                entity.Property(e => e.VMenuId)
                    .HasColumnName("vMenuId")
                    .HasMaxLength(128);

                entity.Property(e => e.ISerialNo).HasColumnName("iSerialNo");

                entity.Property(e => e.NvFabIcon)
                    .IsRequired()
                    .HasColumnName("nvFabIcon")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NvMenuName)
                    .IsRequired()
                    .HasColumnName("nvMenuName")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.NvPageUrl)
                    .IsRequired()
                    .HasColumnName("nvPageUrl")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.PrefixCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VParentMenuId)
                    .HasColumnName("vParentMenuId")
                    .HasMaxLength(128);

                entity.Property(e => e.Published)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedByPK)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValue(DateTime.Now);

                entity.Property(e => e.ModifiedByPK)
                    .HasMaxLength(128);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValue(DateTime.Now);

                entity.Property(e => e.IsMenu)
                    .HasDefaultValue(false);

                entity.HasOne(d => d.VParentMenu)
                    .WithMany(p => p.InverseVParentMenu)
                    .HasForeignKey(d => d.VParentMenuId)
                    .HasConstraintName("FK_AspNetUsersMenu_AspNetUsersMenu");
            });

            modelBuilder.Entity<AspNetUsersMenuControl>(entity =>
            {
                entity.HasKey(e => e.MenuControlId);

                entity.HasIndex(e => new { e.VMenuId, e.OptionId })
                    .HasDatabaseName("IX_AspNetUsersMenuControl")
                    .IsUnique();

                entity.Property(e => e.MenuControlId).HasMaxLength(128);

                entity.Property(e => e.VMenuId)
                    .IsRequired()
                    .HasColumnName("vMenuId")
                    .HasMaxLength(128);

                entity.Property(e => e.OptionId)
                    .IsRequired();

                entity.HasOne(d => d.VMenu)
                    .WithMany(p => p.AspNetUsersMenuControl)
                    .HasForeignKey(d => d.VMenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AspNetUsersMenuControl_AspNetUsersMenu");

                entity.HasOne(d => d.Option)
                    .WithMany(p => p.AspNetUsersMenuControl)
                    .HasForeignKey(d => d.OptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AspNetUsersMenuContol_Options");
            });

            modelBuilder.Entity<AspNetUsersMenuPermission>(entity =>
            {
                entity.HasKey(e => e.VMenuPermissionId);

                entity.HasIndex(e => new { e.VMenuId, e.RoleId })
                    .HasDatabaseName("IX_AspNetUsersMenuPermission")
                    .IsUnique();

                entity.Property(e => e.VMenuPermissionId)
                    .HasColumnName("vMenuPermissionId")
                    .HasMaxLength(128);

                entity.Property(e => e.RoleId).IsRequired().HasMaxLength(128);

                entity.Property(e => e.VMenuId)
                    .IsRequired()
                    .HasColumnName("vMenuId")
                    .HasMaxLength(128);

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.AspNetUsersMenuPermission)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AspNetUsersMenuPermission_AspNetRoles");

                entity.HasOne(d => d.VMenu)
                    .WithMany(p => p.AspNetUsersMenuPermission)
                    .HasForeignKey(d => d.VMenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AspNetUsersMenuPermission_AspNetUsersMenu");
            });

            modelBuilder.Entity<AspNetUsersMenuPermissionControl>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.VMenuPermissionId, e.MenuControlId })
                    .HasDatabaseName("IX_AspNetUsersMenuPermissionControl")
                    .IsUnique();

                entity.Property(e => e.Id).HasMaxLength(128);

                entity.Property(e => e.VMenuPermissionId)
                    .IsRequired()
                    .HasColumnName("vMenuPermissionId")
                    .HasMaxLength(128)
                    .IsUnicode(true);

                entity.Property(e => e.MenuControlId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.MenuPermission)
                    .WithMany(p => p.AspNetUsersMenuPermissionControl)
                    .HasForeignKey(d => d.VMenuPermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AspNetUsersMenuPermissionControl_AspNetUsersMenuPermission");

                entity.HasOne(d => d.MenuControl)
                    .WithMany(p => p.AspNetUsersMenuPermissionControl)
                    .HasForeignKey(d => d.MenuControlId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AspNetUsersMenuPermissionContol_AspNetUsersMenuControl");
            });

            modelBuilder.Entity<AspNetUsersPageVisited>(entity =>
            {
                entity.HasKey(e => e.VPageVisitedId);

                entity.Property(e => e.VPageVisitedId)
                    .HasColumnName("vPageVisitedId")
                    .HasMaxLength(128);

                entity.Property(e => e.DDateVisited)
                    .HasColumnName("dDateVisited")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.NvIpaddress)
                    .IsRequired()
                    .HasColumnName("nvIPAddress")
                    .HasMaxLength(100);

                entity.Property(e => e.NvPageName)
                    .IsRequired()
                    .HasColumnName("nvPageName")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.AspNetUsersPageVisited)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AspNetUsersPageVisited_AspNetUsers");
            });

            modelBuilder.Entity<ChangeLog>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .UseIdentityColumn();

                entity.Property(e => e.VMenuId)
                    .IsRequired()
                    .HasColumnName("vMenuId")
                    .HasMaxLength(128);

                entity.Property(e => e.EventType)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EventName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ObjectType)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.OldContentDetail)
                    .IsRequired()
                    .HasColumnType("text")
                    .IsUnicode(false);

                entity.Property(e => e.ContentDetail)
                    .IsRequired()
                    .HasColumnType("text")
                    .IsUnicode(false);

                entity.Property(e => e.IPAddress)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedByPK)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValue(DateTime.Now);
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .UseIdentityColumn();

                entity.Property(e => e.VSettingId)
                    .HasColumnName("VSettingId")
                    .HasMaxLength(128);

                entity.Property(e => e.VSettingName)
                    .IsRequired()
                    .HasColumnName("VSettingName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.VSettingOption)
                    .HasColumnName("VSettingOption")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.VSettingGroup)
                    .HasColumnName("VSettingGroup")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.VSettingLabel)
                    .HasColumnName("VSettingLabel")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Options>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .UseIdentityColumn();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.OptionGroup)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Published)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedByPK)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.CreatedDate)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValue(DateTime.Now);

                entity.Property(e => e.ModifiedByPK)
                    .HasMaxLength(128);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValue(DateTime.Now);
            });

            modelBuilder.UserPasswordPoliciesEntity();
            modelBuilder.UserPreferencesEntity();
            modelBuilder.RequestCategoriesEntity();
            modelBuilder.RequestTypesEntity();
            modelBuilder.GeneralFieldsEntity();
            modelBuilder.RequestFormEntity();
            modelBuilder.RequestTypeProcessOwnersEntity();
            modelBuilder.LockedRequestTypesEntity();
            modelBuilder.ReasonsEntity();
            modelBuilder.UserIPAddressPerSessionsEntity();
            modelBuilder.AuditTrailsEntity();
            modelBuilder.AuditTrailDetailsEntity();
            modelBuilder.WorkFlowStepsEntity();
            modelBuilder.WorkFlowStepTypesEntity();
            modelBuilder.RequestStepsEntity();
            modelBuilder.RequestStepAssigneesEntity();
            modelBuilder.RequestStepAssigneeDetailsEntity();
            modelBuilder.ComparisonOperatorsEntity();
            modelBuilder.RequestStepConditionsEntity();
            modelBuilder.RequestStepConditionDetailsEntity();
            modelBuilder.RequestTypeWorkFlowsEntity();
            modelBuilder.RequestTypeWorkFlowParallelsEntity();
            modelBuilder.HolidaysEntity();
            modelBuilder.HolidayDatesEntity();
            modelBuilder.HolidayAffectedOfficesEntity();
            modelBuilder.OrganizationEntitiesEntity();
            modelBuilder.OrganizationalStructuresEntity();
            modelBuilder.UserOrganizationalStructuresEntity();
            modelBuilder.ApprovalLevelsEntity();
            modelBuilder.ApprovalLevelDetailsEntity();
            modelBuilder.TicketsEntity();
            modelBuilder.TicketRequestsEntity();
            modelBuilder.TicketRequestDetailsEntity();
            modelBuilder.TicketBaseWorkFlowsEntity();
            modelBuilder.TicketLinksEntity();
            modelBuilder.TicketAttachmentsEntity();
            modelBuilder.LinkTypesEntity();
            modelBuilder.UserNotAllowedLinkTypesEntity();
            modelBuilder.RequestTypesGroupingsEntity();
            modelBuilder.RequestTypesGroupingDetailsEntity();
            modelBuilder.UserRequestTypesGroupingsEntity();
            modelBuilder.FileTypesEntity();
            modelBuilder.TicketBaseWorkFlowAssigneesEntity();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
