using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Симулятор_генетики_вер2
{
    /// <summary>
    /// Представляет менделевский признак
    /// </summary>
    class Mendel : AGene, ICloneable
    {
        public Mendel() { }

        public Mendel(sbyte[] aa, string[] posibl, string n, bool c)
        {
            this.zig = new List<sbyte>() { aa[0], aa[1] };
            this.zig.Capacity = 2;
            this.ress = new string[3] { posibl[0], posibl[1], posibl[2] };
            this.name = n;
            this.codom = c;
        }

        public override string ToString()
        {
            string str = string.Empty;

            if (this.ress != null)
            {
                if (this.zig[0] + this.zig[1] <= 1)
                {
                    str = this.ress[this.zig[0] + this.zig[1]];
                }
                else { str = this.ress[2]; }
            }
            return str;
        }

        public override object Clone()
        {
            return new Mendel(this.zig.ToArray(), this.ress, this.name, this.codom);
        }
    }
}

