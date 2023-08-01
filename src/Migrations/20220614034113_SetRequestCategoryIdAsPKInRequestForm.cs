using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class SetRequestCategoryIdAsPKInRequestForm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestForms",
                table: "RequestForms");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 437, DateTimeKind.Local).AddTicks(6144),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 623, DateTimeKind.Local).AddTicks(4270));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 433, DateTimeKind.Local).AddTicks(7514),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 617, DateTimeKind.Local).AddTicks(4372));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 428, DateTimeKind.Local).AddTicks(1288),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 609, DateTimeKind.Local).AddTicks(2011));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 428, DateTimeKind.Local).AddTicks(239),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 609, DateTimeKind.Local).AddTicks(658));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 425, DateTimeKind.Local).AddTicks(1379),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 604, DateTimeKind.Local).AddTicks(6587));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 389, DateTimeKind.Local).AddTicks(9859),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 557, DateTimeKind.Local).AddTicks(6265));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 389, DateTimeKind.Local).AddTicks(8166),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 557, DateTimeKind.Local).AddTicks(4588));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 408, DateTimeKind.Local).AddTicks(6945),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 581, DateTimeKind.Local).AddTicks(2786));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 408, DateTimeKind.Local).AddTicks(5889),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 581, DateTimeKind.Local).AddTicks(1265));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 367, DateTimeKind.Local).AddTicks(661),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 539, DateTimeKind.Local).AddTicks(1258));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 363, DateTimeKind.Local).AddTicks(7074),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 535, DateTimeKind.Local).AddTicks(7094));

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestForms",
                table: "RequestForms",
                columns: new[] { "RequestCategoryId", "RequestTypeId", "Version", "FieldId" });

            migrationBuilder.AddForeignKey(
                name: "FK_RequestForms_RequestTypes_RequestCategoryId_RequestTypeId_Version",
                table: "RequestForms",
                columns: new[] { "RequestCategoryId", "RequestTypeId", "Version" },
                principalTable: "RequestTypes",
                principalColumns: new[] { "RequestCategoryId", "Id", "Version" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestForms",
                table: "RequestForms");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 623, DateTimeKind.Local).AddTicks(4270),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 437, DateTimeKind.Local).AddTicks(6144));

            migrationBuilder.AddColumn<int>(
                name: "RequestTypeId1",
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
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 433, DateTimeKind.Local).AddTicks(7514));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 609, DateTimeKind.Local).AddTicks(2011),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 428, DateTimeKind.Local).AddTicks(1288));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 609, DateTimeKind.Local).AddTicks(658),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 428, DateTimeKind.Local).AddTicks(239));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 604, DateTimeKind.Local).AddTicks(6587),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 425, DateTimeKind.Local).AddTicks(1379));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 557, DateTimeKind.Local).AddTicks(6265),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 389, DateTimeKind.Local).AddTicks(9859));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 557, DateTimeKind.Local).AddTicks(4588),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 389, DateTimeKind.Local).AddTicks(8166));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 581, DateTimeKind.Local).AddTicks(2786),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 408, DateTimeKind.Local).AddTicks(6945));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 581, DateTimeKind.Local).AddTicks(1265),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 408, DateTimeKind.Local).AddTicks(5889));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 539, DateTimeKind.Local).AddTicks(1258),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 367, DateTimeKind.Local).AddTicks(661));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 11, 38, 44, 535, DateTimeKind.Local).AddTicks(7094),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 11, 41, 13, 363, DateTimeKind.Local).AddTicks(7074));

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestForms",
                table: "RequestForms",
                columns: new[] { "RequestTypeId", "Version", "FieldId" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestForms_RequestTypeRequestCategoryId_RequestTypeId1_RequestTypeVersion",
                table: "RequestForms",
                columns: new[] { "RequestTypeRequestCategoryId", "RequestTypeId1", "RequestTypeVersion" });

            migrationBuilder.AddForeignKey(
                name: "FK_RequestForms_RequestTypes_RequestTypeRequestCategoryId_RequestTypeId1_RequestTypeVersion",
                table: "RequestForms",
                columns: new[] { "RequestTypeRequestCategoryId", "RequestTypeId1", "RequestTypeVersion" },
                principalTable: "RequestTypes",
                principalColumns: new[] { "RequestCategoryId", "Id", "Version" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
