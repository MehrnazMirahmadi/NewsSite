using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainModel.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewsCategories",
                columns: table => new
                {
                    NewsCategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SmallDescription = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParentID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsCategories", x => x.NewsCategoryID);
                    table.ForeignKey(
                        name: "FK_NewsCategories_NewsCategories_ParentID",
                        column: x => x.ParentID,
                        principalTable: "NewsCategories",
                        principalColumn: "NewsCategoryID");
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    NewsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NewsTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SmallDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewsText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    VisitCount = table.Column<int>(type: "int", nullable: false),
                    VoteSumation = table.Column<int>(type: "int", nullable: false),
                    VoteCount = table.Column<int>(type: "int", nullable: false),
                    NewsCategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.NewsID);
                    table.ForeignKey(
                        name: "FK_News_NewsCategories_NewsCategoryID",
                        column: x => x.NewsCategoryID,
                        principalTable: "NewsCategories",
                        principalColumn: "NewsCategoryID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_News_NewsCategoryID",
                table: "News",
                column: "NewsCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_NewsCategories_ParentID",
                table: "NewsCategories",
                column: "ParentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "NewsCategories");
        }
    }
}
