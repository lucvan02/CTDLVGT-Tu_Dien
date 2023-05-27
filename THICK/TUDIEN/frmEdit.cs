using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TUDIEN
{
    public partial class frmEdit : Form
    {
        private DictionaryEntry[] entries;
        DictionaryEntry NewEntry { get; set; }

        private DictionaryEntry entryToEdit;
        public frmEdit()
        {
            InitializeComponent();
        }

        private void frmEdit_Load(object sender, EventArgs e)
        {
            txtEditWord.Text = EditedWord.Word;
            rtbEditDefinition.Text = EditedWord.Definition;
            rtbEditExample.Text = EditedWord.Example;
        }

        private void EditWordForm_Load(object sender, EventArgs e)
        {
            // Set the initial value of txtEditedWord to the selected word
            txtEditWord.Text = SelectedWord;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string editedWord = txtEditWord.Text;

            // Perform any necessary validation or processing of the edited word

            // Update the SelectedWord property with the edited word
            SelectedWord = editedWord;

            // Close the form
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
