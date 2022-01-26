using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Симулятор_генетики_4
{
    /// <summary>
    /// Логика взаимодействия для SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        public SplashScreen(Window www, double w, double h)
        {
            InitializeComponent();

            this.ShowInTaskbar = false;
            this.Owner = www;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.Width = w*0.9;
            this.Height = h*0.8;
        }

        private void legif_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaElement mem = sender as MediaElement;
            mem.Position = TimeSpan.FromMilliseconds(1);
            mem.Play();
        }
    }
}
