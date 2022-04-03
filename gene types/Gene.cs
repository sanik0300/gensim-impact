
using System.Text.Json.Serialization;
using System.Windows;

namespace Симулятор_генетики_4
{
    public class GenPassingArgs : RoutedEventArgs
    {
        public Gene link;
        public GenPassingArgs(Gene el) { this.link = el; }
    }
    [JsonConverter(typeof(DifferentGenesConverter))]
    public abstract class Gene
    {   
        /// <summary>
        /// название гена
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// набор проявлений
        /// </summary>
        public string[] traits { get; set; }
        /// <summary>
        /// вероятность мутации
        /// </summary>
        public int MutProb { get; set ; }
        /// <summary>
        /// для сортировки по времени добавления
        /// </summary>
        public int index { get; set; }
        
        private string optname;
        /// <summary>
        /// проявление на случай неактивности
        /// </summary>
        public string OptName { 
            get { return optname;} 
            set { optname = value == string.Empty ? null : value; } 
        }
        /// <summary>
        /// возможность убить всю особь
        /// </summary>
        public LethalComponent lethal { get; set; }
        
        /// <summary>
        /// связан ли с полом
        /// </summary>
        [JsonIgnore] public bool SxRelated
        {
            get { return this as Binary != null && (this as Binary).sceplen; }
        }
        /// <summary>
        /// 1 или 2 опасных аллеля, помечаемые красным
        /// </summary>
        public int[]? AlertAllels { get; set; }

        public Gene() { }
        protected Gene(string n, string[] trs, int mp)
        {
            Name = n;
            traits = trs;
            MutProb = mp;
            AlertAllels = null;
            index = Population.current.genofond.Count;
            Population.current.taken_aims.Add(false);
        }
        public override string ToString()
        {
            return SxRelated? this.Name+" XY" : this.Name;
        }
        public virtual void PreRemoveNaxer() { }
    } 
}
