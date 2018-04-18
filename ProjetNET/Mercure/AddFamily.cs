using System;
using System.Windows.Forms;

namespace ProjetNET
{
    /// <inheritdoc />
    /// <summary>
    /// Window to enter the data of a family
    /// </summary>
    public partial class AddFamily : Form
    {
        /// <inheritdoc />
        /// <summary>
        /// Initialize an empty window to add a family
        /// </summary>
        public AddFamily()
        {
            InitializeComponent();
            Construct(null);
        }

        /// <inheritdoc />
        /// <summary>
        /// Initialize a window to modify the given family
        /// </summary>
        /// <param name="Family">The family to edit</param>
        public AddFamily(Family Family)
        {
            InitializeComponent();
            Construct(Family);
        }

        /// <summary>
        /// Put the attributes of the given family in the corresponding fields
        /// </summary>
        /// <param name="Family">The family (or null for empty window)</param>
        private void Construct(Family Family)
        {
            StartPosition = FormStartPosition.CenterParent;
            DialogResult = DialogResult.Cancel;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            if (Family != null)
                SetFamily(Family);
            else
                _Id = -1;
        }

        public Family GetFamily()
        {
            Family Family = new Family(_Id, TextBoxName.Text);
            return Family;
        }

        public void SetFamily(Family Family)
        {
            TextBoxName.Text = Family.Name;
            _Id = Family.Reference;
        }

        private void ButtonOK_Click(object Sender, EventArgs Event)
        {
            DialogResult = DialogResult.OK;
            Family Family = GetFamily();
            if (Family != null)
                DBConnect.GetInstance().UpdateOrCreateFamily(Family);
            Close();
        }

        private void ButtonCancel_Click(object Sender, EventArgs Event)
        {
            Close();
        }

        private long _Id;
    }
}
