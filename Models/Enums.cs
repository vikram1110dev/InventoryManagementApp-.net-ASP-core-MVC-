namespace InventoryManagementApp.Models
{
    public enum TransactionType
    {
        Receipt = 1,       // Stock In
        Dispatch = 2,      // Stock Out
        TransferIn = 3,    // Transferred into location
        TransferOut = 4,   // Transferred out of location
        Adjustment = 5     // Cycle count or damage correction
    }

    public enum StockStatusFilter
    {
        All = 0,
        InStock = 1,
        LowStock = 2,
        OutOfStock = 3
    }
}
