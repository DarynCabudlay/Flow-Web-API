using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace workflow.Migrations
{
    public partial class DropIndexInRequestTypeProcessOwners : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
               name: "IX_RequestTypeProcessOwners_RequestCategoryId_RequestTypeId_Version",
               table: "RequestTypeProcessOwners");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
