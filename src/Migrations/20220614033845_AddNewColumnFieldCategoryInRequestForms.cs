using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class AddNewColumnFieldCategoryInRequestForms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 623, DateTimeKind.Local).AddTicks(4270),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 45, 5, 27, DateTimeKind.Local).AddTicks(710));

            migrationBuilder.AddColumn<int>(
                name: "RequestCategoryId",
                table: "RequestForms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 617, DateTimeKind.Local).AddTicks(4372),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 45, 5, 24, DateTimeKind.Local).AddTicks(2888));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 609, DateTimeKind.Local).AddTicks(2011),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 45, 5, 20, DateTimeKind.Local).AddTicks(1217));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 609, DateTimeKind.Local).AddTicks(658),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 45, 5, 20, DateTimeKind.Local).AddTicks(349));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 604, DateTimeKind.Local).AddTicks(6587),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 45, 5, 17, DateTimeKind.Local).AddTicks(9284));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 557, DateTimeKind.Local).AddTicks(6265),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 45, 4, 996, DateTimeKind.Local).AddTicks(5935));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 557, DateTimeKind.Local).AddTicks(4588),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 45, 4, 996, DateTimeKind.Local).AddTicks(5135));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 581, DateTimeKind.Local).AddTicks(2786),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 45, 5, 7, DateTimeKind.Local).AddTicks(2222));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 581, DateTimeKind.Local).AddTicks(1265),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 45, 5, 7, DateTimeKind.Local).AddTicks(1461));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 539, DateTimeKind.Local).AddTicks(1258),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 45, 4, 983, DateTimeKind.Local).AddTicks(8265));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 535, DateTimeKind.Local).AddTicks(7094),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 45, 4, 981, DateTimeKind.Local).AddTicks(1208));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestCategoryId",
                table: "RequestForms");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 45, 5, 27, DateTimeKind.Local).AddTicks(710),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 623, DateTimeKind.Local).AddTicks(4270));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 45, 5, 24, DateTimeKind.Local).AddTicks(2888),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 617, DateTimeKind.Local).AddTicks(4372));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 13, 15, 45, 5, 20, DateTimeKind.Local).AddTicks(1217),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 609, DateTimeKind.Local).AddTicks(2011));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 45, 5, 20, DateTimeKind.Local).AddTicks(349),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 609, DateTimeKind.Local).AddTicks(658));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 45, 5, 17, DateTimeKind.Local).AddTicks(9284),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 604, DateTimeKind.Local).AddTicks(6587));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 13, 15, 45, 4, 996, DateTimeKind.Local).AddTicks(5935),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 557, DateTimeKind.Local).AddTicks(6265));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 45, 4, 996, DateTimeKind.Local).AddTicks(5135),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 557, DateTimeKind.Local).AddTicks(4588));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 13, 15, 45, 5, 7, DateTimeKind.Local).AddTicks(2222),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 581, DateTimeKind.Local).AddTicks(2786));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 45, 5, 7, DateTimeKind.Local).AddTicks(1461),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 581, DateTimeKind.Local).AddTicks(1265));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 13, 15, 45, 4, 983, DateTimeKind.Local).AddTicks(8265),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 539, DateTimeKind.Local).AddTicks(1258));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 45, 4, 981, DateTimeKind.Local).AddTicks(1208),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 535, DateTimeKind.Local).AddTicks(7094));
        }
    }
}
