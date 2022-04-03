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
    /// Логика взаимодействия для ActivatorInput.xaml
    /// </summary>
    public partial class ActivatorInput : UserControl, IReactingUi
    {
        public ActivatorInput()
        {
            InitializeComponent();
            rev.Content = "<->";
            for(int i = 0; i<Population.current.genofond.Count; i++)
            {
                if (!Population.current.taken_aims[i] && !(Population.current.genofond[i] is Activer))
                {
                    Aims.Items.Add(Population.current.genofond[i]);
                }
            }
        }
        public ActivatorInput(Gene a)
        {
            InitializeComponent();
            rev.Content = "<->";
            foreach (int i in (a as Activer).Aims)
                Aims.Items.Add(Population.current.genofond[i].Name);
            gotta_reverse = a.traits[0] == Activer.commandToENA;
            rev_Click(null, null);
        }

        public void CheckFilling()
        {
            if (Aims.SelectedItems.Count == 0)
                throw new Exception("Не выбрано, на что влиять");
        }

        public TextBox[] textboxes
        {
            get { return new TextBox[1] { opt_name };}
        }

        bool gotta_reverse =false;
        public void Clear()
        {
            int[] realaims = (Population.current.genofond[Population.current.genofond.Count - 1] as Activer).Aims;
            for(int i = 0; i<realaims.Length; i++)
            {
                Aims.Items.RemoveAt(realaims[i]-i);
            }         
            opt_name.Text = null;
        }

        public Gene CreateNew(int mutprob)
        {
            int[] futAims = new int[Aims.SelectedItems.Count];
            for(int i =0; i<Aims.SelectedItems.Count; i++)
            {
                futAims[i] = (Aims.SelectedItems[i] as Gene).index;
            }
            return new Activer(opt_name.Text, gotta_reverse? 
                new string[] { "будет активным", null, Activer.commandToENA, } :  new string[] { Activer.commandToENA, null, "будет активным"},
                mutprob, futAims);
        }

        private void rev_Click(object sender, RoutedEventArgs e)
        {
            gotta_reverse = !gotta_reverse;
            domvar.Text = gotta_reverse ? "Не будет" : "Будет активным";
            recvar.Text = gotta_reverse ? "Будет активным" : "Не будет";
        }
    }
}
