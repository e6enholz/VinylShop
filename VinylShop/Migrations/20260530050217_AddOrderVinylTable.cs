using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VinylShop.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderVinylTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "order_vinyls",
                columns: table => new
                {
                    id_order_vinyl = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    deliveryId = table.Column<int>(type: "integer", nullable: false),
                    vinylId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_vinyls", x => x.id_order_vinyl);
                    table.ForeignKey(
                        name: "FK_order_vinyls_deliveries_deliveryId",
                        column: x => x.deliveryId,
                        principalTable: "deliveries",
                        principalColumn: "id_delivery",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_vinyls_vinyls_vinylId",
                        column: x => x.vinylId,
                        principalTable: "vinyls",
                        principalColumn: "id_vinyl",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "deliveries",
                keyColumn: "id_delivery",
                keyValue: 1,
                column: "orderDate",
                value: new DateTime(2026, 5, 30, 5, 2, 17, 103, DateTimeKind.Utc).AddTicks(4491));

            migrationBuilder.CreateIndex(
                name: "IX_order_vinyls_deliveryId",
                table: "order_vinyls",
                column: "deliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_order_vinyls_vinylId",
                table: "order_vinyls",
                column: "vinylId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_vinyls");

            migrationBuilder.UpdateData(
                table: "deliveries",
                keyColumn: "id_delivery",
                keyValue: 1,
                column: "orderDate",
                value: new DateTime(2026, 5, 30, 3, 47, 30, 554, DateTimeKind.Utc).AddTicks(1439));
        }
    }
}
