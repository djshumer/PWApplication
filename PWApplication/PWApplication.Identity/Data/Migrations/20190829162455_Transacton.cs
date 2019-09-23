using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PWApplication.Identity.Data.Migrations
{
    public partial class Transacton : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PWTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AgentId = table.Column<string>(nullable: false),
                    СounteragentId = table.Column<string>(nullable: false),
                    OperationDateTime = table.Column<DateTime>(nullable: false),
                    TransactionAmount = table.Column<decimal>(nullable: false),
                    AgentBalance = table.Column<decimal>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PWTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PWTransactions_AspNetUsers_AgentId",
                        column: x => x.AgentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PWTransactions_AspNetUsers_СounteragentId",
                        column: x => x.СounteragentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PWOperationPairs",
                columns: table => new
                {
                    TransactionOneId = table.Column<Guid>(nullable: false),
                    TransactionTwoId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PWOperationPair", x => new { x.TransactionOneId, x.TransactionTwoId });
                    table.ForeignKey(
                        name: "FK_PWOperationPair_PWTransactions_TransactionOneId",
                        column: x => x.TransactionOneId,
                        principalTable: "PWTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PWOperationPair_PWTransactions_TransactionTwoId",
                        column: x => x.TransactionTwoId,
                        principalTable: "PWTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PWOperationPair_TransactionTwoId",
                table: "PWOperationPairs",
                column: "TransactionTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_PWTransactions_AgentId",
                table: "PWTransactions",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_PWTransactions_СounteragentId",
                table: "PWTransactions",
                column: "СounteragentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PWOperationPairs");

            migrationBuilder.DropTable(
                name: "PWTransactions");
        }
    }
}
