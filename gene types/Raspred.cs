using System;

namespace Симулятор_генетики_4
{
    /// <summary>
    /// йой най, да это же количественные признаки)
    /// </summary>
    class Raspred : Binary
    {
        /// <summary>
        /// друг человека в расчётах :D
        /// </summary>
        static private Random random = new Random();
        public const int num_of_delts = 50;
        /// <summary>
        /// вероятности 
        /// </summary>
        public double[] probabilities {get; set;}
        /// <summary>
        /// суммы вероятностей
        /// </summary>
        public double[] PS { get; set; }
        /// <summary>
        /// шаг признака (минимальный)
        /// </summary>
        public float delta { get; set; }
        public float min { get; set; }
        public float max { get; set; }
        public int border { get; set; }
        public bool reversed { get; set; }
        public float sigma { get; set; }
        public int prec { get; set; }
        public string measure { get; set; }
        /// <summary>
        /// Создание количественника, штош
        /// </summary>
        /// <param name="n">Название</param>
        /// <param name="probs">массив с вероятностями</param>
        /// <param name="mi">минимум</param>
        /// <param name="ma">максимум (чисто для вычисления дельты)</param>
        /// <param name="s">дисперсия</param>
        /// <param name="bdr">граница распределения аллелей</param>
        /// <param name="rev">задом наперёд аллели или нє</param>
        /// <param name="sel">единица измерения</param>
        /// <param name="prc">знаки после запятой</param>
        public Raspred(string n, double[] probs, float mi, float ma, float s, int bdr, bool rev, string sel, int prc, string[] trs=null) : base(n, trs) {
            this.reversed = rev;
            this.min = mi;
            this.max = ma;
            this.delta = (ma - mi) / num_of_delts;
            this.sigma = s;
            this.border = bdr;
            this.measure = sel;
            this.prec = prc;
            PS = new double[num_of_delts+2];
            int imax = num_of_delts / 2;
            for(int i = 1; i<=imax; i++)
            {
                probs[i + imax] = probs[imax - i];
            }
            for (int i = 1; i <= num_of_delts+1; i++)
            {
                PS[i] = probs[i-1] + PS[i - 1];
            }
            this.probabilities = new double[num_of_delts / 2+1];
            for(int i = 0; i<probabilities.Length; i++)
            {
                probabilities[i] = probs[i];
            }
        }
        public Raspred() { }
        public int binarySearch(double what, int l, int h)
        {
            int lo = l, hi = h, mid;
            while (hi - lo > 1)
            {
                mid = (lo + hi) / 2;
                if (what == PS[mid])
                    return mid;
                if (what == PS[lo])
                    return lo;
                if (what == PS[hi])
                    return hi;
                if (what > PS[mid])
                    lo = mid;
                else
                    hi = mid;
            }
            return lo;

        }
    }
}
