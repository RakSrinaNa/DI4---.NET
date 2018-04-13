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
    public partial class AddBrand : Form
    {
        public AddBrand()
        {
            InitializeComponent();
            Construct(null);
        }

        public AddBrand(Brand Brand)
        {
            InitializeComponent();
            Construct(Brand);
        }

        private void Construct(Brand Brand)
        {
        }
    }
}
