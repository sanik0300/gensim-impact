using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Симулятор_генетики_вер2
{
    class Active : AGene 
    {
        public Active() { }

        public Active(List<sbyte> alls, List<int> indxs, List<AGene> home, bool inv=false) {
            zig = alls;
            
            codom = false;
            aims = indxs;
            Session.Current.takenAims.AddRange(indxs);

            ress = new string[3] {"Не активен(ны)", null, "активен(ны)" };
            this.inverted = inv;
            if(inv) { ress = ress.Reverse().ToArray(); }

            name = "Активность: [";
            foreach(int i in this.aims)
            {
                name = name + Cell.na_vixod.dnk[i].name + " ";
            }
            name = name + "]";
        }


        public List<int> aims = new List<int>();
        public bool inverted = false;
        
        public override object Clone()
        {
            Active a = new Active() { zig = this.zig, name = this.name, ress = this.ress };
            a.aims = new List<int>(this.aims);
            return a;
        }

        public override string ToString() { return ress[Convert.ToInt32(zig[0] + zig[1] > 0) * 2]; }

        /*public void RemoveAim(List<T> list, ListBox repres, int constIndx=-1) {

            if (constIndx != -1) { this.aims.Remove(Cell.na_vixod.dnk[constIndx].name); }
           
            repres.Items[list.IndexOf(this)] = this.name;
        }*/
    }
}
