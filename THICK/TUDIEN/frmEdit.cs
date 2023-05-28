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
        public DictionaryEntry EditedEntry { get; set; }
        public frmEdit()
        {
            InitializeComponent();
        }

        private void frmEdit_Load(object sender, EventArgs e)
        {
            if (EditedEntry != null)
            {
                // Hiển thị thông tin từ trong các control
                txtWord.Text = EditedEntry.word;
                txtPartOfSpeech.Text = EditedEntry.partOfSpeech;
                rtbDefinition.Text = EditedEntry.definition;
                rtbExample.Text = EditedEntry.example;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string word = txtWord.Text;
            string partOfSpeech = txtPartOfSpeech.Text;
            string definition = rtbDefinition.Text;
            string example = rtbExample.Text;

            if (string.IsNullOrEmpty(word) || string.IsNullOrEmpty(partOfSpeech) || string.IsNullOrEmpty(definition) || string.IsNullOrEmpty(example))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            EditedEntry.word = word;
            EditedEntry.partOfSpeech = partOfSpeech;
            EditedEntry.definition = definition;
            EditedEntry.example = example;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearInputFields()
        {
            txtWord.Text = "";
            txtPartOfSpeech.Text = "";
            rtbDefinition.Text = "";
            rtbExample.Text = "";
        }
    }
}
