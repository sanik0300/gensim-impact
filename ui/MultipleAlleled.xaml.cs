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
        Button[] kills;
        CheckBox[] alerts;
        TextBox[] aoaba;
        public MultipleAlleled()
        {
            InitializeComponent();
            kills = new Button[3] { kil1, kil2, kil3 };
            alerts = new CheckBox[3] { Alrt0, AlrtA, AlrtB };
            aoaba = new TextBox[4] { OO, AO, BO, AB };
        }
        
        bool contains_lethal = false;
        int kill_where = -1;
        int num_of_alerts = 0;
        public MultipleAlleled(Gene g)
        {
            InitializeComponent();
            nameinput.Text = g.Name;
            OO.Text = g.traits[0];
            AO.Text = g.traits[1];
            BO.Text = g.traits[2];
            kodom.IsChecked = g.traits[2] != g.traits[3];
            AB.Visibility = kodom.IsChecked.Value? Visibility.Visible : Visibility.Hidden;
            AB.Text = g.traits[3];
            kills = new Button[3] { kil1, kil2, kil3 };
            alerts = new CheckBox[3] { Alrt0, AlrtA, AlrtB };
            
            for (int i =0; i<kills.Length; i++)
            {
                if (g.lethal != null && g.lethal.border == i)
                    anylethal_Click(kills[i], null);
                else
                    kills[i].Foreground = Brushes.White;
            }
            if (g.AlertAllels == null)
                return;
            foreach (int a in g.AlertAllels)
                alerts[a].IsChecked = true;

        }
        public TextBox[] textboxes
        {
            get { return new TextBox[5] { nameinput, OO, AO, BO, AB }; }
        }

        public Gene CreateNew(int mutprob)
        {
            Quaternal que = new Quaternal(nameinput.Text, new string[] { OO.Text, AO.Text, BO.Text, (bool)kodom.IsChecked? AB.Text : BO.Text }, mutprob);
            if (contains_lethal)
            {
                que.lethal = new LethalComponent(kill_where, 100);
            }
            return que;
        }

        public void Clear()
        {
            nameinput.Text = null;
            foreach(TextBox box in aoaba)
                box.Text = null;
            foreach (CheckBox chb in alerts) {
                chb.IsChecked = false;
                chb.IsEnabled = true;
            }
            contains_lethal = false;
            num_of_alerts = 0;

            if (kill_where < 0)
                return;
                          
            kills[kill_where].Background = contains_lethal ? Brushes.Black : default;
            kills[kill_where].Foreground = contains_lethal ? Brushes.White : Brushes.Black;                         
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
                throw new Exception("Варианты признаков совпадают");
            }
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
            //номер летального аллеля можно использовать как индекс checkbox'a :DD
            for (int u = 0; u < alerts.Length; u++)
                alerts[u].IsEnabled = true;

            alerts[kill_where].IsEnabled = false;
        }

        private void Alrt_Checked(object sender, RoutedEventArgs e)
        {
            aoaba[Convert.ToInt32((sender as CheckBox).Tag)].Foreground = Brushes.Firebrick;
            num_of_alerts++;
            if (num_of_alerts >= 3 - Convert.ToInt32(contains_lethal))
            {
                int spare;
                do
                {
                    spare = new Random().Next(0, 3);
                }
                while (spare != kill_where);
                alerts[spare].IsChecked = false;
            }               
        }

        private void Alrt_Unchecked(object sender, RoutedEventArgs e)
        {
            aoaba[Convert.ToInt32((sender as CheckBox).Tag)].Foreground = Brushes.Black;
            num_of_alerts--;
        }

        private void Alrt_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CheckBox le_me_sender = sender as CheckBox;
            if (!le_me_sender.IsChecked.Value)
                return;
            if (le_me_sender.IsEnabled)
                num_of_alerts++;
            else
                num_of_alerts--;                          
        }
    }
}
