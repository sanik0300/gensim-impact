using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Симулятор_генетики_4
{
    /// <summary>
    /// Логика взаимодействия для SvodkaOfGens.xaml
    /// </summary>
    public partial class SvodkaOfGens : Window
    {
        Cell c;
        public SvodkaOfGens()
        {
            InitializeComponent();
        }
        public SvodkaOfGens(Cell c)
        {
            this.c = c;
            InitializeComponent();
            this.Title = $"Генотип и фенотип {c.index + 1}-й особи";
            c.BeingDisplayed = true;
            (datagrid_ok.Columns[0] as DataGridTextColumn).Binding = new Binding("Name");
            (datagrid_ok.Columns[1] as DataGridTextColumn).Binding = new Binding("ToLetters");
            (datagrid_ok.Columns[2] as DataGridTextColumn).Binding = new Binding("Result");
            foreach (Trait trait in c.genotype)
            {
                datagrid_ok.Items.Add(trait);              
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            c.BeingDisplayed = false;
        }
    }
}
