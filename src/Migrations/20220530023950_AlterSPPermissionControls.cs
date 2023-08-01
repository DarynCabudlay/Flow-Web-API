using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class AlterSPPermissionControls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 638, DateTimeKind.Local).AddTicks(8114),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 979, DateTimeKind.Local).AddTicks(363));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 638, DateTimeKind.Local).AddTicks(7429),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 978, DateTimeKind.Local).AddTicks(8510));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 636, DateTimeKind.Local).AddTicks(6093),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 973, DateTimeKind.Local).AddTicks(5644));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 615, DateTimeKind.Local).AddTicks(2329),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 912, DateTimeKind.Local).AddTicks(8903));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 615, DateTimeKind.Local).AddTicks(1639),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 912, DateTimeKind.Local).AddTicks(6494));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 625, DateTimeKind.Local).AddTicks(1741),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 943, DateTimeKind.Local).AddTicks(5926));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 625, DateTimeKind.Local).AddTicks(1090),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 943, DateTimeKind.Local).AddTicks(3614));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 601, DateTimeKind.Local).AddTicks(9007),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 876, DateTimeKind.Local).AddTicks(4251));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 599, DateTimeKind.Local).AddTicks(1148),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 869, DateTimeKind.Local).AddTicks(3608));

            var sp = @"ALTER PROCEDURE [dbo].[spPermissionControls]
	                        -- Add the parameters for the stored procedure here
	                        @UserID AS NVARCHAR(120),
	                        @vMenuID AS NVARCHAR(120)
                        AS
                        BEGIN
	                        -- SET NOCOUNT ON added to prevent extra result sets from
	                        -- interfering with SELECT statements.
	                        SET NOCOUNT ON;
	                        SET FMTONLY OFF;

	                        DECLARE @columns NVARCHAR(MAX) = '', @sql NVARCHAR(MAX) = '';

	                        -- Select the Option Name
	                        SELECT  @columns += QUOTENAME(Name) + ','
	                        FROM Options WHERE OptionGroup = 'Button Conrols' AND Published = 1 
	                        ORDER BY Name;

	                        -- Remove the last comma
	                        SET @columns = LEFT(@columns, LEN(@columns) - 1);-- construct dynamic SQL
	
	                        SET @sql = N'
	                        SELECT * FROM (
		                        -- Insert statements for procedure here
		                        SELECT 
			                        ''' + @UserID + ''' AS UserID, ''' + @vMenuID + ''' AS vMenuID, 
			                        A.Name, 
			                        CASE WHEN B.MenuControlId IS NULL THEN 
				                        0 
			                        ELSE 
				                        1 
			                        END Access 
		                        FROM Options A
		                        LEFT JOIN AspNetUsersMenuControl AS MC ON A.Id = MC.OptionId
		                        LEFT JOIN AspNetUsersMenuPermissionControl B ON MC.MenuControlId = B.MenuControlId AND 
				                            B.vMenuPermissionID IN 
				                            (
					                        SELECT 
						                        vMenuPermissionID 
					                        FROM 
						                        AspNetUsersMenuPermission 
					                        WHERE 
						                        vMenuID IN 
						                        (
							                        SELECT 
								                        TOP 1 vMenuID 
							                        FROM 
								                        AspNetUsersMenu 
							                        WHERE 
								                        vMenuID = ''' + @vMenuID + '''
					                            ) AND 
						                        Id in 
						                        (
							                        SelecT 
								                        RoleId 
							                        from 
								                        dbo.AspNetUserRoles AS UR
								                        Inner join dbo.AspNetRoles AS R ON UR.RoleId = R.Id
							                        Where
								                        R.Published = 1 And UserId = ''' + @UserID + '''
						                        )
					                        )
		                        LEFT JOIN AspNetUsersMenuPermission C ON B.vMenuPermissionID = C.vMenuPermissionID
		                        LEFT JOIN AspNetUsersMenu D ON C.vMenuID = D.vMenuID
		                        WHERE A.OptionGroup = ''Button Controls'' AND A.Published = 1 
	                        ) T
	                        PIVOT(
		                        MAX(Access) 
		                        FOR Name IN ('+ @columns +')
	                        ) AS pivot_table;';

	                        -- execute the dynamic SQL
	                        EXECUTE sp_executesql @sql;
                        END";

            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 979, DateTimeKind.Local).AddTicks(363),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 638, DateTimeKind.Local).AddTicks(8114));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 978, DateTimeKind.Local).AddTicks(8510),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 638, DateTimeKind.Local).AddTicks(7429));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 973, DateTimeKind.Local).AddTicks(5644),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 636, DateTimeKind.Local).AddTicks(6093));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 912, DateTimeKind.Local).AddTicks(8903),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 615, DateTimeKind.Local).AddTicks(2329));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 912, DateTimeKind.Local).AddTicks(6494),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 615, DateTimeKind.Local).AddTicks(1639));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 943, DateTimeKind.Local).AddTicks(5926),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 625, DateTimeKind.Local).AddTicks(1741));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 943, DateTimeKind.Local).AddTicks(3614),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 625, DateTimeKind.Local).AddTicks(1090));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 876, DateTimeKind.Local).AddTicks(4251),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 601, DateTimeKind.Local).AddTicks(9007));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 5, 16, 16, 51, 14, 869, DateTimeKind.Local).AddTicks(3608),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 5, 30, 10, 39, 49, 599, DateTimeKind.Local).AddTicks(1148));
        }
    }
}
