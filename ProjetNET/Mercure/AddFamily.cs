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
    /// Window to enter the data of a family
    /// </summary>
    public partial class AddFamily : Form
    {
        /// <summary>
        /// Initialize an empty window to add a family
        /// </summary>
        public AddFamily()
        {
            InitializeComponent();
            Construct(null);
        }

        /// <summary>
        /// Initialize a window to modify the given family
        /// </summary>
        /// <param name="Family">The family to edit</param>
        public AddFamily(Family Family)
        {
            InitializeComponent();
            Construct(Family);
        }

        /// <summary>
        /// Put the attributes of the given family in the corresponding fields
        /// </summary>
        /// <param name="Family">The family (or null for empty window)</param>
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
