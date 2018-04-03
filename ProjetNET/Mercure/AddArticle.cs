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
        private class ComboBoxItem
        {
            public string Name { get; set; }
            public long Value { get; set; }
            public override string ToString() { return Name; }
        }
        
        public AddArticle()
        {
            InitializeComponent();
            Construct(null);
        }
        public AddArticle(Article Article)
        {
            InitializeComponent();
            Construct(Article);
        }

        private void Construct(Article Article)
        {
            ListBrands = new List<long>();
            ListSubFamilies = new List<long>();
            this.DialogResult = DialogResult.Cancel;

            SQLiteConnection Connection = DBConnect.GetInstance().GetConnection();
            SQLiteCommand CommandSelect = new SQLiteCommand("SELECT * FROM Marques", Connection);
            SQLiteDataReader Result = CommandSelect.ExecuteReader();
            if (Result != null)
            {
                if (Result.Read())
                {
                    Object ObjId = Result["RefMarque"];
                    long BrandId = 0;
                    if (ObjId != System.DBNull.Value)
                    {
                        BrandId = Convert.ToInt64(ObjId);
                    }
                    Object ObjName = Result["Nom"];
                    string BrandName = "";
                    if (ObjName != System.DBNull.Value)
                    {
                        BrandName = Convert.ToString(ObjName);
                    }
                    ComboBoxBrand.Items.Add(new ComboBoxItem { Name = BrandName, Value = BrandId });
                }
                Result.Close();
            }
            else
            {
                throw new FieldAccessException("Getting A failed");
            }

            if (Article != null)
                SetArticle(Article);
        }

        public Article GetArticle()
        {
            ComboBoxItem BrandItem = (ComboBoxItem)ComboBoxBrand.SelectedItem;
            ComboBoxItem SFItem = (ComboBoxItem)ComboBoxSubFamily.SelectedItem;
            bool ok;
            float Price;
            ok = float.TryParse(TextBoxPrice.Text, out Price);
            if (!ok)
                return null;
            Article Art = new Article(TextBoxReference.Text, TextBoxDescription.Text, SFItem.Value, BrandItem.Value, Price, 42);
            return Art;
        }

        public void SetArticle(Article Art)
        {
            int IndexBrand = ListBrands.FindIndex(x => x == Art.Brand);
            if (IndexBrand < 0)
                throw new Exception("Incorrect brand ID");
            int IndexSF = ListBrands.FindIndex(x => x == Art.SubFamily);
            if (IndexSF < 0)
                throw new Exception("Incorrect subfamily ID");
            TextBoxReference.Text = Art.Reference;
            TextBoxDescription.Text = Art.Description;
            ComboBoxBrand.SelectedIndex = IndexBrand;
            ComboBoxSubFamily.SelectedIndex = IndexSF;
            TextBoxPrice.Text = Art.Price.ToString();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private List<long> ListBrands;
        private List<long> ListSubFamilies;
    }
}
