using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class AlterTriggerForRequestFormsAuditTrailInsert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var trigger = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[Trig_RequestFormsAuditTrailInsert] ON [dbo].[RequestForms] FOR INSERT
NOT FOR REPLICATION
AS
Begin

	SET NOCOUNT ON;

	Declare @TableName as Varchar(200)
	Declare @ColName as Varchar(100)
	Declare @I as Int 
	Declare @Num_Cols AS Int

	SET @TableName = 'RequestForms'

	Select
		ROW_NUMBER() OVER(ORDER BY RequestTypeId ASC) as RowNum,
		* 
	Into 
		#INSERTED 
	From
		INSERTED

	Declare @MaxRowNum Int,
			@MinRowNum Int

	Select
		@MaxRowNum = Max(RowNum),
		@MinRowNum = Min(RowNum)
	from
		#INSERTED

	If @MaxRowNum > 0 Begin

		Declare @IP Varchar(50)
		Declare @UserId Varchar(128)
		Declare @Device Varchar(100)
		Declare @UsedApp Varchar(100)

		Set @IP = ''
		Set @UserId = (Select SYSTEM_USER)
		Set @Device = (Select RTRIM(HOSTNAME) From Master.dbo.SysProcesses Where SPID = @@SPID)
		Set @UsedApp = (Select RTRIM(PROGRAM_NAME) From Master.dbo.SysProcesses Where SPID = @@SPID)

		If Exists (SELECT '1' from dbo.UserIPAddressPerSessions WHERE SpId = @@SPID) Begin
			Select @IP = IPAddress, @UserId = UserId
			from dbo.UserIPAddressPerSessions Where SpId = @@SPID
		End

		Declare @RequestTypeId as int
		Declare @Version as int
		Declare @FieldId as int
		Declare @TrailDate as Datetime = Getdate()

		Select
			@RequestTypeId = RequestTypeId,
			@Version = Version,
			@FieldId = FieldId
		From
			#INSERTED

		/**** Insert in Audit Trail ****/
		Insert Into dbo.AuditTrails 
		( 
			[Key], 
			TableName, 
			UserId, 
			Date, 
			Key2, 
			Key3, 
			[Transaction],
			IP,
			Action,
			ReasonOfDeletion,
			Device,
			UsedApp
		)
		VALUES
		(
			@RequestTypeId,
			@TableName,
			@UserId,
			@TrailDate,
			@Version,
			@FieldId,
			'RequestTypes',
			@IP,
			'Insert',
			Null,
			@Device,
			@UsedApp
		)

		Declare @AuditTrailId As int = Scope_Identity()
		/**** End **********************/

		/**** Insert in Audit Trail Details ****/
		Set @Num_Cols = (Select COUNT(*) From INFORMATION_SCHEMA.COLUMNS Where TABLE_SCHEMA = 'dbo' And TABLE_NAME = @TableName)
		Set @I = 1

		While @I <= @Num_Cols BEGIN

			SET @ColName = (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @TableName and ORDINAL_POSITION = @I)

			/* List of Columns to be Check */
			If @ColName NOT IN ('RequestTypeId', 'Version', 'FieldId') BEGIN
			
				Declare @SQLQuery as Varchar(8000)

				Set @SQLQuery = '
						INSERT INTO dbo.AuditTrailDetails
						(Id, [Key], TableName, UserId, Date, Field, OldValue, NewValue)
						(
							SELECT
								' + CAST(@AuditTrailId AS VARCHAR(10)) + ',
								' + CAST(@RequestTypeId AS VARCHAR(10)) + ',
								' + '''' + @TableName + '''' + ',
								' + '''' + @UserId + '''' + ',
								Convert(DateTime, ''' + convert(varchar(50), @TrailDate, 121)  + '''),
								' + '''' + @ColName + '''' + ',
								Null,
								Case When I.' + @ColName + ' Is Null Then Null Else CONVERT(VARCHAR(8000), ISNULL(I.' + @ColName + ', '''')) End
							FROM
								#INSERTED AS I
							WHERE
								RowNum = ' + CAST(@MinRowNum AS VARCHAR(10))  + '
						)'

				--Print @SQLQuery

				Exec(@SQLQuery)

			End
	
			Set @I = @I + 1	                
		End
		/**** End **********************/

	End
	
	Drop Table #INSERTED

End";

			migrationBuilder.Sql(trigger);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			string trigger = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[Trig_RequestFormsAuditTrailInsert] ON [dbo].[RequestForms] FOR INSERT
NOT FOR REPLICATION
AS
Begin

	SET NOCOUNT ON;

	Declare @TableName as Varchar(200)
	Declare @ColName as Varchar(100)
	Declare @I as Int 
	Declare @Num_Cols AS Int

	SET @TableName = 'RequestForms'

	Select
		* 
	Into 
		#INSERTED 
	From
		INSERTED
	
	Declare @IP Varchar(50)
	Declare @UserId Varchar(128)
	Declare @Device Varchar(100)
	Declare @UsedApp Varchar(100)

	Set @IP = ''
	Set @UserId = (Select SYSTEM_USER)
	Set @Device = (Select RTRIM(HOSTNAME) From Master.dbo.SysProcesses Where SPID = @@SPID)
	Set @UsedApp = (Select RTRIM(PROGRAM_NAME) From Master.dbo.SysProcesses Where SPID = @@SPID)

	If Exists (SELECT '1' from dbo.UserIPAddressPerSessions WHERE SpId = @@SPID) Begin
		Select @IP = IPAddress, @UserId = UserId
		from dbo.UserIPAddressPerSessions Where SpId = @@SPID
	End

	Declare @RequestTypeId as int
	Declare @Version as int
	Declare @FieldId as int
	Declare @TrailDate as Datetime = Getdate()

	Select
		@RequestTypeId = RequestTypeId,
		@Version = Version,
		@FieldId = FieldId
	From
		#INSERTED

	/**** Insert in Audit Trail ****/
	Insert Into dbo.AuditTrails 
	( 
		[Key], 
		TableName, 
		UserId, 
		Date, 
		Key2, 
		Key3, 
		[Transaction],
		IP,
		Action,
		ReasonOfDeletion,
		Device,
		UsedApp
	)
	VALUES
	(
		@RequestTypeId,
		@TableName,
		@UserId,
		@TrailDate,
		@Version,
		@FieldId,
		'RequestTypes',
		@IP,
		'Insert',
		Null,
		@Device,
		@UsedApp
	)

	Declare @AuditTrailId As int = Scope_Identity()
	/**** End **********************/

	/**** Insert in Audit Trail Details ****/
	Set @Num_Cols = (Select COUNT(*) From INFORMATION_SCHEMA.COLUMNS Where TABLE_SCHEMA = 'dbo' And TABLE_NAME = @TableName)
	Set @I = 1

	While @I <= @Num_Cols BEGIN

		SET @ColName = (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @TableName and ORDINAL_POSITION = @I)

		/* List of Columns to be Check */
		If @ColName NOT IN ('RequestTypeId', 'Version', 'FieldId') BEGIN
			
			Declare @SQLQuery as Varchar(8000)

			Set @SQLQuery = '
					INSERT INTO dbo.AuditTrailDetails
					(Id, [Key], TableName, UserId, Date, Field, OldValue, NewValue)
					(
						SELECT
							' + CAST(@AuditTrailId AS VARCHAR(10)) + ',
							' + CAST(@RequestTypeId AS VARCHAR(10)) + ',
							' + '''' + @TableName + '''' + ',
							' + '''' + @UserId + '''' + ',
							Convert(DateTime, ''' + convert(varchar(50), @TrailDate, 121)  + '''),
							' + '''' + @ColName + '''' + ',
							Null,
							Case When I.' + @ColName + ' Is Null Then Null Else CONVERT(VARCHAR(8000), ISNULL(I.' + @ColName + ', '''')) End
						FROM
							#INSERTED AS I
					)'

			--Print @SQLQuery

			Exec(@SQLQuery)

		End
	
		Set @I = @I + 1	                
	End
	/**** End **********************/

	Drop Table #INSERTED

End";

			migrationBuilder.Sql(trigger);
		}
    }
}
