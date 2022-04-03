using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Unicode;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Encodings.Web;
using System.Windows.Controls;
using System.Windows.Media;

namespace Симулятор_генетики_4
{
    /// <summary>
    /// Заход в игру
    /// </summary>
    class Population
    {
        public const string FilesTypesFilter = "Популяции (*.pop)|*.pop|Просто JSON|*.json|Текстовые файлы|*.txt|Все файлы ¯\\_(ツ)_/¯|*.*";
        public enum Zigotity { XX, ZZ};
        Population() { }
        static private Population le_ses;
        static public Population current { 
            get {
                if (le_ses == null)//При другой форме записи почему-то stack overflow exception вылетало(((
                {
                    le_ses = new Population();
                    le_ses.genofond = new List<Gene>(10);
                    le_ses.taken_aims = new List<bool>(10);
                    le_ses.Zg = Zigotity.XX;
                    le_ses.last_location = new Point(20, 20);
                    le_ses.All_Cells = new List<Cell>(20);
                    le_ses.na_podxode = new Cell() { genotype = new List<Trait>(10) };
                    le_ses.SmthUnSaved = false;
                }
                return le_ses;
            }
        }
        public bool SmthUnSaved;
        public List<Gene> genofond { get; set; }
        /// <summary>
        /// отвечает на вопрос, на какие гены уже воздействуют активаторы
        /// </summary>
        public List<bool> taken_aims { get; set;}
        public List<Cell> All_Cells{ get; set;}
        public List<Cell> SelectedCells = new List<Cell>(50);
        public Cell na_podxode { get; set; }
        public Zigotity Zg { get; set; }
        static public Cell CurrentKid;
        /// <summary>
        /// Сколько вообще XYшных генов
        /// </summary>
        public uint AllChroms { get; set; }
        /// <summary>
        /// Сколько ХУшных генов на выходе
        /// </summary>
        public uint ChromsNaPodxode { get; set; }
        /// <summary>
        /// для размещения кнопок
        /// </summary>
        public Point last_location { get; set; }       
        static private int insert_kuda { get; set; }
        static public int FuturePlace { get { return insert_kuda; } }
        static public int binarySearch(Gene what, IList<Trait> @where)
        {
            insert_kuda = 0;
            if (where.Count == 0)
                return -1;
            int lo=0, hi=where.Count, mid;
            if(what.index == where[0].Index) 
                return 0;
            while (hi - lo > 1)
            {
                mid = lo + (hi - lo) / 2;
                if (what.index == where[mid].Index)
                    return what.index;
                else if (what.index > where[mid].Index)
                    lo = mid;
                else if (what.index < where[mid].Index)
                    hi = mid;
            }
            insert_kuda = what.index < where[0].Index? 0 : hi;
            if (hi - lo == 1)
                return -1;
            else
                return (hi + lo) / 2;
        }

        static private JavaScriptEncoder encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        static private JsonSerializerOptions options = new JsonSerializerOptions() { IgnoreNullValues = true, WriteIndented = true, Encoder = encoder };
        
        public static async Task SavePopulation(string kuda)
        {
            using (FileStream fs = new FileStream(kuda, FileMode.OpenOrCreate, FileAccess.Write))
            {
                await JsonSerializer.SerializeAsync(fs, current, options);
                fs.Close();
                await fs.DisposeAsync();
            }
        }
        public static async Task GetPopulation(string otkudava)
        {
            using (FileStream fs = new FileStream(otkudava, FileMode.Open, FileAccess.Read))
            {
                le_ses = await JsonSerializer.DeserializeAsync<Population>(fs, options);
                fs.Close();
                await fs.DisposeAsync();
            }
        }

        /// <summary>
        /// Для отрисовки всех кнопок сразу
        /// </summary>
        /// <param name="subj"></param>
        /// <param name="nodes">собсна кнопки с холста</param>
        public void DisplayOneGeneStats(Gene subj, Button[] nodes, IList<Cell> cells)
        {
            Trait subjs_result = null;
            GeometryDrawing triangle = new GeometryDrawing()
            {
                Brush = Brushes.Firebrick,
                Geometry = new PathGeometry()
                {
                    Figures = new PathFigureCollection(1) { new PathFigure() {IsClosed = true, IsFilled = true, StartPoint = new Point(0, 0),
                            Segments=new PathSegmentCollection(2) {
                                new LineSegment(new Point(0, Cell.size_karkas.Height), false),
                                new LineSegment(new Point(Cell.size_karkas.Width, Cell.size_karkas.Height), false)
                            } },
                    }
                }
            };
            GeometryDrawing figure = new GeometryDrawing() { Brush = triangle.Brush };
            DrawingGroup drg;
            foreach (Cell c in cells)
            {
                //Такого признака у особи может вообще не быть
                int position = binarySearch(subj, c.genotype);
                if (position < 0)
                    continue;
                //если таки есть, написать на кнопке ааа и очистить обязательно
                drg = (nodes[c.index].Background as DrawingBrush).Drawing as DrawingGroup;
                if (drg.Children.Count > 2)
                    drg.Children.RemoveAt(2);

                subjs_result = c.genotype[position];
                nodes[c.index].Content = subjs_result.ToLetters;

                //Может быть нечего помечать красным
                if (subj.lethal == null && subj.AlertAllels == null)
                    continue;

                figure.Geometry = c.sx == Cell.Sx.F ? new CombinedGeometry(GeometryCombineMode.Intersect, triangle.Geometry, (drg.Children[1] as GeometryDrawing).Geometry) : triangle.Geometry;

                c.Repaint(ref nodes[c.index]);
                switch (subjs_result.HowToMark())
                {
                    case Trait.AlertStates.HasAlert:
                        drg.Children.Add(figure);
                        break;
                    case Trait.AlertStates.IsAlert:
                        (drg.Children[1] as GeometryDrawing).Brush = Brushes.Firebrick;
                        break;
                    case Trait.AlertStates.HasBoth:
                        figure.Brush = Brushes.DarkRed;
                        drg.Children.Add(figure);
                        break;
                    case Trait.AlertStates.HasLethal:
                        if (!c.Alive)
                            break;
                        figure.Brush = Brushes.Black;
                        drg.Children.Add(figure);
                        break;
                }
            }
        }
    }
}
