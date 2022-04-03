using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Симулятор_генетики_4.ui
{
    /// <summary>
    /// Логика взаимодействия для ProbabilityInput.xaml
    /// </summary>
    public partial class ProbabilityInput : UserControl
    {
        public ProbabilityInput()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Создать бегунок для выбора в интервале...
        /// </summary>
        /// <param name="header">что выбираем</param>
        /// <param name="min">минимальное значение</param>
        /// <param name="vis">Видимость букав по углам!</param>
        public ProbabilityInput(string header, double min, bool vis=true)
        {
            InitializeComponent();
            stdTxt = header;
            probability.Text = stdTxt + ":";
            le_Choose.Minimum = min;
            A.Visibility = vis ? Visibility.Visible : Visibility.Hidden;
            a.Visibility = vis ? Visibility.Visible : Visibility.Hidden;
        }
        private string stdTxt;
        public double Value { get { return le_Choose.Value; } set { le_Choose.Value = value; } }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            probability.Text = $"{stdTxt}: {Math.Round(le_Choose.Value)} %";
        }
    }
}
