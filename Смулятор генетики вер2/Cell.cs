using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Симулятор_генетики_вер2
{
    /// <summary>
    /// Представляет клетку
    /// </summary>
    public class Cell
    {
        public List<AGene> dnk = new List<AGene>(); //гены этой клетки для скрещивания, менять нельзя
        public int position { get; set; }
        public Point mesto { get; set; }
        public Color indic { get; set; }

        public bool sx { get; set; }
        public bool alive { get; set; }
        sbyte chr { get; set; }

        public List<int> parents = new List<int>();

        static readonly Random rand = new Random();
        static public Dictionary<string, sbyte> gams = new Dictionary<string, sbyte>(3)
        {
            {"a", 0}, {"Ā", 1}, {"A", 2}
        };
        static public Dictionary<string, sbyte[]> bloods = new Dictionary<string, sbyte[]>(6) 
        { 
            {"00", new sbyte[] {0, 0}}, {"AA", new sbyte[] {1, 1}}, {"A0", new sbyte[] {1, 0}},
            {"BB", new sbyte[] {2, 2}}, {"B0", new sbyte[] {2, 0}}, {"AB", new sbyte[] {1, 2}}
        };
                                
        public Cell() { }
        
        public Cell(Session zaxod, bool sx, Panel p, List<AGene> myOwn, List<Cell> home, Point locate)
        {          
            this.mesto = new Point(locate.X, locate.Y);
            this.sx = sx;
            this.alive = true;
            this.chr += Convert.ToSByte(zaxod.xrom);
            foreach (AGene g in myOwn) { this.dnk.Add((AGene)g.Clone()); } //уравнивать списки НИЗЯ
            this.indic = zaxod.xrom == 0 ? Color.LightGray : (this.sx ? Color.LightPink : Color.LightBlue);
            this.position = home.Count;           

            home.Add(this);

            Button b = new Button() { Size = new Size(45, 45), Location = this.mesto, BackColor = this.indic, Text = home.Count.ToString() };
            p.Controls.Add(b);
            b.Click += (s, a) => { MakeGenom(this.dnk, this.position); b.Focus(); };
        }

        static private DataGridViewCellStyle silent = new DataGridViewCellStyle() { BackColor = Color.LightGreen },
            missed = new DataGridViewCellStyle() { BackColor = Color.LightSalmon },
            inactive = new DataGridViewCellStyle() { BackColor = Color.LightGray, ForeColor = Color.Gray };
        static public void MakeGenom(List<AGene> list, int indx) {
            
            InfoForm table = new InfoForm();
            try
            {
                table.dgv1.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell()) { Name = "gen" });
                table.dgv1.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell()) { Name = "arrs" });
                table.dgv1.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell()) { Name = "treats" }); 
                if (indx >= 0) {
                    try { Form1.wins.Add(indx, table); }
                    catch { }
                }
            }
            catch (InvalidOperationException) { return; }

            for (int i = 0; i < list.Count; i++)
            {
                table.dgv1.Rows.Add();
                table.dgv1["arrs", i].Value = list[i].zig[0].ToString();
                if (list[i].zig.Count == 2) { table.dgv1["arrs", i].Value += ", " + list[i].zig[1].ToString(); }
                
                if (list[i].mut != AGene.Mutations.None) {
                    table.dgv1["arrs", i].Style = list[i].mut == AGene.Mutations.Missense ? missed : silent;
                }
                
                table.dgv1["gen", i].Value = list[i].name;

                if (list[i].active) {
                    try {
                        table.dgv1["treats", i].Style = new DataGridViewCellStyle() { 
                            BackColor = ColorTranslator.FromHtml(list[i].ToString()) };
                    }
                    catch { table.dgv1["treats", i].Value = list[i].ToString(); }
                }
                else {
                    table.dgv1["gen", i].Style = inactive; 
                    table.dgv1["treats", i].Value = "N/A";
                    table.dgv1["treats", i].Style = inactive;
                    table.dgv1["arrs", i].Style = inactive;
                }
            }                  
            table.Text = indx>=0? $"Генотип {indx+1}-ой клетки" : "Предварительный геном";
            table.Show();
        }

        static public void Choose(Panel p, Cell c, List<Cell> list, bool add, Color nb, Color m, Color f)
        {
            if (!Convert.ToBoolean(c.chr)) { p.Controls.OfType<Button>().ToArray<Button>()[c.position].BackColor = nb; }
            else
            {
                p.Controls.OfType<Button>().ToArray<Button>()[c.position].BackColor = c.sx ? f : m;
            }
            if (add && !list.Contains(c)) { list.Add(c); }
            else { list.Remove(c); }
        }

        static public bool AreReady(Cell c1, Cell c2) {
            bool yes = c1.dnk.Count == c2.dnk.Count;
            for (int i = 0; i < c1.dnk.Count; i++)
            {
                try { yes = yes && (c1.dnk[i].name == c2.dnk[i].name) && c1.dnk[i].ress.SequenceEqual(c2.dnk[i].ress); }
                catch (ArgumentNullException) { yes = yes && (c1.dnk[i].name == c2.dnk[i].name); }
            }
            return yes;
        }
    
        static public AGene VotTeXrest(AGene mg, AGene fg, bool isF)
        {
            AGene c1 = (AGene)mg.Clone();
            AGene c2 = (AGene)fg.Clone();

            AGene ofKid = c1;

            if (mg is Chrom)
            {
                if (mg.zig.Count < fg.zig.Count) //меняем местами если что
                {
                    c2 = (AGene)mg.Clone();
                    c1 = (AGene)fg.Clone();
                }
            }
            ofKid.zig = new List<sbyte>() { c1.zig[rand.Next(0, 2)], c2.zig[0]};
            if (mg is Chrom)
            {
                if (isF)
                {
                    try { ofKid.zig[1] = c2.zig[rand.Next(0, 2)]; }
                    catch { }
                }
                else { ofKid.zig.RemoveAt(1); }
            }
            else { ofKid.zig[1] = c2.zig[rand.Next(0, 2)]; }

            if (mg is Mendel) { return (Mendel)ofKid; }
            else if (mg is Chrom) { return (Chrom)ofKid; }
            else if (mg is Blood) { return (Blood)ofKid; }
            else if (mg is Lethal) { return (Lethal)ofKid; }
            else { return (Active)ofKid; }
        }

        static sbyte sumZig(List<sbyte> arr) {
            if (arr.Count() == 2) { return Convert.ToSByte((int)arr[0] + (int)arr[1]); }
            else { return arr[0]; }
        }
        static int startkey;
        public void mutate() {
            foreach (AGene g in this.dnk) {
                if(!(g is Lethal)) {
                    int prevSum = sumZig(g.zig);
                    for (int i = 0; i < g.zig.Count; i++)
                    {              
                        startkey = rand.Next(0, Session.Current.mutProb);
                        if (startkey == 1)
                        {
                            g.zig[i] = Convert.ToSByte(rand.Next(0, 2));
                            if (!g.codom) { g.zig[i] = Convert.ToSByte((int)g.zig[i] * 2); }   
                            g.mut = prevSum == sumZig(g.zig) ? AGene.Mutations.Silence : AGene.Mutations.Missense;
                        }
                    }
                }
            }
        }

        public void activate()
        {
            foreach (Active agen in this.dnk.Where<AGene>(a => a is Active))
            {
                foreach (string a in agen.aims)
                {
                    int indx = this.dnk.FindLastIndex(x => x.name == a);
                    if (indx >= 0)
                    {
                        this.dnk[indx].active = (agen.zig[0] + agen.zig[1] > 0) != agen.inverted;
                    }
                }
                agen.name = agen.represent(this.dnk);
            }
        }

        /// <summary> Определить, живая ли будет клетка </summary>
        public void ToBeOrNot(AGene g, Panel panel) {
            if (g is Lethal)
            {
                this.alive = g.zig[0] + g.zig[1] < Lethal.probability;
                if (!this.alive) { panel.Controls.OfType<Button>().ToArray<Button>()[this.position].BackColor = Color.Black; }
                panel.Controls.OfType<Button>().ToArray<Button>()[Session.Current.allCells.IndexOf(this)].Enabled = this.alive;
            }
        }
    }
}

