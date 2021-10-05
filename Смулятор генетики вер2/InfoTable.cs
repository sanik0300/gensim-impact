using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Симулятор_генетики_вер2
{
    public partial class InfoForm : Form
    {
        public DataGridView dgv1 = new DataGridView();

        public InfoForm()
        {
            InitializeComponent();
            dgv1.Size = new Size(330, 208);
            dgv1.Location = new Point(13, 13);
            this.Controls.Add(dgv1);
        }
    }
}
