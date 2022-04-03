using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Симулятор_генетики_4
{
    public class Mutations_converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch((value as Trait).Mutation)
            {
                case Trait.Mutations.Silence:
                    return Brushes.LightGreen;
                    break;
                case Trait.Mutations.Missence:
                    return Brushes.OrangeRed;
                    break;
                default:
                    return DependencyProperty.UnsetValue;
                    break;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Activity_converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value as Trait).active)
                return Brushes.LightGray;
            LethalComponent lel = Population.current.genofond[(value as Trait).Index].lethal;
            if (lel != null && lel.ReallyKills(value as Trait))
                return Brushes.Black;
            else
                return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Isalive_converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            LethalComponent lel = Population.current.genofond[(value as Trait).Index].lethal as LethalComponent;
            if (lel != null && lel.ReallyKills(value as Trait))
                return Brushes.White;
            else
                return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
