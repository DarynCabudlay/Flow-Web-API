using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class ChangeEditableToStringInRequestForms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 540, DateTimeKind.Local).AddTicks(5880),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 476, DateTimeKind.Local).AddTicks(5698));

            migrationBuilder.AlterColumn<string>(
                name: "Editable",
                table: "RequestForms",
                type: "varchar(3)",
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 537, DateTimeKind.Local).AddTicks(1183),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 473, DateTimeKind.Local).AddTicks(3154));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 532, DateTimeKind.Local).AddTicks(7565),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 469, DateTimeKind.Local).AddTicks(2506));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 532, DateTimeKind.Local).AddTicks(6842),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 469, DateTimeKind.Local).AddTicks(1830));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 530, DateTimeKind.Local).AddTicks(5212),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 467, DateTimeKind.Local).AddTicks(1364));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 508, DateTimeKind.Local).AddTicks(6095),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 446, DateTimeKind.Local).AddTicks(8910));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 508, DateTimeKind.Local).AddTicks(5413),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 446, DateTimeKind.Local).AddTicks(8226));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 519, DateTimeKind.Local).AddTicks(5339),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 456, DateTimeKind.Local).AddTicks(8548));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 519, DateTimeKind.Local).AddTicks(4654),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 456, DateTimeKind.Local).AddTicks(7830));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 497, DateTimeKind.Local).AddTicks(2560),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 435, DateTimeKind.Local).AddTicks(286));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 494, DateTimeKind.Local).AddTicks(7931),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 432, DateTimeKind.Local).AddTicks(2934));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestTypes",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 476, DateTimeKind.Local).AddTicks(5698),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 540, DateTimeKind.Local).AddTicks(5880));

            migrationBuilder.AlterColumn<bool>(
                name: "Editable",
                table: "RequestForms",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestCategories",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 473, DateTimeKind.Local).AddTicks(3154),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 537, DateTimeKind.Local).AddTicks(1183));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 469, DateTimeKind.Local).AddTicks(2506),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 532, DateTimeKind.Local).AddTicks(7565));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 469, DateTimeKind.Local).AddTicks(1830),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 532, DateTimeKind.Local).AddTicks(6842));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 467, DateTimeKind.Local).AddTicks(1364),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 530, DateTimeKind.Local).AddTicks(5212));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 446, DateTimeKind.Local).AddTicks(8910),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 508, DateTimeKind.Local).AddTicks(6095));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 446, DateTimeKind.Local).AddTicks(8226),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 508, DateTimeKind.Local).AddTicks(5413));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 456, DateTimeKind.Local).AddTicks(8548),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 519, DateTimeKind.Local).AddTicks(5339));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 456, DateTimeKind.Local).AddTicks(7830),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 519, DateTimeKind.Local).AddTicks(4654));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 435, DateTimeKind.Local).AddTicks(286),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 497, DateTimeKind.Local).AddTicks(2560));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 6, 20, 9, 58, 36, 432, DateTimeKind.Local).AddTicks(2934),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 6, 20, 15, 0, 0, 494, DateTimeKind.Local).AddTicks(7931));
        }
    }
}
