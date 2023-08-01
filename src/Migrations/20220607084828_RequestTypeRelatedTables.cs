using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class RequestTypeRelatedTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPasswordPolicies_AspNetUsers_UserId",
                table: "UserPasswordPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferences_AspNetUsers_UserId",
                table: "UserPreferences");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "UserPreferences",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserPreferences",
                type: "nvarchar(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UserPreferences",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserPreferences",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserPasswordPolicies",
                type: "nvarchar(128)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 565, DateTimeKind.Local).AddTicks(2885),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 373, DateTimeKind.Local).AddTicks(9880));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 565, DateTimeKind.Local).AddTicks(1711),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 373, DateTimeKind.Local).AddTicks(9209));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 561, DateTimeKind.Local).AddTicks(6095),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 371, DateTimeKind.Local).AddTicks(9104));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 522, DateTimeKind.Local).AddTicks(6749),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 347, DateTimeKind.Local).AddTicks(9221));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 522, DateTimeKind.Local).AddTicks(5365),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 347, DateTimeKind.Local).AddTicks(8358));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 540, DateTimeKind.Local).AddTicks(3606),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 360, DateTimeKind.Local).AddTicks(1990));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 540, DateTimeKind.Local).AddTicks(2396),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 360, DateTimeKind.Local).AddTicks(1189));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 503, DateTimeKind.Local).AddTicks(2610),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 334, DateTimeKind.Local).AddTicks(9319));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 499, DateTimeKind.Local).AddTicks(3009),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 332, DateTimeKind.Local).AddTicks(1076));

            migrationBuilder.CreateTable(
                name: "RequestCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Published = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 567, DateTimeKind.Local).AddTicks(3915)),
                    CreatedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedByPK = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestCategories", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_UserPasswordPolicies_AspNetUsers_UserId",
                table: "UserPasswordPolicies",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferences_AspNetUsers_UserId",
                table: "UserPreferences",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPasswordPolicies_AspNetUsers_UserId",
                table: "UserPasswordPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferences_AspNetUsers_UserId",
                table: "UserPreferences");


            migrationBuilder.DropTable(
                name: "RequestCategories");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "UserPreferences",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserPreferences",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UserPreferences",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "UserPreferences",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserPasswordPolicies",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 373, DateTimeKind.Local).AddTicks(9880),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 565, DateTimeKind.Local).AddTicks(2885));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 373, DateTimeKind.Local).AddTicks(9209),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 565, DateTimeKind.Local).AddTicks(1711));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 371, DateTimeKind.Local).AddTicks(9104),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 561, DateTimeKind.Local).AddTicks(6095));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 347, DateTimeKind.Local).AddTicks(9221),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 522, DateTimeKind.Local).AddTicks(6749));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 347, DateTimeKind.Local).AddTicks(8358),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 522, DateTimeKind.Local).AddTicks(5365));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 360, DateTimeKind.Local).AddTicks(1990),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 540, DateTimeKind.Local).AddTicks(3606));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 360, DateTimeKind.Local).AddTicks(1189),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 540, DateTimeKind.Local).AddTicks(2396));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 334, DateTimeKind.Local).AddTicks(9319),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 503, DateTimeKind.Local).AddTicks(2610));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 5, 30, 10, 49, 12, 332, DateTimeKind.Local).AddTicks(1076),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 7, 16, 48, 27, 499, DateTimeKind.Local).AddTicks(3009));

            migrationBuilder.AddForeignKey(
                name: "FK_UserPasswordPolicies_AspNetUsers_UserId",
                table: "UserPasswordPolicies",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferences_AspNetUsers_UserId",
                table: "UserPreferences",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
