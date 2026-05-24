using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fixmigrationbug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                schema: "account",
                table: "RefreshTokens",
                newName: "TokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_Token",
                schema: "account",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_TokenHash");

            migrationBuilder.AddColumn<string>(
                name: "ReplacedByToken",
                schema: "account",
                table: "RefreshTokens",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAt",
                schema: "account",
                table: "RefreshTokens",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplacedByToken",
                schema: "account",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "RevokedAt",
                schema: "account",
                table: "RefreshTokens");

            migrationBuilder.RenameColumn(
                name: "TokenHash",
                schema: "account",
                table: "RefreshTokens",
                newName: "Token");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_TokenHash",
                schema: "account",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_Token");
        }
    }
}
