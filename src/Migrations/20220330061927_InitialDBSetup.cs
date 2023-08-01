using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class InitialDBSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    NormalizedName = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IndexPage = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    Published = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2022, 3, 30, 14, 19, 27, 81, DateTimeKind.Local).AddTicks(2592)),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValue: new DateTime(2022, 3, 30, 14, 19, 27, 86, DateTimeKind.Local).AddTicks(2710)),
                    ModifiedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Email = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    BlockedAccount = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsersMenu",
                columns: table => new
                {
                    vMenuId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    nvMenuName = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    iSerialNo = table.Column<int>(type: "int", nullable: false),
                    nvFabIcon = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    vParentMenuId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    nvPageUrl = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    PrefixCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Published = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2022, 3, 30, 14, 19, 27, 133, DateTimeKind.Local).AddTicks(7949)),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValue: new DateTime(2022, 3, 30, 14, 19, 27, 133, DateTimeKind.Local).AddTicks(9973)),
                    ModifiedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsersMenu", x => x.vMenuId);
                    table.ForeignKey(
                        name: "FK_AspNetUsersMenu_AspNetUsersMenu",
                        column: x => x.vParentMenuId,
                        principalTable: "AspNetUsersMenu",
                        principalColumn: "vMenuId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChangeLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vMenuId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    EventType = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    EventName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    ObjectType = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    OldContentDetail = table.Column<string>(type: "text", unicode: false, nullable: false),
                    ContentDetail = table.Column<string>(type: "text", unicode: false, nullable: false),
                    IPAddress = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2022, 3, 30, 14, 19, 27, 156, DateTimeKind.Local).AddTicks(7545)),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Options",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    OptionGroup = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Published = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2022, 3, 30, 14, 19, 27, 160, DateTimeKind.Local).AddTicks(9027)),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValue: new DateTime(2022, 3, 30, 14, 19, 27, 161, DateTimeKind.Local).AddTicks(315)),
                    ModifiedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Options", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Setting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VSettingId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    VSettingName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    VSettingOption = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    VSettingGroup = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    VSettingLabel = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", maxLength: 128, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ClaimType = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    ClaimValue = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ClaimType = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    ClaimValue = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsersLoginHistory",
                columns: table => new
                {
                    vULHId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    dLogIn = table.Column<DateTime>(type: "datetime", nullable: false),
                    dLogOut = table.Column<DateTime>(type: "datetime", nullable: true),
                    nvIPAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsersLoginHistory", x => x.vULHId);
                    table.ForeignKey(
                        name: "FK_AspNetUsersLoginHistory_AspNetUsers",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsersPageVisited",
                columns: table => new
                {
                    vPageVisitedId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    dDateVisited = table.Column<DateTime>(type: "datetime", nullable: false),
                    nvPageName = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    nvIPAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsersPageVisited", x => x.vPageVisitedId);
                    table.ForeignKey(
                        name: "FK_AspNetUsersPageVisited_AspNetUsers",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsersProfile",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    MiddleName = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    LastName = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    Company = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Department = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Position = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Rank = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Mobile = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Phone = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Country = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Gender = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2022, 3, 30, 14, 19, 27, 111, DateTimeKind.Local).AddTicks(3452)),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValue: new DateTime(2022, 3, 30, 14, 19, 27, 111, DateTimeKind.Local).AddTicks(5005)),
                    ModifiedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsersProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsersProfile_AspNetUsers",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsersMenuPermission",
                columns: table => new
                {
                    vMenuPermissionId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    vMenuId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsersMenuPermission", x => x.vMenuPermissionId);
                    table.ForeignKey(
                        name: "FK_AspNetUsersMenuPermission_AspNetRoles",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsersMenuPermission_AspNetUsersMenu",
                        column: x => x.vMenuId,
                        principalTable: "AspNetUsersMenu",
                        principalColumn: "vMenuId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsersMenuControl",
                columns: table => new
                {
                    MenuControlId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    vMenuId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    OptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsersMenuControl", x => x.MenuControlId);
                    table.ForeignKey(
                        name: "FK_AspNetUsersMenuContol_Options",
                        column: x => x.OptionId,
                        principalTable: "Options",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsersMenuControl_AspNetUsersMenu",
                        column: x => x.vMenuId,
                        principalTable: "AspNetUsersMenu",
                        principalColumn: "vMenuId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsersMenuPermissionControl",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    vMenuPermissionId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    MenuControlId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsersMenuPermissionControl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsersMenuPermissionContol_AspNetUsersMenuControl",
                        column: x => x.MenuControlId,
                        principalTable: "AspNetUsersMenuControl",
                        principalColumn: "MenuControlId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsersMenuPermissionControl_AspNetUsersMenuPermission",
                        column: x => x.vMenuPermissionId,
                        principalTable: "AspNetUsersMenuPermission",
                        principalColumn: "vMenuPermissionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "([NormalizedName] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "([NormalizedUserName] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsersLoginHistory_UserId",
                table: "AspNetUsersLoginHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsersMenu_vParentMenuId",
                table: "AspNetUsersMenu",
                column: "vParentMenuId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsersMenuControl",
                table: "AspNetUsersMenuControl",
                columns: new[] { "vMenuId", "OptionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsersMenuControl_OptionId",
                table: "AspNetUsersMenuControl",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsersMenuPermission",
                table: "AspNetUsersMenuPermission",
                columns: new[] { "vMenuId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsersMenuPermission_RoleId",
                table: "AspNetUsersMenuPermission",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsersMenuPermissionControl",
                table: "AspNetUsersMenuPermissionControl",
                columns: new[] { "vMenuPermissionId", "MenuControlId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsersMenuPermissionControl_MenuControlId",
                table: "AspNetUsersMenuPermissionControl",
                column: "MenuControlId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsersPageVisited_UserId",
                table: "AspNetUsersPageVisited",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsersLoginHistory");

            migrationBuilder.DropTable(
                name: "AspNetUsersMenuPermissionControl");

            migrationBuilder.DropTable(
                name: "AspNetUsersPageVisited");

            migrationBuilder.DropTable(
                name: "AspNetUsersProfile");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ChangeLogs");

            migrationBuilder.DropTable(
                name: "Setting");

            migrationBuilder.DropTable(
                name: "AspNetUsersMenuControl");

            migrationBuilder.DropTable(
                name: "AspNetUsersMenuPermission");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Options");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsersMenu");
        }
    }
}
