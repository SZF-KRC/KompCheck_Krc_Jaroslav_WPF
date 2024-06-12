using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace KompCheck_Krc_Jaroslav_WPF.Functions
{
    public static class SaveDialog
    {
        /// <summary>
        /// Methode zum asynchronen Öffnen des Dialogfensters
        /// </summary>
        /// <returns>Der Pfad der spezifischen Datei</returns>
        public static Task<string> SaveFileAsync()
        {
            try
            {
                var taskCompletionSource = new TaskCompletionSource<string>();
                var thread = new Thread(() =>
                {
                    string result = SaveFile();
                    taskCompletionSource.SetResult(result);
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                return taskCompletionSource.Task;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return null;
        }

        /// <summary>
        /// Speicherdialoge öffnen
        /// </summary>
        /// <returns>Der Pfad der spezifischen Datei</returns>
        private static string SaveFile()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt"
                };

                return saveFileDialog.ShowDialog() == DialogResult.OK ? saveFileDialog.FileName : null;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return null;
        }
    }
}
