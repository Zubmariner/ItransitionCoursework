using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace iCoursework.Migrations
{
    public partial class RatesCountInInstruction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RatesCount",
                table: "Instructions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatesCount",
                table: "Instructions");
        }
    }
}
