namespace DataBase
{
    public class DBItem
    {
        public int DBItemId { get; set; }
        public float X1 { get; set; }
        public float X2 { get; set; }
        public float Y1 { get; set; }
        public float Y2 { get; set; }
        public string Label { get; set; }
        public float Confidence { get; set; }
        public byte[] Img { get; set; }
    }
}
