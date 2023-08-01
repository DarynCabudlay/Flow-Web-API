using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class AddspPermissionControls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                oldDefaultValue: new DateTime(2022, 4, 4, 15, 30, 18, 17, DateTimeKind.Local).AddTicks(9959));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 628, DateTimeKind.Local).AddTicks(8489),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 4, 15, 30, 18, 17, DateTimeKind.Local).AddTicks(9234));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 624, DateTimeKind.Local).AddTicks(5567),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 4, 15, 30, 18, 15, DateTimeKind.Local).AddTicks(5808));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 566, DateTimeKind.Local).AddTicks(7955),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 4, 4, 15, 30, 17, 990, DateTimeKind.Local).AddTicks(1449));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 566, DateTimeKind.Local).AddTicks(5892),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 4, 15, 30, 17, 990, DateTimeKind.Local).AddTicks(512));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 595, DateTimeKind.Local).AddTicks(2803),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 4, 4, 15, 30, 18, 2, DateTimeKind.Local).AddTicks(5510));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 595, DateTimeKind.Local).AddTicks(808),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 4, 15, 30, 18, 2, DateTimeKind.Local).AddTicks(4704));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 536, DateTimeKind.Local).AddTicks(3756),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 4, 4, 15, 30, 17, 976, DateTimeKind.Local).AddTicks(5048));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 530, DateTimeKind.Local).AddTicks(5095),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 4, 15, 30, 17, 972, DateTimeKind.Local).AddTicks(6477));

            var sp = @"CREATE PROCEDURE [dbo].[spPermissionControls]
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
		                        LEFT JOIN AspNetUsersMenuPermissionControl B ON A.Id = B.OptionID AND 
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

            string procedure = @"Drop procedure spPermissionControls";
            migrationBuilder.Sql(procedure);

            

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "Options",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 4, 4, 15, 30, 18, 17, DateTimeKind.Local).AddTicks(9959),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 628, DateTimeKind.Local).AddTicks(9957));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Options",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 4, 15, 30, 18, 17, DateTimeKind.Local).AddTicks(9234),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 628, DateTimeKind.Local).AddTicks(8489));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ChangeLogs",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 4, 15, 30, 18, 15, DateTimeKind.Local).AddTicks(5808),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 624, DateTimeKind.Local).AddTicks(5567));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 4, 4, 15, 30, 17, 990, DateTimeKind.Local).AddTicks(1449),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 566, DateTimeKind.Local).AddTicks(7955));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersProfile",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 4, 15, 30, 17, 990, DateTimeKind.Local).AddTicks(512),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 566, DateTimeKind.Local).AddTicks(5892));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 4, 4, 15, 30, 18, 2, DateTimeKind.Local).AddTicks(5510),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 595, DateTimeKind.Local).AddTicks(2803));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsersMenu",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 4, 15, 30, 18, 2, DateTimeKind.Local).AddTicks(4704),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 595, DateTimeKind.Local).AddTicks(808));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2022, 4, 4, 15, 30, 17, 976, DateTimeKind.Local).AddTicks(5048),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 536, DateTimeKind.Local).AddTicks(3756));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2022, 4, 4, 15, 30, 17, 972, DateTimeKind.Local).AddTicks(6477),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2022, 4, 5, 13, 31, 45, 530, DateTimeKind.Local).AddTicks(5095));
        }
    }
}
