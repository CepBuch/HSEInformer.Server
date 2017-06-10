using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HSEInformer.Server.Migrations
{
    public partial class zureTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Confirmations_Users_UserId",
                table: "Confirmations");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Confirmations",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Confirmations_UserId",
                table: "Confirmations",
                newName: "IX_Confirmations_MemberId");

            migrationBuilder.AddColumn<string>(
                name: "NewColumn",
                table: "Confirmations",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Confirmations_HseMembers_MemberId",
                table: "Confirmations",
                column: "MemberId",
                principalTable: "HseMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Confirmations_HseMembers_MemberId",
                table: "Confirmations");

            migrationBuilder.DropColumn(
                name: "NewColumn",
                table: "Confirmations");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "Confirmations",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Confirmations_MemberId",
                table: "Confirmations",
                newName: "IX_Confirmations_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Confirmations_Users_UserId",
                table: "Confirmations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
