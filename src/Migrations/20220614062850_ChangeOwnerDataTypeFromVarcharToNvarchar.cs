using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class ChangeOwnerDataTypeFromVarcharToNvarchar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 552, DateTimeKind.Local).AddTicks(86),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 971, DateTimeKind.Local).AddTicks(2856));

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "RequestTypeProcessOwners",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 547, DateTimeKind.Local).AddTicks(9674),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 968, DateTimeKind.Local).AddTicks(581));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 542, DateTimeKind.Local).AddTicks(9546),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 963, DateTimeKind.Local).AddTicks(8287));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 542, DateTimeKind.Local).AddTicks(8778),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 963, DateTimeKind.Local).AddTicks(7351));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 538, DateTimeKind.Local).AddTicks(5181),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 960, DateTimeKind.Local).AddTicks(9187));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 512, DateTimeKind.Local).AddTicks(8979),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 935, DateTimeKind.Local).AddTicks(2061));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 512, DateTimeKind.Local).AddTicks(8133),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 935, DateTimeKind.Local).AddTicks(1153));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 525, DateTimeKind.Local).AddTicks(8004),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 948, DateTimeKind.Local).AddTicks(4054));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 525, DateTimeKind.Local).AddTicks(7226),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 948, DateTimeKind.Local).AddTicks(3209));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 499, DateTimeKind.Local).AddTicks(9568),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 921, DateTimeKind.Local).AddTicks(5111));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 496, DateTimeKind.Local).AddTicks(7864),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 918, DateTimeKind.Local).AddTicks(5690));

            migrationBuilder.CreateIndex(
                name: "IX_RequestTypeProcessOwners_Owner",
                table: "RequestTypeProcessOwners",
                column: "Owner");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTypeProcessOwners_AspNetUsers_Owner",
                table: "RequestTypeProcessOwners",
                column: "Owner",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestTypeProcessOwners_AspNetUsers_Owner",
                table: "RequestTypeProcessOwners");

            migrationBuilder.DropIndex(
                name: "IX_RequestTypeProcessOwners_Owner",
                table: "RequestTypeProcessOwners");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 971, DateTimeKind.Local).AddTicks(2856),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 552, DateTimeKind.Local).AddTicks(86));

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "RequestTypeProcessOwners",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 968, DateTimeKind.Local).AddTicks(581),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 547, DateTimeKind.Local).AddTicks(9674));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 963, DateTimeKind.Local).AddTicks(8287),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 542, DateTimeKind.Local).AddTicks(9546));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 963, DateTimeKind.Local).AddTicks(7351),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 542, DateTimeKind.Local).AddTicks(8778));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 960, DateTimeKind.Local).AddTicks(9187),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 538, DateTimeKind.Local).AddTicks(5181));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 935, DateTimeKind.Local).AddTicks(2061),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 512, DateTimeKind.Local).AddTicks(8979));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 935, DateTimeKind.Local).AddTicks(1153),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 512, DateTimeKind.Local).AddTicks(8133));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 948, DateTimeKind.Local).AddTicks(4054),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 525, DateTimeKind.Local).AddTicks(8004));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 948, DateTimeKind.Local).AddTicks(3209),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 525, DateTimeKind.Local).AddTicks(7226));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 921, DateTimeKind.Local).AddTicks(5111),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 499, DateTimeKind.Local).AddTicks(9568));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 13, 39, 24, 918, DateTimeKind.Local).AddTicks(5690),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 14, 28, 50, 496, DateTimeKind.Local).AddTicks(7864));
        }
    }
}
