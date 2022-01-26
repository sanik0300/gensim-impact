using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Text.Json.Serialization;
using System.Linq;

namespace Симулятор_генетики_4
{
    public class Cell : ICloneable
    {
        public enum Sx { F, M }
        public Sx? sx { get; set; }
        private IEnumerable<Trait> gttp;
        public IEnumerable<Trait> genotype { 
            get { return gttp;}
            set
            {
                gttp = value is List<Trait>? value : value.ToArray();
            } 
        }
        public double[] point { get; set; }
        /// <summary>
        /// ХХ/YY - переносит 2 аллеля или ХУ - 1 хромосома ни о чём
        /// </summary>
        public bool homogametous { get; set; }
        /// <summary>
        /// индекс по времени добавления
        /// </summary>
        private int Index;
        public int index { get { return Index; } set { Index = value; } }
        /// <summary>
        /// номер поколения
        /// </summary>
        public int generation { get; set; }
        /// <summary>
        /// индексы родителей
        /// </summary>
        public int[] parents { get; set; }

        bool alive = true;
        public bool Alive { get { return alive; } set { alive = value; } }
        /// <summary>
        /// Открыта ли уже сводка
        /// </summary>
        public bool BeingDisplayed = false;

        static private SolidColorBrush green = new SolidColorBrush(Color.FromRgb(172, 255, 117)),
                                        pink = new SolidColorBrush(Color.FromRgb(255, 158, 202)),
                                        blue = new SolidColorBrush(Color.FromRgb(140, 226, 255)),
                                        black = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                                        light_green = new SolidColorBrush(Color.FromRgb(218, 255, 181)),
                                        light_pink = new SolidColorBrush(Color.FromRgb(255, 217, 233)),
                                        light_blue = new SolidColorBrush(Color.FromRgb(217, 246, 255));

        public void OpenInfo()
        {
            if (this.BeingDisplayed)
                return;
            new SvodkaOfGens(this).Show();
        }
        static private SolidColorBrush white = new SolidColorBrush(Color.FromRgb(255, 255, 255)); 
        public void MakeButton(Point loc, ref Canvas canvas)
        {
            this.point = new double[2] { loc.X, loc.Y };
            Button b = new Button() { Margin = new Thickness(point[0], point[1], 0, 0), Height = 60, Width = 60 };
            if (this.alive)
            {
                if (this.sx == null)
                    b.Background = green;
                else
                    b.Background = this.sx == Sx.F ? pink : blue;
            }
            else { b.Background = black; }
            b.Content = (this.index + 1).ToString();
            b.Click += B_Click;
            Population.current.last_location = new Point(Population.current.last_location.X + 90, Population.current.last_location.Y);

            canvas.Children.Add(b);
            if(canvas.ActualWidth - point[0] - b.Width < b.Width*2.5)
            {
                canvas.Width = canvas.ActualWidth + b.Width * 2.5;
            }
            if(canvas.ActualHeight - point[1]-b.Height < b.Height * 2.5)
            {
                canvas.Height = canvas.ActualHeight + b.Height * 2.5;
            }             
        }
        public void MakeButton(ref Canvas canvas)
        {
            Button b = new Button() { Margin = new Thickness(point[0], point[1], 0, 0), Height = 60, Width = 60 };
            if (this.alive)
            {
                if (this.sx == null)
                    b.Background = green;
                else
                    b.Background = this.sx == Sx.F ? pink : blue;
            }
            else { b.Background = black; b.Foreground = Brushes.White; }
            b.Content = (this.index + 1).ToString();
            b.Click += B_Click;
            Population.current.last_location = new Point(Population.current.last_location.X + 90, Population.current.last_location.Y);

            canvas.Children.Add(b);
            if (canvas.ActualWidth - point[0] - b.Width < b.Width * 2.5)
            {
                canvas.Width = canvas.ActualWidth + b.Width * 2.5;
            }
            if (canvas.ActualHeight - point[1] - b.Height < b.Height * 2.5)
            {
                canvas.Height = canvas.ActualHeight + b.Height * 2.5;
            }
        }
        public void DrawRelation(ref Canvas canvas)
        {
            if (this.parents == null)
                return;
            Line to_P1 = new Line()
            {
                X1 = this.point[0]+30,
                X2 = Population.current.All_Cells[this.parents[0]].point[0]+30,
                Y1 = this.point[1],
                Y2 = Population.current.All_Cells[this.parents[0]].point[1]+60,
                Stroke = white, StrokeThickness = 3
            };
            canvas.Children.Add(to_P1);
            Line to_P2 = new Line()
            {
                X1 = this.point[0]+30,
                X2 = Population.current.All_Cells[this.parents[1]].point[0]+30,
                Y1 = this.point[1],
                Y2 = Population.current.All_Cells[this.parents[1]].point[1]+60,
                Stroke = white, StrokeThickness = 3
            };
            canvas.Children.Add(to_P2);          
        }
        public void Highlight(ref Button b)
        {
            if (!this.Alive)
                return;
            b.Background = this.sx == null ? light_green : this.sx == Sx.F ? light_pink : light_blue;
        }
        public void Repaint(ref Button b)
        {
            if (!this.Alive)
                return;
            b.Background = this.sx == null ? green : this.sx == Sx.F ? pink : blue;
        }
        private void B_Click(object sender, RoutedEventArgs e){ this.OpenInfo();}

        static private List<Trait> gentype = Population.current.na_podxode.genotype as List<Trait>;
        public object Clone()
        {
            Cell clone = new Cell() { alive = true, BeingDisplayed = false, generation = 0, homogametous = this.homogametous, sx = this.sx };
            if (Population.current.ChromsNaPodxode == 0)
                clone.sx = null;
            clone.genotype = new Trait[gentype.Count];
            for(int i =0; i< gentype.Count; i++)
            {
                (clone.genotype as Trait[])[i] = gentype[i].Clone() as Trait;
            }
            Population.current.All_Cells.Add(clone);
            return clone;
        }
        
        public Cell() { 
            homogametous = true; sx = null; Index = Population.current.All_Cells.Count;
            Population.current.SmthUnSaved = true;
        }
        static private Random random = new Random();
        public Cell(Cell parent1, Cell parent2)
        {
            Population.CurrentKid = this;
            generation = (parent1.generation > parent2.generation) ? parent1.generation + 1 : parent2.generation + 1;
            Index = Population.current.All_Cells.Count;
            sx = parent1.sx == null ? null : (Sx?)random.Next(0, 2);
            this.parents = new int[] { parent1.index, parent2.index };
            homogametous = parent1.sx == null || (sx == Sx.F && Population.current.Zg == Population.Zigotity.XX) || (sx == Sx.M && Population.current.Zg == Population.Zigotity.ZZ);

            List<int> allacts = new List<int>(10), indxs_of_traits = new List<int>(20);
            Trait[] genotype1 = parent1.genotype is Trait[]? parent1.genotype as Trait[] : parent1.genotype.ToArray(),
                    genotype2 = parent2.genotype is Trait[]? parent2.genotype as Trait[] : parent2.genotype.ToArray();
            Trait[] arr = new Trait[genotype1.Length];
            for (int k = 0; k < arr.Length; k++)
            {
                arr[k] = Population.current.genofond[genotype1[k].Index] is Raspred ?
                                                                                new NumericTrait(genotype1[k], genotype2[k], this.homogametous) :
                                                                                new Trait(genotype1[k], genotype2[k], this.homogametous);
                arr[k].Mutate();
                arr[k].CalculateResultingFentype(null, null);
                if (Population.current.genofond[arr[k].Index] is Activer)
                {
                    allacts.Add(arr[k].Index);
                    indxs_of_traits.Add(k);
                }                             
            }
            for(int i = 0; i<indxs_of_traits.Count; i++)
            {
                (Population.current.genofond[allacts[i]] as Activer).deActivate(ref arr[indxs_of_traits[i]], arr);
            }
            foreach(Trait t in arr)
            {
                LethalComponent lel = Population.current.genofond[t.Index].lethal;
                if (lel == null)
                    continue;
                this.alive =! (lel.ReallyKills(t) && t.active);
                if (!this.alive)
                    break;
            }

            this.genotype = arr;       
            Population.current.All_Cells.Add(this);
            Population.CurrentKid = null;
            Population.current.SmthUnSaved = true;
        }
    }    
}
