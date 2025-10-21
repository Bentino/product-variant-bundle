using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductVariantBundle.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Metadata = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "uk_warehouse_code",
                table: "Warehouses",
                column: "Code",
                unique: true);

            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "Id", "Code", "Name", "Address", "Metadata", "CreatedAt", "UpdatedAt", "Status" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "MAIN", "Main Warehouse", "", null, new DateTime(2025, 10, 20, 20, 31, 36, 73, DateTimeKind.Utc).AddTicks(8732), new DateTime(2025, 10, 20, 20, 31, 36, 73, DateTimeKind.Utc).AddTicks(8735), 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Warehouses");
        }
    }
}
