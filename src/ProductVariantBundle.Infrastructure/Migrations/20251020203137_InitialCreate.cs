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
            migrationBuilder.UpdateData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 20, 20, 31, 36, 73, DateTimeKind.Utc).AddTicks(8732), new DateTime(2025, 10, 20, 20, 31, 36, 73, DateTimeKind.Utc).AddTicks(8735) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 20, 20, 10, 7, 736, DateTimeKind.Utc).AddTicks(9780), new DateTime(2025, 10, 20, 20, 10, 7, 736, DateTimeKind.Utc).AddTicks(9782) });
        }
    }
}