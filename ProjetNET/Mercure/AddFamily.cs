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
    public partial class AddFamily : Form
    {
        public AddFamily()
        {
            InitializeComponent();
            Construct(null);
        }
        public AddFamily(Family Family)
        {
            InitializeComponent();
            Construct(Family);
        }

        private void Construct(Family Family)
        {
            StartPosition = FormStartPosition.CenterParent;
            this.DialogResult = DialogResult.Cancel;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
        }
    }
}
