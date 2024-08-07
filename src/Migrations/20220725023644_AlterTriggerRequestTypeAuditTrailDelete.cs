﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class AlterTriggerRequestTypeAuditTrailDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var trigger = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[Trig_RequestTypeAuditTrailDelete] ON [dbo].[RequestTypes] FOR DELETE
NOT FOR REPLICATION
AS
Begin

	SET NOCOUNT ON;

	Declare @TableName as Varchar(200)
	Declare @ColName as Varchar(100)
	Declare @I as Int 
	Declare @Num_Cols AS Int

	SET @TableName = 'RequestTypes'

	Select
		* 
	Into 
		#DELETED 
	From
		Deleted
	
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
	Declare @DeletionReason Varchar(8000)
	Declare @DeletionRemarks Varchar(8000)

	Select
		@RequestTypeId = D.Id,
		@Version = Version,
		@DeletionReason = R.Name,
		@DeletionRemarks = D.DeletionRemarks
	From
		#DELETED AS D
		Left Join dbo.Reasons AS R On D.DeletionReasonId = R.Id

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
		UsedApp,
		DeletionRemarks
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
		'Delete',
		@DeletionReason,
		@Device,
		@UsedApp,
		@DeletionRemarks
	)

	Declare @AuditTrailId As int = Scope_Identity()
	/**** End **********************/

	/**** Insert in Audit Trail Details ****/
	Set @Num_Cols = (Select COUNT(*) From INFORMATION_SCHEMA.COLUMNS Where TABLE_SCHEMA = 'dbo' And TABLE_NAME = @TableName)
	Set @I = 1

	While @I <= @Num_Cols BEGIN

		SET @ColName = (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @TableName and ORDINAL_POSITION = @I)

		/* List of Columns to be Check */
		If @ColName NOT IN ('Id', 'Version') BEGIN
			
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
							Case When D.' + @ColName + ' Is Null Then Null Else CONVERT(VARCHAR(8000), ISNULL(D.' + @ColName + ', '''')) End,
							Null
						FROM
							#DELETED AS D
					)'

			--Print @SQLQuery

			Exec(@SQLQuery)

		End
	
		Set @I = @I + 1	                
	End
	/**** End **********************/

	Drop Table #DELETED

End";

			migrationBuilder.Sql(trigger);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			var trigger = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[Trig_RequestTypeAuditTrailDelete] ON [dbo].[RequestTypes] FOR DELETE
NOT FOR REPLICATION
AS
Begin

	SET NOCOUNT ON;

	Declare @TableName as Varchar(200)
	Declare @ColName as Varchar(100)
	Declare @I as Int 
	Declare @Num_Cols AS Int

	SET @TableName = 'RequestTypes'

	Select
		* 
	Into 
		#DELETED 
	From
		Deleted
	
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
		@RequestTypeId = Id,
		@Version = Version
	From
		#DELETED

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
		'Delete',
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
		If @ColName NOT IN ('Id', 'Version') BEGIN
			
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
							Case When D.' + @ColName + ' Is Null Then Null Else CONVERT(VARCHAR(8000), ISNULL(D.' + @ColName + ', '''')) End,
							Null
						FROM
							#DELETED AS D
					)'

			--Print @SQLQuery

			Exec(@SQLQuery)

		End
	
		Set @I = @I + 1	                
	End
	/**** End **********************/

	Drop Table #DELETED

End";

			migrationBuilder.Sql(trigger);
		}
    }
}
