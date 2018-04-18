using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace ProjetNET
{
    /// <summary>
    /// Window to import XML file data in the database
    /// </summary>
    public partial class Integration : Form
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Integration()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
        }

        /// <summary>
        /// Open a browser to select a file
        /// </summary>
        /// <param name="sender">The object sending the event</param>
        /// <param name="e">The event</param>
        private void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog MyFileDialog = new OpenFileDialog();
            MyFileDialog.Filter = "XML Files|*.xml";
            DialogResult HasClickedOK = MyFileDialog.ShowDialog();
            if (HasClickedOK == DialogResult.OK)
            {
                TextBox1.Text = MyFileDialog.FileName;
            }
        }

        /// <summary>
        /// Update the database from the selected file
        /// </summary>
        /// <param name="sender">The object sending the event</param>
        /// <param name="e">The event</param>
        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (TextBox1.Text == "")
                return;
            NewButton.Enabled = false;
            UpdateButton.Enabled = false;
            XmlDocument Doc = Parser.ParseXML(TextBox1.Text);
            XmlNodeList NodeList = Doc.SelectNodes("/materiels/article");
            int Updated = 0;
            int Added = 0;
            int Progress = 0;
            foreach (XmlNode Node in NodeList)
            {
                string RefArticle = Node.SelectSingleNode("refArticle").InnerText;
                try
                {
                    if (DBConnect.GetInstance().ArticleExists(RefArticle))
                    {
                        if (DBConnect.GetInstance().UpdateArticle(Node))
                            Updated++;
                    }
                    else if(DBConnect.GetInstance().AddArticle(Node))
                        Added++;
                }
                catch (Exception E)
                {
                    System.Windows.Forms.MessageBox.Show("Error " + E.Message);
                }
                ProgressBar1.Value = (Progress * 100) / NodeList.Count;
                Progress++;
            }
            ProgressBar1.Value = 100;
            Close();
            System.Windows.Forms.MessageBox.Show("Updated " + Updated + " elements and created " + Added + " elements");
        }

        /// <summary>
        /// Erase and reload the database from the selected file
        /// </summary>
        /// <param name="sender">The object sending the event</param>
        /// <param name="e">The event</param>
        private void NewButton_Click(object sender, EventArgs e)
        {
            if (TextBox1.Text == "")
                return;
            NewButton.Enabled = false;
            UpdateButton.Enabled = false;
            XmlDocument Doc = Parser.ParseXML(TextBox1.Text);
            DBConnect.GetInstance().Clear();
            XmlNodeList NodeList = Doc.SelectNodes("/materiels/article");
            int Added = 0;
            int Progress = 0;
            foreach(XmlNode Node in NodeList)
            {
                try
                {
                    if (DBConnect.GetInstance().AddArticle(Node))
                        Added++;
                }
                catch (Exception E)
                {
                    System.Windows.Forms.MessageBox.Show("Error " + E.Message);
                }
                ProgressBar1.Value = (Progress * 100) / NodeList.Count;
                Progress++;
            }
            ProgressBar1.Value = 100;
            Close();
            System.Windows.Forms.MessageBox.Show("Added " + Added + " elements");
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
