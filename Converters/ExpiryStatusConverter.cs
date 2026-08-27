using System;
using System.Globalization;
using System.Windows.Data;

namespace PharmacyInventory.Converters
{
    public class ExpiryStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            if (value is DateTime dt)
            {
                var d = DateOnly.FromDateTime(dt.Date);
                return Evaluate(d);
            }
            if (value is DateOnly doVal)
            {
                return Evaluate(doVal);
            }
            return string.Empty;
        }

        private string Evaluate(DateOnly date)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            if (date < today) return "Expired";
            var days = (date.ToDateTime(TimeOnly.MinValue) - today.ToDateTime(TimeOnly.MinValue)).TotalDays;
            if (days <= 30) return "Near expiry";
            return "OK";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
