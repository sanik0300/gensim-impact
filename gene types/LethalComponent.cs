using System;

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
            else if(Population.current.genofond[tr.Index] is Binary)
            {
                return (tr.allels[1]==null? tr.allels[0] : tr.allels[0] + tr.allels[1]) > 0 == this.border > 0;
            }
            else
            {
                if (tr.allels[0] == tr.allels[1])
                    return tr.allels[0] == this.border;
                else
                    return (tr.allels[1] == null ? tr.allels[0] : tr.allels[0] + tr.allels[1]) >= this.border;
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
