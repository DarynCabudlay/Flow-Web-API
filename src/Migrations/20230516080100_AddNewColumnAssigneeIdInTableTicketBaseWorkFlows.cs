using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class AddNewColumnAssigneeIdInTableTicketBaseWorkFlows : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserRequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 826, DateTimeKind.Local).AddTicks(9974),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 911, DateTimeKind.Local).AddTicks(3498));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserOrganizationalStructures",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 788, DateTimeKind.Local).AddTicks(4944),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 874, DateTimeKind.Local).AddTicks(8061));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserNotAllowedLinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 817, DateTimeKind.Local).AddTicks(5535),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 902, DateTimeKind.Local).AddTicks(6220));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Tickets",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 797, DateTimeKind.Local).AddTicks(2539),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 883, DateTimeKind.Local).AddTicks(825));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequests",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 800, DateTimeKind.Local).AddTicks(85),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 885, DateTimeKind.Local).AddTicks(7685));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequestDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 803, DateTimeKind.Local).AddTicks(3924),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 889, DateTimeKind.Local).AddTicks(2362));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketLinks",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 809, DateTimeKind.Local).AddTicks(4872),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 894, DateTimeKind.Local).AddTicks(9242));

            migrationBuilder.AddColumn<int>(
                name: "AssigneeId",
                table: "TicketBaseWorkFlows",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketAttachments",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 812, DateTimeKind.Local).AddTicks(1999),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 897, DateTimeKind.Local).AddTicks(5301));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 820, DateTimeKind.Local).AddTicks(1800),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 905, DateTimeKind.Local).AddTicks(969));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupingDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 823, DateTimeKind.Local).AddTicks(2435),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 907, DateTimeKind.Local).AddTicks(9210));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 714, DateTimeKind.Local).AddTicks(5261),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 805, DateTimeKind.Local).AddTicks(4366));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 705, DateTimeKind.Local).AddTicks(1407),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 795, DateTimeKind.Local).AddTicks(9255));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 696, DateTimeKind.Local).AddTicks(8998),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 787, DateTimeKind.Local).AddTicks(4927));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 696, DateTimeKind.Local).AddTicks(8146),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 787, DateTimeKind.Local).AddTicks(3870));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLocked",
                table: "LockedRequestTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 726, DateTimeKind.Local).AddTicks(6776),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 817, DateTimeKind.Local).AddTicks(243));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "LinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 814, DateTimeKind.Local).AddTicks(5595),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 899, DateTimeKind.Local).AddTicks(7437));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Holidays",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 773, DateTimeKind.Local).AddTicks(9210),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 861, DateTimeKind.Local).AddTicks(9613));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayDates",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 776, DateTimeKind.Local).AddTicks(5288),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 864, DateTimeKind.Local).AddTicks(3508));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayAffectedOffices",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 779, DateTimeKind.Local).AddTicks(2390),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 866, DateTimeKind.Local).AddTicks(7128));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "FileTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 829, DateTimeKind.Local).AddTicks(9169),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 914, DateTimeKind.Local).AddTicks(353));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 694, DateTimeKind.Local).AddTicks(1656),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 784, DateTimeKind.Local).AddTicks(3936));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 664, DateTimeKind.Local).AddTicks(2016),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 753, DateTimeKind.Local).AddTicks(4128));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 664, DateTimeKind.Local).AddTicks(908),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 753, DateTimeKind.Local).AddTicks(3008));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 680, DateTimeKind.Local).AddTicks(2923),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 769, DateTimeKind.Local).AddTicks(7576));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 680, DateTimeKind.Local).AddTicks(1904),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 769, DateTimeKind.Local).AddTicks(6562));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 647, DateTimeKind.Local).AddTicks(7703),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 734, DateTimeKind.Local).AddTicks(4220));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 644, DateTimeKind.Local).AddTicks(1195),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 729, DateTimeKind.Local).AddTicks(6519));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ApprovalLevelDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 793, DateTimeKind.Local).AddTicks(5203),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 879, DateTimeKind.Local).AddTicks(5140));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "TicketBaseWorkFlows");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserRequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 911, DateTimeKind.Local).AddTicks(3498),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 826, DateTimeKind.Local).AddTicks(9974));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserOrganizationalStructures",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 874, DateTimeKind.Local).AddTicks(8061),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 788, DateTimeKind.Local).AddTicks(4944));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserNotAllowedLinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 902, DateTimeKind.Local).AddTicks(6220),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 817, DateTimeKind.Local).AddTicks(5535));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Tickets",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 883, DateTimeKind.Local).AddTicks(825),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 797, DateTimeKind.Local).AddTicks(2539));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequests",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 885, DateTimeKind.Local).AddTicks(7685),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 800, DateTimeKind.Local).AddTicks(85));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequestDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 889, DateTimeKind.Local).AddTicks(2362),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 803, DateTimeKind.Local).AddTicks(3924));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketLinks",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 894, DateTimeKind.Local).AddTicks(9242),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 809, DateTimeKind.Local).AddTicks(4872));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketAttachments",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 897, DateTimeKind.Local).AddTicks(5301),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 812, DateTimeKind.Local).AddTicks(1999));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 905, DateTimeKind.Local).AddTicks(969),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 820, DateTimeKind.Local).AddTicks(1800));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupingDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 907, DateTimeKind.Local).AddTicks(9210),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 823, DateTimeKind.Local).AddTicks(2435));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 805, DateTimeKind.Local).AddTicks(4366),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 714, DateTimeKind.Local).AddTicks(5261));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 795, DateTimeKind.Local).AddTicks(9255),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 705, DateTimeKind.Local).AddTicks(1407));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 787, DateTimeKind.Local).AddTicks(4927),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 696, DateTimeKind.Local).AddTicks(8998));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 787, DateTimeKind.Local).AddTicks(3870),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 696, DateTimeKind.Local).AddTicks(8146));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLocked",
                table: "LockedRequestTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 817, DateTimeKind.Local).AddTicks(243),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 726, DateTimeKind.Local).AddTicks(6776));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "LinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 899, DateTimeKind.Local).AddTicks(7437),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 814, DateTimeKind.Local).AddTicks(5595));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Holidays",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 861, DateTimeKind.Local).AddTicks(9613),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 773, DateTimeKind.Local).AddTicks(9210));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayDates",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 864, DateTimeKind.Local).AddTicks(3508),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 776, DateTimeKind.Local).AddTicks(5288));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayAffectedOffices",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 866, DateTimeKind.Local).AddTicks(7128),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 779, DateTimeKind.Local).AddTicks(2390));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "FileTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 914, DateTimeKind.Local).AddTicks(353),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 829, DateTimeKind.Local).AddTicks(9169));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 784, DateTimeKind.Local).AddTicks(3936),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 694, DateTimeKind.Local).AddTicks(1656));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 753, DateTimeKind.Local).AddTicks(4128),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 664, DateTimeKind.Local).AddTicks(2016));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 753, DateTimeKind.Local).AddTicks(3008),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 664, DateTimeKind.Local).AddTicks(908));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 769, DateTimeKind.Local).AddTicks(7576),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 680, DateTimeKind.Local).AddTicks(2923));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 769, DateTimeKind.Local).AddTicks(6562),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 680, DateTimeKind.Local).AddTicks(1904));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 734, DateTimeKind.Local).AddTicks(4220),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 647, DateTimeKind.Local).AddTicks(7703));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 729, DateTimeKind.Local).AddTicks(6519),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 644, DateTimeKind.Local).AddTicks(1195));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ApprovalLevelDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 14, 0, 6, 879, DateTimeKind.Local).AddTicks(5140),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 793, DateTimeKind.Local).AddTicks(5203));
        }
    }
}
