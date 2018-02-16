using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace labti.Migrations
{
    public partial class ProfesorChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AsignaturaId",
                table: "Profesores");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AsignaturaId",
                table: "Profesores",
                nullable: false,
                defaultValue: 0);
        }
    }
}
