using System;
using System.Windows.Media;

namespace Симулятор_генетики_4
{
    public class LethalComponent 
    {     
        public float border { get; set; }
        public int probability { get; set; }

        public bool ReallyKills(Trait tr)
        {
            if(Population.current.genofond[tr.Index] is Raspred)
            {
                NumericTrait nt = tr as NumericTrait;
                return nt.val - (Population.current.genofond[nt.Index] as Raspred).min <= border || nt.val >= (Population.current.genofond[tr.Index] as Raspred).max - border;
            }
            else
            {   
                //на случай варианта АВ
                if (Population.current.genofond[tr.Index] is Quaternal && tr.allels[0] + tr.allels[1] == 3)
                    return this.border == 1 || this.border == 2;
                return tr.DefiningAllel() == this.border;
            }
        }

        public override string ToString()
        {
            return $"{border}/{probability}";
        }

        public LethalComponent(string s)
        {
            string[] pair = s.Split('/');
            border = Convert.ToSingle(pair[0]);
            probability = Convert.ToInt32(pair[1]);
        }

        public LethalComponent(float bdr, int prob)
        {
            this.border = bdr;
            probability = prob;
        }
    }
}
