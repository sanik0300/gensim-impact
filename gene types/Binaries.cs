
using System.Collections.Generic;
using System.Text;

namespace Симулятор_генетики_4
{
    public class Binary : Gene
    {
        public Binary() { }
        /// <summary>
        /// Вероятность выпадения доминантного аллеля
        /// </summary>
        private int priority { get; set; }
        /// <summary>
        /// Сцепленность с полом, да, класс кхром - не слышали
        /// </summary>
        public bool sceplen{ get; set; }
        public int Priority { get { return priority; } set { priority = value; } }
        /// <summary>
        /// Кодоминирование
        /// </summary>
        public bool kodom{ get; set; }
        public Binary(string n, string[] trs, bool scep = false, bool codom=false) : base(n, trs)
        {
            this.sceplen = scep;
            if(scep)
                Population.current.AllChroms++;
            this.kodom = codom;
        }
        public override void PreRemoveNaxer()
        {
            if (this.sceplen)
                Population.current.AllChroms--;
        }
    }

    public class Activer : Binary
    {
        public Activer() { }
        int[] aims;
        public int[] Aims { get { return aims; } set { aims = value; } }
        static public string  commandToENA = "не будет активным";
        public Activer(string n, string[] trs, int[] ams): base(n, trs)
        {
            this.aims = ams;
            int saved, j;
            for (int i = 0; i < aims.Length-1; i++)
            {
                if (aims[i] < aims[i + 1])
                        continue;
                saved = aims[i + 1];

                for (j = i; j >=0; j--)
                {
                    aims[j + 1] = aims[j]; 
                    if (j>0 && aims[j-1] < saved)
                    {
                        aims[j] = saved;
                        break;
                    }
                }
                if (j <= 0)
                    aims[0] = saved;
            }
            foreach (int a in aims)
                Population.current.taken_aims[a] = true;

            if (string.IsNullOrEmpty(n))
            {
                StringBuilder sb = new StringBuilder("Активность ", Population.current.genofond.Count * 3 + 11);
                for (int i = 0; i < aims.Length;)
                {
                    sb.Append(aims[i].ToString());
                    i++;
                    if (i == aims.Length)
                        break;
                    sb.Append(", ");
                }
                this.name = sb.ToString();
            }
            else { this.name = n; }
        }

        public void deActivate(ref Trait copy_ofthis, IEnumerable<Trait> trs)
        {
            int aj = 0;
            foreach (Trait tr in trs)
            {
                if (aj >= this.aims.Length)
                    break;

                if (tr.Index == this.aims[aj])
                {
                    if (copy_ofthis.Result == commandToENA)
                    {
                        tr.active = false;
                        tr.Result = Population.current.genofond[tr.Index].OptName;
                        if (tr.Result != null)
                            continue;
                        tr.Result = "N/A";
                    }
                    else
                    {
                        tr.active = true;
                        if (tr.Result == "N/A" || tr.Result == Population.current.genofond[tr.Index].OptName)
                            tr.CalculateResultingFentype(null, null);
                        else
                            return;
                    }
                    aj++;
                }            
            }
        }
        public override void PreRemoveNaxer()
        {
            foreach (int i in this.aims)
                Population.current.taken_aims[i] = false;
        }
    }
}
