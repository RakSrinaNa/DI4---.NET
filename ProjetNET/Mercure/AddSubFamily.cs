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
    /// <summary>
    /// Window to enter the data of a subfamily
    /// </summary>
    public partial class AddSubFamily : Form
    {
        /// <summary>
        /// Initialize an empty window to add a subfamily
        /// </summary>
        public AddSubFamily()
        {
            InitializeComponent();
            Construct(null);
        }

        /// <summary>
        /// Initialize a window to modify the given subfamily
        /// </summary>
        /// <param name="SubFamily">The subfamily to edit</param>
        public AddSubFamily(SubFamily SubFamily)
        {
            InitializeComponent();
            Construct(SubFamily);
        }

        /// <summary>
        /// Put the attributes of the given subfamily in the corresponding fields
        /// </summary>
        /// <param name="SubFamily">The subfamily (or null for empty window)</param>
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
