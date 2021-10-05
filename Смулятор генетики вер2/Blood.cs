using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Симулятор_генетики_вер2
{
    class Blood : AGene
    {

        public Blood() { }   
        public Blood(sbyte[] grup) {
            this.name = "группа крови";
            this.zig = new List<sbyte>() { grup[0], grup[1] };
            this.zig.Capacity = 2;
        }
        
        public override string ToString()
        {
            string str = string.Empty;
            switch (this.zig[0] + this.zig[1])
            { 
                case 0:
                    str = "1я";
                    break;
                case 1:
                    str = "2я";
                    break;
                case 2:
                    if (this.zig[0] == this.zig[1])
                    {
                        str = "1я";
                    }
                    else {
                        str = "2я";
                    }
                    break;
                case 3:
                    str = "4я";
                    break;
                case 4:
                    str = "3я";
                    break;            
            }           
            return str;          
        }

        public override object Clone() {
            return new Blood(this.zig.ToArray());
        }
    }
}
