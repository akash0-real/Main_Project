using System;

namespace PriceHawk.Models
{
    [Serializable]
    public class ScanResult
    {
        public string Barcode;
        public string ProductName;
        public string Category;
        public float  Price;
        public float  AmazonPrice;
        public float  FlipkartPrice;
        public float  CromaPrice;
        public string Verdict;
        public string TimingNote;
        public string TimingIcon;
        public DateTime ScannedAt;
    }
}
