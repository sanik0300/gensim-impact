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
    /// Логика взаимодействия для MultipleAlleled.xaml
    /// </summary>
    public partial class MultipleAlleled : UserControl, IReactingUi
    {
        public MultipleAlleled()
        {
            InitializeComponent();
            kills = new Button[3] { kil1, kil2, kil3 };
        }
        Button[] kills;
        bool contains_lethal = false;
        int kill_where;
        public MultipleAlleled(Gene g)
        {
            InitializeComponent();
            nameinput.Text = g.Name;
            OO.Text = g.traits[0];
            AO.Text = g.traits[1];
            BO.Text = g.traits[2];
            kodom.IsChecked = g.traits[2] != g.traits[3];
            AB.Visibility = (bool)kodom.IsChecked ? Visibility.Visible : Visibility.Hidden;
            AB.Text = g.traits[3];
            kills = new Button[3] { kil1, kil2, kil3 };
            for(int i =0; i<kills.Length; i++)
            {
                if (g.lethal != null && g.lethal.border == i)
                    anylethal_Click(kills[i], null);
                else
                    kills[i].Foreground = Brushes.White;
            }
        }
        public TextBox[] textboxes
        {
            get {
                return new TextBox[5] { nameinput, OO, AO, BO, AB };
            }
        }

        public Gene CreateNew()
        {
            Quaternal que = new Quaternal(nameinput.Text, new string[] { OO.Text, AO.Text, BO.Text, (bool)kodom.IsChecked? AB.Text : BO.Text });
            if (contains_lethal)
            {
                que.lethal = new LethalComponent(kill_where, 100);
            }
            return que;
        }

        public void Clear()
        {
            nameinput.Text = null;
            OO.Text = null;
            AO.Text = null;
            BO.Text = null;
            AB.Text = null;
        }

        public void CheckFilling()
        {
            if (nameinput.Text == string.Empty ||
                        OO.Text == string.Empty ||
                        AO.Text == string.Empty ||
                        BO.Text == string.Empty || (AB.Text == string.Empty && (bool)kodom.IsChecked))
            {
                throw new Exception("Не всё заполнено");
            }
            string oo = OO.Text.ToLower(), ao = AO.Text.ToLower(), bo = BO.Text.ToLower(), ab = AB.Text.ToLower();
            if (oo == ao || oo == bo || oo == ab || ao==bo || ao==ab || bo==ab)
            {
                oo = null; ao = null; bo = null; ab = null;
                throw new Exception("Варианты признаков совпадают");
            }
            oo = null; ao = null; bo = null; ab = null;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            optional_pair.Visibility = (bool)kodom.IsChecked ? Visibility.Visible : Visibility.Hidden;
        }

        private void anylethal_Click(object sender, RoutedEventArgs e)
        {
            if (e != null)
            {
                foreach (Button b in kills)
                {
                    b.Background = default;
                    b.Foreground = Brushes.Black;
                }
                contains_lethal = !contains_lethal;
            }
            
            (sender as Button).Background = contains_lethal ? Brushes.Black : default;
            (sender as Button).Foreground = contains_lethal ? Brushes.White : Brushes.Black;
            kill_where = Convert.ToInt32((sender as Button).Tag);
        }
    }
}
