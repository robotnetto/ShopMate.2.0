using System.Globalization;

namespace ShopMate._2._0.Presentation.CostumConvert
{
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = !string.IsNullOrEmpty(value as string);
            if (parameter != null && parameter.ToString() == "!")
            {
                result = !result;
            }
            System.Diagnostics.Debug.WriteLine($"Convert called with value: {value ?? "null"}, result: {result}");
            return result;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
