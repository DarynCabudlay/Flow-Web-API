using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class AlterSPPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 718, DateTimeKind.Local).AddTicks(9826),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 628, DateTimeKind.Local).AddTicks(9957));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 718, DateTimeKind.Local).AddTicks(9065),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 628, DateTimeKind.Local).AddTicks(8489));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 716, DateTimeKind.Local).AddTicks(5910),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 624, DateTimeKind.Local).AddTicks(5567));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 691, DateTimeKind.Local).AddTicks(6732),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 566, DateTimeKind.Local).AddTicks(7955));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 691, DateTimeKind.Local).AddTicks(5748),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 566, DateTimeKind.Local).AddTicks(5892));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 704, DateTimeKind.Local).AddTicks(3043),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 595, DateTimeKind.Local).AddTicks(2803));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 704, DateTimeKind.Local).AddTicks(2211),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 595, DateTimeKind.Local).AddTicks(808));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 677, DateTimeKind.Local).AddTicks(7451),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 536, DateTimeKind.Local).AddTicks(3756));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 674, DateTimeKind.Local).AddTicks(6629),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 530, DateTimeKind.Local).AddTicks(5095));


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
			                        CASE WHEN B.OptionID IS NULL THEN 
				                        0 
			                        ELSE 
				                        1 
			                        END Access 
		                        FROM Options A
								LEFT AspNetUsersMenuControl AS MC ON A.Id = MC.OptionId
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
		                        WHERE A.OptionGroup = ''Button Conrols'' AND A.Published = 1 
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
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 628, DateTimeKind.Local).AddTicks(9957),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 718, DateTimeKind.Local).AddTicks(9826));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 628, DateTimeKind.Local).AddTicks(8489),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 718, DateTimeKind.Local).AddTicks(9065));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 624, DateTimeKind.Local).AddTicks(5567),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 716, DateTimeKind.Local).AddTicks(5910));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 566, DateTimeKind.Local).AddTicks(7955),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 691, DateTimeKind.Local).AddTicks(6732));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 566, DateTimeKind.Local).AddTicks(5892),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 691, DateTimeKind.Local).AddTicks(5748));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 595, DateTimeKind.Local).AddTicks(2803),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 704, DateTimeKind.Local).AddTicks(3043));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 595, DateTimeKind.Local).AddTicks(808),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 704, DateTimeKind.Local).AddTicks(2211));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 536, DateTimeKind.Local).AddTicks(3756),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 677, DateTimeKind.Local).AddTicks(7451));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 530, DateTimeKind.Local).AddTicks(5095),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 7, 16, 16, 59, 674, DateTimeKind.Local).AddTicks(6629));
        }
    }
}
