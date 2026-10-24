using InventoryManagementApp.Models;

namespace InventoryManagementApp.Data
{
    public static class DbInitializer
    {
        public static void Initialize(InventoryDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Products.Any())
            {
                return; // Database has already been seeded
            }

            // 1. Seed Categories
            var categories = new[]
            {
                new Category { Name = "Enterprise Hardware", Code = "HARDWARE", Description = "Servers, laptops, workstations, and high-performance computing hardware." },
                new Category { Name = "Network & Telecom", Code = "NETWORKING", Description = "Enterprise switches, routers, fiber transceivers, and patch panels." },
                new Category { Name = "Industrial Tools & Safety", Code = "TOOLS", Description = "Diagnostic multimeters, power tools, safety gear, and testing devices." },
                new Category { Name = "Peripherals & Accessories", Code = "ACCESSORIES", Description = "Ergonomic mice, mechanical keyboards, docks, and video adapters." }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();

            // 2. Seed Warehouses
            var warehouses = new[]
            {
                new Warehouse { Code = "WH-CENTRAL", Name = "Central Logistics Hub", City = "Chicago", AddressLine = "1000 Industrial Parkway" },
                new Warehouse { Code = "WH-PACIFIC", Name = "Pacific Distribution Depot", City = "Seattle", AddressLine = "450 Harbor Boulevard" },
                new Warehouse { Code = "WH-SOUTH", Name = "Southern Regional Warehouse", City = "Dallas", AddressLine = "220 Commerce Way" }
            };
            context.Warehouses.AddRange(warehouses);
            context.SaveChanges();

            // 3. Seed Locations (Aisle, Shelf, Bin)
            var locations = new[]
            {
                new Location { WarehouseId = warehouses[0].Id, Aisle = "A01", Shelf = "S01", Bin = "B01", MaxVolumeCapacity = 500 },
                new Location { WarehouseId = warehouses[0].Id, Aisle = "A01", Shelf = "S02", Bin = "B02", MaxVolumeCapacity = 500 },
                new Location { WarehouseId = warehouses[0].Id, Aisle = "A02", Shelf = "S01", Bin = "B05", MaxVolumeCapacity = 600 },
                new Location { WarehouseId = warehouses[0].Id, Aisle = "A03", Shelf = "S03", Bin = "B10", MaxVolumeCapacity = 800 },
                new Location { WarehouseId = warehouses[1].Id, Aisle = "P01", Shelf = "S01", Bin = "B01", MaxVolumeCapacity = 400 },
                new Location { WarehouseId = warehouses[1].Id, Aisle = "P02", Shelf = "S04", Bin = "B08", MaxVolumeCapacity = 400 },
                new Location { WarehouseId = warehouses[2].Id, Aisle = "D01", Shelf = "S02", Bin = "B03", MaxVolumeCapacity = 750 }
            };
            context.Locations.AddRange(locations);
            context.SaveChanges();

            // 4. Seed Products
            var products = new[]
            {
                new Product 
                { 
                    SKU = "ELEC-DELL-5530", 
                    Barcode = "884116321456", 
                    Name = "Dell Precision 5530 Workstation 32GB RAM", 
                    CategoryId = categories[0].Id, 
                    UnitCost = 1250.00m, 
                    SellingPrice = 1699.99m, 
                    DefaultReorderPoint = 15,
                    Description = "High-end mobile workstation with Intel Core i7, 32GB DDR4, NVIDIA Quadro P1000."
                },
                new Product 
                { 
                    SKU = "ELEC-LENOVO-X1", 
                    Barcode = "793573214099", 
                    Name = "Lenovo ThinkPad X1 Carbon Gen 11", 
                    CategoryId = categories[0].Id, 
                    UnitCost = 1100.00m, 
                    SellingPrice = 1450.00m, 
                    DefaultReorderPoint = 12,
                    Description = "Ultra-lightweight 14-inch business laptop with 13th Gen Intel Core and 16GB RAM."
                },
                new Product 
                { 
                    SKU = "NETW-CISCO-C9300", 
                    Barcode = "088265882312", 
                    Name = "Cisco Catalyst 9300 48-Port PoE+ Gigabit Switch", 
                    CategoryId = categories[1].Id, 
                    UnitCost = 2800.00m, 
                    SellingPrice = 3750.00m, 
                    DefaultReorderPoint = 5,
                    Description = "Enterprise-class stackable access layer switch with full PoE+ and redundant power."
                },
                new Product 
                { 
                    SKU = "NETW-UBI-U6PRO", 
                    Barcode = "810354027788", 
                    Name = "Ubiquiti UniFi6 Pro WiFi 6 Access Point", 
                    CategoryId = categories[1].Id, 
                    UnitCost = 120.00m, 
                    SellingPrice = 169.00m, 
                    DefaultReorderPoint = 20,
                    Description = "Dual-band WiFi 6 AP with 5.3 Gbps aggregate throughput and PoE support."
                },
                new Product 
                { 
                    SKU = "TOOL-FLUKE-87V", 
                    Barcode = "095969095847", 
                    Name = "Fluke 87V Industrial Digital Multimeter", 
                    CategoryId = categories[2].Id, 
                    UnitCost = 360.00m, 
                    SellingPrice = 485.00m, 
                    DefaultReorderPoint = 8,
                    Description = "True-RMS industrial digital multimeter for high-precision electrical diagnostics."
                },
                new Product 
                { 
                    SKU = "TOOL-MILW-M18F", 
                    Barcode = "045242567812", 
                    Name = "Milwaukee M18 FUEL 1/2\" Hammer Drill/Driver", 
                    CategoryId = categories[2].Id, 
                    UnitCost = 180.00m, 
                    SellingPrice = 249.00m, 
                    DefaultReorderPoint = 10,
                    Description = "Brushless hammer drill kit with REDLITHIUM battery pack."
                },
                new Product 
                { 
                    SKU = "PERI-LOGI-MX3S", 
                    Barcode = "097855149367", 
                    Name = "Logitech MX Master 3S Performance Mouse", 
                    CategoryId = categories[3].Id, 
                    UnitCost = 65.00m, 
                    SellingPrice = 99.99m, 
                    DefaultReorderPoint = 25,
                    Description = "Quiet click ergonomic Bluetooth mouse with 8K DPI sensor and MagSpeed scrolling."
                },
                new Product 
                { 
                    SKU = "PERI-DELL-WD19", 
                    Barcode = "884116345678", 
                    Name = "Dell Thunderbolt Dock WD19TBS 180W", 
                    CategoryId = categories[3].Id, 
                    UnitCost = 195.00m, 
                    SellingPrice = 279.00m, 
                    DefaultReorderPoint = 15,
                    Description = "Universal Thunderbolt 3 modular dock delivering up to 130W power delivery."
                }
            };
            context.Products.AddRange(products);
            context.SaveChanges();

            // 5. Seed Stock Balances (Mix of In Stock, Low Stock, and Out of Stock)
            var balances = new[]
            {
                // In Stock items
                new StockBalance { ProductId = products[0].Id, LocationId = locations[0].Id, QuantityOnHand = 48, QuantityAllocated = 6, ReorderPoint = 15 },
                new StockBalance { ProductId = products[1].Id, LocationId = locations[1].Id, QuantityOnHand = 35, QuantityAllocated = 3, ReorderPoint = 12 },
                new StockBalance { ProductId = products[3].Id, LocationId = locations[3].Id, QuantityOnHand = 80, QuantityAllocated = 10, ReorderPoint = 20 },
                new StockBalance { ProductId = products[7].Id, LocationId = locations[6].Id, QuantityOnHand = 28, QuantityAllocated = 2, ReorderPoint = 15 },

                // Low Stock items (OnHand <= ReorderPoint)
                new StockBalance { ProductId = products[2].Id, LocationId = locations[2].Id, QuantityOnHand = 3, QuantityAllocated = 1, ReorderPoint = 5 },
                new StockBalance { ProductId = products[4].Id, LocationId = locations[4].Id, QuantityOnHand = 4, QuantityAllocated = 0, ReorderPoint = 8 },
                new StockBalance { ProductId = products[6].Id, LocationId = locations[0].Id, QuantityOnHand = 12, QuantityAllocated = 4, ReorderPoint = 25 },

                // Out of Stock item (OnHand == 0)
                new StockBalance { ProductId = products[5].Id, LocationId = locations[5].Id, QuantityOnHand = 0, QuantityAllocated = 0, ReorderPoint = 10 }
            };
            context.StockBalances.AddRange(balances);
            context.SaveChanges();

            // 6. Seed Historical Transactions
            var transactions = new[]
            {
                new StockTransaction 
                { 
                    ProductId = products[0].Id, 
                    LocationId = locations[0].Id, 
                    QuantityChanged = 55, 
                    ResultingBalance = 55, 
                    TransactionType = TransactionType.Receipt, 
                    ReferenceNumber = "PO-2026-0081", 
                    Notes = "Initial manufacturer purchase order receipt.", 
                    CreatedBy = "Vikram Kumar", 
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-10) 
                },
                new StockTransaction 
                { 
                    ProductId = products[0].Id, 
                    LocationId = locations[0].Id, 
                    QuantityChanged = -7, 
                    ResultingBalance = 48, 
                    TransactionType = TransactionType.Dispatch, 
                    ReferenceNumber = "SO-2026-0142", 
                    Notes = "Fulfillment of enterprise client order #142.", 
                    CreatedBy = "Staff Picker A", 
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-2) 
                },
                new StockTransaction 
                { 
                    ProductId = products[2].Id, 
                    LocationId = locations[2].Id, 
                    QuantityChanged = 10, 
                    ResultingBalance = 10, 
                    TransactionType = TransactionType.Receipt, 
                    ReferenceNumber = "PO-2026-0094", 
                    Notes = "Cisco distributor delivery batch.", 
                    CreatedBy = "Vikram Kumar", 
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-7) 
                },
                new StockTransaction 
                { 
                    ProductId = products[2].Id, 
                    LocationId = locations[2].Id, 
                    QuantityChanged = -7, 
                    ResultingBalance = 3, 
                    TransactionType = TransactionType.Dispatch, 
                    ReferenceNumber = "SO-2026-0199", 
                    Notes = "Dispatched for datacenter deployment project.", 
                    CreatedBy = "Staff Picker B", 
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-1) 
                },
                new StockTransaction 
                { 
                    ProductId = products[6].Id, 
                    LocationId = locations[0].Id, 
                    QuantityChanged = -1, 
                    ResultingBalance = 12, 
                    TransactionType = TransactionType.Adjustment, 
                    ReferenceNumber = "ADJ-2026-0003", 
                    Notes = "Cycle count audit: 1 unit damaged in transit.", 
                    CreatedBy = "Inventory Auditor", 
                    CreatedAtUtc = DateTime.UtcNow.AddHours(-12) 
                }
            };
            context.StockTransactions.AddRange(transactions);
            context.SaveChanges();
        }
    }
}
