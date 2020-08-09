using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class initialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    languageName = table.Column<string>(nullable: false),
                    languageCulture = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    BirthDate = table.Column<DateTime>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    LoginType = table.Column<int>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    InvitedBy = table.Column<string>(nullable: true),
                    Language = table.Column<string>(nullable: true),
                    MyInvitationCode = table.Column<string>(nullable: true),
                    isBlocked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "loacalizProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    localizeLangugeId = table.Column<Guid>(nullable: false),
                    localizeTableName = table.Column<string>(nullable: true),
                    localizeAttributeName = table.Column<string>(nullable: true),
                    localizeValue = table.Column<string>(nullable: true),
                    localizeEntityId = table.Column<Guid>(nullable: false),
                    isDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loacalizProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_loacalizProperties_Languages_localizeLangugeId",
                        column: x => x.localizeLangugeId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FriendInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    SenderId = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Processed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendInvitations_Profiles_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    imagesUrl = table.Column<string>(nullable: true),
                    profileId = table.Column<Guid>(nullable: false),
                    isDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Profiles_profileId",
                        column: x => x.profileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfilePasswords",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    profileId = table.Column<Guid>(nullable: false),
                    HashPassword = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilePasswords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfilePasswords_Profiles_profileId",
                        column: x => x.profileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    SenderProfileId = table.Column<Guid>(nullable: false),
                    receiverProfileId = table.Column<Guid>(nullable: false),
                    RelationStatus = table.Column<int>(nullable: false),
                    InvitationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Friends_FriendInvitations_InvitationId",
                        column: x => x.InvitationId,
                        principalTable: "FriendInvitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friends_Profiles_SenderProfileId",
                        column: x => x.SenderProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friends_Profiles_receiverProfileId",
                        column: x => x.receiverProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "CreatedDate", "UpdatedDate", "languageCulture", "languageName" },
                values: new object[] { new Guid("2a90154d-d5a5-473e-a8e0-367ff1c8ec71"), new DateTime(2020, 8, 7, 16, 55, 45, 871, DateTimeKind.Local).AddTicks(6684), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "en-US", "EN" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "CreatedDate", "UpdatedDate", "languageCulture", "languageName" },
                values: new object[] { new Guid("fc39a0fe-3525-4064-8d77-e7d8d08bcef5"), new DateTime(2020, 8, 7, 16, 55, 45, 873, DateTimeKind.Local).AddTicks(5493), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ar-EG", "AR" });

            migrationBuilder.CreateIndex(
                name: "IX_FriendInvitations_SenderId",
                table: "FriendInvitations",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_InvitationId",
                table: "Friends",
                column: "InvitationId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_SenderProfileId",
                table: "Friends",
                column: "SenderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_receiverProfileId",
                table: "Friends",
                column: "receiverProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_loacalizProperties_localizeLangugeId",
                table: "loacalizProperties",
                column: "localizeLangugeId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_profileId",
                table: "Posts",
                column: "profileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilePasswords_profileId",
                table: "ProfilePasswords",
                column: "profileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friends");

            migrationBuilder.DropTable(
                name: "loacalizProperties");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "ProfilePasswords");

            migrationBuilder.DropTable(
                name: "FriendInvitations");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
