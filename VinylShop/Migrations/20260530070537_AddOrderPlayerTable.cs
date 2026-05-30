using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VinylShop.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderPlayerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "order_players",
                columns: table => new
                {
                    id_order_player = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    deliveryId = table.Column<int>(type: "integer", nullable: false),
                    playerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_players", x => x.id_order_player);
                    table.ForeignKey(
                        name: "FK_order_players_deliveries_deliveryId",
                        column: x => x.deliveryId,
                        principalTable: "deliveries",
                        principalColumn: "id_delivery",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_players_players_playerId",
                        column: x => x.playerId,
                        principalTable: "players",
                        principalColumn: "id_player",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "deliveries",
                keyColumn: "id_delivery",
                keyValue: 1,
                column: "orderDate",
                value: new DateTime(2026, 5, 30, 7, 5, 36, 648, DateTimeKind.Utc).AddTicks(7524));

            migrationBuilder.CreateIndex(
                name: "IX_order_players_deliveryId",
                table: "order_players",
                column: "deliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_order_players_playerId",
                table: "order_players",
                column: "playerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_players");

            migrationBuilder.UpdateData(
                table: "deliveries",
                keyColumn: "id_delivery",
                keyValue: 1,
                column: "orderDate",
                value: new DateTime(2026, 5, 30, 5, 2, 17, 103, DateTimeKind.Utc).AddTicks(4491));
        }
    }
}
