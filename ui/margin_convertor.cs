using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Симулятор_генетики_4
{
    class margin_convertor : IMultiValueConverter
    {
        public object[] ConvertBack(object values, Type[] types, object parameter, CultureInfo culture)
        {
            return null;
        }
        public object Convert(object[] values, Type type, object param, CultureInfo ci)
        {
            Thickness result = new Thickness(10, 0, 0, 10);
            foreach(object v in values)
            {
                try { result.Top = result.Top + System.Convert.ToDouble(v); }
                catch { }
            }
            return result;
        }
    }
}
