using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Симулятор_генетики_4.ui
{
    interface IReactingUi
    {
        public TextBox[] textboxes { get; }
        public Gene CreateNew();

        public void CheckFilling();

        public void Clear();
    }
}
