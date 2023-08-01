using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class AdjustPrimaryKeyOnRequestTypesAndRequestForms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestForms_RequestTypes_Version",
                table: "RequestForms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestTypes",
                table: "RequestTypes");

            migrationBuilder.DropIndex(
                name: "IX_RequestTypes_RequestCategoryId",
                table: "RequestTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestForms",
                table: "RequestForms");

            migrationBuilder.DropIndex(
                name: "IX_RequestForms_Version",
                table: "RequestForms");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 900, DateTimeKind.Local).AddTicks(8849),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 1, 37, 18, DateTimeKind.Local).AddTicks(1475));

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                table: "RequestTypes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                table: "RequestForms",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<int>(
                name: "RequestTypeId1",
                table: "RequestForms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestTypeRequestCategoryId",
                table: "RequestForms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestTypeVersion",
                table: "RequestForms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 898, DateTimeKind.Local).AddTicks(1519),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 1, 37, 12, DateTimeKind.Local).AddTicks(4079));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 894, DateTimeKind.Local).AddTicks(2516),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 1, 37, 8, DateTimeKind.Local).AddTicks(6698));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 894, DateTimeKind.Local).AddTicks(1869),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 1, 37, 8, DateTimeKind.Local).AddTicks(6067));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 892, DateTimeKind.Local).AddTicks(2520),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 1, 37, 6, DateTimeKind.Local).AddTicks(7457));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 868, DateTimeKind.Local).AddTicks(9944),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 1, 36, 983, DateTimeKind.Local).AddTicks(7998));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 868, DateTimeKind.Local).AddTicks(9007),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 1, 36, 983, DateTimeKind.Local).AddTicks(6905));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 881, DateTimeKind.Local).AddTicks(3992),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 1, 36, 996, DateTimeKind.Local).AddTicks(4113));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 881, DateTimeKind.Local).AddTicks(3298),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 1, 36, 996, DateTimeKind.Local).AddTicks(3386));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 856, DateTimeKind.Local).AddTicks(2396),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 1, 36, 964, DateTimeKind.Local).AddTicks(7102));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 853, DateTimeKind.Local).AddTicks(3267),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 1, 36, 960, DateTimeKind.Local).AddTicks(9195));

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestTypes",
                table: "RequestTypes",
                columns: new[] { "RequestCategoryId", "Id", "Version" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestForms",
                table: "RequestForms",
                columns: new[] { "RequestTypeId", "Version", "FieldId" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestForms_FieldId",
                table: "RequestForms",
                column: "FieldId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestForms_RequestTypes_RequestTypeRequestCategoryId_RequestTypeId1_RequestTypeVersion",
                table: "RequestForms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestTypes",
                table: "RequestTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestForms",
                table: "RequestForms");

            migrationBuilder.DropIndex(
                name: "IX_RequestForms_FieldId",
                table: "RequestForms");

            migrationBuilder.DropIndex(
                name: "IX_RequestForms_RequestTypeRequestCategoryId_RequestTypeId1_RequestTypeVersion",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "RequestTypeId1",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "RequestTypeRequestCategoryId",
                table: "RequestForms");

            migrationBuilder.DropColumn(
                name: "RequestTypeVersion",
                table: "RequestForms");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 1, 37, 18, DateTimeKind.Local).AddTicks(1475),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 900, DateTimeKind.Local).AddTicks(8849));

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "RequestTypes",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "RequestForms",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 1, 37, 12, DateTimeKind.Local).AddTicks(4079),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 898, DateTimeKind.Local).AddTicks(1519));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 13, 15, 1, 37, 8, DateTimeKind.Local).AddTicks(6698),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 894, DateTimeKind.Local).AddTicks(2516));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 1, 37, 8, DateTimeKind.Local).AddTicks(6067),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 894, DateTimeKind.Local).AddTicks(1869));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 1, 37, 6, DateTimeKind.Local).AddTicks(7457),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 892, DateTimeKind.Local).AddTicks(2520));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 13, 15, 1, 36, 983, DateTimeKind.Local).AddTicks(7998),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 868, DateTimeKind.Local).AddTicks(9944));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 1, 36, 983, DateTimeKind.Local).AddTicks(6905),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 868, DateTimeKind.Local).AddTicks(9007));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 13, 15, 1, 36, 996, DateTimeKind.Local).AddTicks(4113),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 881, DateTimeKind.Local).AddTicks(3992));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 1, 36, 996, DateTimeKind.Local).AddTicks(3386),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 881, DateTimeKind.Local).AddTicks(3298));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 13, 15, 1, 36, 964, DateTimeKind.Local).AddTicks(7102),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 856, DateTimeKind.Local).AddTicks(2396));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 13, 15, 1, 36, 960, DateTimeKind.Local).AddTicks(9195),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 13, 15, 22, 24, 853, DateTimeKind.Local).AddTicks(3267));

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestTypes",
                table: "RequestTypes",
                column: "Version");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestForms",
                table: "RequestForms",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTypes_RequestCategoryId",
                table: "RequestTypes",
                column: "RequestCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestForms_Version",
                table: "RequestForms",
                column: "Version");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestForms_RequestTypes_Version",
                table: "RequestForms",
                column: "Version",
                principalTable: "RequestTypes",
                principalColumn: "Version",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
