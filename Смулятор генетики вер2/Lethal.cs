using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Симулятор_генетики_вер2
{
    class Lethal : AGene
    {
        public static int probability = 160;

        public Lethal(sbyte first, sbyte second) {
            this.name = "летальный ген";
            this.ress = new string[3] { "бобик сдох", null, "оно живое" };
            this.zig = new List<sbyte>() { first, second }; 
        }

        public override object Clone() {
            return new Lethal(this.zig[0], this.zig[1]);
        }
        public override string ToString()
        {
            return ress[Convert.ToInt32(this.zig[0] + this.zig[1] < probability) * 2];
        }
    }
}
