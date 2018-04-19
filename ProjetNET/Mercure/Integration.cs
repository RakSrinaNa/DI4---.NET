using System;
using System.Xml;
using System.Windows.Forms;

namespace ProjetNET
{
    /// <inheritdoc />
    /// <summary>
    /// Window to import XML file data in the database
    /// </summary>
    public partial class Integration : Form
    {
        /// <inheritdoc />
        /// <summary>
        /// Default constructor
        /// </summary>
        public Integration()
        {
            InitializeComponent();
            //Do not allow resizing
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
        }

        /// <summary>
        /// Open a browser to select a file
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void BrowseButton_Click(object Sender, EventArgs Event)
        {
            OpenFileDialog MyFileDialog = new OpenFileDialog();
            MyFileDialog.Filter = @"XML Files|*.xml";
            DialogResult HasClickedOk = MyFileDialog.ShowDialog();
            if (HasClickedOk == DialogResult.OK)
            {
                TextBox1.Text = MyFileDialog.FileName;
            }
        }

        /// <summary>
        /// Update the database from the selected file
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void UpdateButton_Click(object Sender, EventArgs Event)
        {
            if (TextBox1.Text == "")
                return;
            NewButton.Enabled = false;
            UpdateButton.Enabled = false;
            XmlDocument Doc = Parser.ParseXml(TextBox1.Text);
            XmlNodeList NodeList = Doc.SelectNodes("/materiels/article");
            int Updated = 0;
            int Added = 0;
            int Progress = 0;
            foreach (XmlNode Node in NodeList)
            {
                string RefArticle = Node.SelectSingleNode("refArticle").InnerText;
                try
                {
                    if (DbConnect.GetInstance().ArticleExists(RefArticle))
                    {
                        if (DbConnect.GetInstance().UpdateArticle(Node))
                            Updated++;
                    }
                    else if(DbConnect.GetInstance().AddArticle(Node))
                        Added++;
                }
                catch (Exception E)
                {
                    MessageBox.Show(@"Error " + E.Message);
                }
                ProgressBar1.Value = (Progress * 100) / NodeList.Count;
                Progress++;
            }
            ProgressBar1.Value = 100;
            Close();
            MessageBox.Show(@"Updated " + Updated + @" elements and created " + Added + @" elements");
        }

        /// <summary>
        /// Erase and reload the database from the selected file
        /// </summary>
        /// <param name="Sender">The object sending the event</param>
        /// <param name="Event">The event</param>
        private void NewButton_Click(object Sender, EventArgs Event)
        {
            if (TextBox1.Text == "")
                return;
            NewButton.Enabled = false;
            UpdateButton.Enabled = false;
            XmlDocument Doc = Parser.ParseXml(TextBox1.Text);
            DbConnect.GetInstance().Clear();
            XmlNodeList NodeList = Doc.SelectNodes("/materiels/article");
            int Added = 0;
            int Progress = 0;
            foreach(XmlNode Node in NodeList)
            {
                try
                {
                    if (DbConnect.GetInstance().AddArticle(Node))
                        Added++;
                }
                catch (Exception E)
                {
                    MessageBox.Show(@"Error " + E.Message);
                }
                ProgressBar1.Value = (Progress * 100) / NodeList.Count;
                Progress++;
            }
            ProgressBar1.Value = 100;
            Close();
            MessageBox.Show(@"Added " + Added + @" elements");
        }
    }
}
