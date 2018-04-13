using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjetNET
{
    public partial class AddSubFamily : Form
    {
        public AddSubFamily()
        {
            InitializeComponent();
            Construct(null);
        }
        public AddSubFamily(SubFamily SubFamily)
        {
            InitializeComponent();
            Construct(SubFamily);
        }

        private void Construct(SubFamily SubFamily)
        {
            StartPosition = FormStartPosition.CenterParent;
            this.DialogResult = DialogResult.Cancel;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
        }
    }
}
