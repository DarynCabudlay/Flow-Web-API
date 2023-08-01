using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class RemoveRequestCategoryIdAsCompositeKeyinRequestTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestForms_RequestTypes_RequestCategoryId_RequestTypeId_Version",
                table: "RequestForms");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestTypeProcessOwners_RequestTypes_RequestCategoryId_RequestTypeId_Version",
                table: "RequestTypeProcessOwners");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestTypes",
                table: "RequestTypes");

            migrationBuilder.DropIndex(
                name: "IX_RequestTypeProcessOwners_RequestCategoryId_RequestTypeId_Version",
                table: "RequestTypeProcessOwners");

            migrationBuilder.DropIndex(
                name: "IX_RequestForms_RequestCategoryId_RequestTypeId_Version",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "RequestCategoryId",
                table: "RequestTypeProcessOwners");

            migrationBuilder.DropColumn(
                name: "RequestCategoryId",
                table: "RequestForms");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 476, DateTimeKind.Local).AddTicks(5698),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 843, DateTimeKind.Local).AddTicks(789));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 473, DateTimeKind.Local).AddTicks(3154),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 836, DateTimeKind.Local).AddTicks(3213));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 469, DateTimeKind.Local).AddTicks(2506),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 828, DateTimeKind.Local).AddTicks(1070));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 469, DateTimeKind.Local).AddTicks(1830),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 827, DateTimeKind.Local).AddTicks(9426));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 467, DateTimeKind.Local).AddTicks(1364),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 822, DateTimeKind.Local).AddTicks(7337));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 446, DateTimeKind.Local).AddTicks(8910),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 747, DateTimeKind.Local).AddTicks(9424));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 446, DateTimeKind.Local).AddTicks(8226),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 747, DateTimeKind.Local).AddTicks(7371));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 456, DateTimeKind.Local).AddTicks(8548),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 785, DateTimeKind.Local).AddTicks(9382));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 456, DateTimeKind.Local).AddTicks(7830),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 785, DateTimeKind.Local).AddTicks(6887));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 435, DateTimeKind.Local).AddTicks(286),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 714, DateTimeKind.Local).AddTicks(6079));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 432, DateTimeKind.Local).AddTicks(2934),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 709, DateTimeKind.Local).AddTicks(4750));

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestTypes",
                table: "RequestTypes",
                columns: new[] { "Id", "Version" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestTypes_RequestCategoryId",
                table: "RequestTypes",
                column: "RequestCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTypeProcessOwners_RequestCategoryId_RequestTypeId_Version",
                table: "RequestTypeProcessOwners",
                columns: new[] { "RequestTypeId", "Version" });

            migrationBuilder.AddForeignKey(
                name: "FK_RequestForms_RequestTypes_RequestTypeId_Version",
                table: "RequestForms",
                columns: new[] { "RequestTypeId", "Version" },
                principalTable: "RequestTypes",
                principalColumns: new[] { "Id", "Version" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTypeProcessOwners_RequestTypes_RequestTypeId_Version",
                table: "RequestTypeProcessOwners",
                columns: new[] { "RequestTypeId", "Version" },
                principalTable: "RequestTypes",
                principalColumns: new[] { "Id", "Version" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestForms_RequestTypes_RequestTypeId_Version",
                table: "RequestForms");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestTypeProcessOwners_RequestTypes_RequestTypeId_Version",
                table: "RequestTypeProcessOwners");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestTypes",
                table: "RequestTypes");

            migrationBuilder.DropIndex(
                name: "IX_RequestTypes_RequestCategoryId",
                table: "RequestTypes");

            migrationBuilder.DropIndex(
                name: "IX_RequestTypeProcessOwners_RequestCategoryId_RequestTypeId_Version",
                table: "RequestTypeProcessOwners");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 843, DateTimeKind.Local).AddTicks(789),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 476, DateTimeKind.Local).AddTicks(5698));

            migrationBuilder.AddColumn<int>(
                name: "RequestCategoryId",
                table: "RequestTypeProcessOwners",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                defaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 836, DateTimeKind.Local).AddTicks(3213),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 473, DateTimeKind.Local).AddTicks(3154));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 828, DateTimeKind.Local).AddTicks(1070),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 469, DateTimeKind.Local).AddTicks(2506));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 827, DateTimeKind.Local).AddTicks(9426),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 469, DateTimeKind.Local).AddTicks(1830));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 822, DateTimeKind.Local).AddTicks(7337),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 467, DateTimeKind.Local).AddTicks(1364));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 747, DateTimeKind.Local).AddTicks(9424),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 446, DateTimeKind.Local).AddTicks(8910));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 747, DateTimeKind.Local).AddTicks(7371),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 446, DateTimeKind.Local).AddTicks(8226));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 785, DateTimeKind.Local).AddTicks(9382),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 456, DateTimeKind.Local).AddTicks(8548));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 785, DateTimeKind.Local).AddTicks(6887),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 456, DateTimeKind.Local).AddTicks(7830));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 714, DateTimeKind.Local).AddTicks(6079),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 435, DateTimeKind.Local).AddTicks(286));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 14, 15, 0, 22, 709, DateTimeKind.Local).AddTicks(4750),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 432, DateTimeKind.Local).AddTicks(2934));

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestTypes",
                table: "RequestTypes",
                columns: new[] { "RequestCategoryId", "Id", "Version" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestTypeProcessOwners_RequestCategoryId_RequestTypeId_Version",
                table: "RequestTypeProcessOwners",
                columns: new[] { "RequestCategoryId", "RequestTypeId", "Version" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestForms_RequestCategoryId_RequestTypeId_Version",
                table: "RequestForms",
                columns: new[] { "RequestCategoryId", "RequestTypeId", "Version" });

            migrationBuilder.AddForeignKey(
                name: "FK_RequestForms_RequestTypes_RequestCategoryId_RequestTypeId_Version",
                table: "RequestForms",
                columns: new[] { "RequestCategoryId", "RequestTypeId", "Version" },
                principalTable: "RequestTypes",
                principalColumns: new[] { "RequestCategoryId", "Id", "Version" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTypeProcessOwners_RequestTypes_RequestCategoryId_RequestTypeId_Version",
                table: "RequestTypeProcessOwners",
                columns: new[] { "RequestCategoryId", "RequestTypeId", "Version" },
                principalTable: "RequestTypes",
                principalColumns: new[] { "RequestCategoryId", "Id", "Version" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
