using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avto;

public class PriceWithDiscountConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int price && price > 0)
        {
            double discount = ((Services)parameter)?.discount ?? 0;
            double discountedPrice = price * (1 - discount / 100.0);
            return $"Цена со скидкой: {discountedPrice:F2}";
        }
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
