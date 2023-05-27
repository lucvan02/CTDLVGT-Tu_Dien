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
    public partial class frmAdd : Form
    {
        private DictionaryEntry[] entries;
        DictionaryEntry NewEntry { get; set; }
        public frmAdd()
        {
            InitializeComponent();
        }

        private void frmAdd_Load(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
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

            DictionaryEntry entry = new DictionaryEntry(word, partOfSpeech, definition, example);
            MainForm mainForm = Application.OpenForms["MainForm"] as MainForm;
            mainForm?.AddEntry(entry);

            MessageBox.Show($"Đã thêm từ [{word}] vào từ điển!");
            ClearInputFields();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public DictionaryEntry GetDictionaryEntry()
        {
            string word = txtWord.Text;
            string partOfSpeech = txtPartOfSpeech.Text;
            string definition = rtbDefinition.Text;
            string example = rtbExample.Text;

            return new DictionaryEntry(word, partOfSpeech, definition, example);
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
