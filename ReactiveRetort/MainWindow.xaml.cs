using Retort.ViewModels;
using System.Windows;
using Convertidor.Services;

namespace Retort
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var converterService = new ImageConverterService();
            DataContext = new MainWindowViewModel(converterService); 

           

            this.BindCommand(ViewModel, x => x.Compress);
            this.BindCommand(ViewModel, x => x.CancelConversion);

            this.OneWayBind(ViewModel, x => x.Images, x => x.Images.ItemsSource);
            this.OneWayBind(ViewModel, x => x.ImagesCount, x => x.ImageCount.Text);

            this.OneWayBind(ViewModel, x => x.IsBusy, x => x.ProgressGroup.Visibility, () => false);
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
            this.ViewModel.DraggedFolders.AddRange((string[])e.Data.GetData(DataFormats.FileDrop, false));
        }
        #endregion

    }
}
