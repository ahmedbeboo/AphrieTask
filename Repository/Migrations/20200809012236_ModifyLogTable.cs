using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class ModifyLogTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Exception",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "LogEvent",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "MessageTemplate",
                table: "Log");

            migrationBuilder.AlterColumn<int>(
                name: "Level",
                table: "Log",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MSG",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "requestInfo",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "responseInfo",
                table: "Log",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "userInfo",
                table: "Log",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: new Guid("2a90154d-d5a5-473e-a8e0-367ff1c8ec71"),
                column: "CreatedDate",
                value: new DateTime(2020, 8, 9, 3, 22, 35, 927, DateTimeKind.Local).AddTicks(7281));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: new Guid("fc39a0fe-3525-4064-8d77-e7d8d08bcef5"),
                column: "CreatedDate",
                value: new DateTime(2020, 8, 9, 3, 22, 35, 929, DateTimeKind.Local).AddTicks(5572));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MSG",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "requestInfo",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "responseInfo",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "userInfo",
                table: "Log");

            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "Log",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "Exception",
                table: "Log",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogEvent",
                table: "Log",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Log",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageTemplate",
                table: "Log",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: new Guid("2a90154d-d5a5-473e-a8e0-367ff1c8ec71"),
                column: "CreatedDate",
                value: new DateTime(2020, 8, 8, 16, 21, 33, 642, DateTimeKind.Local).AddTicks(616));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: new Guid("fc39a0fe-3525-4064-8d77-e7d8d08bcef5"),
                column: "CreatedDate",
                value: new DateTime(2020, 8, 8, 16, 21, 33, 643, DateTimeKind.Local).AddTicks(4190));
        }
    }
}
