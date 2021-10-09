using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Симулятор_генетики_вер2
{
    /// <summary>
    /// Представляет заход в игру и может быть только 1
    /// </summary>
    public class Session
    {
        private static Session currentSes;
        private Session() { }
        public static Session Current
        {
            get
            {
                if (currentSes == null)
                {
                    currentSes = new Session();
                }
                return currentSes;
            }
            set { currentSes = value; }
        }

        //Что нужно для сохраненной игры
        public Point crtCell = new Point(20, 20);
        public int xrom = 0; //количество хромосомных генов клетки на подходе
        public int toDelCel = -1;
        public int mutProb = 10; //вероятность мутации (1 из) 
        public List<Cell> allCells = new List<Cell>();
        /// <summary>
        /// Индексы генов какие будут в клетке
        /// </summary>
        public List<int> willBeOnMe = new List<int>();
        public List<int> takenAims = new List<int>();

        public void AddFromScratch(List<AGene> kuda, AGene shto, ListBox repres, ListBox foracts=null)
        {
            kuda.Add(shto);
            if (shto is Chrom)
            {
                repres.Items.Add(shto.name + " XY");
            }
            else { repres.Items.Add(shto.name); }
            if (foracts != null && !(shto is Active)) { foracts.Items.Add(shto.name); }
        }

        public void DeleteFromHist<T>(List<T> list, ListBox repres, int indx, ListBox foracts=null)
        {
            if (foracts != null) { foracts.Items.Remove(repres.SelectedItem); }
            repres.Items.RemoveAt(indx);
            list.RemoveAt(indx);
        }

        public void ClearReg<T>(List<T> list, ListBox repres)
        {
            list.Clear();
            repres.Items.Clear();
        }

        JsonSerializerSettings settings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, TypeNameHandling = TypeNameHandling.All, Formatting = Formatting.Indented };

        public void serialize(SaveFileDialog svd)
        {
            using (FileStream fs = new FileStream(svd.FileName, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(JsonConvert.SerializeObject(this, settings));
                }
            }
        }

        public Session deserialize(OpenFileDialog opd)
        {
            using (FileStream fs = new FileStream(opd.FileName, FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    return JsonConvert.DeserializeObject<Session>(sr.ReadToEnd(), settings);
                }
            }
        }

        public void makeBodies(Panel panel)
        {
            foreach (Cell c in this.allCells)
            {
                Button b = new Button() { Size = new Size(45, 45), Location = c.mesto, Text = (c.position + 1).ToString() };
                b.BackColor = c.alive ? c.indic : Color.Black;
                b.Enabled = c.alive;
                b.Click += (s, a) => Cell.MakeGenom(c.dnk, c.position);
                panel.Controls.Add(b);
            }
        }

        public void registerNew(List<AGene> list, ListBox repres, ListBox foracts)
        {
            repres.Items.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                repres.Items.Add(string.Empty);
                repres.Items[i] = (list[i] is Chrom) ? list[i].name + " XY" : list[i].name;

                foracts.Items.Add(list[i].name);
            }
        }
    }
}
