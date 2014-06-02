using Convertidor.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.IO;
using System.Diagnostics;
using Convertidor.Services;
using System.Threading;
using System.Reactive;

namespace Retort.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        ImageConverterService converterService;

        public ReactiveCommand Compress { get; protected set; }
        public ReactiveCommand CancelConversion { get; protected set; }

        public readonly ReactiveList<string> DraggedFolders = new ReactiveList<string>();

        public ReactiveList<OwnImage> Images { get; protected set; }

        ObservableAsPropertyHelper<int> imagesCount;
        public int ImagesCount
        {
            get { return imagesCount.Value; }
        }

        ObservableAsPropertyHelper<bool> isBusy;
        public bool IsBusy
        {
            get { return isBusy.Value; }
        }

        public MainWindowViewModel()
        {
            this.converterService = new ImageConverterService();

            Images = new ReactiveList<OwnImage>();

            // when something changes in the draggedFolders collection
            // keep an eye on ShouldReset
            var addedOrRemoved = Observable.Merge(
                DraggedFolders.ItemsAdded,
                DraggedFolders.ItemsRemoved
                );

            DraggedFolders.ItemsAdded
                .SelectMany(x => LoadImagesFromFolder(x))
                .Subscribe(x => Images.AddRange(x));

            imagesCount = Images.CountChanged.ToProperty(this, x => x.ImagesCount);

            //commands
            Compress = this.WhenAnyValue(x => x.ImagesCount, x => x > 0).ToCommand();
            
            isBusy = Compress.IsExecuting.ToProperty(this, x => x.IsBusy);

            var execution = Compress.RegisterAsync(x => ConvertImages(x).TakeUntil(CancelConversion));
            execution.Subscribe(x => Images.Remove(x));

            Compress.ThrownExceptions
                .Select(ex => new UserError(ex.Message, ex.Source))
                .Subscribe(ex => Debug.WriteLine(ex.ErrorMessage));

            CancelConversion = this.WhenAnyValue(x => x.ImagesCount, x => x.IsBusy, (count, busy) => count > 0 && busy).ToCommand();
            CancelConversion.ThrownExceptions
                .Select(ex => new UserError(ex.Message, ex.Source))
                .Subscribe(ex => Debug.WriteLine(ex.ErrorMessage));
        }

        private IObservable<IList<OwnImage>> LoadImagesFromFolder(string draggedFolder)
        {
            return Observable.Return(draggedFolder)
                                .SelectMany(x => new DirectoryInfo(x).GetFiles("*.jpg"))
                                .Select(x => OwnImage.CreateFrom(x))
                                .ToList();
        }

        private IObservable<OwnImage> ConvertImages(object o)
        {
            return this.converterService.ConvertImages(Images.ToObservable());
        }

    }
}
