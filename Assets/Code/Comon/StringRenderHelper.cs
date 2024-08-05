
using System;
using System.Globalization;


internal class StringRenderHelper
{
    private const string DEFAULT_SEPARATOR = ".";
    private const string DEFAULT_GROUP_SEPARATOR = " ";
    
    private const string K_FORMAT0 = "N0";
    private const string K_FORMAT1 = "N1";
    private const string K_FORMAT2 = "N2";

    public readonly static NumberFormatInfo NumericFormat = new CultureInfo("en-US", false).NumberFormat;

    public static string GetGroupSeparetedCurrencyPostfixString(double number, int digitsAfterPoint = 2, string separator = DEFAULT_SEPARATOR)
    {
        return GetCurrencyString(ref number, digitsAfterPoint, separator, DEFAULT_GROUP_SEPARATOR);
    }

    public static string GetCurrencyPostfixString(double number, int digitsAfterPoint = 2, string separator = DEFAULT_SEPARATOR)
    {
        return GetCurrencyString(ref number, digitsAfterPoint, separator, string.Empty);
    }

    private static string GetCurrencyString(ref double number, int digitsAfterPoint = 2, string separator = DEFAULT_SEPARATOR, string groupSeparator = "")
    {
        string currency = Constants.DEFAULT_CURRENCY_SIGN;

        number = Math.Round(number, digitsAfterPoint);

        NumericFormat.NumberDecimalSeparator = string.IsNullOrEmpty(separator) ? DEFAULT_SEPARATOR : separator;
        NumericFormat.NumberGroupSeparator = string.IsNullOrEmpty(groupSeparator) ? DEFAULT_GROUP_SEPARATOR : groupSeparator;

        return number.ToString(number % 1 == 0 ? K_FORMAT0 : digitsAfterPoint switch
        {
            0 => K_FORMAT0,
            1 => K_FORMAT1,           
            _ => K_FORMAT2,
        }, NumericFormat) + currency;
    }

    public static string GetAddressAsStreetOrSelf(string address_street)
    {
        var parts = address_street.Split(',');
        if (parts.Length > 1)
        {
           return $"{parts[parts.Length - 2]},{parts[parts.Length - 1]}";
        }
        else
        {
            return address_street;
        }
    }
    public static string GetAddressStreet(string address_street)
    {
        var parts = address_street.Split(',');
        if (parts.Length > 1)
        {
            return $"{parts[parts.Length - 2]}";
        }
        else
        {
            return string.Empty;
        }
    }
    public static string GetAddressHouseNumber(string address_street)
    {
        var parts = address_street.Split(',');
        if (parts.Length > 1)
        {
            return $"{parts[parts.Length - 1]}";
        }
        else
        {
            return string.Empty;
        }
    }
}