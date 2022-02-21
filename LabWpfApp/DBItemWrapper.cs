using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using DataBase;
using YOLOConsole.DataStructures;

namespace LabWpfApp
{
    public class DBItemWrapper : DBItem
    {
        string imgName;
        CroppedBitmap BitmapImg;

        public DBItemWrapper(YoloV4Result res)
        {
            X1 = res.BBox[0];
            X2 = res.BBox[1];
            Y1 = res.BBox[2];
            Y2 = res.BBox[3];
            Label = res.Label;
            Confidence = res.Confidence;
            imgName = res.ImgName;
            SetBitmapImg();
            Img = ConvertBitmapSrcToByte(BitmapImg);
        }

        private void SetBitmapImg()
        {
            var uri = new Uri(imgName, UriKind.RelativeOrAbsolute);
            var fileImg = new BitmapImage(uri);
            fileImg.Freeze();
            var rectangle = new Int32Rect((int)X1, (int)X2, (int)(Y1 - X1),
                (int)(Y2 - X2));
            BitmapImg = new CroppedBitmap(fileImg, rectangle);
            BitmapImg.Freeze();
        }

        public static byte[] ConvertBitmapSrcToByte(BitmapSource src)
        {
            var encoder = new JpegBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(src));
            using var ms = new MemoryStream();
            encoder.Save(ms);
            var byteImg = ms.ToArray();

            return byteImg;
        }
    }
}