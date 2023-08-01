using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class ChangesInTablesTicketRequestsAndDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketRequest_Tickets_TicketId",
                table: "TicketRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketRequestDetail_GeneralFields_FieldId",
                table: "TicketRequestDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketRequestDetail_TicketRequest_TicketRequestId",
                table: "TicketRequestDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketRequestDetail",
                table: "TicketRequestDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketRequest",
                table: "TicketRequest");

            migrationBuilder.RenameTable(
                name: "TicketRequestDetail",
                newName: "TicketRequestDetails");

            migrationBuilder.RenameTable(
                name: "TicketRequest",
                newName: "TicketRequests");

            migrationBuilder.RenameIndex(
                name: "IX_TicketRequestDetail_FieldId",
                table: "TicketRequestDetails",
                newName: "IX_TicketRequestDetails_FieldId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketRequest_TicketId",
                table: "TicketRequests",
                newName: "IX_TicketRequests_TicketId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserRequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 911, DateTimeKind.Local).AddTicks(3498),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 845, DateTimeKind.Local).AddTicks(8683));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserOrganizationalStructures",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 874, DateTimeKind.Local).AddTicks(8061),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 806, DateTimeKind.Local).AddTicks(8335));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserNotAllowedLinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 902, DateTimeKind.Local).AddTicks(6220),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 836, DateTimeKind.Local).AddTicks(4525));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Tickets",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 883, DateTimeKind.Local).AddTicks(825),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 815, DateTimeKind.Local).AddTicks(4653));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketLinks",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 894, DateTimeKind.Local).AddTicks(9242),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 828, DateTimeKind.Local).AddTicks(551));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketAttachments",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 897, DateTimeKind.Local).AddTicks(5301),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 830, DateTimeKind.Local).AddTicks(6528));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 905, DateTimeKind.Local).AddTicks(969),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 839, DateTimeKind.Local).AddTicks(3261));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupingDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 907, DateTimeKind.Local).AddTicks(9210),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 842, DateTimeKind.Local).AddTicks(3530));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 805, DateTimeKind.Local).AddTicks(4366),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 723, DateTimeKind.Local).AddTicks(4077));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 795, DateTimeKind.Local).AddTicks(9255),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 712, DateTimeKind.Local).AddTicks(2741));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 787, DateTimeKind.Local).AddTicks(4927),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 704, DateTimeKind.Local).AddTicks(2901));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 787, DateTimeKind.Local).AddTicks(3870),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 704, DateTimeKind.Local).AddTicks(1924));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLocked",
                table: "LockedRequestTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 817, DateTimeKind.Local).AddTicks(243),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 736, DateTimeKind.Local).AddTicks(6547));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "LinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 899, DateTimeKind.Local).AddTicks(7437),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 833, DateTimeKind.Local).AddTicks(900));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Holidays",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 861, DateTimeKind.Local).AddTicks(9613),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 792, DateTimeKind.Local).AddTicks(8139));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayDates",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 864, DateTimeKind.Local).AddTicks(3508),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 795, DateTimeKind.Local).AddTicks(1125));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayAffectedOffices",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 866, DateTimeKind.Local).AddTicks(7128),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 798, DateTimeKind.Local).AddTicks(1507));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "FileTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 914, DateTimeKind.Local).AddTicks(353),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 848, DateTimeKind.Local).AddTicks(6726));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 784, DateTimeKind.Local).AddTicks(3936),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 701, DateTimeKind.Local).AddTicks(4253));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 753, DateTimeKind.Local).AddTicks(4128),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 671, DateTimeKind.Local).AddTicks(7691));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 753, DateTimeKind.Local).AddTicks(3008),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 671, DateTimeKind.Local).AddTicks(6546));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 769, DateTimeKind.Local).AddTicks(7576),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 687, DateTimeKind.Local).AddTicks(5038));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 769, DateTimeKind.Local).AddTicks(6562),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 687, DateTimeKind.Local).AddTicks(3974));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 734, DateTimeKind.Local).AddTicks(4220),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 654, DateTimeKind.Local).AddTicks(7169));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 729, DateTimeKind.Local).AddTicks(6519),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 651, DateTimeKind.Local).AddTicks(2085));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ApprovalLevelDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 879, DateTimeKind.Local).AddTicks(5140),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 811, DateTimeKind.Local).AddTicks(7486));

            migrationBuilder.AlterColumn<string>(
                name: "ValueCode",
                table: "TicketRequestDetails",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 8000);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequestDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 889, DateTimeKind.Local).AddTicks(2362),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 822, DateTimeKind.Local).AddTicks(1469));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequests",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 885, DateTimeKind.Local).AddTicks(7685),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 818, DateTimeKind.Local).AddTicks(3451));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketRequestDetails",
                table: "TicketRequestDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketRequests",
                table: "TicketRequests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketRequestDetails_GeneralFields_FieldId",
                table: "TicketRequestDetails",
                column: "FieldId",
                principalTable: "GeneralFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketRequestDetails_TicketRequests_TicketRequestId",
                table: "TicketRequestDetails",
                column: "TicketRequestId",
                principalTable: "TicketRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketRequests_Tickets_TicketId",
                table: "TicketRequests",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketRequestDetails_GeneralFields_FieldId",
                table: "TicketRequestDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketRequestDetails_TicketRequests_TicketRequestId",
                table: "TicketRequestDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketRequests_Tickets_TicketId",
                table: "TicketRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketRequests",
                table: "TicketRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketRequestDetails",
                table: "TicketRequestDetails");

            migrationBuilder.RenameTable(
                name: "TicketRequests",
                newName: "TicketRequest");

            migrationBuilder.RenameTable(
                name: "TicketRequestDetails",
                newName: "TicketRequestDetail");

            migrationBuilder.RenameIndex(
                name: "IX_TicketRequests_TicketId",
                table: "TicketRequest",
                newName: "IX_TicketRequest_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketRequestDetails_FieldId",
                table: "TicketRequestDetail",
                newName: "IX_TicketRequestDetail_FieldId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserRequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 845, DateTimeKind.Local).AddTicks(8683),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 911, DateTimeKind.Local).AddTicks(3498));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserOrganizationalStructures",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 806, DateTimeKind.Local).AddTicks(8335),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 874, DateTimeKind.Local).AddTicks(8061));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserNotAllowedLinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 836, DateTimeKind.Local).AddTicks(4525),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 902, DateTimeKind.Local).AddTicks(6220));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Tickets",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 815, DateTimeKind.Local).AddTicks(4653),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 883, DateTimeKind.Local).AddTicks(825));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketLinks",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 828, DateTimeKind.Local).AddTicks(551),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 894, DateTimeKind.Local).AddTicks(9242));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketAttachments",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 830, DateTimeKind.Local).AddTicks(6528),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 897, DateTimeKind.Local).AddTicks(5301));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 839, DateTimeKind.Local).AddTicks(3261),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 905, DateTimeKind.Local).AddTicks(969));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupingDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 842, DateTimeKind.Local).AddTicks(3530),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 907, DateTimeKind.Local).AddTicks(9210));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 723, DateTimeKind.Local).AddTicks(4077),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 805, DateTimeKind.Local).AddTicks(4366));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 712, DateTimeKind.Local).AddTicks(2741),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 795, DateTimeKind.Local).AddTicks(9255));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 704, DateTimeKind.Local).AddTicks(2901),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 787, DateTimeKind.Local).AddTicks(4927));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 704, DateTimeKind.Local).AddTicks(1924),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 787, DateTimeKind.Local).AddTicks(3870));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLocked",
                table: "LockedRequestTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 736, DateTimeKind.Local).AddTicks(6547),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 817, DateTimeKind.Local).AddTicks(243));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "LinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 833, DateTimeKind.Local).AddTicks(900),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 899, DateTimeKind.Local).AddTicks(7437));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Holidays",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 792, DateTimeKind.Local).AddTicks(8139),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 861, DateTimeKind.Local).AddTicks(9613));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayDates",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 795, DateTimeKind.Local).AddTicks(1125),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 864, DateTimeKind.Local).AddTicks(3508));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayAffectedOffices",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 798, DateTimeKind.Local).AddTicks(1507),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 866, DateTimeKind.Local).AddTicks(7128));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "FileTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 848, DateTimeKind.Local).AddTicks(6726),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 914, DateTimeKind.Local).AddTicks(353));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 701, DateTimeKind.Local).AddTicks(4253),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 784, DateTimeKind.Local).AddTicks(3936));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 671, DateTimeKind.Local).AddTicks(7691),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 753, DateTimeKind.Local).AddTicks(4128));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 671, DateTimeKind.Local).AddTicks(6546),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 753, DateTimeKind.Local).AddTicks(3008));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 687, DateTimeKind.Local).AddTicks(5038),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 769, DateTimeKind.Local).AddTicks(7576));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 687, DateTimeKind.Local).AddTicks(3974),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 769, DateTimeKind.Local).AddTicks(6562));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 654, DateTimeKind.Local).AddTicks(7169),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 734, DateTimeKind.Local).AddTicks(4220));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 651, DateTimeKind.Local).AddTicks(2085),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 729, DateTimeKind.Local).AddTicks(6519));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ApprovalLevelDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 811, DateTimeKind.Local).AddTicks(7486),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 879, DateTimeKind.Local).AddTicks(5140));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequest",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 818, DateTimeKind.Local).AddTicks(3451),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 885, DateTimeKind.Local).AddTicks(7685));

            migrationBuilder.AlterColumn<string>(
                name: "ValueCode",
                table: "TicketRequestDetail",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 8000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequestDetail",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 5, 9, 50, 59, 822, DateTimeKind.Local).AddTicks(1469),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 889, DateTimeKind.Local).AddTicks(2362));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketRequest",
                table: "TicketRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketRequestDetail",
                table: "TicketRequestDetail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketRequest_Tickets_TicketId",
                table: "TicketRequest",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketRequestDetail_GeneralFields_FieldId",
                table: "TicketRequestDetail",
                column: "FieldId",
                principalTable: "GeneralFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketRequestDetail_TicketRequest_TicketRequestId",
                table: "TicketRequestDetail",
                column: "TicketRequestId",
                principalTable: "TicketRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
