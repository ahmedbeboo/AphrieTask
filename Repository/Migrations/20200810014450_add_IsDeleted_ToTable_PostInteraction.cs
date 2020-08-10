using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class add_IsDeleted_ToTable_PostInteraction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "PostsInteraction",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: new Guid("2a90154d-d5a5-473e-a8e0-367ff1c8ec71"),
                column: "CreatedDate",
                value: new DateTime(2020, 8, 10, 3, 44, 50, 146, DateTimeKind.Local).AddTicks(6313));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: new Guid("fc39a0fe-3525-4064-8d77-e7d8d08bcef5"),
                column: "CreatedDate",
                value: new DateTime(2020, 8, 10, 3, 44, 50, 148, DateTimeKind.Local).AddTicks(6568));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "PostsInteraction");

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
    }
}
