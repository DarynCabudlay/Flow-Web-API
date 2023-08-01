using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class NewColumnConditionInTextInTableRequestStepConditions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 228, DateTimeKind.Local).AddTicks(6564),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 202, DateTimeKind.Local).AddTicks(4852));

            migrationBuilder.AddColumn<string>(
                name: "ConditionInText",
                table: "RequestStepConditions",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 222, DateTimeKind.Local).AddTicks(7973),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 196, DateTimeKind.Local).AddTicks(9198));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 217, DateTimeKind.Local).AddTicks(9842),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 192, DateTimeKind.Local).AddTicks(3591));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 217, DateTimeKind.Local).AddTicks(8942),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 192, DateTimeKind.Local).AddTicks(2835));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLocked",
                table: "LockedRequestTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 240, DateTimeKind.Local).AddTicks(8839),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 213, DateTimeKind.Local).AddTicks(6569));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 215, DateTimeKind.Local).AddTicks(4830),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 189, DateTimeKind.Local).AddTicks(9405));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 190, DateTimeKind.Local).AddTicks(3640),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 165, DateTimeKind.Local).AddTicks(3344));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 190, DateTimeKind.Local).AddTicks(2711),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 165, DateTimeKind.Local).AddTicks(2397));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 203, DateTimeKind.Local).AddTicks(1746),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 178, DateTimeKind.Local).AddTicks(2357));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 203, DateTimeKind.Local).AddTicks(894),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 178, DateTimeKind.Local).AddTicks(1508));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 174, DateTimeKind.Local).AddTicks(7423),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 151, DateTimeKind.Local).AddTicks(4931));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 170, DateTimeKind.Local).AddTicks(9336),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 148, DateTimeKind.Local).AddTicks(8387));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConditionInText",
                table: "RequestStepConditions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 202, DateTimeKind.Local).AddTicks(4852),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 228, DateTimeKind.Local).AddTicks(6564));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 196, DateTimeKind.Local).AddTicks(9198),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 222, DateTimeKind.Local).AddTicks(7973));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 192, DateTimeKind.Local).AddTicks(3591),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 217, DateTimeKind.Local).AddTicks(9842));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 192, DateTimeKind.Local).AddTicks(2835),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 217, DateTimeKind.Local).AddTicks(8942));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLocked",
                table: "LockedRequestTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 213, DateTimeKind.Local).AddTicks(6569),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 240, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 189, DateTimeKind.Local).AddTicks(9405),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 215, DateTimeKind.Local).AddTicks(4830));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 165, DateTimeKind.Local).AddTicks(3344),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 190, DateTimeKind.Local).AddTicks(3640));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 165, DateTimeKind.Local).AddTicks(2397),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 190, DateTimeKind.Local).AddTicks(2711));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 178, DateTimeKind.Local).AddTicks(2357),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 203, DateTimeKind.Local).AddTicks(1746));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 178, DateTimeKind.Local).AddTicks(1508),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 203, DateTimeKind.Local).AddTicks(894));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 151, DateTimeKind.Local).AddTicks(4931),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 174, DateTimeKind.Local).AddTicks(7423));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 10, 14, 11, 28, 7, 148, DateTimeKind.Local).AddTicks(8387),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 10, 14, 13, 12, 38, 170, DateTimeKind.Local).AddTicks(9336));
        }
    }
}
