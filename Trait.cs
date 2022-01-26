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
        public  enum Mutations { Missence, Silence}
        public string Name { get; set; }
        protected string result;
        public string Result { get { return result; } set { result = value; } }
        public Mutations? Mutation { get { return mutation; } set { mutation = value; } }
        private Mutations? mutation = null;
        public  bool active { get; set; }
        public byte?[] allels { get; set; }
        public int Index { get; set; }
        
        private  static string[] letters = new string[3] {"0", "A", "B" };
        
        [JsonIgnore]
        public string ToLetters
        {
            get
            {
                StringBuilder result = new StringBuilder();
                foreach(byte? b in allels)
                {
                    if (b == null)
                        break;
                    if (Population.current.genofond[this.Index] is Quaternal)
                        result.Append(letters[(int)b]);
                    else
                        result.Append(b > 0 ? 'A' : 'a');
                }
                return result.ToString();
            }
        }
        public Trait() { this.active = true; }
        protected Trait(Trait other)
        {
            this.Index = other.Index;
            this.Name = other.Name;
            this.result = other.Result;
            this.mutation = other.mutation;
            this.active = other.active;
            this.allels = new byte?[2] { other.allels[0], other.allels[1] };
        }
        public object Clone() { return this is NumericTrait? new NumericTrait(this) : new Trait(this); }

        public virtual void CalculateResultingFentype(object sender, RoutedEventArgs e)
        {
            if (active == false)
                return;
            if (this is NumericTrait)
            {
                (this as NumericTrait).CalculateResultingFentype(sender, e);
                return;
            };
                
            //а иначе не новый пустой массив, а генотип свежесоздаваемой клетки - Population.CurrentKid
            //которая не является nullом только во время скрещивания
            int? indx = allels[1] != null ? allels[0] + allels[1] : allels[0];

            if (indx > 2 ||(indx==1 && allels[1]==null)) //Больше 2 аллелей не бывает... ну пока что
                indx = 2;

            result = Population.current.genofond[this.Index].traits[Convert.ToInt32(indx)];
        }

        static Random randomizer = new Random();
        private byte FuckupAllel(bool quad, bool codom)
        {
            if (quad)
                return Convert.ToByte(randomizer.Next(1, 3));

            int result = randomizer.Next(0, 2);
            if (!codom)
                result = result * 2;
            return (byte)result;
        }
        public void Mutate()
        {
            int seed = randomizer.Next(0, 100);
            if(Population.current.genofond[this.Index].MutProb==0 || seed > Population.current.genofond[this.Index].MutProb)
                return;

            int prevSum = Convert.ToInt32(allels[0] + Convert.ToByte(allels[1]));
            bool prevEqual = allels[0] == allels[1];

            bool quad, kodom = false;
            Binary bin = Population.current.genofond[this.Index] as Binary;
            quad = bin == null;
            if (bin != null)
                kodom = bin.kodom;

            if(randomizer.Next(0,2)==0 || allels[1]==null) //выбираем какую цифру в массиве испортить сначала
            {
                allels[0] = FuckupAllel(quad, kodom);
            }
            else
            {
                allels[1] = FuckupAllel(quad, kodom);
                if (randomizer.Next(1, 3) == 2)//а портить ли ещё одну?
                    allels[0] = FuckupAllel(quad, kodom);
            }
            this.mutation = (prevSum == Convert.ToInt32(allels[0] + Convert.ToByte(allels[1]))) && prevEqual == (allels[0] == allels[1]) ? Mutations.Silence : Mutations.Missence;
        }

        public Trait(int indx, string n, byte?[] als)
        {
            this.Index = indx;
            this.allels = new byte?[2] { als[0], als[1] };
            this.Name = n;
            this.active = true;
            if (Population.current.genofond[indx].traits != null)
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
            for(int i = 0; i<2; i++)
            {
                if(i==1 && Population.current.genofond[t1.Index].SxRelated && !homo)
                    return;
                
                Binary asbin = Population.current.genofond[t1.Index] as Binary;
                if (asbin !=null && parental[i].allels[0] != parental[i].allels[1] && parental[i].allels[i]!=null)
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
    }
    public class NumericTrait : Trait
    {
        public float val { get; set; }
        static private Random randomizer = new Random();
        public NumericTrait(Trait other) : base(other)
        {
            this.val = (other as NumericTrait).val;
        }
        public NumericTrait(Trait t1, Trait t2, bool homo=false) : base(t1, t2, homo){ }
        public NumericTrait(int indx, string n, byte?[] als) : base(indx, n, als){ CalculateResultingFentype(null, null); }
        public override void CalculateResultingFentype(object sender, RoutedEventArgs e)
        {
            Raspred owner = Population.current.genofond[this.Index] as Raspred;
            double seed;
            int indx;
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
            result = $"{Math.Round(this.val, owner.prec)} {owner.measure}";
        }
    }
}
