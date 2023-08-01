using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class InitialTableForTicketCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestTypes_RequestCategories_RequestCategoryId",
                table: "RequestTypes");

            migrationBuilder.DropIndex(
                name: "IX_RequestStepAssigneeDetails_RequestStepAssigneeId",
                table: "RequestStepAssigneeDetails");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserOrganizationalStructures",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 522, DateTimeKind.Local).AddTicks(7017),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 732, DateTimeKind.Local).AddTicks(8177));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 409, DateTimeKind.Local).AddTicks(2802),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 621, DateTimeKind.Local).AddTicks(9944));

            migrationBuilder.AlterColumn<string>(
                name: "Assignee",
                table: "RequestStepAssigneeDetails",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 396, DateTimeKind.Local).AddTicks(6588),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 612, DateTimeKind.Local).AddTicks(4119));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "OrganizationEntities",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "OrganizationalStructures",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 384, DateTimeKind.Local).AddTicks(7015),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 604, DateTimeKind.Local).AddTicks(4622));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 384, DateTimeKind.Local).AddTicks(5583),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 604, DateTimeKind.Local).AddTicks(3233));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLocked",
                table: "LockedRequestTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 425, DateTimeKind.Local).AddTicks(8039),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 638, DateTimeKind.Local).AddTicks(2883));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Holidays",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 499, DateTimeKind.Local).AddTicks(9529),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 710, DateTimeKind.Local).AddTicks(4071));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayDates",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 504, DateTimeKind.Local).AddTicks(8111),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 714, DateTimeKind.Local).AddTicks(2311));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayAffectedOffices",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 509, DateTimeKind.Local).AddTicks(3041),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 717, DateTimeKind.Local).AddTicks(1293));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 380, DateTimeKind.Local).AddTicks(3810),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 599, DateTimeKind.Local).AddTicks(5700));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 336, DateTimeKind.Local).AddTicks(9988),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 552, DateTimeKind.Local).AddTicks(3738));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 336, DateTimeKind.Local).AddTicks(8278),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 552, DateTimeKind.Local).AddTicks(2011));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 359, DateTimeKind.Local).AddTicks(2887),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 574, DateTimeKind.Local).AddTicks(845));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 359, DateTimeKind.Local).AddTicks(1432),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 573, DateTimeKind.Local).AddTicks(9029));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 311, DateTimeKind.Local).AddTicks(7382),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 527, DateTimeKind.Local).AddTicks(2707));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 306, DateTimeKind.Local).AddTicks(1503),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 521, DateTimeKind.Local).AddTicks(3108));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "ApprovalLevelDetails",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ApprovalLevelDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 530, DateTimeKind.Local).AddTicks(1913),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 743, DateTimeKind.Local).AddTicks(8491));

            migrationBuilder.CreateTable(
                name: "FileTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    MimeType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Published = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 590, DateTimeKind.Local).AddTicks(230)),
                    ModifiedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LinkTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    URL = table.Column<string>(type: "varchar(max)", maxLength: 5000, nullable: false),
                    Published = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 563, DateTimeKind.Local).AddTicks(1846)),
                    ModifiedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestTypesGroupings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    Published = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 573, DateTimeKind.Local).AddTicks(9625)),
                    ModifiedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestTypesGroupings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketNo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    RushTicketNo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    RequestTypeId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DetailedStatus = table.Column<int>(type: "int", nullable: true),
                    CancellationReason = table.Column<int>(type: "int", nullable: true),
                    CancellationRemarks = table.Column<string>(type: "varchar(max)", maxLength: 8000, nullable: true),
                    UserOrganizationalStructureId = table.Column<int>(type: "int", nullable: false),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 536, DateTimeKind.Local).AddTicks(5859)),
                    ModifiedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_RequestTypes_RequestTypeId_Version",
                        columns: x => new { x.RequestTypeId, x.Version },
                        principalTable: "RequestTypes",
                        principalColumns: new[] { "Id", "Version" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_UserOrganizationalStructures_UserOrganizationalStructureId",
                        column: x => x.UserOrganizationalStructureId,
                        principalTable: "UserOrganizationalStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    FileTypeId = table.Column<int>(type: "int", nullable: false),
                    OriginalFileName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    MaskedFileName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    FilePath = table.Column<string>(type: "varchar(max)", maxLength: 5000, nullable: false),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 559, DateTimeKind.Local).AddTicks(6637)),
                    ModifiedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketAttachments_FileTypes_FileTypeId",
                        column: x => x.FileTypeId,
                        principalTable: "FileTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    LinkTypeId = table.Column<int>(type: "int", nullable: false),
                    Link = table.Column<string>(type: "varchar(max)", nullable: false),
                    LinkText = table.Column<string>(type: "varchar(max)", nullable: false),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 555, DateTimeKind.Local).AddTicks(9186)),
                    ModifiedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketLinks_LinkTypes_LinkTypeId",
                        column: x => x.LinkTypeId,
                        principalTable: "LinkTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserNotAllowedLinkTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LinkTypeId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Published = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 568, DateTimeKind.Local).AddTicks(2713)),
                    ModifiedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotAllowedLinkTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotAllowedLinkTypes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNotAllowedLinkTypes_LinkTypes_LinkTypeId",
                        column: x => x.LinkTypeId,
                        principalTable: "LinkTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestTypesGroupingDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestTypesGroupingId = table.Column<int>(type: "int", nullable: false),
                    RequestTypeId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Published = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 578, DateTimeKind.Local).AddTicks(6311)),
                    ModifiedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestTypesGroupingDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestTypesGroupingDetails_RequestTypes_RequestTypeId_Version",
                        columns: x => new { x.RequestTypeId, x.Version },
                        principalTable: "RequestTypes",
                        principalColumns: new[] { "Id", "Version" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestTypesGroupingDetails_RequestTypesGroupings_RequestTypesGroupingId",
                        column: x => x.RequestTypesGroupingId,
                        principalTable: "RequestTypesGroupings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRequestTypesGroupings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RequestTypesGroupId = table.Column<int>(type: "int", nullable: false),
                    Published = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 583, DateTimeKind.Local).AddTicks(9088)),
                    ModifiedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRequestTypesGroupings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRequestTypesGroupings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRequestTypesGroupings_RequestTypesGroupings_RequestTypesGroupId",
                        column: x => x.RequestTypesGroupId,
                        principalTable: "RequestTypesGroupings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketBaseWorkFlows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    RequestTypeWorkFlowId = table.Column<int>(type: "int", nullable: false),
                    StepId = table.Column<int>(type: "int", nullable: false),
                    StepTypeId = table.Column<int>(type: "int", nullable: false),
                    AssigneeType = table.Column<int>(type: "int", nullable: false),
                    RequiredToExecute = table.Column<int>(type: "int", nullable: false),
                    TAT = table.Column<int>(type: "int", nullable: false),
                    Assignees = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicableButtons = table.Column<string>(type: "varchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketBaseWorkFlows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketBaseWorkFlows_RequestTypeWorkFlows_RequestTypeWorkFlowId",
                        column: x => x.RequestTypeWorkFlowId,
                        principalTable: "RequestTypeWorkFlows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketBaseWorkFlows_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "varchar(max)", maxLength: 8000, nullable: true),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 542, DateTimeKind.Local).AddTicks(3369)),
                    ModifiedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketRequest_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketRequestDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketRequestId = table.Column<int>(type: "int", nullable: false),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    IsLov = table.Column<bool>(type: "bit", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    Value2 = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 547, DateTimeKind.Local).AddTicks(1423)),
                    ModifiedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketRequestDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketRequestDetail_GeneralFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "GeneralFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketRequestDetail_TicketRequest_TicketRequestId",
                        column: x => x.TicketRequestId,
                        principalTable: "TicketRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StepTypeName",
                table: "WorkFlowStepTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestStepAssignee",
                table: "RequestStepAssigneeDetails",
                columns: new[] { "RequestStepAssigneeId", "Assignee" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestCategoryName",
                table: "RequestCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReasonNameAndReasonType",
                table: "Reasons",
                columns: new[] { "Name", "ReasonTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HolidayName",
                table: "Holidays",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileTypeName",
                table: "FileTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LinkTypeName",
                table: "LinkTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestTypesGroupingDetails_RequestTypeId_Version",
                table: "RequestTypesGroupingDetails",
                columns: new[] { "RequestTypeId", "Version" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestTypesGroupingRequestTypeVersion",
                table: "RequestTypesGroupingDetails",
                columns: new[] { "RequestTypesGroupingId", "RequestTypeId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestTypesGroupingName",
                table: "RequestTypesGroupings",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketAttachments_FileTypeId",
                table: "TicketAttachments",
                column: "FileTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAndRequestTypeWorkFlow",
                table: "TicketBaseWorkFlows",
                columns: new[] { "TicketId", "RequestTypeWorkFlowId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketBaseWorkFlows_RequestTypeWorkFlowId",
                table: "TicketBaseWorkFlows",
                column: "RequestTypeWorkFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketLinks_LinkTypeId",
                table: "TicketLinks",
                column: "LinkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketRequest_TicketId",
                table: "TicketRequest",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketRequestAndField",
                table: "TicketRequestDetail",
                columns: new[] { "TicketRequestId", "FieldId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketRequestDetail_FieldId",
                table: "TicketRequestDetail",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketNo",
                table: "Tickets",
                column: "TicketNo",
                unique: true,
                filter: "[TicketNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_RequestTypeId_Version",
                table: "Tickets",
                columns: new[] { "RequestTypeId", "Version" });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UserOrganizationalStructureId",
                table: "Tickets",
                column: "UserOrganizationalStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_UserIdLinkTypeCategory",
                table: "UserNotAllowedLinkTypes",
                columns: new[] { "UserId", "LinkTypeId", "Category" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserNotAllowedLinkTypes_LinkTypeId",
                table: "UserNotAllowedLinkTypes",
                column: "LinkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAndRequestTypeGrouping",
                table: "UserRequestTypesGroupings",
                columns: new[] { "UserId", "RequestTypesGroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRequestTypesGroupings_RequestTypesGroupId",
                table: "UserRequestTypesGroupings",
                column: "RequestTypesGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTypes_RequestCategories_RequestCategoryId",
                table: "RequestTypes",
                column: "RequestCategoryId",
                principalTable: "RequestCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestTypes_RequestCategories_RequestCategoryId",
                table: "RequestTypes");

            migrationBuilder.DropTable(
                name: "RequestTypesGroupingDetails");

            migrationBuilder.DropTable(
                name: "TicketAttachments");

            migrationBuilder.DropTable(
                name: "TicketBaseWorkFlows");

            migrationBuilder.DropTable(
                name: "TicketLinks");

            migrationBuilder.DropTable(
                name: "TicketRequestDetail");

            migrationBuilder.DropTable(
                name: "UserNotAllowedLinkTypes");

            migrationBuilder.DropTable(
                name: "UserRequestTypesGroupings");

            migrationBuilder.DropTable(
                name: "FileTypes");

            migrationBuilder.DropTable(
                name: "TicketRequest");

            migrationBuilder.DropTable(
                name: "LinkTypes");

            migrationBuilder.DropTable(
                name: "RequestTypesGroupings");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_StepTypeName",
                table: "WorkFlowStepTypes");

            migrationBuilder.DropIndex(
                name: "IX_RequestStepAssignee",
                table: "RequestStepAssigneeDetails");

            migrationBuilder.DropIndex(
                name: "IX_RequestCategoryName",
                table: "RequestCategories");

            migrationBuilder.DropIndex(
                name: "IX_ReasonNameAndReasonType",
                table: "Reasons");

            migrationBuilder.DropIndex(
                name: "IX_HolidayName",
                table: "Holidays");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserOrganizationalStructures",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 732, DateTimeKind.Local).AddTicks(8177),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 522, DateTimeKind.Local).AddTicks(7017));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 621, DateTimeKind.Local).AddTicks(9944),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 409, DateTimeKind.Local).AddTicks(2802));

            migrationBuilder.AlterColumn<string>(
                name: "Assignee",
                table: "RequestStepAssigneeDetails",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 612, DateTimeKind.Local).AddTicks(4119),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 396, DateTimeKind.Local).AddTicks(6588));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "OrganizationEntities",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "OrganizationalStructures",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 604, DateTimeKind.Local).AddTicks(4622),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 384, DateTimeKind.Local).AddTicks(7015));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 604, DateTimeKind.Local).AddTicks(3233),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 384, DateTimeKind.Local).AddTicks(5583));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLocked",
                table: "LockedRequestTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 638, DateTimeKind.Local).AddTicks(2883),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 425, DateTimeKind.Local).AddTicks(8039));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Holidays",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 710, DateTimeKind.Local).AddTicks(4071),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 499, DateTimeKind.Local).AddTicks(9529));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayDates",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 714, DateTimeKind.Local).AddTicks(2311),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 504, DateTimeKind.Local).AddTicks(8111));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayAffectedOffices",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 717, DateTimeKind.Local).AddTicks(1293),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 509, DateTimeKind.Local).AddTicks(3041));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 599, DateTimeKind.Local).AddTicks(5700),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 380, DateTimeKind.Local).AddTicks(3810));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 552, DateTimeKind.Local).AddTicks(3738),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 336, DateTimeKind.Local).AddTicks(9988));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 552, DateTimeKind.Local).AddTicks(2011),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 336, DateTimeKind.Local).AddTicks(8278));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 574, DateTimeKind.Local).AddTicks(845),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 359, DateTimeKind.Local).AddTicks(2887));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 573, DateTimeKind.Local).AddTicks(9029),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 359, DateTimeKind.Local).AddTicks(1432));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 527, DateTimeKind.Local).AddTicks(2707),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 311, DateTimeKind.Local).AddTicks(7382));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 521, DateTimeKind.Local).AddTicks(3108),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 306, DateTimeKind.Local).AddTicks(1503));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "ApprovalLevelDetails",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ApprovalLevelDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 13, 15, 1, 27, 743, DateTimeKind.Local).AddTicks(8491),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 17, 16, 37, 26, 530, DateTimeKind.Local).AddTicks(1913));

            migrationBuilder.CreateIndex(
                name: "IX_RequestStepAssigneeDetails_RequestStepAssigneeId",
                table: "RequestStepAssigneeDetails",
                column: "RequestStepAssigneeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTypes_RequestCategories_RequestCategoryId",
                table: "RequestTypes",
                column: "RequestCategoryId",
                principalTable: "RequestCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
