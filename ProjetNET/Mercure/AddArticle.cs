using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace ProjetNET
{
    /// <inheritdoc />
    /// <summary>
    /// Window to enter the data of an article
    /// </summary>
    public partial class AddArticle : Form
    {
        /// <summary>
        /// TODO: Clément
        /// </summary>
        private class ComboBoxItem
        {
            public string Name { get; set; }
            public long Value { get; set; }
            public override string ToString() { return Name; }
        }

        /// <inheritdoc />
        /// <summary>
        /// Initialize an empty window to add an article
        /// </summary>
        public AddArticle()
        {
            InitializeComponent();
            Construct(null);
        }

        /// <inheritdoc />
        /// <summary>
        /// Initialize a window to modify the given article
        /// </summary>
        /// <param name="Article">The article to edit. If null, no article will be loaded.</param>
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
            DialogResult = DialogResult.Cancel;
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
                    object ObjId = ResultBrands["RefMarque"];
                    long BrandId = 0;
                    if (ObjId != DBNull.Value)
                    {
                        BrandId = Convert.ToInt64(ObjId);
                    }
                    object ObjName = ResultBrands["Nom"];
                    string BrandName = "";
                    if (ObjName != DBNull.Value)
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
            SQLiteCommand CommandSelectSf = new SQLiteCommand("SELECT * FROM SousFamilles ORDER BY RefFamille", Connection);
            SQLiteDataReader ResultSf = CommandSelectSf.ExecuteReader();
            if (ResultSf != null)
            {
                while (ResultSf.Read())
                {
                    object ObjId = ResultSf["RefSousFamille"];
                    long SfId = 0;
                    if (ObjId != DBNull.Value)
                    {
                        SfId = Convert.ToInt64(ObjId);
                    }
                    object ObjName = ResultSf["Nom"];
                    string SfName = "";
                    if (ObjName != DBNull.Value)
                    {
                        SfName = Convert.ToString(ObjName);
                    }
                    ComboBoxSubFamily.Items.Add(new ComboBoxItem { Name = SfName, Value = SfId });
                }
                ResultSf.Close();
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
            ComboBoxItem SfItem = (ComboBoxItem)ComboBoxSubFamily.SelectedItem;
            Article Art = new Article(TextBoxReference.Text, TextBoxDescription.Text, SfItem.Value, BrandItem.Value, (double)NumericUpDownPrice.Value, (int)NumericUpDownQuantity.Value);
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
            for (int SfIndex = 0; SfIndex < ComboBoxSubFamily.Items.Count; SfIndex++)
            {
                if (((ComboBoxItem)(ComboBoxSubFamily.Items[SfIndex])).Value == Art.SubFamily)
                {
                    ComboBoxSubFamily.SelectedIndex = SfIndex;
                    break;
                }
            }
            TextBoxReference.Text = Art.Reference;
            TextBoxDescription.Text = Art.Description;
            NumericUpDownPrice.Value = (decimal)Art.Price;
            NumericUpDownQuantity.Value = Art.Quantity;
        }

        /// <summary>
        /// Validate the creation (or editing)
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void ButtonOK_Click(object Sender, EventArgs Event)
        {
            Article Article = GetArticle();
            if (!Regex.IsMatch(Article.Reference, @"^F[0-9]{7}$"))
            {
                MessageBox.Show(@"Wrong reference");
                return;
            }
            DialogResult = DialogResult.OK;
            
            if(Article != null)
                DBConnect.GetInstance().UpdateOrCreateArticle(Article);
            Close();
        }

        /// <summary>
        /// Close the window without saving the changes
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void ButtonCancel_Click(object Sender, EventArgs Event)
        {
            Close();
        }
    }
}
