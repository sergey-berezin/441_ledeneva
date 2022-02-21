using System.Windows;
using System.Windows.Media.Imaging;

namespace YOLOConsole.DataStructures
{
    public class YoloV4Result
    {
        protected string imgName;
        float[] bbox;
        string label;
        float confidence;

        public string ImgName => imgName;

        /// <summary>
        /// x1, y1, x2, y2 in page coordinates.
        /// <para>left, top, right, bottom.</para>
        /// </summary>
        public float[] BBox => bbox;

        /// <summary>
        /// The Bbox category.
        /// </summary>
        public string Label => label;

        /// <summary>
        /// Confidence level.
        /// </summary>
        public float Confidence => confidence;

        public YoloV4Result(float[] bb, string lab, float conf)
        {
            bbox = bb;
            label = lab;
            confidence = conf;
        }

        public void SetImgName(string name)
        {
            imgName = name;
        }
    }
}
