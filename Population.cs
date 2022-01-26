using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Unicode;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Encodings.Web;

namespace Симулятор_генетики_4
{
    /// <summary>
    /// Заход в игру
    /// </summary>
    class Population
    {
        public enum Zigotity { XX, ZZ};
        Population() { }
        static private Population le_ses;
        static public Population current { 
            get {
                if (le_ses == null)
                {
                    le_ses = new Population();
                    le_ses.genofond = new List<Gene>(10);
                    le_ses.taken_aims = new List<bool>(10);
                    le_ses.last_location= new Point(20, 20);
                    le_ses.All_Cells = new List<Cell>(20);
                    le_ses.na_podxode = new Cell() { genotype = new List<Trait>(10), sx=null };
                    le_ses.Zg= Zigotity.XX;
                    le_ses.SmthUnSaved = false;
                }
                return le_ses;
            }
        }
        public bool SmthUnSaved;
        public List<Gene> genofond { get; set; }
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
        static public int binarySearch(Gene what, List<Trait> @where)
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
    }
}
