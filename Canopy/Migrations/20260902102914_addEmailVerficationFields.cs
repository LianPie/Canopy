using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Canopy.Migrations
{
    /// <inheritdoc />
    public partial class addEmailVerficationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User_Token",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "User_EmailVerificationCode",
                table: "User",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "User_VerificationCodeExpiry",
                table: "User",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User_EmailVerificationCode",
                table: "User");

            migrationBuilder.DropColumn(
                name: "User_VerificationCodeExpiry",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "User_Token",
                table: "User",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
