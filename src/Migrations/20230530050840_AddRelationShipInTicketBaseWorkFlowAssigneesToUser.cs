using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class AddRelationShipInTicketBaseWorkFlowAssigneesToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserRequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 778, DateTimeKind.Local).AddTicks(3049),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 256, DateTimeKind.Local).AddTicks(269));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserOrganizationalStructures",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 749, DateTimeKind.Local).AddTicks(6598),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 219, DateTimeKind.Local).AddTicks(1936));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserNotAllowedLinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 772, DateTimeKind.Local).AddTicks(4541),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 247, DateTimeKind.Local).AddTicks(275));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Tickets",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 756, DateTimeKind.Local).AddTicks(6504),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 227, DateTimeKind.Local).AddTicks(5943));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequests",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 758, DateTimeKind.Local).AddTicks(8280),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 230, DateTimeKind.Local).AddTicks(2802));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequestDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 761, DateTimeKind.Local).AddTicks(4372),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 233, DateTimeKind.Local).AddTicks(6431));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketLinks",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 765, DateTimeKind.Local).AddTicks(5737),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 238, DateTimeKind.Local).AddTicks(9454));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketAttachments",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 767, DateTimeKind.Local).AddTicks(6746),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 241, DateTimeKind.Local).AddTicks(6706));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 774, DateTimeKind.Local).AddTicks(3290),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 249, DateTimeKind.Local).AddTicks(6108));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupingDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 776, DateTimeKind.Local).AddTicks(1682),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 252, DateTimeKind.Local).AddTicks(5432));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 694, DateTimeKind.Local).AddTicks(4482),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 148, DateTimeKind.Local).AddTicks(5004));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 686, DateTimeKind.Local).AddTicks(7726),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 138, DateTimeKind.Local).AddTicks(9170));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 680, DateTimeKind.Local).AddTicks(3702),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 130, DateTimeKind.Local).AddTicks(5413));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 680, DateTimeKind.Local).AddTicks(2956),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 130, DateTimeKind.Local).AddTicks(4383));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLocked",
                table: "LockedRequestTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 703, DateTimeKind.Local).AddTicks(3717),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 160, DateTimeKind.Local).AddTicks(32));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "LinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 770, DateTimeKind.Local).AddTicks(1293),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 244, DateTimeKind.Local).AddTicks(641));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Holidays",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 739, DateTimeKind.Local).AddTicks(2700),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 206, DateTimeKind.Local).AddTicks(3857));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayDates",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 741, DateTimeKind.Local).AddTicks(1518),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 208, DateTimeKind.Local).AddTicks(7793));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayAffectedOffices",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 743, DateTimeKind.Local).AddTicks(538),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 211, DateTimeKind.Local).AddTicks(1146));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "FileTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 780, DateTimeKind.Local).AddTicks(1168),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 258, DateTimeKind.Local).AddTicks(7776));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 678, DateTimeKind.Local).AddTicks(1341),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 127, DateTimeKind.Local).AddTicks(6094));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 654, DateTimeKind.Local).AddTicks(4726),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 96, DateTimeKind.Local).AddTicks(9025));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 654, DateTimeKind.Local).AddTicks(3596),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 96, DateTimeKind.Local).AddTicks(7233));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 667, DateTimeKind.Local).AddTicks(1353),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 113, DateTimeKind.Local).AddTicks(983));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 667, DateTimeKind.Local).AddTicks(566),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 112, DateTimeKind.Local).AddTicks(9911));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 641, DateTimeKind.Local).AddTicks(2513),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 72, DateTimeKind.Local).AddTicks(2289));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 638, DateTimeKind.Local).AddTicks(4870),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 67, DateTimeKind.Local).AddTicks(7557));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ApprovalLevelDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 753, DateTimeKind.Local).AddTicks(5777),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 223, DateTimeKind.Local).AddTicks(9231));

            migrationBuilder.CreateIndex(
                name: "IX_TicketBaseWorkFlowAssignees_UserId",
                table: "TicketBaseWorkFlowAssignees",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketBaseWorkFlowAssignees_AspNetUsers_UserId",
                table: "TicketBaseWorkFlowAssignees",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketBaseWorkFlowAssignees_AspNetUsers_UserId",
                table: "TicketBaseWorkFlowAssignees");

            migrationBuilder.DropIndex(
                name: "IX_TicketBaseWorkFlowAssignees_UserId",
                table: "TicketBaseWorkFlowAssignees");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserRequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 256, DateTimeKind.Local).AddTicks(269),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 778, DateTimeKind.Local).AddTicks(3049));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserOrganizationalStructures",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 219, DateTimeKind.Local).AddTicks(1936),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 749, DateTimeKind.Local).AddTicks(6598));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserNotAllowedLinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 247, DateTimeKind.Local).AddTicks(275),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 772, DateTimeKind.Local).AddTicks(4541));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Tickets",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 227, DateTimeKind.Local).AddTicks(5943),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 756, DateTimeKind.Local).AddTicks(6504));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequests",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 230, DateTimeKind.Local).AddTicks(2802),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 758, DateTimeKind.Local).AddTicks(8280));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketRequestDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 233, DateTimeKind.Local).AddTicks(6431),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 761, DateTimeKind.Local).AddTicks(4372));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketLinks",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 238, DateTimeKind.Local).AddTicks(9454),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 765, DateTimeKind.Local).AddTicks(5737));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "TicketAttachments",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 241, DateTimeKind.Local).AddTicks(6706),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 767, DateTimeKind.Local).AddTicks(6746));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupings",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 249, DateTimeKind.Local).AddTicks(6108),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 774, DateTimeKind.Local).AddTicks(3290));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypesGroupingDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 252, DateTimeKind.Local).AddTicks(5432),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 776, DateTimeKind.Local).AddTicks(1682));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 148, DateTimeKind.Local).AddTicks(5004),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 694, DateTimeKind.Local).AddTicks(4482));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 138, DateTimeKind.Local).AddTicks(9170),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 686, DateTimeKind.Local).AddTicks(7726));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 130, DateTimeKind.Local).AddTicks(5413),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 680, DateTimeKind.Local).AddTicks(3702));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 130, DateTimeKind.Local).AddTicks(4383),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 680, DateTimeKind.Local).AddTicks(2956));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLocked",
                table: "LockedRequestTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 160, DateTimeKind.Local).AddTicks(32),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 703, DateTimeKind.Local).AddTicks(3717));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "LinkTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 244, DateTimeKind.Local).AddTicks(641),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 770, DateTimeKind.Local).AddTicks(1293));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Holidays",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 206, DateTimeKind.Local).AddTicks(3857),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 739, DateTimeKind.Local).AddTicks(2700));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayDates",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 208, DateTimeKind.Local).AddTicks(7793),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 741, DateTimeKind.Local).AddTicks(1518));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "HolidayAffectedOffices",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 211, DateTimeKind.Local).AddTicks(1146),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 743, DateTimeKind.Local).AddTicks(538));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "FileTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 258, DateTimeKind.Local).AddTicks(7776),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 780, DateTimeKind.Local).AddTicks(1168));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 127, DateTimeKind.Local).AddTicks(6094),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 678, DateTimeKind.Local).AddTicks(1341));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 96, DateTimeKind.Local).AddTicks(9025),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 654, DateTimeKind.Local).AddTicks(4726));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 96, DateTimeKind.Local).AddTicks(7233),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 654, DateTimeKind.Local).AddTicks(3596));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 113, DateTimeKind.Local).AddTicks(983),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 667, DateTimeKind.Local).AddTicks(1353));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 112, DateTimeKind.Local).AddTicks(9911),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 667, DateTimeKind.Local).AddTicks(566));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 72, DateTimeKind.Local).AddTicks(2289),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 641, DateTimeKind.Local).AddTicks(2513));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 67, DateTimeKind.Local).AddTicks(7557),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 638, DateTimeKind.Local).AddTicks(4870));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ApprovalLevelDetails",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2023, 5, 29, 13, 44, 1, 223, DateTimeKind.Local).AddTicks(9231),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2023, 5, 30, 13, 8, 39, 753, DateTimeKind.Local).AddTicks(5777));
        }
    }
}
