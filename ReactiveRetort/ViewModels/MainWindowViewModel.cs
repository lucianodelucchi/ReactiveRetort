using ReactiveRetort.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.IO;
using System.Diagnostics;
using ReactiveRetort.Services;
using System.Threading;
using System.Reactive;

namespace ReactiveRetort.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel(ImageConverterService converterService)
        {
            this.converterService = converterService;

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

            var canExecuteCompress = this.WhenAnyValue(x => x.ImagesCount)
                .Select(x => x > 0);

            //commands
            Compress = ReactiveCommand.CreateAsyncObservable(
                canExecuteCompress,
                _ => {
                    var ret = ConvertImages().TakeUntil(CancelConversion);
                    return ret; 
                });

            isBusy = Compress.IsExecuting.ToProperty(this, x => x.IsBusy);

            Compress.ObserveOn(RxApp.MainThreadScheduler).Subscribe(x => Images.Remove(x));

            Compress.ThrownExceptions
                .Select(ex => new UserError(ex.Message, ex.Source))
                .Subscribe(ex => Debug.WriteLine(ex.ErrorMessage));

            var canExecuteCancelConversion = this.WhenAnyValue(x => x.ImagesCount, x => x.IsBusy, (count, busy) => count > 0 && busy)
                .Select(x => x);

            CancelConversion = ReactiveCommand.CreateAsyncObservable(canExecuteCancelConversion, _ => { return Observable.Return<Unit>(Unit.Default); });
            CancelConversion.ThrownExceptions
                .Select(ex => new UserError(ex.Message, ex.Source))
                .Subscribe(ex => Debug.WriteLine(ex.ErrorMessage));

            CancelConversion.Subscribe(_ => Debug.WriteLine("executed"));
        }

        ImageConverterService converterService;

        public ReactiveCommand<OwnImage> Compress { get; private set; }
        public ReactiveCommand<Unit> CancelConversion { get; private set; }

        public readonly ReactiveList<string> DraggedFolders = new ReactiveList<string>();

        public ReactiveList<OwnImage> Images { get; protected set; }

        private readonly ObservableAsPropertyHelper<int> imagesCount;
        public int ImagesCount
        {
            get { return imagesCount.Value; }
        }

        private readonly ObservableAsPropertyHelper<bool> isBusy;
        public bool IsBusy
        {
            get { return isBusy.Value; }
        }

        private IObservable<IList<OwnImage>> LoadImagesFromFolder(string draggedFolder)
        {
            return Observable.Return(draggedFolder)
                                .SelectMany(x => new DirectoryInfo(x).GetFiles("*.jpg"))
                                .Select(x => OwnImage.CreateFrom(x))
                                .ToList();
        }

        private IObservable<OwnImage> ConvertImages()
        {
            return this.converterService.ConvertImages(Images.ToObservable());
        }
    }
}
