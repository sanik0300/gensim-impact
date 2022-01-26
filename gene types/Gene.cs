
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
        protected string name;
        public string Name { get { return name; }  set { name = value; } }
        public string[] traits { get; set; }
        /// <summary>
        /// вероятность мутации
        /// </summary>
        protected int mutation_prob;
        public int MutProb { get { return mutation_prob; } set { mutation_prob = value; } }
        /// <summary>
        /// для сортировки по времени добавления
        /// </summary>
        public int index { get; set; }
        private string optname;
        public string OptName { 
            get { return optname;} 
            set { optname = value == string.Empty ? null : value; } 
        }

        public LethalComponent lethal { get; set; }
        public Gene() {}

        
        [JsonIgnore] public bool SxRelated
        {
            get { return this as Binary != null && (this as Binary).sceplen; }
        }
        protected Gene(string n, string[] trs)
        {
            name = n;
            traits = trs;
            index = Population.current.genofond.Count;
            Population.current.taken_aims.Add(false);
        }
        public override string ToString()
        {
            return SxRelated? this.name+" XY" : this.name;
        }
        public virtual void PreRemoveNaxer() { }
    } 
}
