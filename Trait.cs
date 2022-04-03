using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;

namespace Симулятор_генетики_4
{
    public class Trait : ICloneable
    {
        public enum Mutations { Missence, Silence }
        public enum AlertStates { None, HasAlert, IsAlert, HasLethal, HasBoth, IsAlertWithLethal }//IsDeadWithAlert - это за счёт hasalert`a и всегда чёрного цвета дохлых особей
        public string Name { get; set; }

        public string Result { get; set; }
        public Mutations? Mutation { get; set; }
        public bool active { get; set; }
        public byte?[] allels { get; set; }
        /// <summary>
        /// внимание! это позиция в генфонде того гена, случаем которого есть данный признак
        /// </summary>
        public int Index { get; set; }

        private static char[] letters = new char[3] { '0', 'A', 'B' };
        private static char[] sxs = new char[2] { 'X', 'Y' };

        [JsonIgnore]
        public string ToLetters
        {
            get
            {
                StringBuilder result = new StringBuilder();
                Gene crt = Population.current.genofond[this.Index];
                for (int i = 0; i < 2; i++)
                {
                    if (crt.SxRelated)
                        result.Append(sxs[i]);

                    if (this.allels[i] == null)
                        break;

                    if (crt is Quaternal)
                        result.Append(letters[(int)allels[i]]);
                    else
                        result.Append(allels[i] > 0 ? 'A' : 'a');
                }
                return result.ToString();
            }
        }
        public Trait() { this.active = true; Mutation = null; }
        protected Trait(Trait other)
        {
            this.Index = other.Index;
            this.Name = other.Name;
            this.Result = other.Result;
            this.Mutation = other.Mutation;
            this.active = other.active;
            this.allels = new byte?[2] { other.allels[0], other.allels[1] };
        }
        public object Clone() { return this is NumericTrait ? new NumericTrait(this) : new Trait(this); }

        public byte DefiningAllel()
        {
            byte max = this.allels[0].Value;
            if (this.allels[1] != null && this.allels[1].Value > max)
                max = this.allels[1].Value;
            return max;
        }

        public virtual void CalculateResultingFentype(object sender, RoutedEventArgs e)
        {
            if (active == false)
                return;
            if (this is NumericTrait) {
                (this as NumericTrait).CalculateResultingFentype(sender, e);
                return;
            };

            byte sum;
            Gene gen = Population.current.genofond[this.Index];
            if (gen is Binary && this.allels[0] == 1 && this.allels[1] != 0)//АА с кодоминированием
                sum = 2;
            else if (gen is Quaternal && this.allels[0] + this.allels[1] == 3)//АВ
                sum = 3;
            else
                sum = this.DefiningAllel();

            Result = gen.traits[sum];
        }

        private byte FuckupAllel(bool quad, bool codom, Random rnd)
        {
            if (quad)
                return Convert.ToByte(rnd.Next(1, 3));

            int result = rnd.Next(0, 2);
            if (!codom)
                result = result * 2;
            return (byte)result;
        }
        public void Mutate()
        {
            Random randomizer = new Random();
            int seed = randomizer.Next(0, 100);
            if (Population.current.genofond[this.Index].MutProb == 0 || seed > Population.current.genofond[this.Index].MutProb)
                return;

            int prevSum = allels[0].Value + Convert.ToInt32(allels[1]);
            bool prevEqual = allels[1] == null || allels[0] == allels[1];

            bool quad, kodom = false;
            Binary bin = Population.current.genofond[this.Index] as Binary;
            quad = bin == null;
            if (bin != null)
                kodom = bin.kodom;

            if (randomizer.Next(0, 2) == 0 || allels[1] == null) //выбираем какую цифру в массиве испортить сначала
            {
                allels[0] = FuckupAllel(quad, kodom, randomizer);
            }
            else
            {
                allels[1] = FuckupAllel(quad, kodom, randomizer);
                if (randomizer.Next(1, 3) == 2)//а портить ли ещё одну?
                    allels[0] = FuckupAllel(quad, kodom, randomizer);
            }
            this.Mutation = (prevSum == allels[0] + Convert.ToByte(allels[1])) && prevEqual == (allels[1].HasValue || allels[0] == allels[1]) ? Mutations.Silence : Mutations.Missence;
        }
        public Trait(Gene gene)
        {
            this.Index = gene.index;
            this.Name = gene.Name;
            this.active = true;
            byte good_allel = 0;
            byte delta = gene is Quaternal || (gene is Binary && (gene as Binary).kodom)? (byte)1 : (byte)2;
            while (true)
            {
                if (gene is Activer && gene.traits[good_allel] == Activer.commandToENA)
                {
                    good_allel+= delta; continue;
                }                    
                if (gene.lethal != null && gene.lethal.border == good_allel)
                {
                    good_allel+=delta; continue;
                }
                if(gene.AlertAllels != null)
                {
                    foreach (int al in gene.AlertAllels)
                    {
                        if (al == good_allel)
                            good_allel+=delta; break;
                    }
                }
                break;
            }
            this.allels = new byte?[2] { good_allel, good_allel };
            CalculateResultingFentype(null, null);
        }

        public Trait(Trait t1, Trait t2, bool homo)
        {
            if (t1.Index != t2.Index)
                throw new Exception("Генотипы не совпадают");
            this.active = true;
            this.Index = t1.Index;
            this.allels = new byte?[2] { null, null };
            Trait[] parental = new Trait[2] { t1, t2 };
            this.Name = t1.Name;
            for (int i = 0; i < 2; i++)
            {
                if (i == 1 && Population.current.genofond[t1.Index].SxRelated && !homo)
                    return;
                
                Random randomizer = new Random();
                Binary asbin = Population.current.genofond[t1.Index] as Binary;
                if (asbin != null && parental[i].allels[0] != parental[i].allels[1] && parental[i].allels[i] != null)
                {
                    int seed = randomizer.Next(0, 101);
                    this.allels[i] = (byte?)(seed <= asbin.Priority ? 1 + Convert.ToByte(!asbin.kodom) : 0);
                }
                else
                {
                    int wher = randomizer.Next(0, 2);
                    this.allels[i] = parental[i].allels[wher] == null ? parental[i].allels[1 - wher] : parental[i].allels[wher];
                }
            }
        }
        public AlertStates HowToMark()
        {
            bool has_red = false, has_black = false,
            fully_red = false;
            Gene subj = Population.current.genofond[this.Index];

            byte dAL_clamped = this.DefiningAllel();
            foreach (byte? ga in this.allels)
            {
                if (ga == null)
                    continue;
                if (subj.lethal != null && ga == subj.lethal.border)
                {
                    has_black = true;                
                    continue;
                }
                if (subj.AlertAllels == null)
                    continue;
                foreach (int al in subj.AlertAllels)
                {
                    has_red = has_red || ga == al;
                    fully_red = fully_red || al == dAL_clamped;
                }                                
            }
            if (has_red)
            {
                if (fully_red)
                    return has_black ? AlertStates.IsAlertWithLethal : AlertStates.IsAlert;
                else
                    return has_black ? AlertStates.HasBoth : AlertStates.HasAlert;
            }
            if (has_black)
                return AlertStates.HasLethal;

            return AlertStates.None;
        }
    }
    public class NumericTrait : Trait
    {
        public float val { get; set; }
        public NumericTrait(Trait other) : base(other)
        {
            this.val = (other as NumericTrait).val;
        }
        public NumericTrait(Trait t1, Trait t2, bool homo=false) : base(t1, t2, homo){ }
        public NumericTrait(Gene g) : base(g) { }
        public override void CalculateResultingFentype(object sender, RoutedEventArgs e)
        {
            Raspred owner = Population.current.genofond[this.Index] as Raspred;
            double seed;
            int indx;
            Random randomizer = new Random();
            double rand = randomizer.NextDouble();
            if (!owner.reversed == (allels[0] + allels[1] == 0)) {
                seed = rand * owner.PS[owner.border];
                indx = owner.binarySearch(seed, 0, owner.border);
            }
            else{
                seed = owner.PS[owner.border] + rand * (1 - owner.PS[owner.border]);
                indx = owner.binarySearch(seed, owner.border, owner.PS.Length-1);
            }
            val = owner.min + (float)(owner.delta * (indx + randomizer.NextDouble()));
            if (val > owner.max)
                val = owner.max;
            Result = $"{Math.Round(this.val, owner.prec)} {owner.measure}";
        }
    }
}
