using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class addLogTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    MessageTemplate = table.Column<string>(nullable: true),
                    Level = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    LogEvent = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: new Guid("2a90154d-d5a5-473e-a8e0-367ff1c8ec71"),
                column: "CreatedDate",
                value: new DateTime(2020, 8, 7, 19, 51, 48, 357, DateTimeKind.Local).AddTicks(9237));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: new Guid("fc39a0fe-3525-4064-8d77-e7d8d08bcef5"),
                column: "CreatedDate",
                value: new DateTime(2020, 8, 7, 19, 51, 48, 359, DateTimeKind.Local).AddTicks(3616));
        }
    }
}
