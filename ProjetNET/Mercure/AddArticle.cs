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
            this.DialogResult = DialogResult.Cancel;

            SQLiteConnection Connection = DBConnect.GetInstance().GetConnection();
            SQLiteCommand CommandSelectBrands = new SQLiteCommand("SELECT * FROM Marques", Connection);
            SQLiteDataReader ResultBrands = CommandSelectBrands.ExecuteReader();
            if (ResultBrands != null)
            {
                while (ResultBrands.Read())
                {
                    Object ObjId = ResultBrands["RefMarque"];
                    long BrandId = 0;
                    if (ObjId != System.DBNull.Value)
                    {
                        BrandId = Convert.ToInt64(ObjId);
                    }
                    Object ObjName = ResultBrands["Nom"];
                    string BrandName = "";
                    if (ObjName != System.DBNull.Value)
                    {
                        BrandName = Convert.ToString(ObjName);
                    }
                    ComboBoxBrand.Items.Add(new ComboBoxItem { Name = BrandName, Value = BrandId });
                }
                ResultBrands.Close();
            }
            else
            {
                throw new FieldAccessException("Getting brands failed");
            }
            SQLiteCommand CommandSelectSF = new SQLiteCommand("SELECT * FROM SousFamilles", Connection);
            SQLiteDataReader ResultSF = CommandSelectSF.ExecuteReader();
            if (ResultSF != null)
            {
                while (ResultSF.Read())
                {
                    Object ObjId = ResultSF["RefSousFamille"];
                    long SFId = 0;
                    if (ObjId != System.DBNull.Value)
                    {
                        SFId = Convert.ToInt64(ObjId);
                    }
                    Object ObjName = ResultSF["Nom"];
                    string SFName = "";
                    if (ObjName != System.DBNull.Value)
                    {
                        SFName = Convert.ToString(ObjName);
                    }
                    ComboBoxSubFamily.Items.Add(new ComboBoxItem { Name = SFName, Value = SFId });
                }
                ResultSF.Close();
            }
            else
            {
                throw new FieldAccessException("Getting subfamily failed");
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
            for(int BrandIndex = 0; BrandIndex < ComboBoxBrand.Items.Count; BrandIndex++)
            {
                if(((ComboBoxItem)(ComboBoxBrand.Items[BrandIndex])).Value == Art.Brand)
                {
                    ComboBoxBrand.SelectedIndex = BrandIndex;
                    break;
                }
            }
            for (int SFIndex = 0; SFIndex < ComboBoxSubFamily.Items.Count; SFIndex++)
            {
                if (((ComboBoxItem)(ComboBoxSubFamily.Items[SFIndex])).Value == Art.SubFamily)
                {
                    ComboBoxSubFamily.SelectedIndex = SFIndex;
                    break;
                }
            }
            TextBoxReference.Text = Art.Reference;
            TextBoxDescription.Text = Art.Description;
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
    }
}
