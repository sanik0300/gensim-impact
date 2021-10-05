using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Симулятор_генетики_вер2
{
    public partial class Form1 : Form
    {
        static bool sel = false;
        static Rectangle videl = new Rectangle();
        Graphics ge;       
        static Point begin = new Point();
        static Color ofSelected = Color.Magenta; //цвет выбранной кнопки
        static public Dictionary<int, InfoForm> wins = new Dictionary<int, InfoForm>();

        List<Cell> mates = new List<Cell>();

        Session ses = Session.Current;

        static T customControl<T>(string txt, Size s, Point loc, bool vis = true, bool checkd = false, bool enbld = true) where T : Control, new()
        {
            T item = new T() { Text = txt, Size = s, Location = loc, Visible = vis, Enabled = enbld };

            if (item is RadioButton) { (item as RadioButton).Checked = checkd; }
            else if (item is CheckBox) { (item as CheckBox).Checked = checkd; }

            return item;
        }
        //создать сцуко контрол с заданными параметрами

        // Элементы учётки для АА аа генов
        public GroupBox mend = customControl<GroupBox>("Аутосомный", new Size(162, 204), new Point(13, 36), false); //групбокс д/р
        public TextBox nameIdent = customControl<TextBox>(string.Empty, new Size(149, 20), new Point(6, 39)),
                       domGenRes = customControl<TextBox>(string.Empty, new Size(149, 20), new Point(6, 91)), //input доминантный признак 
                        recGenRes = customControl<TextBox>(string.Empty, new Size(149, 20), new Point(6, 176)); //input рецессивный признак 
        Label whatName = customControl<Label>("название", new Size(100, 12), new Point(6, 23)),
              mendDom = customControl<Label>("A", new Size(15, 12), new Point(6, 75)), //"доминантный алель"      
              mendRec = customControl<Label>("a", new Size(15, 12), new Point(6, 153)); //"рецессивный алель"     
        Button domColor = customControl<Button>("A", new Size(35, 20), new Point(20, 65)), //кнопка с доминантным цветом
               recColor = customControl<Button>("a", new Size(35, 20), new Point(20, 153)),//кнопка с рецессивным цветом
               yesofgen = customControl<Button>("✓", new Size(20, 20), new Point(126, 65)),
               noofgen = customControl<Button>("✖", new Size(20, 20), new Point(126, 153));

        //Неполное доминирование
        public CheckBox hasHalf = customControl<CheckBox>("Кодоминирование", new Size(140, 15), new Point(6, 111)); //будет ли неполное доминирование
        public TextBox halfGenRes = customControl<TextBox>(string.Empty, new Size(149, 20), new Point(5, 127), true, false, false); //input неполный алель

        //Группы крови
        public GroupBox groups = customControl<GroupBox>("Группа крови", new Size(161, 115), new Point(13, 83), false);
        public RadioButton g1 = customControl<RadioButton>("1", new Size(40, 17), new Point(7, 19), true, true),
                           g2 = customControl<RadioButton>("2", new Size(40, 17), new Point(7, 42)),
                           g3 = customControl<RadioButton>("3", new Size(40, 17), new Point(7, 65)),
                           g4 = customControl<RadioButton>("4", new Size(40, 17), new Point(7, 88));

        //Для сборки гена из 2х букв
        public GroupBox gamPut = customControl<GroupBox>("подставить гаметы", new Size(162, 90), new Point(13, 270), false);
        //Кнопки гамет
        Button big1st = customControl<Button>("A", new Size(35, 23), new Point(41, 21)),
               small1st = customControl<Button>("a", new Size(35, 23), new Point(81, 21)),           
               big2nd = customControl<Button>("A", new Size(35, 23), new Point(41, 56)),
               small2nd = customControl<Button>("a", new Size(35, 23), new Point(81, 56)); 

        //просто значки Х и У
        Label x1st = customControl<Label>("X1", new Size(35, 13), new Point(6, 26)),
              xOrY = customControl<Label>("X2", new Size(35, 13), new Point(6, 61));

        //Кнопки групп крови
        Button first = customControl<Button>("00", new Size(45, 23), new Point(112, 19)),
                      homo2 = customControl<Button>("AA", new Size(45, 23), new Point(63, 42), false),
                      het2 = customControl<Button>("A0", new Size(45, 23), new Point(112, 42), false),
                      homo3 = customControl<Button>("BB", new Size(45, 23), new Point(63, 65), false),
                      het3 = customControl<Button>("B0", new Size(45, 23), new Point(112, 65), false),
                      fourth = customControl<Button>("AB", new Size(45, 23), new Point(112, 88), false);
        //Смэрть
        GroupBox howLively = customControl<GroupBox>("живучесть", new Size(161, 112), new Point(13, 107), false);
        CheckBox ifTendsTo = customControl<CheckBox>("Является ли носителем", new Size(100, 17), new Point(6, 19), true, true);
        TrackBar percents = customControl<TrackBar>(string.Empty, new Size(149, 45), new Point(6, 42));
        Label indic = customControl<Label>("Вероятность:", new Size(150, 13), new Point(6, 87));

        //Переключатели активности
        GroupBox act = customControl<GroupBox>("Переключатель", new Size(162, 160), new Point(13, 155), false);
        ListBox futObjs = customControl<ListBox>("тут текст есть", new Size(144, 69), new Point(6, 24)), crtAims;
        Button domAct = customControl<Button>("А", new Size(35, 20), new Point(6, 99)),
                recAct = customControl<Button>("a", new Size(35, 20), new Point(6, 132));

        Label domIdent = new Label() { Text = "Будет активным", ForeColor = ofSelected, Location = new Point(50, 100) }, 
            recident = new Label() { Text = "не будет", Location = new Point(50, 132) };

        Button[] begn, end, doms, acts;
        RadioButton[] kinds;
        TextBox[] switchables;
        
        //--------------Методы наделения событиями ---------------
        void ProvideCheck(RadioButton rb, Button autoBut) //для автоматического выбора группы крови при выборе
        {
            rb.CheckedChanged += (s, a) => {
                if (rb.Checked) { 
                    foreach (Button b in groups.Controls.OfType<Button>()) { b.Visible = (b.Location.Y == rb.Location.Y) && rb.Checked; }
                    autoBut.PerformClick();
                }
            };
        } 
        void provideClick<I>(Button[] ovr, Dictionary<string, I> dict, int ind = -1) // оформить клик ВСЕМ
        {
            foreach (Button but in ovr) {
                but.Click += (s, a) => {
                    foreach (Button b in ovr) { b.BackColor = Button.DefaultBackColor; }
                    but.BackColor = ofSelected;

                    if (ind == -1) /*если -1 то это группа крови*/ { ses.willBeOnMe[listBox2.SelectedIndex].zig = new List<sbyte>(dict[but.Text] as sbyte[]); }
                    
                    else {
                        try { ses.willBeOnMe[listBox2.SelectedIndex].zig[ind] = Convert.ToSByte(dict[but.Text]); }
                        catch (ArgumentOutOfRangeException)
                        { ses.willBeOnMe[listBox2.SelectedIndex].zig.Add(Convert.ToSByte(dict[but.Text])); }
                    }
                };
            }
        }
        void provideColor(Button b, TextBox tb) {
            b.Click += (s, a) => {
                colorDialog1.ShowDialog();
                tb.BackColor = colorDialog1.Color;
                foreach (TextBox tt in switchables) { 
                    tt.ReadOnly = true;
                    tt.Text = null;
                }       
            };
        }
        void MixColor()//смешивание цвета в неполном гене 
        {
            if (hasHalf.Checked) {
                halfGenRes.BackColor = Color.FromArgb((domGenRes.BackColor.R + recGenRes.BackColor.R) / 2, 
                                                    (domGenRes.BackColor.G + recGenRes.BackColor.G) / 2, 
                                                    (domGenRes.BackColor.B + recGenRes.BackColor.B) / 2); }
        }
        KeyEventArgs forbthnstosee = new KeyEventArgs(Keys.None);
        void provideBinary(Button b, TextBox pair) {
            b.Click += (s, a) => {
                if (forbthnstosee.KeyCode == Keys.ShiftKey) { 
                    yesofgen.Text = (yesofgen.Text == "✓") ? "✖" : "✓";
                    noofgen.Text = (noofgen.Text == "✓") ? "✖" : "✓";
                }
                else {
                    clearmend(true);
                    domGenRes.Text = yesofgen.Text;
                    hasHalf.Checked = false;
                    recGenRes.Text = noofgen.Text;
                }   
            };
            b.TextChanged += (s, a) => { pair.Text = b.Text; };
        }
        void identInvertion(Button b, Label lel) {
            b.BackColorChanged += (s, a) => {
                lel.ForeColor = b.BackColor == Button.DefaultBackColor? DefaultForeColor : ofSelected;
                lel.Text = b.BackColor == Button.DefaultBackColor ? "не будет" : "Будет активным";
            };
        }
        void ifPutHalfIGene(Control pad, CheckBox chkb) //включить/выключить неполный домген
        {
            pad.Enabled = chkb.Checked;
            if (!chkb.Checked && pad is TextBox) { pad.Text = null; pad.BackColor = DefaultBackColor; }
            else if (domGenRes.BackColor != DefaultBackColor || recGenRes.BackColor != DefaultBackColor) { MixColor(); }
        }
        //--------------Методы наделения событиями --------------
        public Form1()
        {                     
            //Для кнопок гамет    
            begn = new Button[2] { small1st, big1st };
            end = new Button[2] { small2nd, big2nd, };
            doms = new Button[2] { big1st, big2nd };
            acts = new Button[2] { domAct, recAct };
            switchables = new TextBox[3] { recGenRes, halfGenRes, domGenRes };

            InitializeComponent();

            openFileDialog1.Filter = "Чистая линия|*.cln|Файл сценария|*.json|Текстовый файл|*.txt";
            saveFileDialog1.Filter = "Чистая линия|*.cln|Файл сценария|*.json|Текстовый файл|*.txt";

            //Упорядочить радиокнопки выбора типа гена
            kinds = new RadioButton[5] { radioButton1, radioButton2, radioButton3, radioButton6, radioButton7 };

            this.Controls.AddRange(new Control[] { mend, gamPut, groups, howLively, act});
            ge = panel1.CreateGraphics();
        }
        void clearmend(bool leave = false){  //очистить поля ввода
          TextBox[] aim = leave ? switchables : mend.Controls.OfType<TextBox>().ToArray();

            foreach (TextBox tb in aim) {
                    tb.ReadOnly = listBox2.SelectedIndex != -1;
                    tb.Text = null;
                    tb.BackColor = Color.White;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            hasHalf.CheckedChanged += (s, a) => ifPutHalfIGene(halfGenRes, hasHalf);

            mend.Controls.AddRange(new Control[] { whatName, nameIdent, mendDom, domGenRes, mendRec, recGenRes, hasHalf, halfGenRes, domColor, recColor, yesofgen, noofgen });
            mend.DoubleClick += (s, a) => clearmend();
            
            //Для групп крови       
            groups.Controls.AddRange(new Control[] { g1, g2, g3, g4, first, het2, homo2, het3, homo3, fourth });
            provideClick<sbyte[]>(groups.Controls.OfType<Button>().ToArray<Button>(), Cell.bloods);
            ProvideCheck(g1, first);
            ProvideCheck(g2, homo2);
            ProvideCheck(g3, homo3);
            ProvideCheck(g4, fourth);

            first.BackColor = ofSelected;
            fourth.BackColor = ofSelected;

            gamPut.Controls.AddRange(new Control[] { big1st, big2nd, small1st, small2nd, x1st, xOrY });
            
            provideClick<sbyte>(begn, Cell.gams, 0);
            provideClick<sbyte>(end, Cell.gams, 1);

            //для леталок
            howLively.Controls.AddRange(new Control[] { ifTendsTo, percents, indic });
            percents.Maximum = Lethal.probability - 1;
            percents.ValueChanged += (s, a) => {
                ses.willBeOnMe[listBox2.SelectedIndex].zig[0] = Convert.ToSByte(percents.Value);
                indic.Text = "Вероятность: " + percents.Value.ToString() + "/" + Lethal.probability.ToString();
            };
            ifTendsTo.CheckedChanged += (s, a) => {
                ifPutHalfIGene(percents, ifTendsTo);
                if (ifTendsTo.Checked) { ses.willBeOnMe[listBox2.SelectedIndex].zig[0] = Convert.ToSByte(percents.Value); }
                else { ses.willBeOnMe[listBox2.SelectedIndex].zig[0] = 0; }
            };

            //для цветных
            provideColor(domColor, domGenRes);
            provideColor(recColor, recGenRes);

            domGenRes.BackColorChanged += (s, a) => MixColor();
            recGenRes.BackColorChanged += (s, a) => MixColor(); 

            ToolTip toD = new ToolTip(), toR = new ToolTip();
            toD.SetToolTip(domGenRes, "Выбрать доминантный цвет");
            toR.SetToolTip(recGenRes, "Выбрать рецессивный цвет");

            //для да/нет-ных
            provideBinary(yesofgen, domGenRes);
            provideBinary(noofgen, recGenRes);

            //для активаторов
            crtAims = new ListBox() { Size = futObjs.Size, Location = futObjs.Location, SelectionMode = SelectionMode.None, BackColor = SystemColors.ActiveCaption, Visible = false };
            act.Controls.AddRange(new Control[] { futObjs, crtAims, domAct, recAct, domIdent, recident});
            domAct.BackColor = ofSelected;
            futObjs.SelectionMode = SelectionMode.MultiExtended;

            domAct.Click += (s, a) => { foreach (Button b in acts) { b.BackColor = (b.BackColor == ofSelected) ? Button.DefaultBackColor : ofSelected; } };
            recAct.Click += (s, a) => { foreach (Button b in acts) { b.BackColor = (b.BackColor == ofSelected) ? Button.DefaultBackColor : ofSelected; } };

            identInvertion(domAct, domIdent);
            identInvertion(recAct, recident);
        }
        
        //-------События мышки----------------------------------
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (sel) {
                ge.Clear(Color.FromArgb(192, 192, 255));

                if (begin.X > e.X && begin.Y > e.Y) { videl = new Rectangle(e.X, e.Y, begin.X - e.X, begin.Y - e.Y); }

                else if (begin.X > e.X) { videl = new Rectangle(e.X, begin.Y, begin.X - e.X, e.Y - begin.Y); }

                else if (begin.Y > e.Y) { videl = new Rectangle(begin.X, e.Y, e.X - begin.X, begin.Y - e.Y); }

                else { videl = new Rectangle(begin.X, begin.Y, e.X - begin.X, e.Y - begin.Y); }
                ge.DrawRectangle(ppap, videl);

                for (int i = 0; i < ses.allCells.Count; i++) {
                    if (videl.IntersectsWith(new Rectangle(panel1.Controls.OfType<Button>().ToArray<Button>()[i].Location, panel1.Controls.OfType<Button>().ToArray<Button>()[i].Size)) && !mates.Contains(ses.allCells[i]))
                    {
                        Cell.Choose(panel1, ses.allCells[i], mates, true, Color.Gray, Color.Blue, Color.HotPink);
                    }
                    else if (!videl.IntersectsWith(new Rectangle(panel1.Controls.OfType<Button>().ToArray<Button>()[i].Location, panel1.Controls.OfType<Button>().ToArray<Button>()[i].Size)) && mates.Contains(ses.allCells[i]))
                    {
                        Cell.Choose(panel1, ses.allCells[i], mates, false, Color.LightGray, Color.LightBlue, Color.LightPink);
                    }
                }
            }
        }
        private void panel1_MouseClick(object sender, MouseEventArgs e) { ses.crtCell = e.Location; }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            panel1.Focus();
            sel = true;
            begin = e.Location;
            ge.Clear(panel1.BackColor);
        }
        private async void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            sel = false;
            ge.Clear(panel1.BackColor);
            await Task.Run(() => DrawRelation());

            if (e.Button == MouseButtons.Right && mates.Count >= 2) {
                for (int i = 1; i < mates.Count; i++) {
                    if (!(mates[i].parents.SequenceEqual<int>(mates[i - 1].parents) ||
                       mates[i].parents.SequenceEqual<int>(mates[i - 1].parents.Reverse<int>()))) { return; }
                }
                MessageBox.Show("Скоро здесь будет расщепление", "А пока ремонт", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }   
        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e) { //Сбросить выделение
            foreach (Cell c in ses.allCells) { Cell.Choose(panel1, c, mates, false, Color.LightGray, Color.LightBlue, Color.LightPink); }
            listBox2.SelectedIndex = -1;
        }
        //---------события мышки-----------------------------------

        //-----Взаимодействие с окном----------------
        private void Form1_KeyUp(object sender, KeyEventArgs e) { forbthnstosee = e; }
        private void Form1_SizeChanged(object sender, EventArgs e) { ge = panel1.CreateGraphics(); }
        private async void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            forbthnstosee = e;
            if (e.KeyCode == Keys.Space) {
                if (mates.Count == 2)
                {
                    panel1.Focus();
                    if (ses.xrom > 0 && (mates[0].sx == mates[1].sx)) {
                        MessageBox.Show("выбранные клетки одного пола", "неcоcтыков очка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (Cell.AreReady(mates[0], mates[1])) {
                        Cell c = new Cell(ses, Convert.ToBoolean(new Random().Next(0, 2)), panel1, new List<AGene>(), ses.allCells, ses.crtCell);
                        c.parents.Add(mates[0].position);
                        c.parents.Add(mates[1].position);

                        for (int i = 0; i < mates[0].dnk.Count; i++) {
                            c.dnk.Add(Cell.VotTeXrest(mates[0].dnk[i], mates[1].dnk[i], c.sx));
                            c.mutate();
                            c.activate();
                            c.ToBeOrNot(c.dnk[i], panel1);
                        }
                        if (panel1.Width - c.mesto.X < 90) { panel1.Size = new Size(panel1.Width + 40, panel1.Height); }
                        if (panel1.Height - c.mesto.Y < 90) { panel1.Size = new Size(panel1.Width, panel1.Height + 40); }

                        await Task.Run(() => DrawRelation());
                    }
                    else { MessageBox.Show("Выбранные клетки с разными генотипами", "даешь скрещиваемость", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
                else { 
                    if (mates.Count != 0) { MessageBox.Show($"Ты как {mates.Count}х скрещивать собрался 0_о", "даешь скрещиваемость", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    return;           
                }
            }
            else if (e.KeyCode == Keys.Delete) {
                foreach (Cell m in mates)
                {
                    panel1.Controls.RemoveAt(m.position);
                    try { Form1.wins[m.position].Dispose(); } catch { }

                    foreach (Cell c in ses.allCells) {
                        if (c.parents.Contains(m.position)) { c.parents.Remove(m.position); }

                        if (c.position > m.position) {
                            panel1.Controls[c.position - 1].Text = c.position.ToString();
                            c.position -= 1;
                        }
                    }
                    ses.allCells.Remove(m);
                }
                mates.Clear();
                await Task.Run(() => DrawRelation());
            }
        }
        private void Form1_Click(object sender, EventArgs e)/*тоже сбросить выбор*/ { listBox2.SelectedIndex = -1; act.Text = "Задействованые гены"; }       //-----Взаимодействие с окном----------------


        //-------------------Выбор разных видов генов------------------        
        List<Control> crtTypeTools = new List<Control>();
        int radioindx = 0;
        void Place() /*сoбственно весь  метод расстановки */ 
        {
            foreach(Control c in crtTypeTools) { c.Visible = false; }
            crtTypeTools.Clear();
   
            if (radioButton1.Checked || radioButton2.Checked) {
                mend.Visible = true;
                crtTypeTools.Add(mend);        
            }
            else if (radioButton7.Checked) { act.Visible = true; crtTypeTools.Add(act); }

            if (listBox2.SelectedIndex >= 0) {
                if (ses.willBeOnMe[listBox2.SelectedIndex] is Blood) { crtTypeTools.Add(groups); }
                else if (ses.willBeOnMe[listBox2.SelectedIndex] is Lethal) { crtTypeTools.Add(howLively); }
                else { crtTypeTools.Add(gamPut); }
                
                crtTypeTools[crtTypeTools.Count-1].Visible = true;
            }
            
            for (int i = 0; i < kinds.Length; i++) { 
                if(i<=radioindx) { kinds[i].Location = new Point(13, radioButton1.Location.Y + 23 * i); }     
                
                try { 
                    crtTypeTools[0].Location = new Point(13, kinds[radioindx].Bottom + 6);                   
                    if (crtTypeTools.Count == 2) { crtTypeTools[1].Location = new Point(12, crtTypeTools[0].Bottom + 6); }  
                }
                catch { }               
                
                if(i>radioindx) {
                    try { kinds[i].Location = new Point(13, crtTypeTools[crtTypeTools.Count-1].Bottom + 6 + 23 * (i-radioindx-1)); }
                    catch { kinds[i].Location = new Point(13, radioButton1.Location.Y + 23 * i); }
                }
            }
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e) { mend.Text = "Аутосомный";   radioindx = 0;  Place(); }
        private void radioButton2_CheckedChanged(object sender, EventArgs e) { mend.Text = "Сцепленный с полом";    radioindx = 1;   Place(); }
        private void radioButton6_CheckedChanged(object sender, EventArgs e) { radioindx = 2;   Place(); }
        private void radioButton3_CheckedChanged(object sender, EventArgs e) { radioindx = 3;   Place(); }
        private void radioButton7_CheckedChanged(object sender, EventArgs e) { radioindx = 4;   Place(); }
        //------------------ выбор разных видов генов---------------
  
        //-------------Добавление клеток или признаков
        private void button2_Click(object sender, EventArgs e) //добавить клетку
        {
            try { if (ses.crtCell == ses.allCells.Last<Cell>().mesto) { ses.crtCell = new Point(ses.crtCell.X + 50, ses.crtCell.Y); } }
            catch { }
            Cell c = new Cell(ses, radioButton5.Checked, panel1, new List<AGene>(from g in ses.willBeOnMe orderby g.name select g), ses.allCells, ses.crtCell);
            c.activate();
        }
        private void button1_Click(object sender, EventArgs e) //добавить признак
        {
            if (radioButton1.Checked || radioButton2.Checked)
            {
                if (nameIdent.Text == string.Empty || nameIdent.Text == null) {
                    MessageBox.Show("Заполните название", "Не всё заполнено", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                foreach (TextBox tb in switchables) {
                    tb.Text = tb.Text.Trim(' ').ToLower();
                    if (tb.Enabled == true && (tb.Text == string.Empty || tb.Text == null) && tb.BackColor == DefaultBackColor) {
                        MessageBox.Show("Заполните все открытые поля", "Не всё заполнено", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;//если не все заполнено или нет цветов, принудительно закончить метод
                    }
                }
                if (listBox1.Items.Contains(nameIdent.Text) || listBox1.Items.Contains(nameIdent.Text + " XY")) {
                    MessageBox.Show("Такое имя уже есть", "Сов падение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if ((domGenRes.Text == recGenRes.Text || domGenRes.Text == halfGenRes.Text || recGenRes.Text == halfGenRes.Text) &&
                (domGenRes.BackColor == recGenRes.BackColor || domGenRes.BackColor == halfGenRes.BackColor || recGenRes.BackColor == halfGenRes.BackColor)) {
                    MessageBox.Show("Варианты признаков совпадают", "ачё всмысле", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                sbyte[] initgen = hasHalf.Checked ? new sbyte[2] { 1, 1 } : new sbyte[2] { 2, 2 };
                string[] futArr;
                if (!(domGenRes.ReadOnly && recGenRes.ReadOnly)) {
                    futArr = new string[3] { recGenRes.Text, halfGenRes.Text, domGenRes.Text };
                }
                else { futArr = new string[3] { ColorTranslator.ToHtml(recGenRes.BackColor), ColorTranslator.ToHtml(halfGenRes.BackColor), ColorTranslator.ToHtml(domGenRes.BackColor) }; }

                if (!halfGenRes.Enabled) { futArr[1] = null; }

                if (radioButton1.Checked) { ses.AddFromScratch(ses.templGens, new Mendel(initgen, futArr, nameIdent.Text, hasHalf.Checked), listBox1, futObjs); }
                else if (radioButton2.Checked) {
                    ses.AddFromScratch(ses.templGens, new Chrom(radioButton5.Checked, initgen, futArr, nameIdent.Text, hasHalf.Checked), listBox1, futObjs);
                    editAllSxs(ses.templGens);
                }
            }

            else if (radioButton3.Checked) {
                if (!listBox1.Items.Contains("группа крови")) { ses.AddFromScratch(ses.templGens, new Blood(new sbyte[] { 0, 0 }), listBox1, futObjs); }
                else { MessageBox.Show("Группа крови уже добавлена", "Сов падение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
            }

            else if (radioButton6.Checked) {
                if (!listBox1.Items.Contains("летальный ген")) { ses.AddFromScratch(ses.templGens, new Lethal(0, 0), listBox1, futObjs); }
                else { MessageBox.Show("Летальный ген уже добавлен", "Сов падение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
            }

            else if (radioButton7.Checked) {                
                List<string> links = new List<string>();
                switch (futObjs.SelectedIndices.Count)
                {
                    case 0:
                        MessageBox.Show("сначала что-то выберите, а", "ошиб 0чка вышла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    default:
                        foreach (int i in futObjs.SelectedIndices) { links.Add(ses.templGens[i].name); }
           
                        ses.AddFromScratch(ses.templGens, new Active(new List<sbyte>() { 2, 2 }, links, ses.templGens, recAct.BackColor != DefaultBackColor), listBox1);
                        
                        foreach (string a in ses.takenAims) { futObjs.Items.Remove(a); } //удалить его цели из потенциальных
                        break;
                }    
            }

            radioButton3.Enabled = !listBox1.Items.Contains("группа крови");
            radioButton6.Enabled = !listBox1.Items.Contains("летальный ген");
            radioButton7.Enabled = futObjs.Items.Count > 0;

            clearmend();
            foreach (Button b in new Button[] { button3, button4, button7 }) { b.Enabled = true; }
        }

        void newTreat(int indx) {//это к методу ниже, прост вызывать придется несколько раз       
            ses.xrom += Convert.ToInt32(ses.templGens[indx] is Chrom);
            groupBox1.Visible = ses.xrom != 0;

            ses.AddFromScratch(ses.willBeOnMe, (AGene)ses.templGens[indx].Clone(), listBox2);

            foreach (Active g in ses.willBeOnMe.Where(x => x is Active)) { //если это активатор        
                listBox2.Items[listBox2.Items.IndexOf(g.name)] = g.represent(ses.willBeOnMe);
                g.name = g.represent(ses.willBeOnMe);
            }

            foreach (Button b in new Button[] { button2, button5, button6, button14 }) { b.Enabled = true; }
            button3.Enabled = false;
            button7.Enabled = ses.willBeOnMe.Count != ses.templGens.Count;
        }
        private void button3_Click(object sender, EventArgs e) //Скинуть признак будущей клетке
        {
            try { newTreat(listBox1.SelectedIndex); }
            catch (ArgumentOutOfRangeException)
            {
                if (listBox1.Items.Count == 1) { newTreat(0); }
                else { MessageBox.Show("Сначала выберите что переносить", "Ничего не выбрано", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); }
            }
            editAllSxs(ses.willBeOnMe);
        }
        //-------------добавление клеток или признаков

        //--------------------------Удаление генов---------
        private void button4_Click(object sender, EventArgs e) //удалить признак ВООБЩЕ
        {
            if (listBox1.SelectedIndex == -1)  {                  
                MessageBox.Show("Сначала выберите что удалять", "Ничего не выбрано", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            if (ses.templGens[listBox1.SelectedIndex] is Blood) { radioButton3.Enabled = true; }
            else if (ses.templGens[listBox1.SelectedIndex] is Lethal) { radioButton6.Enabled = true; }

            if (ses.templGens[listBox1.SelectedIndex] is Active) {
                foreach (string a in (ses.templGens[listBox1.SelectedIndex] as Active).aims) {
                    ses.takenAims.Remove(a); //удалить его цели из общих занятых
                    futObjs.Items.Add(a); //вернуть их в возможные
                }
            }
            else { 
                futObjs.Items.Remove(ses.templGens[listBox1.SelectedIndex].name);
                
                listBox2.SelectedIndex = listBox2.Items.IndexOf(listBox1.SelectedItem);       
                button5.PerformClick(); //Влечёт за собой удаление копии гена также из списка на выход

                ses.DeleteFromHist(ses.templGens, listBox1, listBox1.SelectedIndex); //потом сам ген 

                List<AGene> ien = ses.templGens.Where(x => x is Active).ToList(); //потом отсортировать только активаторы
                for (int i = 0; i < ien.Count(); i++) {
                    Active a = ien[i] as Active;
                    if (a.aims.Count == 0) //если ничего не остаётся    
                    {        
                        ses.DeleteFromHist(ses.templGens, listBox1, listBox1.Items.IndexOf(a.name));                 
                        MessageBox.Show($"Переключатель №{ses.templGens.IndexOf(a)} удалён за ненадобностью");    
                    }
                }
            }

            foreach (Button button in new Button[] { button3, button4, button5, button6, button7 }) { button.Enabled = ses.templGens.Count != 0; }
            radioButton7.Enabled = (futObjs.Items.Count > 0);
        }
        private void button5_Click(object sender, EventArgs e) //удалить признак у будущей клетки
        {
            try { if (ses.willBeOnMe[listBox2.SelectedIndex] is Chrom) { ses.xrom--; } }
            catch { }
            groupBox1.Visible = ses.xrom != 0;

            ses.DeleteFromHist(ses.willBeOnMe, listBox2, listBox2.SelectedIndex);

            List<AGene> closestacts = ses.willBeOnMe.Where(x => x is Active).ToList();
            IEnumerable<string> names = from gen in ses.willBeOnMe select gen.name;
            
            for (int i = 0; i < closestacts.Count; i++) {
                if ((closestacts[i] as Active).aims.Intersect(names).Count() == 0)
                {
                    ses.DeleteFromHist(ses.willBeOnMe, listBox2, listBox2.Items.IndexOf(closestacts[i].name));
                    MessageBox.Show($"{closestacts[i].name} убран, так как убраны все его цели \n Но не удалён :]");
                }
                else { (closestacts[i] as Active).RemoveAim_Rename(ses.willBeOnMe, listBox2); }
            }
         
            button7.Enabled = true;
            foreach (Button b in new Button[] { button2, button5, button6 }) { b.Enabled = ses.willBeOnMe.Count > 0; } 
        }
        private void button6_Click(object sender, EventArgs e) // Зачистить геном клетки
        {
            ses.ClearReg(ses.willBeOnMe, listBox2);

            Place();

            foreach (Button b in new Button[] { button2, button5, button6 }) { b.Enabled = false; }
            foreach (Button b in new Button[] { button3, button7 }) { b.Enabled = true; }
            foreach (RadioButton rb in kinds) { rb.Enabled = true; }

            ses.xrom = 0;
            groupBox1.Visible = false;
        }    
        //--------------------------удаление генов---------     
        
        private void button7_Click(object sender, EventArgs e) //скопировать клетке все возможные гены
        {
            ses.ClearReg(ses.willBeOnMe, listBox2);
            ses.willBeOnMe.AddRange(ses.templGens);
            listBox2.Items.AddRange(listBox1.Items);

            foreach (Button b in new Button[] { button2, button5, button6 }) { b.Enabled = true; }
            foreach (Button b in new Button[] { button3, button7 }) { b.Enabled = false; }

            editAllSxs(ses.willBeOnMe);

            foreach (AGene ig in ses.templGens) { ses.xrom += Convert.ToInt32(ig is Chrom); }
            groupBox1.Visible = ses.xrom != 0;
        }

        /*Смена цвета кнопки удаления из общака*/
        private void button4_EnabledChanged(object sender, EventArgs e) { button4.BackColor = button4.Enabled ? Color.Maroon : Color.RosyBrown; }
        private void button5_EnabledChanged(object sender, EventArgs e) //Смена цвета кнопки удаления у будущей клетки
        {
            button5.BackColor = button5.Enabled ? Color.OrangeRed : Color.DarkSalmon;
            button5.ForeColor = button5.Enabled ? DefaultForeColor : Color.White;
        }
        private void listBox1_MouseClick(object sender, MouseEventArgs e)//выбрать ген из общака ИЛИ просто сбросить выбор
        {
            try { button3.Enabled = !listBox2.Items.Contains(listBox1.SelectedItem); }
            catch (ArgumentNullException) { }
            listBox2.SelectedIndex = -1;
            Place();
        }
       
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)//Просмотр генов будущей клетки без возможности изменения
        {
            foreach (RadioButton rb in kinds) { rb.Enabled = listBox2.SelectedIndex == -1; }
            clearmend();

            crtAims.Visible = listBox2.SelectedIndex >= 0;
            futObjs.Visible = listBox2.SelectedIndex == -1;
            act.Text = listBox2.SelectedIndex >= 0 ? "Задействованые гены" : "Выберите ген(ы)";

            hasHalf.Enabled = listBox2.SelectedIndex == -1;
            if (listBox2.SelectedIndex != -1)
            {
                foreach (Button gb in gamPut.Controls.OfType<Button>()) { gb.BackColor = Button.DefaultBackColor; }

                if (ses.willBeOnMe[listBox2.SelectedIndex] is Mendel || ses.willBeOnMe[listBox2.SelectedIndex] is Chrom || ses.willBeOnMe[listBox2.SelectedIndex] is Active)
                {
                    nameIdent.Text = ses.willBeOnMe[listBox2.SelectedIndex].name;
                    for (int i = 0; i <= 2; i++) { switchables[i].Text = ses.willBeOnMe[listBox2.SelectedIndex].ress[i]; }

                    hasHalf.Checked = ses.willBeOnMe[listBox2.SelectedIndex].ress[1] != null;
                    foreach(Button b in doms) {
                        b.Text = ses.willBeOnMe[listBox2.SelectedIndex].ress[1] != null? "Ā" : "A";
                    }
                    foreach (Button b in end) { b.Enabled = true; }
                    foreach (Label lab in new Label[] { x1st, xOrY }) { lab.Visible = ses.willBeOnMe[listBox2.SelectedIndex] is Chrom; }

                    
                    try {
                        domGenRes.BackColor = ColorTranslator.FromHtml(ses.willBeOnMe[listBox2.SelectedIndex].ress[2]);
                        recGenRes.BackColor = ColorTranslator.FromHtml(ses.willBeOnMe[listBox2.SelectedIndex].ress[0]);
                        foreach (TextBox tb in switchables) { tb.Text = null; }
                    }
                    catch { }

                    if (ses.willBeOnMe[listBox2.SelectedIndex] is Mendel) { radioButton1.Checked = true; }
                    else if (ses.willBeOnMe[listBox2.SelectedIndex] is Chrom) {
                        radioButton2.Checked = true;
                        foreach (Button b in end) { b.Enabled = radioButton5.Checked; }
                    }
                    else if (ses.willBeOnMe[listBox2.SelectedIndex] is Active) {
                        radioButton7.Checked = true;
                        crtAims.Items.Clear();
                        foreach(string i in (ses.willBeOnMe[listBox2.SelectedIndex] as Active).aims) 
                        {                          
                            if(listBox2.Items.Contains(i) || listBox2.Items.Contains(i + " XY")) { crtAims.Items.Add(i); }                          
                        }
                    }

                        begn[Convert.ToInt32(ses.willBeOnMe[listBox2.SelectedIndex].zig[0] > 0)].BackColor = ofSelected;
                    try { end[Convert.ToInt32(ses.willBeOnMe[listBox2.SelectedIndex].zig[1] > 0)].BackColor = ofSelected; }
                    catch (ArgumentOutOfRangeException) { }
                }
                radioButton3.Checked = ses.willBeOnMe[listBox2.SelectedIndex] is Blood;
                radioButton6.Checked = ses.willBeOnMe[listBox2.SelectedIndex] is Lethal;
            }
            Place();

            button14.Enabled = listBox2.SelectedIndex != -1;
        }

        //----------------------------- Изменение пола --------------------
        private void radioButton4_CheckedChanged(object sender, EventArgs e) { //включить/выключить половину гамет в связи с полом клетки
            foreach (Button b in end) {
                if (ses.willBeOnMe[listBox2.SelectedIndex] is Chrom) { 
                     b.Enabled = radioButton5.Checked; b.BackColor = DefaultBackColor; 
                }                      
            }
            editAllSxs(ses.willBeOnMe);
            editAllSxs(ses.templGens);
        }
        private void radioButton5_CheckedChanged(object sender, EventArgs e) { if (radioButton5.Checked) { big2nd.BackColor = ofSelected; } }
        void editAllSxs(List<AGene> list)  
        { //поменять пол всем хромосомным генам 
            foreach (AGene g in list.Where(x => x is Chrom)) {
                if (radioButton4.Checked) {
                    try { (g as Chrom).zig.RemoveAt(1); }
                    catch (ArgumentOutOfRangeException) { }
                }
                else {
                    if ((g as Chrom).zig.Count == 1) { (g as Chrom).zig.Add(2); }
                }
            }
            xOrY.Text = radioButton4.Checked ? "Y" : "X2";
        }  
        
        //-----------------------------изменение пола --------------------
        private void button14_Click(object sender, EventArgs e) { Cell.MakeGenom(ses.willBeOnMe, -1); }
       
        Pen ppap = new Pen(Color.White, 2);
        Pen dash = new Pen(Color.White, 2) { DashPattern = new float[2] { 4, 4 } };
        void DrawRelation() {
            Pen locPen;
            foreach (Cell p in ses.allCells) { //для вообще каждой клетки        
                foreach (int el in p.parents) {//проходимся по родителям         
                    if (p.parents.Count == 2) {//если оба на месте                   
                        locPen = p.alive ? ppap : dash; //сама клетка живая или нет?
                        ge.DrawLine(locPen, new Point(panel1.Controls[el].Location.X + 22, panel1.Controls[el].Bottom), new Point(panel1.Controls[p.position].Location.X + 22, panel1.Controls[p.position].Location.Y));
                    }
                }
            }
        }

        //---------------------------------сохраняем сессию--------
        private void button8_Click(object sender, EventArgs e) { saveFileDialog1.ShowDialog(); }//просто открываем модальное окно
        private async void saveFileDialog1_FileOk(object sender, CancelEventArgs e) { 
            await Task.Run(() => ses.serialize(saveFileDialog1));
            this.Text = saveFileDialog1.FileName + " - Симулятор генетики";
        }
        private void button9_Click(object sender, EventArgs e) { openFileDialog1.ShowDialog(); }
        private async void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            ses = await Task<Session>.Run(() => ses.deserialize(openFileDialog1));

            ses.registerNew(ses.templGens, listBox1, futObjs);
            ses.registerNew(ses.willBeOnMe, listBox2, futObjs);

            foreach(Button b in new Button[] { button2, button4, button5, button6}) { b.Enabled = ses.willBeOnMe.Count > 0; }
            this.Text = openFileDialog1.FileName + " - Симулятор генетики";

            ge.Clear(panel1.BackColor);
            ses.makeBodies(panel1);
            DrawRelation();
        }
    }
} 