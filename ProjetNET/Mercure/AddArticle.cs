using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace ProjetNET
{
    /// <summary>
    /// Window to enter the data of an article
    /// </summary>
    public partial class AddArticle : Form
    {
        /// <summary>
        /// TODO
        /// </summary>
        private class ComboBoxItem
        {
            public string Name { get; set; }
            public long Value { get; set; }
            public override string ToString() { return Name; }
        }

        /// <summary>
        /// Initialize an empty window to add an article
        /// </summary>
        public AddArticle()
        {
            InitializeComponent();
            Construct(null);
        }

        /// <summary>
        /// Initialize a window to modify the given article
        /// </summary>
        /// <param name="Article">The article to edit</param>
        public AddArticle(Article Article)
        {
            InitializeComponent();
            Construct(Article);
        }

        /// <summary>
        /// Put the attributes of the given article in the corresponding fields
        /// </summary>
        /// <param name="Article">The article (or null for empty window)</param>
        private void Construct(Article Article)
        {
            StartPosition = FormStartPosition.CenterParent;
            this.DialogResult = DialogResult.Cancel;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

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
            SQLiteCommand CommandSelectSF = new SQLiteCommand("SELECT * FROM SousFamilles ORDER BY RefFamille", Connection);
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

        /// <summary>
        /// Return the article created (or edited) by the window
        /// </summary>
        /// <returns>The new article</returns>
        public Article GetArticle()
        {
            ComboBoxItem BrandItem = (ComboBoxItem)ComboBoxBrand.SelectedItem;
            ComboBoxItem SFItem = (ComboBoxItem)ComboBoxSubFamily.SelectedItem;
            Article Art = new Article(TextBoxReference.Text, TextBoxDescription.Text, SFItem.Value, BrandItem.Value, (double)NumericUpDownPrice.Value, (int)NumericUpDownQuantity.Value);
            return Art;
        }

        /// <summary>
        /// Edit the given article with the new data
        /// </summary>
        /// <param name="Art">The article to edit</param>
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
            NumericUpDownPrice.Value = (decimal)Art.Price;
            NumericUpDownQuantity.Value = (decimal)Art.Quantity;
        }

        /// <summary>
        /// Validate the creation (or editing)
        /// </summary>
        /// <param name="sender">The object sending the event</param>
        /// <param name="e">The event</param>
        private void ButtonOK_Click(object sender, EventArgs e)
        {
            Article Article = GetArticle();
            if (!Regex.IsMatch(Article.Reference, @"^F[0-9]{7}$"))
            {
                System.Windows.Forms.MessageBox.Show("Wrong reference");
                return;
            }
            this.DialogResult = DialogResult.OK;
            
            if(Article != null)
                DBConnect.GetInstance().UpdateOrCreateArticle(Article);
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
    }
}
