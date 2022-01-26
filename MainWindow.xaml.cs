using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.ComponentModel;

namespace Симулятор_генетики_4
{
    public partial class MainWindow : Window
    {
        StackPanel[] boxes;
        UserControl current_ctr;
        ui.ProbabilityInput mutator;
        StackPanel main_box;
        Button[] clickables = new Button[0];
        List<Gene> Genfond = Population.current.genofond;
        Cell only_mutable = Population.current.na_podxode;
        List<Trait> Gentype = Population.current.na_podxode.genotype as List<Trait>;
        public MainWindow()
        {
            InitializeComponent();
            boxes = this.types.Children.OfType<StackPanel>().ToArray();
            Title = $"симулятор генетики {Assembly.GetExecutingAssembly().GetName().Version}" ;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            current_ctr = null;
            if (main_box != null)
                main_box.Children.Clear();

            main_box = boxes[Convert.ToInt32((sender as RadioButton).Tag)];
            switch (main_box.Tag)
            {
                case "1":
                    current_ctr = (e is GenPassingArgs) ? new MultipleAlleled((e as GenPassingArgs).link) : new MultipleAlleled();
                    break;
                case "2":
                    current_ctr = (e is GenPassingArgs) ? new ActivatorInput((e as GenPassingArgs).link) : new ActivatorInput();
                    break;
                case "3":
                    current_ctr = (e is GenPassingArgs) ? new ui.RaspredInput((e as GenPassingArgs).link) : new ui.RaspredInput();
                    break;
                default:
                    current_ctr = (e is GenPassingArgs) ? new BinaryGenSettings((e as GenPassingArgs).link) : new BinaryGenSettings();
                    break;
            }
            main_box.Children.Add(current_ctr);
            if (!(current_ctr is MultipleAlleled)) { main_box.Children.Add(new ui.ProbabilityInput("Приоритет", 0) { Value = 50 }); }
            mutator = new ui.ProbabilityInput("Вероятность мутации", 0, false);
            main_box.Children.Add(mutator);
            if (current_ctr is ActivatorInput)
                return;
            main_box.Children.Add(new TextBlock() { Text = "Проявление неактивности (необязательно):", TextWrapping = TextWrapping.Wrap });
            main_box.Children.Add(new TextBox());
        }

        private void AddTrait_Click(object sender, RoutedEventArgs e)//AddGene это блимн
        {
            foreach (TextBox box in (current_ctr as ui.IReactingUi).textboxes)
                box.Text = box.Text.Trim(' ');
            
            try { (current_ctr as ui.IReactingUi).CheckFilling();}
            catch (Exception ex) { 
                MessageBox.Show(ex.Message, null, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }            
            
            Gene g = (current_ctr as ui.IReactingUi).CreateNew();
            foreach(Gene gen in Genfond)
                if(gen.Name == g.Name)
                {
                    g = null;
                    MessageBox.Show("Такое имя уже есть", null, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }        
                
            g.MutProb = Convert.ToInt32(mutator.Value);
            if(g is Binary)
                (g as Binary).Priority = Convert.ToInt32((main_box.Children[1] as ui.ProbabilityInput).Value);
            if(!(g is Activer))
                g.OptName = (main_box.Children[main_box.Children.Count-1] as TextBox).Text.Trim(' ');            

            Genfond.Add(g);
            allgens.Items.Add(g.ToString());
            (current_ctr as ui.IReactingUi).Clear();

            ebash_all.IsEnabled = ebash_to_cell.IsEnabled = save.IsEnabled = naxer_gen.IsEnabled = true;
            ZigControl.Visibility = Population.current.AllChroms > 0 ? Visibility.Visible : Visibility.Hidden;
            Population.current.SmthUnSaved = true;
        }

        private void ebash_to_cell_Click(object sender, RoutedEventArgs e)
        {
            if (allgens.SelectedIndex == -1) return;

            Gene g = Genfond[allgens.SelectedIndex];
            //Future Place уже получено бинарным поиском при попытке выбрать ген
            if (g is Raspred)
                Gentype.Insert(Population.FuturePlace, new NumericTrait(g.index, g.Name, new byte?[] { 0, 0 }));
            else if ((g.lethal!=null && g.lethal.border == 0) || g as Activer != null && (g as Activer).traits[0]==Activer.commandToENA)
                Gentype.Insert(Population.FuturePlace, new Trait(g.index, g.Name, new byte?[] { 2, 2 }));
            else
                Gentype.Insert(Population.FuturePlace, new Trait(g.index, g.Name, new byte?[] { 0, 0 }));
            resultgens.Items.Insert(Population.FuturePlace, allgens.SelectedItem);
            Population.current.ChromsNaPodxode += Convert.ToUInt32(g.SxRelated);

            addlife.IsEnabled = clear.IsEnabled = navixod.IsEnabled = predv.IsEnabled = true;
            ebash_to_cell.IsEnabled = false;
            ebash_all.IsEnabled = Genfond.Count != Gentype.Count;
            choosesx.Visibility = Population.current.ChromsNaPodxode > 0 ? Visibility.Visible : Visibility.Hidden;
            Population.current.SmthUnSaved = true;
        }

        private void ebash_all_Click(object sender, RoutedEventArgs e)
        {
            for(int i =0; i<Genfond.Count; i++)
            {
                allgens.SelectedIndex = i;
                ebash_to_cell_Click(ebash_to_cell, null);
            }
            ebash_all.IsEnabled = false;
        }

        private void allgens_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(main_box!=null)
                main_box.Children.Clear();
            ebash_to_cell.IsEnabled = true;
            
            if (allgens.SelectedIndex == -1)
                return;

            if (Population.binarySearch(Genfond[allgens.SelectedIndex], Gentype) != -1)
                ebash_to_cell.IsEnabled = false;      
            
            RadioButton senmder;
            Gene sel = Genfond[allgens.SelectedIndex];
            if (sel is Activer)
                senmder = rb2;
            else if (sel is Quaternal)
                senmder = rb1;
            else if (sel is Raspred)
                senmder = rb3;
            else
                senmder = rb0;
            RadioButton_Checked(senmder, new GenPassingArgs(sel));
            mutator.Value = sel.MutProb;
            if (sel is Binary)
                (main_box.Children[1] as ui.ProbabilityInput).Value = (sel as Binary).Priority;
            foreach (UIElement element in main_box.Children)
                element.IsEnabled = false;
        }

        private void resultgens_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (resultgens.SelectedIndex == -1 || resultgens.Tag!=null) return;

            allgens.SelectedIndex = Gentype[resultgens.SelectedIndex].Index;      
            allgens_SelectionChanged(null, null);

            Binary b = Genfond[allgens.SelectedIndex] as Binary;
            main_box.Children.Add(new ui.GametInput(Gentype[resultgens.SelectedIndex], b!=null&&b.kodom, b==null));
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i<Gentype.Count; i++)
            {
                resultgens.SelectedIndex = 0;
                navixod_Click(sender, e);
            }
        }

        private void navixod_Click(object sender, RoutedEventArgs e)
        {
            if (resultgens.SelectedIndex < 0) return;
            main_box.Children.Clear();

            if (Genfond[Gentype[resultgens.SelectedIndex].Index].SxRelated)
            {
                if (--Population.current.ChromsNaPodxode < 1)
                    only_mutable.sx = null;
            }

            Gentype.RemoveAt(resultgens.SelectedIndex);
            resultgens.Items.RemoveAt(resultgens.SelectedIndex);
            ebash_to_cell.IsEnabled = ebash_all.IsEnabled = true;
            addlife.IsEnabled = navixod.IsEnabled = clear.IsEnabled = predv.IsEnabled = Gentype.Count > 0;          
            choosesx.Visibility = Population.current.ChromsNaPodxode > 0 ? Visibility.Visible : Visibility.Hidden;
            Population.current.SmthUnSaved = true;
        }

        private void naxer_gen_Click(object sender, RoutedEventArgs e)
        {           
            int possible_indx = Population.binarySearch(Genfond[allgens.SelectedIndex], Gentype);
            
            if (possible_indx >= 0)
            {
                resultgens.Tag = "deletion";
                resultgens.SelectedIndex = possible_indx;
                
                for(int i = possible_indx+1; i<Gentype.Count; i++)
                    Gentype[i].Index = Gentype[i].Index - 1;
                
                navixod_Click(null, null);
            }
            for(int i = allgens.SelectedIndex + 1; i<Genfond.Count; i++)
                Genfond[i].index = Genfond[i].index - 1;
            
            Genfond[allgens.SelectedIndex].PreRemoveNaxer();

            Genfond.RemoveAt(allgens.SelectedIndex);
            allgens.Items.RemoveAt(allgens.SelectedIndex);

            resultgens.Tag = null;
            naxer_gen.IsEnabled = ebash_to_cell.IsEnabled = ebash_all.IsEnabled = Genfond.Count > 0;
            save.IsEnabled = Genfond.Count > 0 || Population.current.All_Cells.Count > 0;
            ZigControl.Visibility = Population.current.AllChroms > 0 ? Visibility.Visible : Visibility.Hidden;
            Population.current.SmthUnSaved = Genfond.Count > 0;
        }

        private void addlife_Click(object sender, RoutedEventArgs e)
        {
            naxer_gen.Visibility = Visibility.Hidden;
            Cell newlymade = Population.current.na_podxode.Clone() as Cell;
            newlymade.MakeButton(Population.current.last_location, ref canvas);
            clickables = canvas.Children.OfType<Button>().ToArray();
        }
        private void Change_Sx(object sender, RoutedEventArgs e) { 
            
            Population.current.Zg = xxxy.IsChecked.Value ? Population.Zigotity.XX : Population.Zigotity.ZZ;
            if (isF == null)
                return;
            only_mutable.sx = isF.IsChecked.Value ? Cell.Sx.F : Cell.Sx.M;
            only_mutable.homogametous = (only_mutable.sx == Cell.Sx.F && Population.current.Zg == Population.Zigotity.XX) || (only_mutable.sx == Cell.Sx.M && Population.current.Zg == Population.Zigotity.ZZ);
            if (main_box!=null && Gentype.Count>0)
                ui.GametInput.AccordingToChroms();
        }

        private void Button_Click(object sender, RoutedEventArgs e) { Population.current.na_podxode.OpenInfo(); }

        static private bool pressed = false;
        Line[] cross = new Line[2] { null, null};
        SolidColorBrush selection = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            pressed = true;
            foreach (Line el in cross) { canvas.Children.Remove(el); }
            for (byte i=0; i<2; i++) { cross[i] = null; }

            Point point = e.GetPosition(sender as Canvas);
            cross[0] = new Line() { X1 = point.X, X2 = point.X, Y1 = 0, Y2 = canvas.ActualHeight, Stroke=selection};
            cross[1] = new Line() { X1 = 0, X2 = canvas.ActualWidth, Y1 = point.Y, Y2 = point.Y, Stroke = selection };
            foreach(Line el in cross) { canvas.Children.Add(el); }
            Population.current.last_location = new Point(point.X, point.Y);
        }
        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            canvas.Children.Remove(region);            
            pressed = false;

            for (int i = 0; i < clickables.Length; i++)
            {
                if (region != null && clickables[i].Margin.Left > region.Margin.Left && clickables[i].Margin.Top > region.Margin.Top &&
                    region.Margin.Top + region.ActualHeight > clickables[i].Margin.Top + clickables[i].ActualHeight &&
                    region.Margin.Left + region.ActualWidth > clickables[i].Margin.Left + clickables[i].ActualWidth && Population.current.All_Cells[i].Alive)
                    Population.current.SelectedCells.Add(Population.current.All_Cells[i]);

                foreach (Cell c in Population.current.SelectedCells)
                    c.Highlight(ref clickables[c.index]);
            }
            region = null;
        }
        Rectangle region;
        Point current;
        double deltaX, deltaY;
        private void canvas_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key != Key.LeftShift && e.Key != Key.RightShift)
                return;

            foreach (Cell cel in Population.current.SelectedCells)
                cel.Repaint(ref clickables[cel.index]);

            if (Population.current.SelectedCells.Count >= 2)
            {
                if (Population.current.SelectedCells.Count > 2)
                {
                    MessageBox.Show($"нельзя скрещивать {Population.current.SelectedCells.Count} сразу))", "слишком много", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (Population.current.SelectedCells[0].sx != null && Population.current.SelectedCells[0].sx == Population.current.SelectedCells[1].sx)
                {
                    MessageBox.Show($"выбранные особи одного пола", "несостыковочка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    Cell c = new Cell(Population.current.SelectedCells[0], Population.current.SelectedCells[1]);
                    c.MakeButton(Population.current.last_location, ref canvas);
                    c.DrawRelation(ref canvas);
                    clickables = canvas.Children.OfType<Button>().ToArray();
                }

            }           
            Population.current.SelectedCells.Clear();
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog() { DefaultExt = ".pop" };
            sfd.FileOk += Sfd_FileOk;
            sfd.ShowDialog();         
        }
        private async void Sfd_FileOk(object sender, CancelEventArgs e) {
            Population.current.SmthUnSaved = false;
            await Population.SavePopulation((sender as SaveFileDialog).FileName);
            MessageBox.Show($"Успешно записано:\n {Genfond.Count} генов,\n{Population.current.All_Cells.Count} особей", "сохранить удалось", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Population.current.SmthUnSaved)
            {
                MessageBoxResult whattodo = MessageBox.Show("Есть несохраннённые изменения, сохранить сначала их?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                if (whattodo == MessageBoxResult.Yes)
                    save_Click(sender, null);             
            }
        }
        private void open_Click(object sender, RoutedEventArgs e)
        {
            Window_Closing(sender, new CancelEventArgs());
            OpenFileDialog ofd = new OpenFileDialog() { Filter = "Популяции (*.pop)|*.pop|Просто JSON|*.json|Текстовые файлы|*.txt|Все файлы ¯\\_(ツ)_/¯|*.*" };
            ofd.FileOk += Ofd_FileOk;
            ofd.ShowDialog();
        }
        private async void Ofd_FileOk(object sender, CancelEventArgs e) {
            SplashScreen spc = new SplashScreen(this, midcol.ActualWidth, this.Height);
            spc.Show();
            this.IsEnabled = false;
            try
            {
                await Population.GetPopulation((sender as OpenFileDialog).FileName);
            }
            catch {
                spc.Close();
                this.IsEnabled = true;
                MessageBox.Show("Файл повреждён или не является популяцией", "ошибочка вышла", MessageBoxButton.OK, MessageBoxImage.Error);
                Population.current.SmthUnSaved = false;
                return;
            }
            
            Genfond = Population.current.genofond; //низя удалять, это изменение ссылок :<
            Gentype = Population.current.na_podxode.genotype as List<Trait>;
            only_mutable = Population.current.na_podxode;

            allgens.Items.Clear();
            foreach (Gene g in Genfond)
                allgens.Items.Add(g.ToString());
            resultgens.Items.Clear();
            foreach (Trait t in Gentype)
                resultgens.Items.Add(t.Name);
            canvas.Children.Clear();
            foreach (Cell c in Population.current.All_Cells) { 
                c.MakeButton(ref canvas);
                c.DrawRelation(ref canvas);
            }
            clickables = canvas.Children.OfType<Button>().ToArray();
            ebash_all.IsEnabled = ebash_to_cell.IsEnabled = naxer_gen.IsEnabled = Genfond.Count > 0;
            navixod.IsEnabled = clear.IsEnabled = addlife.IsEnabled = Gentype.Count > 0;
            spc.Close();
            this.IsEnabled = true;
            Title = $"{System.IO.Path.GetFileNameWithoutExtension((sender as OpenFileDialog).FileName)} - симулятор генетики {Assembly.GetExecutingAssembly().GetName().Version}";
            MessageBox.Show($"Успешно выгружено:\n{Genfond.Count} генов,\n{Population.current.All_Cells.Count} особей", "открыть удалось", MessageBoxButton.OK, MessageBoxImage.Information);
            GC.Collect();
        }
        
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {           
            if (!pressed) return;
            if (region == null)
            {
                region = new Rectangle() { Margin = new Thickness(Population.current.last_location.X, Population.current.last_location.Y, 0, 0), Stroke = selection };
                canvas.Children.Add(region);
            }               
            current = e.GetPosition(sender as Canvas);
            deltaX = current.X - Population.current.last_location.X;
            deltaY = current.Y - Population.current.last_location.Y;
            region.Width = Math.Abs(deltaX);
            region.Height = Math.Abs(deltaY);
            
            if (deltaX < 0)
                region.Margin = new Thickness(Population.current.last_location.X + deltaX, region.Margin.Top, 0, 0);
            if(deltaY <0)
                region.Margin = new Thickness(region.Margin.Left, Population.current.last_location.Y + deltaY, 0, 0);                     
        }     
    }
}
