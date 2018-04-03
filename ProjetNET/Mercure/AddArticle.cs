using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;

namespace ProjetNET
{
    public partial class AddArticle : Form
    {
        public AddArticle()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Cancel;

            SQLiteConnection Connection = DBConnect.GetInstance().GetConnection();
            SQLiteCommand CommandSelect = new SQLiteCommand("SELECT * FROM Marques", Connection);
            SQLiteDataReader Result = CommandSelect.ExecuteReader();
            if (Result != null)
            {
                if (Result.Read())
                {
                    Object Obj = Result["RefMarque"];
                    if (Obj != System.DBNull.Value)
                    {
                        ListBrands.Add(Convert.ToInt64(Obj));
                    }
                }
                Result.Close();
            }
            else
            {
                throw new FieldAccessException("Getting A failed");
            }
        }

        public Article GetArticle()
        {
            Article Art = new Article();
            Art.Reference = TextBoxReference.Text;
            Art.Description = TextBoxDescription.Text;
            Art.Brand = 0;// ComboBoxBrand.SelectedItem;
            Art.SubFamily = 0;// ComboBoxSubFamily.SelectedItem;
            bool ok;
            float Price;
            ok = float.TryParse(TextBoxPrice.Text, out Price);
            if (!ok)
                return null;
            Art.Price = Price;
            return Art;
        }

        public void SetArticle(Article Art)
        {
            TextBoxReference.Text = Art.Reference;
            TextBoxDescription.Text = Art.Description;
            Art.Brand = ListBrands[ComboBoxBrand.SelectedIndex]; // TODO
            Art.SubFamily = ComboBoxSubFamily.SelectedIndex; // TODO
            TextBoxPrice.Text = Art.Price.ToString();
        }


        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private List<long> ListBrands;
        private List<long> ListSubFamilies;
    }
}
