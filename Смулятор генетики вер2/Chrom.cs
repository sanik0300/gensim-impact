using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Симулятор_генетики_вер2
{
    /// <summary>
    /// Представляет половую хромосому
    /// </summary>
    class Chrom : AGene, ICloneable
    {
        public Chrom() { }

        public Chrom(bool isF, sbyte[] aa, string[] isx, string n, bool c)
        {
            this.zig = isF ? new List<sbyte>() { aa[0], aa[1] } : new List<sbyte>() { aa[0] };
            this.ress = new string[3] { isx[0], isx[1], isx[2] };
            this.name = n;
            this.codom = c;
        }

        public override object Clone()
        {
            return new Chrom(this.zig.Count == 2, this.zig.ToArray(), this.ress, this.name, this.codom);
        }

        public override string ToString()
        {
            string str = string.Empty;
            if (this.ress != null)
            {
                str = this.zig.Count == 1 ? "м " : "ж ";
                int sum = this.zig.Count == 2 ? this.zig[0] + this.zig[1] : this.zig[0];

                if (sum <= 1) { str = this.ress[sum]; }
                else { str = this.ress[2]; }
            }
            return str;
        }
    }
}
