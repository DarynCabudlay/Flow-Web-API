using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class AlterTriggerForLockedRequestTypesAuditTrailUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var trigger = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[Trig_LockedRequestTypeAuditTrailUpdate] ON [dbo].[LockedRequestTypes] FOR UPDATE
NOT FOR REPLICATION
AS
BEGIN

	SET NOCOUNT ON;

	Declare @TableName as Varchar(200)
	Declare @ColName as Varchar(100)
	Declare @I as Int
	Declare @ColumnId as Int
	Declare @Num_Cols AS Int

	SET @TableName = 'LockedRequestTypes'

	Select
		ROW_NUMBER() OVER(ORDER BY RequestTypeId ASC) as RowNum,
		* 
	Into 
		#INSERTED 
	From
		INSERTED

	Select
		*
	Into
		#DELETED
	From
		DELETED

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
		Declare @TrailDate as Datetime = Getdate()

		Select
			@RequestTypeId = RequestTypeId,
			@Version = Version
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
			Null,
			'RequestTypes',
			@IP,
			'Update',
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

			SET @ColumnId = (SELECT 
								COLUMNPROPERTY(OBJECT_ID(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'ColumnID') AS COLUMN_ID
							FROM INFORMATION_SCHEMA.COLUMNS
							WHERE TABLE_NAME = @TableName And COLUMN_NAME = @ColName)

			/* List of Columns to be Check */
			If @ColName NOT IN ('RequestTypeId', 'Version') BEGIN

				/* Check if column is updated */
				If (SUBSTRING(COLUMNS_UPDATED(),(@ColumnId - 1) / 8 + 1, 1)) & POWER(2, (@ColumnId - 1) % 8) = POWER(2, (@ColumnId - 1) % 8) Begin
			
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
									Case When D.[' + @ColName + '] Is Null Then Null Else Convert(Varchar(8000), Isnull(D.[' + @ColName + '], '''')) End,
									Case When I.[' + @ColName + '] Is Null Then Null Else Convert(Varchar(8000), Isnull(I.[' + @ColName + '], '''')) End
								FROM
									#DELETED D
									LEFT JOIN #INSERTED I ON I.RequestTypeId = D.RequestTypeId AND I.Version = D.Version
								WHERE
									Convert(Varchar(8000), Isnull(D.[' + @ColName + '], '''')) <> Convert(Varchar(8000), Isnull(I.[' + @ColName + '], '''')) And
									I.RowNum = ' + CAST(@MinRowNum AS VARCHAR(10))  + '
							)'
					Exec(@SQLQuery)
			
				End 

			End
	
			Set @I = @I + 1	                
		End
		/**** End **********************/
		/**** Delete Audit Trail When Theres No Details ****/
		Delete 
		from 
			dbo.AuditTrails 
		Where 
			Id = @AuditTrailId And
			Id Not In
			(Select Id from dbo.AuditTrailDetails)

	End
	
	Drop Table #INSERTED
	Drop Table #DELETED

End";

			migrationBuilder.Sql(trigger);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			string trigger = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[Trig_LockedRequestTypeAuditTrailUpdate] ON [dbo].[LockedRequestTypes] FOR UPDATE
NOT FOR REPLICATION
AS
BEGIN

	SET NOCOUNT ON;

	Declare @TableName as Varchar(200)
	Declare @ColName as Varchar(100)
	Declare @I as Int 
	Declare @Num_Cols AS Int

	SET @TableName = 'LockedRequestTypes'

	Select
		* 
	Into 
		#INSERTED 
	From
		INSERTED

	Select
		*
	Into
		#DELETED
	From
		DELETED
	
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
	Declare @TrailDate as Datetime = Getdate()

	Select
		@RequestTypeId = RequestTypeId,
		@Version = Version
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
		Null,
		'RequestTypes',
		@IP,
		'Update',
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
		If @ColName NOT IN ('RequestTypeId', 'Version') BEGIN

			/* Check if column is updated */
			If (SUBSTRING(COLUMNS_UPDATED(),(@i - 1) / 8 + 1, 1)) & POWER(2, (@i - 1) % 8) = POWER(2, (@i - 1) % 8) Begin
			
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
								Case When D.[' + @ColName + '] Is Null Then Null Else Convert(Varchar(8000), Isnull(D.[' + @ColName + '], '''')) End,
								Case When I.[' + @ColName + '] Is Null Then Null Else Convert(Varchar(8000), Isnull(I.[' + @ColName + '], '''')) End
							FROM
								#DELETED D
								LEFT JOIN #INSERTED I ON I.RequestTypeId = D.RequestTypeId AND I.Version = D.Version
							WHERE
								Convert(Varchar(8000), Isnull(D.[' + @ColName + '], '''')) <> Convert(Varchar(8000), Isnull(I.[' + @ColName + '], ''''))
						)'
				Exec(@SQLQuery)
			
			End 

		End
	
		Set @I = @I + 1	                
	End
	/**** End **********************/
    /**** Delete Audit Trail When Theres No Details ****/
	Delete 
	from 
		dbo.AuditTrails 
	Where 
		Id = @AuditTrailId And
		Id Not In
		(Select Id from dbo.AuditTrailDetails)

	Drop Table #INSERTED
	Drop Table #DELETED

End";

			migrationBuilder.Sql(trigger);
		}
    }
}
