using ReactiveUI;
using Retort.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reactive.Linq;
using ReactiveUI.Xaml;

namespace Retort
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IViewFor<MainWindowViewModel>
    {
        public MainWindow()
        {
            ViewModel = new MainWindowViewModel();

            InitializeComponent();

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

        public MainWindowViewModel ViewModel
        {
            get { return (MainWindowViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MainWindowViewModel), typeof(MainWindow), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainWindowViewModel)value; }
        }
    }
}
