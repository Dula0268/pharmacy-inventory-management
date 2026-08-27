using System;

namespace PharmacyInventory.Helpers
{
    internal static class Guard
    {
        public static void NotNull(object? value, string name)
        {
            if (value is null)
                throw new ArgumentNullException(name);
        }
    }
}
