using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class RemoveFieldRequestWorkFlowIdInTableTicketBaseWorkFlows : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketBaseWorkFlows_RequestTypeWorkFlows_RequestTypeWorkFlowId",
                table: "TicketBaseWorkFlows");

            migrationBuilder.DropIndex(
                name: "IX_TicketAndRequestTypeWorkFlow",
                table: "TicketBaseWorkFlows");

            migrationBuilder.DropIndex(
                name: "IX_TicketBaseWorkFlows_RequestTypeWorkFlowId",
                table: "TicketBaseWorkFlows");

            migrationBuilder.DropColumn(
                name: "RequestTypeWorkFlowId",
                table: "TicketBaseWorkFlows");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserRequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 740, DateTimeKind.Local).AddTicks(5006),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 826, DateTimeKind.Local).AddTicks(9974));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserOrganizationalStructures",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 715, DateTimeKind.Local).AddTicks(1835),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 788, DateTimeKind.Local).AddTicks(4944));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserNotAllowedLinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 734, DateTimeKind.Local).AddTicks(324),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 817, DateTimeKind.Local).AddTicks(5535));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Tickets",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 720, DateTimeKind.Local).AddTicks(4742),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 797, DateTimeKind.Local).AddTicks(2539));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequests",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 722, DateTimeKind.Local).AddTicks(6485),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 800, DateTimeKind.Local).AddTicks(85));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequestDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 724, DateTimeKind.Local).AddTicks(9147),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 803, DateTimeKind.Local).AddTicks(3924));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketLinks",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 728, DateTimeKind.Local).AddTicks(5369),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 809, DateTimeKind.Local).AddTicks(4872));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketAttachments",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 730, DateTimeKind.Local).AddTicks(3085),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 812, DateTimeKind.Local).AddTicks(1999));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 735, DateTimeKind.Local).AddTicks(8426),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 820, DateTimeKind.Local).AddTicks(1800));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupingDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 737, DateTimeKind.Local).AddTicks(9994),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 823, DateTimeKind.Local).AddTicks(2435));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 664, DateTimeKind.Local).AddTicks(8806),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 714, DateTimeKind.Local).AddTicks(5261));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 658, DateTimeKind.Local).AddTicks(1793),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 705, DateTimeKind.Local).AddTicks(1407));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 650, DateTimeKind.Local).AddTicks(9665),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 696, DateTimeKind.Local).AddTicks(8998));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 650, DateTimeKind.Local).AddTicks(8970),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 696, DateTimeKind.Local).AddTicks(8146));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLocked",
                table: "LockedRequestTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 673, DateTimeKind.Local).AddTicks(7625),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 726, DateTimeKind.Local).AddTicks(6776));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "LinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 731, DateTimeKind.Local).AddTicks(9273),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 814, DateTimeKind.Local).AddTicks(5595));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Holidays",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 705, DateTimeKind.Local).AddTicks(9386),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 773, DateTimeKind.Local).AddTicks(9210));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayDates",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 708, DateTimeKind.Local).AddTicks(1125),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 776, DateTimeKind.Local).AddTicks(5288));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayAffectedOffices",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 709, DateTimeKind.Local).AddTicks(8542),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 779, DateTimeKind.Local).AddTicks(2390));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "FileTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 742, DateTimeKind.Local).AddTicks(4857),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 829, DateTimeKind.Local).AddTicks(9169));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 648, DateTimeKind.Local).AddTicks(8485),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 694, DateTimeKind.Local).AddTicks(1656));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 621, DateTimeKind.Local).AddTicks(3222),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 664, DateTimeKind.Local).AddTicks(2016));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 621, DateTimeKind.Local).AddTicks(1079),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 664, DateTimeKind.Local).AddTicks(908));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 639, DateTimeKind.Local).AddTicks(75),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 680, DateTimeKind.Local).AddTicks(2923));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 638, DateTimeKind.Local).AddTicks(9416),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 680, DateTimeKind.Local).AddTicks(1904));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 606, DateTimeKind.Local).AddTicks(6599),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 647, DateTimeKind.Local).AddTicks(7703));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 602, DateTimeKind.Local).AddTicks(8939),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 644, DateTimeKind.Local).AddTicks(1195));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ApprovalLevelDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 718, DateTimeKind.Local).AddTicks(1434),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 793, DateTimeKind.Local).AddTicks(5203));

            migrationBuilder.CreateIndex(
                name: "IX_TicketAndStepId",
                table: "TicketBaseWorkFlows",
                columns: new[] { "TicketId", "StepId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TicketAndStepId",
                table: "TicketBaseWorkFlows");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserRequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 826, DateTimeKind.Local).AddTicks(9974),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 740, DateTimeKind.Local).AddTicks(5006));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserOrganizationalStructures",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 788, DateTimeKind.Local).AddTicks(4944),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 715, DateTimeKind.Local).AddTicks(1835));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserNotAllowedLinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 817, DateTimeKind.Local).AddTicks(5535),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 734, DateTimeKind.Local).AddTicks(324));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Tickets",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 797, DateTimeKind.Local).AddTicks(2539),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 720, DateTimeKind.Local).AddTicks(4742));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequests",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 800, DateTimeKind.Local).AddTicks(85),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 722, DateTimeKind.Local).AddTicks(6485));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequestDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 803, DateTimeKind.Local).AddTicks(3924),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 724, DateTimeKind.Local).AddTicks(9147));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketLinks",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 809, DateTimeKind.Local).AddTicks(4872),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 728, DateTimeKind.Local).AddTicks(5369));

            migrationBuilder.AddColumn<int>(
                name: "RequestTypeWorkFlowId",
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
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 730, DateTimeKind.Local).AddTicks(3085));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 820, DateTimeKind.Local).AddTicks(1800),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 735, DateTimeKind.Local).AddTicks(8426));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupingDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 823, DateTimeKind.Local).AddTicks(2435),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 737, DateTimeKind.Local).AddTicks(9994));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 714, DateTimeKind.Local).AddTicks(5261),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 664, DateTimeKind.Local).AddTicks(8806));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 705, DateTimeKind.Local).AddTicks(1407),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 658, DateTimeKind.Local).AddTicks(1793));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 696, DateTimeKind.Local).AddTicks(8998),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 650, DateTimeKind.Local).AddTicks(9665));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 696, DateTimeKind.Local).AddTicks(8146),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 650, DateTimeKind.Local).AddTicks(8970));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLocked",
                table: "LockedRequestTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 726, DateTimeKind.Local).AddTicks(6776),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 673, DateTimeKind.Local).AddTicks(7625));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "LinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 814, DateTimeKind.Local).AddTicks(5595),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 731, DateTimeKind.Local).AddTicks(9273));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Holidays",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 773, DateTimeKind.Local).AddTicks(9210),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 705, DateTimeKind.Local).AddTicks(9386));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayDates",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 776, DateTimeKind.Local).AddTicks(5288),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 708, DateTimeKind.Local).AddTicks(1125));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayAffectedOffices",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 779, DateTimeKind.Local).AddTicks(2390),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 709, DateTimeKind.Local).AddTicks(8542));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "FileTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 829, DateTimeKind.Local).AddTicks(9169),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 742, DateTimeKind.Local).AddTicks(4857));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 694, DateTimeKind.Local).AddTicks(1656),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 648, DateTimeKind.Local).AddTicks(8485));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 664, DateTimeKind.Local).AddTicks(2016),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 621, DateTimeKind.Local).AddTicks(3222));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 664, DateTimeKind.Local).AddTicks(908),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 621, DateTimeKind.Local).AddTicks(1079));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 680, DateTimeKind.Local).AddTicks(2923),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 639, DateTimeKind.Local).AddTicks(75));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 680, DateTimeKind.Local).AddTicks(1904),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 638, DateTimeKind.Local).AddTicks(9416));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 647, DateTimeKind.Local).AddTicks(7703),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 606, DateTimeKind.Local).AddTicks(6599));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 644, DateTimeKind.Local).AddTicks(1195),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 602, DateTimeKind.Local).AddTicks(8939));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ApprovalLevelDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 16, 16, 0, 59, 793, DateTimeKind.Local).AddTicks(5203),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 22, 12, 6, 48, 718, DateTimeKind.Local).AddTicks(1434));

            migrationBuilder.CreateIndex(
                name: "IX_TicketAndRequestTypeWorkFlow",
                table: "TicketBaseWorkFlows",
                columns: new[] { "TicketId", "RequestTypeWorkFlowId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketBaseWorkFlows_RequestTypeWorkFlowId",
                table: "TicketBaseWorkFlows",
                column: "RequestTypeWorkFlowId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketBaseWorkFlows_RequestTypeWorkFlows_RequestTypeWorkFlowId",
                table: "TicketBaseWorkFlows",
                column: "RequestTypeWorkFlowId",
                principalTable: "RequestTypeWorkFlows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
