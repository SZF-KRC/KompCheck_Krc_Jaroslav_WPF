using KompCheck_Krc_Jaroslav_WPF.ToDo;
using System.Windows;

namespace KompCheck_Krc_Jaroslav_WPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Manager.FirstList = lstFirst;
            Manager.SecondList = lstSecond;
            Manager.ThirdList = lstThird;

            Manager.Progress1 = txtbProgress1;
            Manager.Progress2 = txtbProgress2;

            Manager.Name1 = txtbName1;
            Manager.Name2 = txtbName2;
            Manager.Name3 = txtbName3;

            Manager.Bar1 = pbBook1;
            Manager.Bar2 = pbBook2;

        }

        private async void btnAddBooks_Click(object sender, RoutedEventArgs e)
        {
           await Manager.EnterBooks();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Manager.PrintResult();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Manager.Save();
        }
    }
}
