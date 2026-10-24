using InventoryManagementApp.Data;
using InventoryManagementApp.Models;
using InventoryManagementApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementApp.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly InventoryDbContext _context;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(InventoryDbContext context, ILogger<InventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DashboardViewModel> GetDashboardMetricsAsync()
        {
            var totalProducts = await _context.Products.CountAsync(p => p.IsActive);
            var balances = await _context.StockBalances
                .Include(sb => sb.Product)
                .Include(sb => sb.Location)
                    .ThenInclude(l => l.Warehouse)
                .AsNoTracking()
                .ToListAsync();

            var totalUnits = balances.Sum(b => b.QuantityOnHand);
            var totalValue = balances.Sum(b => b.QuantityOnHand * b.Product.UnitCost);
            var lowStockAlerts = balances.Where(b => b.IsLowStock).ToList();
            var outOfStockCount = balances.Count(b => b.IsOutOfStock);
            var totalWarehouses = await _context.Warehouses.CountAsync(w => w.IsActive);

            var recentTransactions = await _context.StockTransactions
                .Include(t => t.Product)
                .Include(t => t.Location)
                    .ThenInclude(l => l.Warehouse)
                .OrderByDescending(t => t.CreatedAtUtc)
                .Take(7)
                .AsNoTracking()
                .ToListAsync();

            var categoryCounts = await _context.Products
                .Include(p => p.Category)
                .GroupBy(p => p.Category.Name)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category, x => x.Count);

            return new DashboardViewModel
            {
                TotalProductsCount = totalProducts,
                TotalStockQuantity = totalUnits,
                TotalInventoryValue = totalValue,
                LowStockCount = lowStockAlerts.Count,
                OutOfStockCount = outOfStockCount,
                TotalWarehousesCount = totalWarehouses,
                LowStockAlerts = lowStockAlerts,
                RecentTransactions = recentTransactions,
                CategoryDistribution = categoryCounts
            };
        }

        public async Task<InventorySearchViewModel> SearchInventoryAsync(InventorySearchViewModel criteria)
        {
            // Dropdown options
            criteria.CategoryOptions = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();

            criteria.WarehouseOptions = await _context.Warehouses
                .Where(w => w.IsActive)
                .OrderBy(w => w.Name)
                .Select(w => new SelectListItem { Value = w.Id.ToString(), Text = $"{w.Name} ({w.Code})" })
                .ToListAsync();

            var query = _context.StockBalances
                .Include(sb => sb.Product)
                    .ThenInclude(p => p.Category)
                .Include(sb => sb.Location)
                    .ThenInclude(l => l.Warehouse)
                .AsNoTracking()
                .AsQueryable();

            // 1. Multi-Criteria: Text search across SKU, Barcode, Product Name
            if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
            {
                var term = criteria.SearchTerm.Trim().ToLower();
                query = query.Where(sb => 
                    sb.Product.SKU.ToLower().Contains(term) ||
                    (sb.Product.Barcode != null && sb.Product.Barcode.ToLower().Contains(term)) ||
                    sb.Product.Name.ToLower().Contains(term));
            }

            // 2. Multi-Criteria: Category filter
            if (criteria.CategoryId.HasValue && criteria.CategoryId.Value > 0)
            {
                query = query.Where(sb => sb.Product.CategoryId == criteria.CategoryId.Value);
            }

            // 3. Multi-Criteria: Warehouse filter
            if (criteria.WarehouseId.HasValue && criteria.WarehouseId.Value > 0)
            {
                query = query.Where(sb => sb.Location.WarehouseId == criteria.WarehouseId.Value);
            }

            // 4. Multi-Criteria: Stock status condition filter
            query = criteria.Status switch
            {
                StockStatusFilter.InStock => query.Where(sb => sb.QuantityOnHand > 0),
                StockStatusFilter.LowStock => query.Where(sb => sb.QuantityOnHand > 0 && sb.QuantityOnHand <= sb.ReorderPoint),
                StockStatusFilter.OutOfStock => query.Where(sb => sb.QuantityOnHand <= 0),
                _ => query
            };

            criteria.TotalCount = await query.CountAsync();

            // Paging & Projection
            var paged = await query
                .OrderBy(sb => sb.Product.Name)
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .Select(sb => new InventoryItemRowViewModel
                {
                    StockBalanceId = sb.Id,
                    ProductId = sb.ProductId,
                    SKU = sb.Product.SKU,
                    Barcode = sb.Product.Barcode ?? "N/A",
                    ProductName = sb.Product.Name,
                    CategoryName = sb.Product.Category.Name,
                    LocationId = sb.LocationId,
                    WarehouseName = sb.Location.Warehouse.Name,
                    LocationCode = sb.Location.LocationCode,
                    UnitCost = sb.Product.UnitCost,
                    SellingPrice = sb.Product.SellingPrice,
                    QuantityOnHand = sb.QuantityOnHand,
                    QuantityAllocated = sb.QuantityAllocated,
                    AvailableQuantity = Math.Max(0, sb.QuantityOnHand - sb.QuantityAllocated),
                    ReorderPoint = sb.ReorderPoint,
                    IsLowStock = sb.QuantityOnHand <= sb.ReorderPoint && sb.QuantityOnHand > 0,
                    IsOutOfStock = sb.QuantityOnHand <= 0
                })
                .ToListAsync();

            criteria.Results = paged;
            return criteria;
        }

        public async Task<StockOperationViewModel?> GetStockOperationDetailsAsync(int stockBalanceId, TransactionType type)
        {
            var stock = await _context.StockBalances
                .Include(sb => sb.Product)
                .Include(sb => sb.Location)
                    .ThenInclude(l => l.Warehouse)
                .FirstOrDefaultAsync(sb => sb.Id == stockBalanceId);

            if (stock == null) return null;

            return new StockOperationViewModel
            {
                StockBalanceId = stock.Id,
                ProductId = stock.ProductId,
                ProductName = stock.Product.Name,
                SKU = stock.Product.SKU,
                LocationId = stock.LocationId,
                LocationCode = stock.Location.LocationCode,
                WarehouseName = stock.Location.Warehouse.Name,
                CurrentOnHand = stock.QuantityOnHand,
                CurrentAllocated = stock.QuantityAllocated,
                OperationType = type,
                Quantity = 1
            };
        }

        public async Task<(bool Success, string Message)> ProcessStockOperationAsync(StockOperationViewModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var stock = await _context.StockBalances
                    .Include(sb => sb.Product)
                    .FirstOrDefaultAsync(sb => sb.Id == model.StockBalanceId);

                if (stock == null)
                {
                    return (false, "Stock balance record not found.");
                }

                int qtyChanged = 0;

                switch (model.OperationType)
                {
                    case TransactionType.Receipt: // Stock In
                        qtyChanged = model.Quantity;
                        stock.QuantityOnHand += qtyChanged;
                        break;

                    case TransactionType.Dispatch: // Stock Out
                        if (stock.AvailableQuantity < model.Quantity)
                        {
                            return (false, $"Dispatch rejected: Requested quantity ({model.Quantity}) exceeds available stock ({stock.AvailableQuantity}).");
                        }
                        qtyChanged = -model.Quantity;
                        stock.QuantityOnHand += qtyChanged;
                        break;

                    case TransactionType.Adjustment: // Cycle count or damage correction
                        // model.Quantity represents the new absolute count or delta
                        qtyChanged = model.Quantity - stock.QuantityOnHand;
                        if (model.Quantity < 0)
                        {
                            return (false, "Adjusted quantity cannot be negative.");
                        }
                        stock.QuantityOnHand = model.Quantity;
                        break;

                    default:
                        return (false, "Unsupported operation type.");
                }

                stock.LastStockTakeDate = DateTime.UtcNow;

                // Log immutable ledger transaction
                var ledgerEntry = new StockTransaction
                {
                    ProductId = stock.ProductId,
                    LocationId = stock.LocationId,
                    QuantityChanged = qtyChanged,
                    ResultingBalance = stock.QuantityOnHand,
                    TransactionType = model.OperationType,
                    ReferenceNumber = model.ReferenceNumber.Trim(),
                    Notes = model.Notes?.Trim(),
                    CreatedBy = string.IsNullOrWhiteSpace(model.OperatorName) ? "Staff Operator" : model.OperatorName.Trim(),
                    CreatedAtUtc = DateTime.UtcNow
                };

                _context.StockTransactions.Add(ledgerEntry);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, $"Stock updated successfully. New balance: {stock.QuantityOnHand} units.");
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                _logger.LogWarning("Concurrency conflict detected while updating StockBalance ID {Id}", model.StockBalanceId);
                return (false, "Concurrency conflict: This stock balance was modified by another operator. Please refresh and try again.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error processing stock operation for StockBalance ID {Id}", model.StockBalanceId);
                return (false, $"System error: {ex.Message}");
            }
        }

        public async Task<List<StockTransaction>> GetTransactionHistoryAsync(string? search = null, TransactionType? type = null, int limit = 100)
        {
            var query = _context.StockTransactions
                .Include(t => t.Product)
                .Include(t => t.Location)
                    .ThenInclude(l => l.Warehouse)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(t => 
                    t.ReferenceNumber.ToLower().Contains(term) ||
                    t.Product.SKU.ToLower().Contains(term) ||
                    t.Product.Name.ToLower().Contains(term) ||
                    t.CreatedBy.ToLower().Contains(term));
            }

            if (type.HasValue)
            {
                query = query.Where(t => t.TransactionType == type.Value);
            }

            return await query
                .OrderByDescending(t => t.CreatedAtUtc)
                .Take(limit)
                .ToListAsync();
        }
    }
}
