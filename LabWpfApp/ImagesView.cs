using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace LabWpfApp
{
    public class ImagesView : DbViewModel
    {
        private string _selected;

        public ImagesView(AppViewModel vm) : base(vm)
        {
        }

        public void SetSelected(string imgType)
        {
            _selected = imgType;
        }

        public override IEnumerator<BitmapImage> GetEnumerator()
        {
            return Vm.DbManager.GetImages(_selected).Select(ConvertByteToBitmapImg).GetEnumerator();
        }
    }
}