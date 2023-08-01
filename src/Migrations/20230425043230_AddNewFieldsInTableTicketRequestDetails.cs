using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class AddNewFieldsInTableTicketRequestDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserRequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 190, DateTimeKind.Local).AddTicks(2455),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 223, DateTimeKind.Local).AddTicks(8015));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserOrganizationalStructures",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 149, DateTimeKind.Local).AddTicks(1360),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 190, DateTimeKind.Local).AddTicks(7711));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserNotAllowedLinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 179, DateTimeKind.Local).AddTicks(8256),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 215, DateTimeKind.Local).AddTicks(5139));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Tickets",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 157, DateTimeKind.Local).AddTicks(5947),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 198, DateTimeKind.Local).AddTicks(917));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequestDetail",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 164, DateTimeKind.Local).AddTicks(8211),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 203, DateTimeKind.Local).AddTicks(4401));

            migrationBuilder.AddColumn<string>(
                name: "ValueCode",
                table: "TicketRequestDetail",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ValueCode2",
                table: "TicketRequestDetail",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequest",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 160, DateTimeKind.Local).AddTicks(2998),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 200, DateTimeKind.Local).AddTicks(3662));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketLinks",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 171, DateTimeKind.Local).AddTicks(1458),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 208, DateTimeKind.Local).AddTicks(5687));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketAttachments",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 173, DateTimeKind.Local).AddTicks(7183),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 210, DateTimeKind.Local).AddTicks(7897));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 182, DateTimeKind.Local).AddTicks(7178),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 217, DateTimeKind.Local).AddTicks(8095));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupingDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 185, DateTimeKind.Local).AddTicks(9377),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 220, DateTimeKind.Local).AddTicks(7166));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 71, DateTimeKind.Local).AddTicks(2940),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 128, DateTimeKind.Local).AddTicks(4700));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 59, DateTimeKind.Local).AddTicks(2519),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 120, DateTimeKind.Local).AddTicks(2955));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 49, DateTimeKind.Local).AddTicks(9220),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 112, DateTimeKind.Local).AddTicks(7390));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 49, DateTimeKind.Local).AddTicks(7797),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 112, DateTimeKind.Local).AddTicks(6567));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLocked",
                table: "LockedRequestTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 85, DateTimeKind.Local).AddTicks(3643),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 138, DateTimeKind.Local).AddTicks(8580));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "LinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 176, DateTimeKind.Local).AddTicks(1673),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 212, DateTimeKind.Local).AddTicks(8578));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Holidays",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 135, DateTimeKind.Local).AddTicks(2802),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 179, DateTimeKind.Local).AddTicks(1408));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayDates",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 137, DateTimeKind.Local).AddTicks(7061),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 181, DateTimeKind.Local).AddTicks(2715));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayAffectedOffices",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 140, DateTimeKind.Local).AddTicks(1611),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 183, DateTimeKind.Local).AddTicks(3710));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "FileTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 193, DateTimeKind.Local).AddTicks(5556),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 226, DateTimeKind.Local).AddTicks(1873));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 45, DateTimeKind.Local).AddTicks(4373),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 110, DateTimeKind.Local).AddTicks(1142));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 7, DateTimeKind.Local).AddTicks(9434),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 83, DateTimeKind.Local).AddTicks(814));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 7, DateTimeKind.Local).AddTicks(8095),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 82, DateTimeKind.Local).AddTicks(9785));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 29, DateTimeKind.Local).AddTicks(8900),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 97, DateTimeKind.Local).AddTicks(4779));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 29, DateTimeKind.Local).AddTicks(7501),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 97, DateTimeKind.Local).AddTicks(3849));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 28, 986, DateTimeKind.Local).AddTicks(2346),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 66, DateTimeKind.Local).AddTicks(9146));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 28, 982, DateTimeKind.Local).AddTicks(5458),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 63, DateTimeKind.Local).AddTicks(7193));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ApprovalLevelDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 154, DateTimeKind.Local).AddTicks(737),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 195, DateTimeKind.Local).AddTicks(333));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValueCode",
                table: "TicketRequestDetail");

            migrationBuilder.DropColumn(
                name: "ValueCode2",
                table: "TicketRequestDetail");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserRequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 223, DateTimeKind.Local).AddTicks(8015),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 190, DateTimeKind.Local).AddTicks(2455));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserOrganizationalStructures",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 190, DateTimeKind.Local).AddTicks(7711),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 149, DateTimeKind.Local).AddTicks(1360));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserNotAllowedLinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 215, DateTimeKind.Local).AddTicks(5139),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 179, DateTimeKind.Local).AddTicks(8256));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Tickets",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 198, DateTimeKind.Local).AddTicks(917),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 157, DateTimeKind.Local).AddTicks(5947));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequestDetail",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 203, DateTimeKind.Local).AddTicks(4401),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 164, DateTimeKind.Local).AddTicks(8211));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequest",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 200, DateTimeKind.Local).AddTicks(3662),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 160, DateTimeKind.Local).AddTicks(2998));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketLinks",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 208, DateTimeKind.Local).AddTicks(5687),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 171, DateTimeKind.Local).AddTicks(1458));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketAttachments",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 210, DateTimeKind.Local).AddTicks(7897),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 173, DateTimeKind.Local).AddTicks(7183));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 217, DateTimeKind.Local).AddTicks(8095),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 182, DateTimeKind.Local).AddTicks(7178));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupingDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 220, DateTimeKind.Local).AddTicks(7166),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 185, DateTimeKind.Local).AddTicks(9377));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 128, DateTimeKind.Local).AddTicks(4700),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 71, DateTimeKind.Local).AddTicks(2940));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 120, DateTimeKind.Local).AddTicks(2955),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 59, DateTimeKind.Local).AddTicks(2519));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 112, DateTimeKind.Local).AddTicks(7390),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 49, DateTimeKind.Local).AddTicks(9220));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 112, DateTimeKind.Local).AddTicks(6567),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 49, DateTimeKind.Local).AddTicks(7797));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLocked",
                table: "LockedRequestTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 138, DateTimeKind.Local).AddTicks(8580),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 85, DateTimeKind.Local).AddTicks(3643));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "LinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 212, DateTimeKind.Local).AddTicks(8578),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 176, DateTimeKind.Local).AddTicks(1673));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Holidays",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 179, DateTimeKind.Local).AddTicks(1408),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 135, DateTimeKind.Local).AddTicks(2802));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayDates",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 181, DateTimeKind.Local).AddTicks(2715),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 137, DateTimeKind.Local).AddTicks(7061));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayAffectedOffices",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 183, DateTimeKind.Local).AddTicks(3710),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 140, DateTimeKind.Local).AddTicks(1611));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "FileTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 226, DateTimeKind.Local).AddTicks(1873),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 193, DateTimeKind.Local).AddTicks(5556));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 110, DateTimeKind.Local).AddTicks(1142),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 45, DateTimeKind.Local).AddTicks(4373));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 83, DateTimeKind.Local).AddTicks(814),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 7, DateTimeKind.Local).AddTicks(9434));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 82, DateTimeKind.Local).AddTicks(9785),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 7, DateTimeKind.Local).AddTicks(8095));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 97, DateTimeKind.Local).AddTicks(4779),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 29, DateTimeKind.Local).AddTicks(8900));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 97, DateTimeKind.Local).AddTicks(3849),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 29, DateTimeKind.Local).AddTicks(7501));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 66, DateTimeKind.Local).AddTicks(9146),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 28, 986, DateTimeKind.Local).AddTicks(2346));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 63, DateTimeKind.Local).AddTicks(7193),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 28, 982, DateTimeKind.Local).AddTicks(5458));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ApprovalLevelDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 4, 24, 7, 58, 42, 195, DateTimeKind.Local).AddTicks(333),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 4, 25, 12, 32, 29, 154, DateTimeKind.Local).AddTicks(737));
        }
    }
}
