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
            // Create BatchOperations table
            migrationBuilder.CreateTable(
                name: "BatchOperations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    failure_count = table.Column<int>(type: "integer", nullable: false),
                    idempotency_key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    operation_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    result_data = table.Column<string>(type: "jsonb", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    success_count = table.Column<int>(type: "integer", nullable: false),
                    total_items = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchOperations", x => x.id);
                });

            // Create ProductMasters table
            migrationBuilder.CreateTable(
                name: "ProductMasters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Attributes = table.Column<string>(type: "jsonb", nullable: true),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMasters", x => x.Id);
                });

            // Create ProductBundles table
            migrationBuilder.CreateTable(
                name: "ProductBundles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Metadata = table.Column<string>(type: "jsonb", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBundles", x => x.Id);
                });

            // Create ProductVariants table
            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Attributes = table.Column<string>(type: "jsonb", nullable: true),
                    CombinationKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ProductMasterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariants_ProductMasters_ProductMasterId",
                        column: x => x.ProductMasterId,
                        principalTable: "ProductMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create SellableItems table
            migrationBuilder.CreateTable(
                name: "SellableItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    BundleId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    SKU = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Type = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    VariantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellableItems", x => x.Id);
                    table.CheckConstraint("ck_sellable_item_one_of", "(\"VariantId\" IS NOT NULL AND \"BundleId\" IS NULL) OR (\"VariantId\" IS NULL AND \"BundleId\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_SellableItems_ProductBundles_BundleId",
                        column: x => x.BundleId,
                        principalTable: "ProductBundles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellableItems_ProductVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create Warehouses table
            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Metadata = table.Column<string>(type: "jsonb", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            // Create VariantOptions table
            migrationBuilder.CreateTable(
                name: "VariantOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProductMasterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VariantOptions_ProductMasters_ProductMasterId",
                        column: x => x.ProductMasterId,
                        principalTable: "ProductMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create BundleItems table
            migrationBuilder.CreateTable(
                name: "BundleItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    BundleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    SellableItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BundleItems", x => x.Id);
                    table.CheckConstraint("ck_quantity_positive", "\"Quantity\" > 0");
                    table.ForeignKey(
                        name: "FK_BundleItems_ProductBundles_BundleId",
                        column: x => x.BundleId,
                        principalTable: "ProductBundles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BundleItems_SellableItems_SellableItemId",
                        column: x => x.SellableItemId,
                        principalTable: "SellableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create InventoryRecords table
            migrationBuilder.CreateTable(
                name: "InventoryRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    OnHand = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Reserved = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SellableItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryRecords_SellableItems_SellableItemId",
                        column: x => x.SellableItemId,
                        principalTable: "SellableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryRecords_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create VariantOptionValues table
            migrationBuilder.CreateTable(
                name: "VariantOptionValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    ProductVariantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    VariantOptionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantOptionValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VariantOptionValues_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VariantOptionValues_VariantOptions_VariantOptionId",
                        column: x => x.VariantOptionId,
                        principalTable: "VariantOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create all indexes
            migrationBuilder.CreateIndex(
                name: "ix_batch_operations_expires_at",
                table: "BatchOperations",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "ix_batch_operations_idempotency_key",
                table: "BatchOperations",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_batch_operations_operation_type",
                table: "BatchOperations",
                column: "operation_type");

            migrationBuilder.CreateIndex(
                name: "uk_bundle_sellable_item",
                table: "BundleItems",
                columns: new[] { "BundleId", "SellableItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BundleItems_SellableItemId",
                table: "BundleItems",
                column: "SellableItemId");

            migrationBuilder.CreateIndex(
                name: "uk_inventory_sellable_warehouse",
                table: "InventoryRecords",
                columns: new[] { "SellableItemId", "WarehouseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRecords_WarehouseId",
                table: "InventoryRecords",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "idx_bundle_metadata",
                table: "ProductBundles",
                column: "Metadata")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "idx_product_master_attributes",
                table: "ProductMasters",
                column: "Attributes")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "uk_product_master_slug",
                table: "ProductMasters",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_product_variant_attributes",
                table: "ProductVariants",
                column: "Attributes")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "uk_variant_combination",
                table: "ProductVariants",
                columns: new[] { "ProductMasterId", "CombinationKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SellableItems_BundleId",
                table: "SellableItems",
                column: "BundleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_sellable_item_sku",
                table: "SellableItems",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SellableItems_VariantId",
                table: "SellableItems",
                column: "VariantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_variant_option_slug_per_product",
                table: "VariantOptions",
                columns: new[] { "ProductMasterId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VariantOptionValues_ProductVariantId",
                table: "VariantOptionValues",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "uk_option_value_per_option",
                table: "VariantOptionValues",
                columns: new[] { "VariantOptionId", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uk_warehouse_code",
                table: "Warehouses",
                column: "Code",
                unique: true);

            // Insert seed data
            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "Id", "Address", "Code", "CreatedAt", "Metadata", "Name", "Status", "UpdatedAt" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "", "MAIN", new DateTime(2025, 10, 20, 20, 31, 36, 73, DateTimeKind.Utc).AddTicks(8732), null, "Main Warehouse", 1, new DateTime(2025, 10, 20, 20, 31, 36, 73, DateTimeKind.Utc).AddTicks(8735) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "VariantOptionValues");
            migrationBuilder.DropTable(name: "InventoryRecords");
            migrationBuilder.DropTable(name: "BundleItems");
            migrationBuilder.DropTable(name: "VariantOptions");
            migrationBuilder.DropTable(name: "SellableItems");
            migrationBuilder.DropTable(name: "ProductVariants");
            migrationBuilder.DropTable(name: "ProductBundles");
            migrationBuilder.DropTable(name: "ProductMasters");
            migrationBuilder.DropTable(name: "Warehouses");
            migrationBuilder.DropTable(name: "BatchOperations");
        }
    }
}
