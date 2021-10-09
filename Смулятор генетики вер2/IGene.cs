using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Симулятор_генетики_вер2
{
    /// <summary>
    /// Абстракция для гена
    /// </summary>
    public abstract class AGene 
    {
        public string name { get; set; }
        public List<sbyte> zig { get; set; }
        public string[] ress { get; set; }
        public Mutations mut { get; set; }
        public bool[] lethalities { get; set; }

        public bool codom { get; set; }//Есть ли кодоминирование
        public bool active = true;

        public abstract override string ToString();
        
        public abstract object Clone();

        public enum Mutations { None, Silence, Missense }
    }
}
