using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Forms;
using DataBase;
using YOLOConsole;
using YOLOConsole.DataStructures;

namespace LabWpfApp
{
    public class AppViewModel : INotifyPropertyChanged
    {
        private bool _inProgress;
        private bool _isWritingToDb;
        private string _inputFolder = "";
        private BufferBlock<IReadOnlyList<YoloV4Result>> _output = new();

        private CancellationTokenSource _tokenSrc = new();

        public AppViewModel()
        {
            ImgTypesCollection = new ImgTypesView(this);
            ImagesCollection = new ImagesView(this);
        }

        public string InputFolder
        {
            get => _inputFolder;
            set
            {
                _inputFolder = value;
                RaisePropertyChanged();
            }
        }

        public DataBaseManager DbManager { get; } = new();

        public ImgTypesView ImgTypesCollection { get; }

        public ImagesView ImagesCollection { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void BrowseHandler()
        {
            var browseDlg = new FolderBrowserDialog();
            browseDlg.SelectedPath = @"C:\Users\kuris\Documents\GitHub\441_ledeneva\YOLOConsole\Assets";
            if (browseDlg.ShowDialog() == DialogResult.OK)
                InputFolder = browseDlg.SelectedPath;
        }

        public async Task RunHandler()
        {
            if (_inputFolder == "" || _inProgress)
                return;

            await ClassifyImages();
        }

        public void StopHandler()
        {
            _tokenSrc.Cancel();
        }

        public void ClearHandler()
        {
            DbManager.Clear();
        }

        public void SelectionChangedHandler(string arg)
        {
            if (arg == null)
                return;

            ImagesCollection.SetSelected(arg[(arg.IndexOf(' ') + 1)..]);
            if (!_isWritingToDb)
                ImagesCollection.RaiseCollectionChanged();
        }

        private async Task ClassifyImages()
        {
            _inProgress = true;
            _output = new BufferBlock<IReadOnlyList<YoloV4Result>>();
            _tokenSrc = new CancellationTokenSource();
            Classifier.ClassifyAsync(_inputFolder, _tokenSrc.Token, _output);
            await TypesProcessingAsync(_output);
            _inProgress = false;
        }

        private async Task TypesProcessingAsync(ISourceBlock<IReadOnlyList<YoloV4Result>> src)
        {
            while (await src.OutputAvailableAsync())
            {
                var data = src.Receive();
                foreach (var item in data)
                {
                    _isWritingToDb = true;
                    if (_tokenSrc.IsCancellationRequested) break;
                    await DbManager.AddAsync(new DBItemWrapper(item));
                }

                if (_tokenSrc.IsCancellationRequested) break;
            }

            _isWritingToDb = false;
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}