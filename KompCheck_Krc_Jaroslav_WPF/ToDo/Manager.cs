using KompCheck_Krc_Jaroslav_WPF.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using ListBox = System.Windows.Controls.ListBox;

namespace KompCheck_Krc_Jaroslav_WPF.ToDo
{
    public static class Manager
    {
        private static int _linesBook1;
        private static int _linesBook2;

        private static int _wordsBook1;
        private static int _wordsBook2;

        private static string book1Path;
        private static string book2Path;

        private static Dictionary<string, int> _top20Book1 = new Dictionary<string, int>();
        private static Dictionary<string, int> _top20Book2 = new Dictionary<string, int>();

        private static Dictionary<string, int> _allWordsBook1 = new Dictionary<string, int>();
        private static Dictionary<string, int> _allWordsBook2 = new Dictionary<string, int>();


        private static int uniqueWordsBook1;
        private static int uniqueWordsBook2;
        private static int allContainsWords;
        private static double percentContains;

        public static System.Windows.Controls.ProgressBar Bar1 {  get; set; }
        public static System.Windows.Controls.ProgressBar Bar2 {  get; set; }

        public static TextBlock Progress1 { get; set; }
        public static TextBlock Progress2 { get; set; }

        public static  TextBlock Name1 { get; set; }
        public static TextBlock Name2 { get; set; }
        public static TextBlock Name3 { get; set; }
       
        public static ListBox FirstList { get; set; }
        public static ListBox SecondList { get; set; }
        public static ListBox ThirdList { get; set; }

        /// <summary>
        /// Zwei Bücher gleichzeitig asynchron laden.
        /// </summary>
        /// <returns>Wir geben den Faden lose zurück</returns>
        public static async Task EnterBooks()
        {
            try
            {
                CleanWindow();
                MessageBox.Show("Enter first book please...", "First book", MessageBoxButtons.OK, MessageBoxIcon.Information);
                book1Path = await OpenDialog.OpenFileAsync("Enter first book");
                MessageBox.Show("Enter second book please...", "First book", MessageBoxButtons.OK, MessageBoxIcon.Information);
                book2Path = await OpenDialog.OpenFileAsync("Enter second book");

                if (book1Path != null && book2Path != null)
                {
                    var book1Task =  ProcessBooks(book1Path, 1);
                    var book2Task =  ProcessBooks(book2Path, 2);

                    var result = await Task.WhenAll(book1Task,book2Task);

                    _linesBook1 = result[0].lines;
                    _wordsBook1 = result[0].words;
                    _top20Book1 = result[0].top20;
                    _allWordsBook1 = result[0].allWords;
                    Name1.Text ="* First book loaded successfully.";

                    _linesBook2 = result[1].lines;
                    _wordsBook2 = result[1].words;
                    _top20Book2 = result[1].top20;
                    _allWordsBook2 = result[1].allWords;
                    Name2.Text = "* Second book loaded successfully.";
                  
                    CountPercentMatches();
                }
                else
                {
                    if (book1Path == null) { Name1.Text="--- Failed to load the first book. ---"; }

                    if (book2Path == null) { Name2.Text = "--- Failed to load the second book. ---"; }
                }
            }
            catch (FileNotFoundException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        /// <summary>
        /// Öffnen Sie ein bestimmtes Buch und berechnen Sie die Wortzeile und Prozentsätze
        /// </summary>
        /// <param name="bookPath">Pfad eines Buches</param>
        /// <param name="bookNumber">Buchnummerierung</param>
        /// <returns>Gibt die Gesamtzahl der Zeilen, Wörter, ein Wörterbuch mit den 20 am häufigsten verwendeten Wörtern und ein Wörterbuch mit allen Wörtern zurück</returns>
        private static async Task<(int lines, int words, Dictionary<string, int> top20, Dictionary<string, int> allWords)> ProcessBooks(string bookPath, int bookNumber)
        {
            try
            {
                int lines = 0;
                int words = 0;
                int percent = 0;

                Dictionary<string, int> wordCounts = new Dictionary<string, int>();
                int allLines = File.ReadAllLines(bookPath).Count();

                using (StreamReader reader = new StreamReader(bookPath))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        lines++;
                        string pattern = @"\b\w+\b";
                        MatchCollection matches = Regex.Matches(line, pattern);
                        words += matches.Count;

                        foreach (Match match in matches)
                        {
                            string word = match.Value.ToLower();
                            if (wordCounts.ContainsKey(word)) { wordCounts[word]++; }
                            else { wordCounts[word] = 1; }
                        }
                        percent = (int)Math.Round((double)(100 * lines) / allLines);

                        switch (bookNumber)
                        {
                            case 1: Progress1.Text = $"Loading: {percent}%"; Bar1.Value = percent ; break;
                            case 2: Progress2.Text = $"Loading: {percent}%"; Bar2.Value = percent; break;                          
                        }
                    }
                }
                return (lines, words, wordCounts
                                      .OrderByDescending(top20 => top20.Value)
                                      .Take(20)
                                      .ToDictionary(top20 => top20.Key, top20 => top20.Value), wordCounts);
            }
            catch(FileNotFoundException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return (0, 0, null, null);
        }

        /// <summary>
        /// Buchdaten in der List Box anzeigen
        /// </summary>
        public static void PrintResult()
        {
            CleanWindow();
            if (string.IsNullOrEmpty(book1Path) || string.IsNullOrEmpty(book2Path))
            {
                MessageBox.Show("Books have not been entered", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            FileInfo infoBook1 = new FileInfo(book1Path);
            FileInfo infoBook2 = new FileInfo(book2Path);
      
            Name1.Text = $"Book1: {infoBook1.Name}";
            FirstList.Items.Add($"Path: {book1Path}\nLines: {_linesBook1}\nWords: {_wordsBook1}\nTop20 words:");

            foreach (var x in _top20Book1.OrderByDescending(x => x.Value)) { FirstList.Items.Add($"{x.Key} : {x.Value}"); }

            Name2.Text = $"Book2: {infoBook2.Name}";
            SecondList.Items.Add($"Path: {book2Path}\nLines: {_linesBook2}\nWords: {_wordsBook2}\nTop20 words:");
            foreach (var x in _top20Book2.OrderByDescending(x => x.Value)) { SecondList.Items.Add($"{x.Key} : {x.Value}"); }

            Name3.Text = "Extra info:";
            ThirdList.Items.Add($"All common words: {allContainsWords}\nUnique words of the first book: {uniqueWordsBook1}\nUnique words of the second book: {uniqueWordsBook2}\n{percentContains}% of the words match");        
        }

        /// <summary>
        /// Speichern eines Textdokuments mit allen Daten
        /// </summary>
        public static async void Save()
        {
            try
            {
                if (string.IsNullOrEmpty(book1Path) || string.IsNullOrEmpty(book2Path))
                {
                    MessageBox.Show("Books have not been entered", "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string savePath = await SaveDialog.SaveFileAsync();
                if (savePath != null)
                {
                    using (StreamWriter writer = new StreamWriter(savePath))
                    {
                        await writer.WriteLineAsync($"Book 1:\nPath: {book1Path}\nLines: {_linesBook1}\nWords: {_wordsBook1}\nTop20 words:");
                        foreach (var x in _top20Book1.OrderByDescending(x => x.Value)) { await writer.WriteLineAsync($"{x.Key} : {x.Value}"); }

                        await writer.WriteLineAsync($"\nBook 2:\nPath: {book2Path}\nLines: {_linesBook2}\nWords: {_wordsBook2}\nTop20 words:");
                        foreach (var x in _top20Book2.OrderByDescending(x => x.Value)) { await writer.WriteLineAsync($"{x.Key} : {x.Value}"); }

                        await writer.WriteLineAsync($"\n\nAll common words: {allContainsWords}\nUnique words of the first book: {uniqueWordsBook1}\nUnique words of the second book: {uniqueWordsBook2}\n\t{percentContains}% of the words match\n\n");
                    }
                    Name3.Text = "Data saved successfully";               
                }
            }
            catch (IOException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        /// <summary>
        /// Erkennung einzigartiger Wörter, aller gängigen Wörter und anschließende Anzeige auf der Konsole
        /// </summary>
        private static void CountPercentMatches()
        {
            uniqueWordsBook1 = _allWordsBook1.Keys.Count;
            uniqueWordsBook2 = _allWordsBook2.Keys.Count;
            allContainsWords = _allWordsBook1.Where(x => _allWordsBook2.ContainsKey(x.Key)).Count();
            percentContains = 0;

            if (uniqueWordsBook1 > uniqueWordsBook2)
            {
                percentContains = Math.Round((double)(allContainsWords * 100) / uniqueWordsBook1);
            }
            else
            {
                percentContains = Math.Round((double)(allContainsWords * 100) / uniqueWordsBook2);
            }
        }

        /// <summary>
        /// Reinigung aller Ausgänge
        /// </summary>
        private static void CleanWindow()
        {
            FirstList.Items.Clear();
            SecondList.Items.Clear();
            ThirdList.Items.Clear();

            Name1.Text = string.Empty;
            Name2.Text = string.Empty;
            Name3.Text = string.Empty;

            Progress1.Text = string.Empty;
            Progress2.Text = string.Empty;

            Bar1.Value = 0;
            Bar2.Value = 0;
        }
    }
}
