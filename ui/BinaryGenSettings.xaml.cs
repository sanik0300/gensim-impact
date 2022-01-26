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
using Симулятор_генетики_4.ui;

namespace Симулятор_генетики_4
{
    /// <summary>
    /// Interaction logic for BinaryGenSettings.xaml
    /// </summary>
    public partial class BinaryGenSettings : UserControl, IReactingUi
    {
        public BinaryGenSettings() { 
            InitializeComponent();
            deaths = new Button[2] { RecKill, DomKill };
        }
        bool contains_lethal = false;
        Button[] deaths;
        public BinaryGenSettings(Gene g)
        {
            InitializeComponent();     
            nameinput.Text = g.Name;
            isChromeable.IsChecked = g.SxRelated;
            dom.Text = g.traits[2];
            codom.Text = g.traits[1];
            rec.Text = g.traits[0];
            iscodoming.IsChecked = (g as Binary).kodom;
            deaths = new Button[2] { RecKill, DomKill };
            
            for(int i =0; i<deaths.Length; i++)
            {
                if (g.lethal != null && g.lethal.border==i)
                    DomKill_Click(deaths[i], null);
                else
                    deaths[i].Foreground = Brushes.White;
            }
        }

        public TextBox[] textboxes { 
            get
            {
                TextBox[] result = iscodoming.IsChecked.Value ? new TextBox[4] { nameinput, dom, codom, rec } : new TextBox[3] { nameinput, dom, rec };
                return result;
            } 
        }

        private void iscodoming_Checked(object sender, RoutedEventArgs e) { codom.IsEnabled = iscodoming.IsChecked.Value; }

        public Gene CreateNew()
        {

            Binary bin = new Binary(nameinput.Text, 
                new string[3] { rec.Text, iscodoming.IsChecked.Value ? codom.Text : null, dom.Text }, 
                isChromeable.IsChecked.Value, 
                iscodoming.IsChecked.Value);
            if (contains_lethal)
            {
                bin.lethal = new LethalComponent(Convert.ToInt32(DomKill.Background==Brushes.Black)+Convert.ToInt32(!(bool)iscodoming.IsChecked && DomKill.Background == Brushes.Black), 100);
            }
            return bin;
        }

        public void Clear()
        {
            foreach(TextBox tb in this.textboxes) { tb.Text = null; }
            iscodoming.IsChecked = false;
            codom.IsEnabled = false;
        }

        public void CheckFilling()
        {
            string dt = dom.Text.ToLower(), rt = rec.Text.ToLower(), ct = codom.Text.ToLower();
            if(nameinput.Text == string.Empty ||
                        dom.Text == string.Empty ||
                        (codom.Text == string.Empty && iscodoming.IsChecked.Value) ||
                        rec.Text == string.Empty)
            {
                throw new Exception("Не всё заполнено");
            }         
            else if (dt == rt || (dt == ct && iscodoming.IsChecked.Value) || (rt == ct && iscodoming.IsChecked.Value))
            {
                dt = null; rt = null; ct = null;
                throw new Exception("Варианты признаков совпадают");
            }
            dt = null; rt = null; ct = null;
        }

        private void yes_Click(object sender, RoutedEventArgs e)
        {
            dom.Text = "Да";
            rec.Text = "Нет";
            codom.Text = null;
            iscodoming.IsChecked = false;
        }
        private void reverse_yes_Click(object sender, RoutedEventArgs e)
        {
            dom.Text = "Нет";
            rec.Text = "Да";
            codom.Text = null;
            iscodoming.IsChecked = false;
        }

        private void DomKill_Click(object sender, RoutedEventArgs e)
        {
            if (e != null)
            {
                foreach (Button b in deaths)
                {
                    b.Background = default;
                    b.Foreground = Brushes.Black;
                }
                contains_lethal = !contains_lethal;  
            }                    
            
            (sender as Button).Background = contains_lethal ? Brushes.Black : default;
            (sender as Button).Foreground = contains_lethal ? Brushes.White : Brushes.Black;
        }
    }
}
