using System;
using System.Data;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace TUDIEN
{
    public partial class MainForm : Form
    {
        private DictionaryEntry[] entries;
        public MainForm()
        {
            InitializeComponent();
            txtFileName.Text = "n20dccn037_mang.txt";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            entries = new DictionaryEntry[0];
        }

        




        private void btnLookUp_Click(object sender, EventArgs e)
        {
            string word = txtWord.Text;

            if (string.IsNullOrEmpty(word))
            {
                MessageBox.Show("Vui lòng nhập từ cần tìm!");
                return;
            }

            LookupWord(word);
            ClearInputFields();
        }

        private void btnSaveToFile_Click(object sender, EventArgs e)
        {
            string fileName = txtFileName.Text;

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Vui lòng nhập tên file vào ô [Tên file]","Chưa điền tên file", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        /*private void LookupWord(string word)
        {
            int[] matchingIndexes = FindMatchingIndexes(word);
            if (matchingIndexes.Length == 0)
            {
                MessageBox.Show("Không tìm thấy từ này!");
                return;
            }

            string result = "";
            foreach (int index in matchingIndexes)
            {
                var entry = entries[index];
                result += "Word: " + entry.word + Environment.NewLine;
                result += "Part of Speech: " + entry.partOfSpeech + Environment.NewLine;
                result += "Definition: " + entry.definition + Environment.NewLine;
                result += "Example: " + entry.example + Environment.NewLine + Environment.NewLine;
            }

            MessageBox.Show(result);
        }*/
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
                rtbLookupResult.AppendText("Word: " + entry.word + Environment.NewLine);
                rtbLookupResult.AppendText("Part of Speech: " + entry.partOfSpeech + Environment.NewLine);
                rtbLookupResult.AppendText("Definition: " + entry.definition + Environment.NewLine);
                rtbLookupResult.AppendText("Example: " + entry.example + Environment.NewLine + Environment.NewLine);
            }
        }



        private int CompareWords(string word1, string word2)
        {
            // So sánh hai từ theo thứ tự từ điển, không phân biệt hoa thường
            return string.Compare(word1, word2, StringComparison.OrdinalIgnoreCase);
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


        private void SaveDictionaryToFile(string fileName)
        {
            if (entries.Length == 0)
            {
                MessageBox.Show("Từ điển trống! Không có từ để lưu vào file.");
                return;
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

                MessageBox.Show("Đã lưu từ điển vào file "+ fileName);
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
                }
            }

            MessageBox.Show("Đã tải dữ liệu từ file " + fileName);
        }

        private void ClearInputFields()
        {
            txtWord.Text = "";
        }



        private void RefreshEntryList()
        {
            lstEntries.Items.Clear(); // Clear the current items in the ListBox

            if (entries.Length == 0)
            {
                MessageBox.Show("Từ điển trống. Có thể bạn chưa chọn file tải dữ liệu!");
                return;
            }

            foreach (var entry in entries)
            {
                lstEntries.Items.Add(entry.word); // Add each word to the ListBox
            }
        }



        private void lstEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = lstEntries.SelectedIndex;
            if (selectedIndex >= 0)
            {
                DictionaryEntry selectedEntry = entries[selectedIndex];

                string word = selectedEntry.word;

                txtWord.Text = word;

                string partOfSpeech = selectedEntry.partOfSpeech;
                string definition = selectedEntry.definition;
                string example = selectedEntry.example;

                /*string entryDetails = $"Từ: {word}\nLoại từ: {partOfSpeech}\nĐịnh nghĩa:\n {definition}\nVí dụ:\n {example}";*/

                /*MessageBox.Show(entryDetails);*/

                string entryDetails = $"-Word: {word}\n\n-PartOfSpeech: {partOfSpeech}\n\n-Definition:\n {definition}\n\n-Example:\n {example}";

                rtbLookupResult.Text = entryDetails;
            }
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

        private void txtWord_TextChanged(object sender, EventArgs e)
        {
            string searchKeyword = txtWord.Text;
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                // Find the index of the first entry that starts with the search keyword
                int index = FindMatchingIndex(searchKeyword);
                if (index != -1)
                {
                    // Select the entry in the ListBox
                    lstEntries.SelectedIndex = index;
                    lstEntries.TopIndex = index;
                }
            }
            else
            {
                // Clear the selection when the search keyword is empty
                lstEntries.ClearSelected();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát khỏi chương trình không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close(); // Close the form
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            string word = txtWord.Text;
            DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa từ '{word}' không?", "Xác nhận xóa từ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                RemoveEntry(word);
                ClearInputFields();
            }
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
            frmEdit editForm = new frmEdit();

            // Pass the selected word from txtWord to the EditWordForm
            editForm.SelectedWord = txtWord.Text;

            // Show the EditWordForm
            editForm.ShowDialog();

            // After the EditWordForm is closed, update the value in txtWord
            txtWord.Text = editForm.SelectedWord;
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



/*Trong chương trình này, cấu trúc dữ liệu chính là một mảng entries của lớp DictionaryEntry, trong đó mỗi mục trong mảng đại diện cho một từ trong từ điển.

Các phương thức sắp xếp và tìm kiếm trong chương trình này sử dụng phương pháp tìm kiếm tuyến tính (linear search) và sắp xếp nổi bọt (bubble sort).

Tìm kiếm tuyến tính: Trong phương thức LookupWord, chúng ta duyệt qua từng mục trong mảng entries để tìm kiếm từ cần tra. 
Phương pháp tìm kiếm này có độ phức tạp O(n), trong đó n là số lượng từ trong từ điển. Điều này có nghĩa là thời gian tìm kiếm tăng theo tỉ lệ tuyến tính với số lượng từ.

Sắp xếp nổi bọt: Trong phương thức SortEntries, chúng ta sử dụng thuật toán sắp xếp nổi bọt để sắp xếp các mục trong mảng entries theo thứ tự từ điển. 
Thuật toán sắp xếp nổi bọt hoạt động bằng cách so sánh các cặp phần tử liền kề và hoán đổi chúng nếu cần, đến khi không còn phần tử nào cần hoán đổi nữa. 
Độ phức tạp của thuật toán sắp xếp nổi bọt là O(n^2), trong đó n là số lượng từ trong từ điển. Điều này có nghĩa là thời gian sắp xếp tăng theo tỉ lệ bình phương với số lượng từ.

Tóm lại, chương trình này sử dụng cấu trúc dữ liệu mảng và áp dụng các phương pháp tìm kiếm tuyến tính và sắp xếp nổi bọt để thao tác trên từ điển. 
Đây là những phương pháp đơn giản và hiệu quả trong trường hợp từ điển có số lượng từ nhỏ. 
Tuy nhiên, trong các ứng dụng thực tế với số lượng từ lớn, các thuật toán tìm kiếm và sắp xếp hiệu quả hơn như tìm kiếm nhị phân và sắp xếp nhanh có thể được áp dụng để cải thiện hiệu suất.*/