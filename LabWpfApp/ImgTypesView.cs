using System.Collections.Generic;

namespace LabWpfApp
{
    public class ImgTypesView : DbViewModel
    {
        public ImgTypesView(AppViewModel vm) : base(vm)
        {
        }

        public override IEnumerator<string> GetEnumerator()
        {
            return Vm.DbManager.GetImgTypes().GetEnumerator();
        }
    }
}