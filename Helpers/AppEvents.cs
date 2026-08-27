using System;

namespace PharmacyInventory.Helpers
{
    public static class AppEvents
    {
        // Raised when sales or inventory-affecting operations occur.
        public static event EventHandler? SalesChanged;

        public static void NotifySalesChanged()
        {
            try
            {
                SalesChanged?.Invoke(null, EventArgs.Empty);
            }
            catch
            {
                // swallow exceptions from handlers to avoid crashing app
            }
        }
    }
}
