using System;

namespace RetailOptimizationPlatform.Core.Exceptions
{
    /// <summary>
    /// Custom exception thrown when an requested inventory item cannot be found in the database.
    /// </summary>
    public class InventoryNotFoundException : Exception
    {
        public InventoryNotFoundException()
            : base("The requested inventory item was not found.")
        {
        }

        public InventoryNotFoundException(string message)
            : base(message)
        {
        }

        public InventoryNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InventoryNotFoundException(int itemId)
            : base($"Inventory item with ID {itemId} was not found in the system.")
        {
        }
    }
}