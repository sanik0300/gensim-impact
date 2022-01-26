using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Симулятор_генетики_4.ui
{
    /// <summary>
    /// Логика взаимодействия для RaspredInput.xaml
    /// </summary>
    public partial class RaspredInput : UserControl, IReactingUi
    {
        double screen_delta;
        bool revers = false;
        private void basic_fix()
        {
            reverse.Width *= 2;
            reverse.Content = "<->";
            dispersion.Maximum = (double)Raspred.num_of_delts / 6;
            this.Loaded += UserControl_Loaded;
            this.screen.SizeChanged += screen_SizeChanged;
        }
        public RaspredInput()
        {
            InitializeComponent();
            priorities.Value = 25;
            basic_fix();
        }
        public RaspredInput(Gene g)
        {
            InitializeComponent();
            basic_fix();

            Raspred er = g as Raspred;
            if (er == null)
                return;
            this.probabilities = er.probabilities;
            this.revers = er.reversed;
            nameinput.Text = er.Name;
            min_num.Text = er.min.ToString();
            max_num.Text = er.max.ToString();
            priorities.Value = er.border;
            dispersion.Value = er.sigma;
            units.Text = er.measure;
            precision.Value = er.prec;
            polyline.Points.Clear();        
            isdeadly.IsChecked = g.lethal!= null;
            if (g.lethal != null)
            {               
                lethalmax_ValueChanged(null, new RoutedPropertyChangedEventArgs<double>(lethalmax.Value, er.lethal.border / er.delta));
                ded_bdr_txt.Text = $"Жить можно при значениях от {er.min + er.border} до {er.max - er.border} {er.measure}";
            }           

            for (int i = 0; i < Raspred.num_of_delts; i++)
            {
                if (i >= Raspred.num_of_delts / 2)
                    polyline.Points.Add(new Point(i * screen_delta, screen.Height - er.probabilities[Raspred.num_of_delts-1-i] * screen_delta * 10));
                else
                    polyline.Points.Add(new Point(i * screen_delta, screen.Height - er.probabilities[i] * screen_delta * 10));
            }                                  
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            lethalmax.Margin = new Thickness(0, 0, lethalmax.ActualWidth/2, 0);
            screen_delta = screen.ActualWidth / Raspred.num_of_delts;
            screen.Children.Add(polyline);
            if (revers)
                reverse_Click(null, null);
        }

        public TextBox[] textboxes
        {
            get
            {
                return new TextBox[] { nameinput, min_num, max_num, units };
            }
        }

        public void CheckFilling()
        {
            foreach(TextBox tb in this.textboxes)
            {
                if (string.IsNullOrEmpty(tb.Text))
                    throw new Exception("Заполните все открытые поля");
            }

            float local_min = Convert.ToSingle(min_num.Text),
                  local_max = Convert.ToSingle(max_num.Text);
            if (local_max == local_min)
                throw new Exception("\"Наибольшее\" значение равно \"наименьшему\", так не пойдёт");
            if (local_min>local_max)
                throw new Exception("\"Наибольшее\" значение меньше \"наименьшего\", так не пойдёт");
            if (local_max > 100 && units.Text=="%")
            {
                string good_end = "ов";
                if (local_max == 1)
                    good_end = string.Empty;                   
                else if (local_max % 10 < 5 && local_max % 10 >1)
                    good_end = "а";
                    
                throw new Exception($"Ну не бывает {local_max} процент{good_end} :\"]");
            }
            try { int wtf = Convert.ToInt32(units.Text);}
            catch { return; }
            throw new Exception("Не может единица измерения быть числом");
        }

        public void Clear()
        {
            nameinput.Text = min_num.Text = max_num.Text = units.Text = null;
        }

        public Gene CreateNew()
        {         
            Raspred rer = new Raspred(nameinput.Text, this.probabilities, Convert.ToSingle(min_num.Text), Convert.ToSingle(max_num.Text), 
                                (float)dispersion.Value, (int)priorities.Value, this.revers, units.Text, (int)precision.Value);
            if ((bool)isdeadly.IsChecked)
            {
                rer.lethal = new LethalComponent((rer.max - rer.min)*(float)(lethalmax.Value / Raspred.num_of_delts), 100);
            }
            return rer;
        }

        private void priorities_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(e.NewValue < 5 || e.NewValue > 45)
            {
                priorities.Value = e.OldValue;
                return;
            }
            col1.Width = new GridLength(this.ActualWidth==0? 150 : priorities.Value / priorities.Maximum * this.ActualWidth);
        }

        private void reverse_Click(object sender, RoutedEventArgs e)
        {
            if(e!=null)
                revers = !revers;
            string swap = AAa.Text;
            Brush swpColor = (AAa.Parent as Border).BorderBrush;
            AAa.Text = a_smol.Text;
            (AAa.Parent as Border).BorderBrush = (a_smol.Parent as Border).BorderBrush;
            a_smol.Text = swap;
            (a_smol.Parent as Border).BorderBrush = swpColor;
        }
        double[] probabilities = new double[Raspred.num_of_delts+1];
        Polyline polyline = new Polyline() { Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)), StrokeThickness=3 };
        Line[] @where_die;
        private void DrawCurva()
        {
            if (screen == null) return;
            screen_delta = screen.ActualWidth / Raspred.num_of_delts;
            polyline.Points.Clear();
            for (int i = 0; i<=Raspred.num_of_delts/2; i++)
            {
                double res = NormRasp(i-Raspred.num_of_delts/2, dispersion.Value);
                probabilities[i] = res;         
                Point y = new Point(i * screen_delta, screen.ActualHeight - res * screen_delta * 200);
                polyline.Points.Add(y);
            }
            for(int i = 1; i <=Raspred.num_of_delts / 2; i++)
            {
                polyline.Points.Add(new Point(screen_delta*(Raspred.num_of_delts/2+i), polyline.Points[Raspred.num_of_delts/2 - i].Y));
            }
        }
        private double NormRasp(int x, double sigma) 
        {
            double @base = 1 / (sigma * Math.Sqrt(2 * Math.PI));
            double pow = -x*x / (2 * sigma * sigma);
            return @base * Math.Pow(Math.E, pow);
        }
        private void dispersion_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {DrawCurva();}

        private void screen_SizeChanged(object sender, SizeChangedEventArgs e){
            priorities_ValueChanged(sender, new RoutedPropertyChangedEventArgs<double>(priorities.Value, priorities.Value));
            DrawCurva();
        }
        private void priorities_MouseDoubleClick(object sender, MouseButtonEventArgs e) {priorities.Value = 25;}

        private void min_num_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txt = (sender as TextBox).Text;
            if (string.IsNullOrEmpty(txt))
                return;
            int last = txt.Length - 1;
            if (!char.IsDigit(txt[last]) && txt[last]!='.' && txt[last] != ',')
            {
                (sender as TextBox).Text = new StringBuilder(txt).Remove(last, 1).ToString();
                (sender as TextBox).CaretIndex = last;
            }
        }

        private void precision_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (precision == null)
                return;
            precision.Value = Math.Round(precision.Value);
            digits.Text = $"Знаков после запятой: {precision.Value}";
        }

        private void lethalmax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue > 20)
            {
                lethalmax.Value = e.OldValue;
                return;
            }
            ded_bdr_txt.Text = $"в пределах значения: {Math.Round(lethalmax.Value/Raspred.num_of_delts*100)} %";
            if (where_die == null)
                return;
            where_die[0].X1 = where_die[0].X2 = screen_delta * lethalmax.Value;
            where_die[1].X1 = where_die[1].X2 = screen_delta * (Raspred.num_of_delts - lethalmax.Value);
        }

        private void isdeadly_Checked(object sender, RoutedEventArgs e) {         
            if ((bool)isdeadly.IsChecked)
            {
                where_die = new Line[2] { new Line() { X1 = screen_delta*lethalmax.Value, X2 = screen_delta * lethalmax.Value, Y1 = 0, Y2 = screen.ActualHeight, Stroke=polyline.Stroke},
                                           new Line() {X1 = screen_delta*(Raspred.num_of_delts -lethalmax.Value), X2 = screen_delta * (Raspred.num_of_delts -lethalmax.Value), Y1 = 0, Y2 = screen.ActualHeight, Stroke=polyline.Stroke} };
                foreach (Line line in where_die)
                    screen.Children.Add(line);
            }
            else
            {
                foreach (Line line in where_die)
                    screen.Children.Remove(line);
                where_die = null;
            }    
            
            leth_bdrs.IsEnabled = (bool)(sender as CheckBox).IsChecked;       
        }
    }
}
