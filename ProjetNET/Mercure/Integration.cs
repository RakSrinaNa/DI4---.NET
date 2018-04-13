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
    public partial class Integration : Form
    {
        public Integration()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
        }

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

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (TextBox1.Text == "")
                return;
            NewButton.Enabled = false;
            UpdateButton.Enabled = false;
            XmlDocument Doc = Parser.ParseXML(TextBox1.Text);
            XmlNodeList NodeList = Doc.SelectNodes("/materiels/article");
            int Progress = 0;
            foreach (XmlNode Node in NodeList)
            {
                string RefArticle = Node.SelectSingleNode("refArticle").InnerText;
                if (DBConnect.GetInstance().ArticleExists(RefArticle))
                    DBConnect.GetInstance().UpdateArticle(Node);
                else
                    DBConnect.GetInstance().AddArticle(Node);
                ProgressBar1.Value = (Progress * 100) / NodeList.Count;
                Progress++;
            }
            ProgressBar1.Value = 100;
            Close();
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            if (TextBox1.Text == "")
                return;
            NewButton.Enabled = false;
            UpdateButton.Enabled = false;
            XmlDocument Doc = Parser.ParseXML(TextBox1.Text);
            DBConnect.GetInstance().Clear();
            XmlNodeList NodeList = Doc.SelectNodes("/materiels/article");
            int Progress = 0;
            foreach(XmlNode Node in NodeList)
            {
                DBConnect.GetInstance().AddArticle(Node);
                ProgressBar1.Value = (Progress * 100) / NodeList.Count;
                Progress++;
            }
            ProgressBar1.Value = 100;
            Close();
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
