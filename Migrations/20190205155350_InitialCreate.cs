using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectTo_Issue.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatBreed",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BreedName = table.Column<string>(nullable: true),
                    CreatedByWUPeopleId = table.Column<string>(nullable: true),
                    CreatedByDisplayName = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    UpdatedByWUPeopleId = table.Column<string>(nullable: true),
                    UpdatedByDisplayName = table.Column<string>(nullable: true),
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
                name: "Cat",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MeowLoudness = table.Column<int>(nullable: false),
                    CatBreedId = table.Column<int>(nullable: true),
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
                    table.ForeignKey(
                        name: "FK_Cat_CatBreed_CatBreedId",
                        column: x => x.CatBreedId,
                        principalTable: "CatBreed",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cat_CatBreedId",
                table: "Cat",
                column: "CatBreedId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cat");

            migrationBuilder.DropTable(
                name: "GenericAudit");

            migrationBuilder.DropTable(
                name: "CatBreed");
        }
    }
}
