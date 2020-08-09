using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class AddTable_RegisterAttempt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegisterAttempt",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    profileId = table.Column<Guid>(nullable: false),
                    OTP = table.Column<string>(nullable: true),
                    IsUsed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisterAttempt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegisterAttempt_Profiles_profileId",
                        column: x => x.profileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_RegisterAttempt_profileId",
                table: "RegisterAttempt",
                column: "profileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegisterAttempt");

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: new Guid("2a90154d-d5a5-473e-a8e0-367ff1c8ec71"),
                column: "CreatedDate",
                value: new DateTime(2020, 8, 7, 17, 1, 20, 623, DateTimeKind.Local).AddTicks(1966));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: new Guid("fc39a0fe-3525-4064-8d77-e7d8d08bcef5"),
                column: "CreatedDate",
                value: new DateTime(2020, 8, 7, 17, 1, 20, 625, DateTimeKind.Local).AddTicks(6196));
        }
    }
}
