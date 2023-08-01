using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class AddTableLockedRequestTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 585, DateTimeKind.Local).AddTicks(9215),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 951, DateTimeKind.Local).AddTicks(8924));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 578, DateTimeKind.Local).AddTicks(8543),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 946, DateTimeKind.Local).AddTicks(9449));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 570, DateTimeKind.Local).AddTicks(7499),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 940, DateTimeKind.Local).AddTicks(3996));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 570, DateTimeKind.Local).AddTicks(6148),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 940, DateTimeKind.Local).AddTicks(3099));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 566, DateTimeKind.Local).AddTicks(9700),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 937, DateTimeKind.Local).AddTicks(2590));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 531, DateTimeKind.Local).AddTicks(5640),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 904, DateTimeKind.Local).AddTicks(5128));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 531, DateTimeKind.Local).AddTicks(3930),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 904, DateTimeKind.Local).AddTicks(4013));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 549, DateTimeKind.Local).AddTicks(7364),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 920, DateTimeKind.Local).AddTicks(4143));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 549, DateTimeKind.Local).AddTicks(6281),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 920, DateTimeKind.Local).AddTicks(3115));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 511, DateTimeKind.Local).AddTicks(6934),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 882, DateTimeKind.Local).AddTicks(9529));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 508, DateTimeKind.Local).AddTicks(2427),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 879, DateTimeKind.Local).AddTicks(7398));

            migrationBuilder.CreateTable(
                name: "LockedRequestTypes",
                columns: table => new
                {
                    RequestTypeId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    User = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    DateLocked = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 605, DateTimeKind.Local).AddTicks(6771))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LockedRequestTypes", x => new { x.RequestTypeId, x.Version });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LockedRequestTypes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 951, DateTimeKind.Local).AddTicks(8924),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 585, DateTimeKind.Local).AddTicks(9215));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 946, DateTimeKind.Local).AddTicks(9449),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 578, DateTimeKind.Local).AddTicks(8543));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 940, DateTimeKind.Local).AddTicks(3996),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 570, DateTimeKind.Local).AddTicks(7499));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 940, DateTimeKind.Local).AddTicks(3099),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 570, DateTimeKind.Local).AddTicks(6148));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 937, DateTimeKind.Local).AddTicks(2590),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 566, DateTimeKind.Local).AddTicks(9700));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 904, DateTimeKind.Local).AddTicks(5128),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 531, DateTimeKind.Local).AddTicks(5640));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 904, DateTimeKind.Local).AddTicks(4013),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 531, DateTimeKind.Local).AddTicks(3930));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 920, DateTimeKind.Local).AddTicks(4143),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 549, DateTimeKind.Local).AddTicks(7364));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 920, DateTimeKind.Local).AddTicks(3115),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 549, DateTimeKind.Local).AddTicks(6281));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 882, DateTimeKind.Local).AddTicks(9529),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 511, DateTimeKind.Local).AddTicks(6934));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 7, 4, 12, 28, 26, 879, DateTimeKind.Local).AddTicks(7398),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 7, 4, 13, 10, 20, 508, DateTimeKind.Local).AddTicks(2427));
        }
    }
}
