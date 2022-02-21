using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LabWpfApp
{
    public abstract class DbViewModel : INotifyCollectionChanged, IEnumerable
    {
        protected DbViewModel(AppViewModel vm)
        {
            Vm = vm;
            Vm.DbManager.DataChanged += RaiseCollectionChanged;
        }

        public AppViewModel Vm { get; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public static BitmapImage ConvertByteToBitmapImg(byte[] arr)
        {
            using var ms = new MemoryStream(arr);
            var img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.StreamSource = ms;
            img.EndInit();
            return img;
        }

        public void RaiseCollectionChanged()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                CollectionChanged?.Invoke(this,
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });
        }

        public abstract IEnumerator GetEnumerator();
    }
}