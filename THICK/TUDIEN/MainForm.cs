using System;
using System.Data;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace TUDIEN
{
    public partial class MainForm : Form
    {
        private DictionaryEntry[] entries;
        public MainForm()
        {
            InitializeComponent();
            txtFileName.Text = "n20dccn037.txt";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            entries = new DictionaryEntry[0];
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var addEntryForm = new frmAdd())
            {
                if (addEntryForm.ShowDialog() == DialogResult.OK)
                {
                    DictionaryEntry entry = addEntryForm.GetDictionaryEntry();
                    AddEntry(entry);
                    ClearInputFields();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int selectedIndex = lstEntries.SelectedIndex;

            // Kiểm tra nếu không có từ nào được chọn
            if (selectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn từ cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DictionaryEntry selectedEntry = entries[selectedIndex];

            frmEdit editForm = new frmEdit();
            editForm.EditedEntry = selectedEntry;
            editForm.ShowDialog();

            if (editForm.DialogResult == DialogResult.OK)
            {
                DictionaryEntry editedEntry = editForm.EditedEntry;
                MessageBox.Show("Từ đã được sửa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshEntryList();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            string word = txtWord.Text;
            int selectedIndex = lstEntries.SelectedIndex;

            // Kiểm tra nếu không có từ nào được chọn
            if (selectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn từ cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }

            DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa từ '{word}' không?", "Xác nhận xóa từ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                RemoveEntry(word);
                ClearInputFields();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát khỏi chương trình không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnLookUp_Click(object sender, EventArgs e)
        {
            string word = txtWord.Text;

            if (string.IsNullOrEmpty(word))
            {
                MessageBox.Show("Vui lòng nhập từ cần tìm!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            LookupWord(word);
        }

        private void btnSaveToFile_Click(object sender, EventArgs e)
        {
            string fileName = txtFileName.Text;

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Vui lòng nhập tên file vào ô [File]","Chưa điền tên file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveDictionaryToFile(fileName);
            ClearInputFields();
        }

        private void btnLoadFromFile_Click(object sender, EventArgs e)
        {
            string fileName = txtFileName.Text;

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Vui lòng nhập tên file vào ô [Tên file]", "Chưa điền tên file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadDictionaryFromFile(fileName);
            ClearInputFields();
        }


        private void btnBrowser_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Chọn file từ điển";
                openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = openFileDialog.FileName;
                    txtFileName.Text = fileName;
                }
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Chọn vị trí lưu từ điển";
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = saveFileDialog.FileName;
                    txtFileName.Text = fileName;
                }
            }
        }

        public void AddEntry(DictionaryEntry entry)
        {
            DictionaryEntry[] newArray = new DictionaryEntry[entries.Length + 1];
            for (int i = 0; i < entries.Length; i++)
            {
                newArray[i] = entries[i];
            }
            newArray[newArray.Length - 1] = entry;
            entries = newArray;
            SortEntries();
            RefreshEntryList();
        }

        private void RemoveEntry(string word)
        {
            int index = FindEntryIndex(word);
            if (index != -1)
            {
                entries[index] = null;
                entries = entries.Where(entry => entry != null).ToArray();
            }
            RefreshEntryList();
        }

        private int CompareWords(string word1, string word2)
        {
            // So sánh hai từ theo thứ tự từ điển, không phân biệt hoa thường
            return string.Compare(word1, word2, StringComparison.OrdinalIgnoreCase);
        }

        private void SortEntries()
        {
            for (int i = 0; i < entries.Length - 1; i++)
            {
                for (int j = i + 1; j < entries.Length; j++)
                {
                    if (CompareWords(entries[i].word, entries[j].word) > 0)
                    {
                        // Hoán đổi vị trí các mục
                        DictionaryEntry temp = entries[i];
                        entries[i] = entries[j];
                        entries[j] = temp;
                    }
                }
            }
        }

        private void LookupWord(string word)
        {
            int[] matchingIndexes = FindMatchingIndexes(word);
            if (matchingIndexes.Length == 0)
            {
                rtbLookupResult.Text = "Không tìm thấy từ này!";
                return;
            }

            rtbLookupResult.Text = "";
            foreach (int index in matchingIndexes)
            {
                var entry = entries[index];

                rtbLookupResult.SelectionColor = Color.Blue;
                rtbLookupResult.AppendText("Word: ");

                rtbLookupResult.SelectionColor = Color.Red;
                rtbLookupResult.AppendText(entry.word + Environment.NewLine);

                rtbLookupResult.SelectionColor = Color.Blue;
                rtbLookupResult.AppendText("Part of Speech: ");

                rtbLookupResult.SelectionColor = Color.DarkOrange;
                rtbLookupResult.AppendText(entry.partOfSpeech + Environment.NewLine);

                rtbLookupResult.SelectionColor = Color.Blue;
                rtbLookupResult.AppendText("Definition: \n");

                rtbLookupResult.SelectionColor = Color.Black;
                rtbLookupResult.AppendText(entry.definition + Environment.NewLine);

                rtbLookupResult.SelectionColor = Color.Blue;
                rtbLookupResult.AppendText("Example: \n");

                rtbLookupResult.SelectionColor = Color.Black;
                rtbLookupResult.AppendText(entry.example + Environment.NewLine + Environment.NewLine);
            }
        }     

        private int FindEntryIndex(string word)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                if (string.Equals(entries[i].word, word, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

        private int[] FindMatchingIndexes(string word)
        {
            int[] indexes = new int[0];
            for (int i = 0; i < entries.Length; i++)
            {
                if (string.Equals(entries[i].word, word, StringComparison.OrdinalIgnoreCase))
                {
                    Array.Resize(ref indexes, indexes.Length + 1);
                    indexes[indexes.Length - 1] = i;
                }
            }
            return indexes;
        }

        private int FindMatchingIndex(string searchKeyword)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].word.StartsWith(searchKeyword, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

        private void SaveDictionaryToFile(string fileName)
        {
            if (entries.Length == 0)
            {
                DialogResult result = MessageBox.Show("Từ điển trống! Không có từ để lưu vào file. Tiếp tục lưu?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    foreach (var entry in entries)
                    {
                        writer.WriteLine(entry.word);
                        writer.WriteLine(entry.partOfSpeech);
                        writer.WriteLine(entry.definition);
                        writer.WriteLine(entry.example);
                    }
                }

                MessageBox.Show("Đã lưu từ điển vào file "+ fileName, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã có lỗi khi lưu từ điển: " + ex.Message);
            }
        }

        private void LoadDictionaryFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                MessageBox.Show("File not found.");
                return;
            }

            entries = new DictionaryEntry[0];
            int lineCount = 0;
            using (StreamReader reader = new StreamReader(fileName))
            {               
                while (!reader.EndOfStream)
                {
                    string word = reader.ReadLine();
                    string partOfSpeech = reader.ReadLine();
                    string definition = reader.ReadLine();
                    string example = reader.ReadLine();

                    DictionaryEntry entry = new DictionaryEntry(word, partOfSpeech, definition, example);

                    AddEntry(entry);
                    lineCount += 4;
                }              
            }
            MessageBox.Show("Tải dữ liệu từ file " + fileName, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (lineCount == 0)
                {
                    MessageBox.Show("File không có từ nào để tải lên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
        }

        private void ClearInputFields()
        {
            txtWord.Text = "";
            rtbLookupResult.Clear();
        }

        private void RefreshEntryList()
        {
            lstEntries.Items.Clear();

            if (entries.Length == 0)
            {
                MessageBox.Show("Từ điển trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var entry in entries)
            {
                lstEntries.Items.Add(entry.word);
            }
        }

        private void lstEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = lstEntries.SelectedIndex;
            if (selectedIndex >= 0)
            {
                DictionaryEntry selectedEntry = entries[selectedIndex];
                txtWord.Text = selectedEntry.word;

                rtbLookupResult.Clear();

                rtbLookupResult.SelectionColor = Color.Blue;
                rtbLookupResult.AppendText("Word: ");

                rtbLookupResult.SelectionColor = Color.Red;
                rtbLookupResult.AppendText(selectedEntry.word + Environment.NewLine);

                rtbLookupResult.SelectionColor = Color.Blue;
                rtbLookupResult.AppendText("Part of Speech: ");

                rtbLookupResult.SelectionColor = Color.DarkOrange;
                rtbLookupResult.AppendText(selectedEntry.partOfSpeech + Environment.NewLine);

                rtbLookupResult.SelectionColor = Color.Blue;
                rtbLookupResult.AppendText("Definition: \n");

                rtbLookupResult.SelectionColor = Color.Black;
                rtbLookupResult.AppendText(selectedEntry.definition + Environment.NewLine);

                rtbLookupResult.SelectionColor = Color.Blue;
                rtbLookupResult.AppendText("Example: \n");

                rtbLookupResult.SelectionColor = Color.Black;
                rtbLookupResult.AppendText(selectedEntry.example + Environment.NewLine + Environment.NewLine);
            }
        }

        

        private void txtWord_TextChanged(object sender, EventArgs e)
        {
            string searchKeyword = txtWord.Text;
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                int index = FindMatchingIndex(searchKeyword);
                if (index != -1)
                {
                    lstEntries.TopIndex = index;
                }
            }
        }

    }

    public class DictionaryEntry
    {
        public string word;
        public string partOfSpeech;
        public string definition;
        public string example;
        public DictionaryEntry(string w, string p, string d, string e)
        {
            word = w;
            partOfSpeech = p;
            definition = d;
            example = e;
        }
    }
}