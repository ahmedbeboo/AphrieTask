using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class AddTable_PostsInteraction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostsInteraction",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    postId = table.Column<Guid>(nullable: false),
                    postReact = table.Column<int>(nullable: false),
                    postComments = table.Column<string>(nullable: true),
                    userInteractId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsInteraction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostsInteraction_Posts_postId",
                        column: x => x.postId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostsInteraction_Profiles_userInteractId",
                        column: x => x.userInteractId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_PostsInteraction_postId",
                table: "PostsInteraction",
                column: "postId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsInteraction_userInteractId",
                table: "PostsInteraction",
                column: "userInteractId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostsInteraction");

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: new Guid("2a90154d-d5a5-473e-a8e0-367ff1c8ec71"),
                column: "CreatedDate",
                value: new DateTime(2020, 8, 7, 16, 55, 45, 871, DateTimeKind.Local).AddTicks(6684));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: new Guid("fc39a0fe-3525-4064-8d77-e7d8d08bcef5"),
                column: "CreatedDate",
                value: new DateTime(2020, 8, 7, 16, 55, 45, 873, DateTimeKind.Local).AddTicks(5493));
        }
    }
}
