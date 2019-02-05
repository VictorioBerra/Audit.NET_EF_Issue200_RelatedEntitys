using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectTo_Issue.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cat",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MeowLoudness = table.Column<int>(nullable: false),
                    CreatedByWUPeopleId = table.Column<string>(nullable: false),
                    CreatedByDisplayName = table.Column<string>(nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    UpdatedByWUPeopleId = table.Column<string>(nullable: false),
                    UpdatedByDisplayName = table.Column<string>(nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatBreed",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BreedName = table.Column<string>(nullable: true),
                    CreatedByWUPeopleId = table.Column<string>(nullable: false),
                    CreatedByDisplayName = table.Column<string>(nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    UpdatedByWUPeopleId = table.Column<string>(nullable: false),
                    UpdatedByDisplayName = table.Column<string>(nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatBreed", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GenericAudit",
                columns: table => new
                {
                    GenericAuditId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PrimaryKey = table.Column<string>(nullable: false),
                    EntityType = table.Column<string>(nullable: false),
                    Action = table.Column<string>(nullable: false),
                    AuditDateUtc = table.Column<DateTime>(nullable: false),
                    AuditIdentity = table.Column<string>(nullable: false),
                    AuditIdentityDisplayName = table.Column<string>(nullable: false),
                    CorrelationId = table.Column<string>(nullable: false),
                    MSDuration = table.Column<int>(nullable: false),
                    NumObjectsEffected = table.Column<int>(nullable: false),
                    Success = table.Column<bool>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true),
                    AuditData = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericAudit", x => x.GenericAuditId);
                });

            migrationBuilder.CreateTable(
                name: "CatBreedLine",
                columns: table => new
                {
                    CatId = table.Column<int>(nullable: false),
                    CatBreedId = table.Column<int>(nullable: false),
                    CreatedByWUPeopleId = table.Column<string>(nullable: true),
                    CreatedByDisplayName = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    UpdatedByWUPeopleId = table.Column<string>(nullable: true),
                    UpdatedByDisplayName = table.Column<string>(nullable: true),
                    UpdatedOnUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatBreedLine", x => new { x.CatId, x.CatBreedId });
                    table.ForeignKey(
                        name: "FK_CatBreedLine_CatBreed_CatBreedId",
                        column: x => x.CatBreedId,
                        principalTable: "CatBreed",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatBreedLine_Cat_CatId",
                        column: x => x.CatId,
                        principalTable: "Cat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatBreedLine_CatBreedId",
                table: "CatBreedLine",
                column: "CatBreedId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatBreedLine");

            migrationBuilder.DropTable(
                name: "GenericAudit");

            migrationBuilder.DropTable(
                name: "CatBreed");

            migrationBuilder.DropTable(
                name: "Cat");
        }
    }
}
