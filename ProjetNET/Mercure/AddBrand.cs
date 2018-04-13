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

        public Brand GetBrand()
        {
            Brand Brand = new Brand(ID, TextBoxName.Text);
            return Brand;
        }

        public void SetBrand(Brand Brand)
        {
            TextBoxName.Text = Brand.Name;
            ID = Brand.Reference;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Brand Brand = GetBrand();
            if (Brand != null)
                DBConnect.GetInstance().UpdateOrCreateBrand(Brand);
            this.Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private long ID;
    }
}
