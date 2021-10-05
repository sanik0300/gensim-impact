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

        public Active(List<sbyte> zs, List<string> indxs, List<AGene> home, bool inv=false) {
            zig = zs;
            
            codom = false;
            aims = new List<string>(indxs);
            Session.Current.takenAims.AddRange(indxs);

            ress = new string[3] {"Не активен(ны)", null, "активен(ны)" };
            this.inverted = inv;
            if(inv) { ress = ress.Reverse().ToArray(); }

            name = this.represent(home);
        }


        public List<string> aims = new List<string>();
        public bool inverted = false;
        
        public override object Clone()
        {
            Active a = new Active() { zig = this.zig, name = this.name, ress = this.ress };
            a.aims = new List<string>(this.aims);
            return a;
        }

        public override string ToString() { return ress[Convert.ToInt32(zig[0] + zig[1] > 0) * 2]; }

        public string represent(List<AGene> list) {

            string result = string.Empty;
            foreach (string a in aims) {
                int num = list.FindLastIndex(x => x.name == a);
                if(num >=0) { result = result + num.ToString();}
                
            }
            result.Trim(',');
            result = result + " переключатель";
            return result;
        }

        public void RemoveAim_Rename(List<AGene> list, ListBox repres, int constIndx=-1) {

            if (constIndx != -1) { this.aims.Remove(Session.Current.templGens[constIndx].name); }
           
            this.name = this.represent(list);
            repres.Items[list.IndexOf(this)] = this.name;
        }
    }
}
