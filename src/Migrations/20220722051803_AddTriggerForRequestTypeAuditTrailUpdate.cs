using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class AddTriggerForRequestTypeAuditTrailUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var trigger = @"SET ANSI_NULLS ON
							GO
							SET QUOTED_IDENTIFIER ON
							GO
							CREATE TRIGGER [dbo].[Trig_RequestTypeAuditTrailUpdate] ON [dbo].[RequestTypes] FOR UPDATE
							NOT FOR REPLICATION
							AS
							BEGIN

								SET NOCOUNT ON;

								Declare @TableName as Varchar(200)
								Declare @ColName as Varchar(100)
								Declare @I as Int 
								Declare @Num_Cols AS Int

								SET @TableName = 'RequestTypes'

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

								Set @IP = ''
								Set @UserId = (Select SYSTEM_USER)

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
									ReasonOfDeletion
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
									Null
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
															Case When D.' + @ColName + ' Is Null Then Null Else Convert(Varchar(8000), Isnull(D.' + @ColName + ', '''')) End,
															Case When I.' + @ColName + ' Is Null Then Null Else Convert(Varchar(8000), Isnull(I.' + @ColName + ', '''')) End
														FROM
															#DELETED D
															LEFT JOIN #INSERTED I ON I.ID = D.ID AND I.Version = D.Version
														WHERE
															Convert(Varchar(8000), Isnull(D.' + @ColName + ', '''')) <> Convert(Varchar(8000), Isnull(I.' + @ColName + ', ''''))
													)'
											Exec(@SQLQuery)
			
										End 

									End
	
									Set @I = @I + 1	                
								End
								/**** End **********************/

								Drop Table #INSERTED
								Drop Table #DELETED

							End

							";

			migrationBuilder.Sql(trigger);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			string trigger = @"Drop Trigger Trig_RequestTypeAuditTrailUpdate";
			migrationBuilder.Sql(trigger);
		}
    }
}
