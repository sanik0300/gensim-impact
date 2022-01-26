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
    /// Логика взаимодействия для GametInput.xaml
    /// </summary>
    public partial class GametInput : UserControl
    {
        /// <summary>
        /// Какой массив из _als заполняет данный... набор кнопочек
        /// </summary>
        Button[] letter1, letter2;
        static Color highlight = Color.FromRgb(200, 200, 220), def = Color.FromRgb(251, 251, 251);
        static SolidColorBrush selection = new SolidColorBrush(highlight), @default=new SolidColorBrush(def);
        static Trait link;
        static private GametInput exemple;
        /// <summary>
        /// Умножать ли аллель на 2 - у Кватерналов такого никогда не будет!
        /// </summary>
        byte increase;
        int backup;
        int danger;
        private void CommonClick(Button but, Button[] pair)
        {
            foreach (Button b in pair)
                b.Background = @default;
            but.Background = selection;           
            link.CalculateResultingFentype(but, null);
            Activer aaa = Population.current.genofond[link.Index] as Activer;
            if (aaa == null)
                return;
            aaa.deActivate(ref link, Population.current.na_podxode.genotype);
        }
        private void a1_Click(object sender, RoutedEventArgs e)
        {
            link.allels[0] = 0;
            CommonClick(sender as Button, letter1);            
        }
        private void A1_Click_1(object sender, RoutedEventArgs e)
        {
            link.allels[0] = increase;
            CommonClick(sender as Button, letter1);           
        }
        private void B1_Click(object sender, RoutedEventArgs e)
        {
            link.allels[0] = 2;
            CommonClick(sender as Button, letter1);
        }

        private void a2_Click(object sender, RoutedEventArgs e)
        {
            link.allels[1] = 0;
            CommonClick(sender as Button, letter2);         
            backup = 0;
        }
        private void A2_Click_1(object sender, RoutedEventArgs e)
        {
            link.allels[1] = increase;
            CommonClick(sender as Button, letter2);        
            backup = 1;
        }
        private void B2_Click(object sender, RoutedEventArgs e)
        {
            link.allels[1] = 2;
            CommonClick(sender as Button, letter2);   
            backup = 2;
        }

        public GametInput(Trait t, bool kodom, bool quad)
        {
            InitializeComponent();
            exemple = this;
            letter1 = new Button[3] { a1, A1, B1 };
            letter2 = new Button[3] { a2, A2, B2 };
            link = t;
            increase =(kodom || quad)? Convert.ToByte(1) : Convert.ToByte(2);
            if (Population.current.genofond[link.Index].SxRelated)
            {
                foreach (Button b in letter2)
                    b.IsEnabled = Population.current.na_podxode.homogametous;
            }
            if (Population.current.genofond[link.Index].lethal != null && !(Population.current.genofond[link.Index] is Raspred))
            {
                danger = (int)Population.current.genofond[link.Index].lethal.border;
                
                if(danger > 0)
                {
                    A1.IsEnabled = false;
                    A2.IsEnabled = false;
                }
                else
                {
                    a1.IsEnabled = false;
                }
            }
            a1.Content = a2.Content = quad? '0' : 'a';
            letter1[Convert.ToInt32(quad? (int)t.allels[0] : Convert.ToInt32(t.allels[0]>0))].Background = selection;
            letter2[Convert.ToInt32(quad? (int)t.allels[1] : Convert.ToInt32(t.allels[1] > 0))].Background = selection;
            B1.Visibility = B2.Visibility = quad ? Visibility.Visible : Visibility.Collapsed;
                     
            stable_chrom.Visibility = Population.current.genofond[t.Index].SxRelated ? Visibility.Visible : Visibility.Hidden;
            optional_chr.Visibility = stable_chrom.Visibility;
        }
        static public void AccordingToChroms()
        {
            bool homo = Population.current.na_podxode.homogametous;
            if (Population.current.genofond[link.Index].SxRelated)
                foreach (Button b in exemple.letter2)
                    b.IsEnabled = homo;

            foreach (Trait t in Population.current.na_podxode.genotype)
            {
                if (!Population.current.genofond[t.Index].SxRelated)
                    continue;

                if (homo)
                    t.allels[1] = exemple.backup == 1 ? exemple.increase : (byte?)exemple.backup;
                else
                    t.allels[1] = null;

                t.CalculateResultingFentype(exemple.letter2[Convert.ToInt32(exemple.backup>0)], null);
            }
        }
    }
}
