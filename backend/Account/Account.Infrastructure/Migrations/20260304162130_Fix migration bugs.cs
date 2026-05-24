using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fixmigrationbugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                schema: "account",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                schema: "account",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Players",
                schema: "account",
                table: "Players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                schema: "account",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailCodes",
                schema: "account",
                table: "EmailCodes");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "account",
                newName: "users",
                newSchema: "account");

            migrationBuilder.RenameTable(
                name: "Players",
                schema: "account",
                newName: "players",
                newSchema: "account");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                schema: "account",
                newName: "refresh_tokens",
                newSchema: "account");

            migrationBuilder.RenameTable(
                name: "EmailCodes",
                schema: "account",
                newName: "email_verification_codes",
                newSchema: "account");

            migrationBuilder.RenameColumn(
                name: "TournamentsId",
                schema: "account",
                table: "players",
                newName: "TournamentsIds");

            migrationBuilder.RenameColumn(
                name: "GamesId",
                schema: "account",
                table: "players",
                newName: "GamesIds");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "account",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_TokenHash",
                schema: "account",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_TokenHash");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                schema: "account",
                table: "users",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                schema: "account",
                table: "users",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Elo",
                schema: "account",
                table: "players",
                type: "integer",
                nullable: false,
                defaultValue: 1000,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "TokenHash",
                schema: "account",
                table: "refresh_tokens",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ReplacedByToken",
                schema: "account",
                table: "refresh_tokens",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsUsed",
                schema: "account",
                table: "email_verification_codes",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsManuallyDeactivated",
                schema: "account",
                table: "email_verification_codes",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "HashedCode",
                schema: "account",
                table: "email_verification_codes",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "account",
                table: "email_verification_codes",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                schema: "account",
                table: "users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_players",
                schema: "account",
                table: "players",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_tokens",
                schema: "account",
                table: "refresh_tokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_email_verification_codes",
                schema: "account",
                table: "email_verification_codes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                schema: "account",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Username",
                schema: "account",
                table: "users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_email_verification_codes_Email",
                schema: "account",
                table: "email_verification_codes",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_email_verification_codes_Email_IsUsed",
                schema: "account",
                table: "email_verification_codes",
                columns: new[] { "Email", "IsUsed" });

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_users_UserId",
                schema: "account",
                table: "refresh_tokens",
                column: "UserId",
                principalSchema: "account",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_users_UserId",
                schema: "account",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                schema: "account",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_Email",
                schema: "account",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_Username",
                schema: "account",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_players",
                schema: "account",
                table: "players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_tokens",
                schema: "account",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_email_verification_codes",
                schema: "account",
                table: "email_verification_codes");

            migrationBuilder.DropIndex(
                name: "IX_email_verification_codes_Email",
                schema: "account",
                table: "email_verification_codes");

            migrationBuilder.DropIndex(
                name: "IX_email_verification_codes_Email_IsUsed",
                schema: "account",
                table: "email_verification_codes");

            migrationBuilder.RenameTable(
                name: "users",
                schema: "account",
                newName: "Users",
                newSchema: "account");

            migrationBuilder.RenameTable(
                name: "players",
                schema: "account",
                newName: "Players",
                newSchema: "account");

            migrationBuilder.RenameTable(
                name: "refresh_tokens",
                schema: "account",
                newName: "RefreshTokens",
                newSchema: "account");

            migrationBuilder.RenameTable(
                name: "email_verification_codes",
                schema: "account",
                newName: "EmailCodes",
                newSchema: "account");

            migrationBuilder.RenameColumn(
                name: "TournamentsIds",
                schema: "account",
                table: "Players",
                newName: "TournamentsId");

            migrationBuilder.RenameColumn(
                name: "GamesIds",
                schema: "account",
                table: "Players",
                newName: "GamesId");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_UserId",
                schema: "account",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_TokenHash",
                schema: "account",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_TokenHash");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                schema: "account",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                schema: "account",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<int>(
                name: "Elo",
                schema: "account",
                table: "Players",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "TokenHash",
                schema: "account",
                table: "RefreshTokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "ReplacedByToken",
                schema: "account",
                table: "RefreshTokens",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsUsed",
                schema: "account",
                table: "EmailCodes",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsManuallyDeactivated",
                schema: "account",
                table: "EmailCodes",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "HashedCode",
                schema: "account",
                table: "EmailCodes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "account",
                table: "EmailCodes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                schema: "account",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Players",
                schema: "account",
                table: "Players",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                schema: "account",
                table: "RefreshTokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailCodes",
                schema: "account",
                table: "EmailCodes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                schema: "account",
                table: "RefreshTokens",
                column: "UserId",
                principalSchema: "account",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
