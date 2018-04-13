using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjetNET
{
    public partial class SubFamilies : Form
    {
        private int SortColumn;

        public SubFamilies()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;

            listView1.FullRowSelect = true;
            listView1.MouseClick += new MouseEventHandler(OnClickSubFamily);

            LoadDatabase();
        }

        private void OnClickSubFamily(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 2)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    UpdateSubFamily((SubFamily) Item.Tag);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                ContextMenu ContextMenu = new ContextMenu();

                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    MenuItem MenuAdd = new MenuItem("Add sub family");
                    MenuAdd.Click += new EventHandler((o, evt) => { UpdateSubFamily(null); });
                    MenuItem MenuMod = new MenuItem("Edit sub family");
                    MenuMod.Click += new EventHandler((o, evt) => { UpdateSubFamily((SubFamily) Item.Tag); });
                    MenuItem MenuDel = new MenuItem("Delete sub family");
                    MenuDel.Click += new EventHandler((o, evt) =>
                    {
                        DBConnect.GetInstance().DeleteSubFamily(((SubFamily) Item.Tag).Reference);
                        LoadDatabase();
                    });

                    ContextMenu.MenuItems.Add(MenuAdd);
                    ContextMenu.MenuItems.Add(MenuMod);
                    ContextMenu.MenuItems.Add(MenuDel);
                }

                MenuItem MenuRfh = new MenuItem("Refresh");
                MenuRfh.Click += new EventHandler((o, evt) => LoadDatabase());
                ContextMenu.MenuItems.Add(MenuRfh);

                ContextMenu.Show(this, e.Location);
            }
        }

        private void LoadDatabase()
        {
            LoadDatabase(true);
        }

        private void LoadDatabase(bool ShouldReloadData)
        {
            if (ShouldReloadData)
            {
                listView1.Columns.Clear();
                listView1.Items.Clear();
                SQLiteDataAdapter Adapter = new SQLiteDataAdapter(
                    "SELECT RefSousFamille, RefFamille, Nom FROM SousFamilles",
                    DBConnect.GetInstance().GetConnection());
                DataTable Dt = new DataTable();
                Adapter.Fill(Dt);

                listView1.KeyDown += new KeyEventHandler(OnListViewKeyDown);
                listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
                listView1.ColumnClick += new ColumnClickEventHandler(OnColumnClick);
                listView1.ListViewItemSorter = new ListViewItemComparer();

                listView1.Columns.Add("RefSousFamille");
                listView1.Columns.Add("RefFamille");
                listView1.Columns.Add("Nom");

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    DataRow Dr = Dt.Rows[i];
                    ListViewItem ListItem = new ListViewItem(Dr["RefSousFamille"].ToString());
                    ListItem.SubItems.Add(Dr["RefFamille"].ToString());
                    ListItem.SubItems.Add(Dr["Nom"].ToString());
                    ListItem.Tag = new SubFamily(
                        Convert.ToInt64(Dr["RefSousFamille"].ToString()),
                        Convert.ToInt64(Dr["RefFamille"].ToString()),
                        Dr["Nom"].ToString()
                    );
                    listView1.Items.Add(ListItem);
                }

                Adapter.Dispose();
            }
            else
            {
                listView1.Groups.Clear();
            }

            foreach (ListViewItem ListItem in listView1.Items)
            {
                ListItem.Group = null;
                ListViewGroup Group = null;
                foreach (ListViewGroup GroupTest in listView1.Groups)
                {
                    if (GroupTest.Header == ListItem.SubItems[SortColumn].Text)
                    {
                        Group = GroupTest;
                        break;
                    }
                }

                if (Group == null)
                {
                    Group = new ListViewGroup();
                    Group.Name = ListItem.SubItems[SortColumn].Text;
                    Group.Header = ListItem.SubItems[SortColumn].Text;
                    listView1.Groups.Add(Group);
                }

                Group = listView1.Groups[ListItem.SubItems[SortColumn].Text];
                Group.Items.Add(ListItem);
                ListItem.Group = Group;
            }
        }

        private void OnColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewItemComparer LastComparer = (ListViewItemComparer) listView1.ListViewItemSorter;
            if (LastComparer.GetCol() == e.Column)
            {
                LastComparer.Invert();
                listView1.Sort();
            }
            else
            {
                SortColumn = e.Column;
                LoadDatabase(false);
                listView1.ListViewItemSorter = new ListViewItemComparer(e.Column);
            }
        }

        private class ListViewItemComparer : IComparer
        {
            private int Col;
            private bool Inverted;

            public ListViewItemComparer()
            {
                Col = 0;
                Inverted = false;
            }

            public int GetCol()
            {
                return Col;
            }

            public void Invert()
            {
                Inverted = !Inverted;
            }

            public ListViewItemComparer(int ArgCol)
            {
                Col = ArgCol;
                Inverted = false;
            }

            public int Compare(object x, object y)
            {
                switch (Col)
                {
                    case 0:
                    case 1:
                        return (Inverted ? -1 : 1) *
                               (int) (long.Parse(((ListViewItem) x).SubItems[Col].Text,
                                          System.Globalization.CultureInfo.CurrentUICulture) -
                                      long.Parse(((ListViewItem) y).SubItems[Col].Text,
                                          System.Globalization.CultureInfo.CurrentUICulture));
                    default:
                        return (Inverted ? -1 : 1) * String.Compare(((ListViewItem) x).SubItems[Col].Text,
                                   ((ListViewItem) y).SubItems[Col].Text);
                }
            }
        }

        private void OnListViewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys) e.KeyCode == Keys.Enter)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    ListViewItem Item = listView1.SelectedItems[0];
                    UpdateSubFamily((SubFamily) Item.Tag);
                }
            }
            else if ((Keys) e.KeyCode == Keys.F5)
            {
                LoadDatabase();
            }
            else if ((Keys) e.KeyCode == Keys.Delete)
            {
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    ListViewItem Item = listView1.SelectedItems[i];
                    DBConnect.GetInstance().DeleteSubFamily(((SubFamily) Item.Tag).Reference);
                }

                LoadDatabase();
            }
        }

        private void UpdateSubFamily(SubFamily SubFamily)
        {
            AddSubFamily AddSubFamily = new AddSubFamily(SubFamily);
            AddSubFamily.ShowDialog();
            LoadDatabase();
        }
    }
}