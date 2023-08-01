using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class DropNotNeededColumnsAndRelationshipInRequestForms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.DropIndex(
                name: "IX_RequestForms_RequestTypeRequestCategoryId_RequestTypeId1_RequestTypeVersion",
                table: "RequestForms");

            migrationBuilder.DropForeignKey(
              name: "FK_RequestForms_RequestTypes_RequestTypeRequestCategoryId_RequestTypeId1_RequestTypeVersion",
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


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
