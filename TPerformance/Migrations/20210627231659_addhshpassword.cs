using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TPerformance.Migrations
{
    public partial class addhshpassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_topics_customers_CustomerId",
                table: "topics");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "topics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "customers",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "customers",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_topics_customers_CustomerId",
                table: "topics",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_topics_customers_CustomerId",
                table: "topics");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "customers");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "topics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_topics_customers_CustomerId",
                table: "topics",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
