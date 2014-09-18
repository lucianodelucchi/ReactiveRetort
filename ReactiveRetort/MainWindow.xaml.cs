using ReactiveRetort.ViewModels;
using System.Windows;
using ReactiveRetort.Services;
using MahApps.Metro.Controls;

namespace ReactiveRetort
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var converterService = new ImageConverterService();
            DataContext = new MainWindowViewModel(converterService); 
        }

        #region Code not following Rx
        private void Droparea_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Droparea_Drop(object sender, DragEventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).DraggedFolders.AddRange((string[])e.Data.GetData(DataFormats.FileDrop, false));
        }
        #endregion

    }
}
