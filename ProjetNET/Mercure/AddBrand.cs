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
    /// indow to enter the data of a brand
    /// </summary>
    public partial class AddBrand : Form
    {
        /// <summary>
        ///  Initialize an empty window to add a brand
        /// </summary>
        public AddBrand()
        {
            InitializeComponent();
            Construct(null);
        }

        /// <summary>
        /// Initialize a window to modify the given brand
        /// </summary>
        /// <param name="Brand">The brand to edit</param>
        public AddBrand(Brand Brand)
        {
            InitializeComponent();
            Construct(Brand);
        }

        /// <summary>
        /// Put the attributes of the given brand in the corresponding fields
        /// </summary>
        /// <param name="Brand">The brand (or null for empty window)</param>
        private void Construct(Brand Brand)
        {
            StartPosition = FormStartPosition.CenterParent;
            this.DialogResult = DialogResult.Cancel;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            if (Brand != null)
                SetBrand(Brand);
            else
                ID = -1;
        }

        /// <summary>
        /// Return the brand created (or edited) by the window
        /// </summary>
        /// <returns>The new brand</returns>
        public Brand GetBrand()
        {
            Brand Brand = new Brand(ID, TextBoxName.Text);
            return Brand;
        }

        /// <summary>
        /// Edit the given brand with the new data
        /// </summary>
        /// <param name="Brand">The brand to edit</param>
        public void SetBrand(Brand Brand)
        {
            TextBoxName.Text = Brand.Name;
            ID = Brand.Reference;
        }

        /// <summary>
        /// Validate the creation (or editing)
        /// </summary>
        /// <param name="sender">The object sending the event</param>
        /// <param name="e">The event</param>
        private void ButtonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Brand Brand = GetBrand();
            if (Brand != null)
                DBConnect.GetInstance().UpdateOrCreateBrand(Brand);
            this.Close();
        }

        /// <summary>
        /// Close the window without saving the changes
        /// </summary>
        /// <param name="sender">The object sending the event</param>
        /// <param name="e">The event</param>
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private long ID;
    }
}
