using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Симулятор_генетики_4
{
    public class Cell : ICloneable
    {
        public enum Sx { F, M }
        public Sx? sx { get; set; }
        public IList<Trait> genotype { get; set; }
        public double[] point { get; set; }
        /// <summary>
        /// ХХ/YY - переносит 2 аллеля или ХУ - 1 хромосома ни о чём
        /// </summary>
        public bool homogametous { get; set; }
        /// <summary>
        /// индекс по времени добавления
        /// </summary>
        //нужен для отображения на кнопках и вообще мгновенного взаимодействия с соответствующей кнопкой!
        public int index { get; set; } 
        /// <summary>
        /// номер поколения
        /// </summary>
        public int generation { get; set; }
        /// <summary>
        /// индексы родителей
        /// </summary>
        public int[] parents { get; set; }

        public bool Alive { get; set; }
        /// <summary>
        /// Открыта ли уже сводка
        /// </summary>
        public bool BeingDisplayed = false;

        static public Rect size_karkas = new Rect(0, 0, 60, 60);
        static private SolidColorBrush light_green = new SolidColorBrush(Color.FromRgb(218, 255, 181)),
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
            this.MakeButton(ref canvas);
        }
        public void MakeButton(ref Canvas canvas)
        {      
            Button b = new Button() { Margin = new Thickness(point[0], point[1], 0, 0), Height = size_karkas.Height, Width = size_karkas.Height };
            b.Background = new DrawingBrush() 
            {
                Drawing = new DrawingGroup() {
                    Children = new DrawingCollection() { new GeometryDrawing() { Geometry = new RectangleGeometry(size_karkas)} }
                }
            };
            GeometryDrawing absolute_bg = new GeometryDrawing();
            if (this.sx == Sx.F)
                absolute_bg.Geometry = new EllipseGeometry(new Point(size_karkas.Width / 2, size_karkas.Height / 2), size_karkas.Width / 2, size_karkas.Height / 2);
            else
                absolute_bg.Geometry = new RectangleGeometry(size_karkas);                  
            ((b.Background as DrawingBrush).Drawing as DrawingGroup).Children.Add(absolute_bg);

            b.Click += B_Click;
            Population.current.last_location = new Point(Population.current.last_location.X + size_karkas.Width*1.5, Population.current.last_location.Y);

            canvas.Children.Add(b);
            if (canvas.ActualWidth - point[0] - b.Width < b.Width * 2.5)
                canvas.Width = canvas.ActualWidth + b.Width * 2.5;
            
            if (canvas.ActualHeight - point[1] - b.Height < b.Height * 2.5)
                canvas.Height = canvas.ActualHeight + b.Height * 2.5;           
        }
        public void DrawRelation(ref Canvas canvas)
        {
            if (this.parents == null)
                return;
            for (int i = 0; i < 2; i++) {
                Line line = new Line() {
                    X1 = this.point[0] + size_karkas.Width/2,
                    X2 = Population.current.All_Cells[this.parents[i]].point[0] + size_karkas.Width / 2,
                    Y1 = this.point[1],
                    Y2 = Population.current.All_Cells[this.parents[i]].point[1] + size_karkas.Height,
                    Stroke = white, StrokeThickness = 3
                };
                canvas.Children.Add(line);
            }
        }

        private void change_colour(Button btn, Brush dead, Brush generic, Brush m, Brush f) {
            GeometryDrawing BG = ((btn.Background as DrawingBrush).Drawing as DrawingGroup).Children[1] as GeometryDrawing;
            if (!this.Alive) {
                BG.Brush = dead; return;
            }
            BG.Brush = this.sx == null ? generic : this.sx == Sx.F ? f : m;
            btn.Foreground = this.Alive ? Brushes.Black : Brushes.White;
        }

        public void Highlight(ref Button b) { change_colour(b, Brushes.DarkGray, light_green, light_blue, light_pink); }
        public void Repaint(ref Button b) { change_colour(b, Brushes.Black, Brushes.Lime, Brushes.LightBlue, Brushes.Pink); }
        private void B_Click(object sender, RoutedEventArgs e){ this.OpenInfo();}

        static private List<Trait> gentype = Population.current.na_podxode.genotype as List<Trait>;
        public object Clone()
        {
            Cell clone = new Cell() { generation = this.generation, homogametous = this.homogametous, sx = this.sx };
            if (Population.current.ChromsNaPodxode == 0)
                clone.sx = null;
            clone.genotype = new Trait[gentype.Count];
            for(int i =0; i< gentype.Count; i++)
            {
                clone.genotype[i] = gentype[i].Clone() as Trait;
            }
            Population.current.All_Cells.Add(clone);
            return clone;
        }
        
        public Cell() { 
            homogametous = true; 
            sx = null; 
            index = Population.current.All_Cells.Count;
            this.Alive = true;
            this.BeingDisplayed = false;
            Population.current.SmthUnSaved = true;
        }
        public Cell(Cell parent1, Cell parent2)
        {
            Population.CurrentKid = this;
            generation = (parent1.generation > parent2.generation) ? parent1.generation + 1 : parent2.generation + 1;
            index = Population.current.All_Cells.Count;
            sx = parent1.sx == null ? null : (Sx?)(new Random()).Next(0, 2);
            this.parents = new int[] { parent1.index, parent2.index };
            homogametous = parent1.sx == null || (sx == Sx.F && Population.current.Zg == Population.Zigotity.XX) || (sx == Sx.M && Population.current.Zg == Population.Zigotity.ZZ);

            List<int> allacts = new List<int>(10), indxs_of_traits = new List<int>(20);          
            Trait[] arr = new Trait[parent1.genotype.Count];
            for (int k = 0; k < arr.Length; k++)
            {
                arr[k] = Population.current.genofond[parent1.genotype[k].Index] is Raspred ?
                                                                                new NumericTrait(parent1.genotype[k], parent2.genotype[k], this.homogametous) :
                                                                                new Trait(parent1.genotype[k], parent2.genotype[k], this.homogametous);
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
            this.Alive = true;
            foreach(Trait t in arr)
            {
                LethalComponent lel = Population.current.genofond[t.Index].lethal;
                if (lel == null)
                    continue;
                this.Alive = !(lel.ReallyKills(t) && t.active);
                if (!this.Alive)
                    break;
            }

            this.genotype = arr;       
            Population.current.All_Cells.Add(this);
            Population.CurrentKid = null;
            Population.current.SmthUnSaved = true;
        }
    }    
}
