using System;
using System.Collections.Generic;

namespace PriceHawk.Models
{
    [Serializable]
    public class Product
    {
        public string Id;
        public string Name;
        public string Subtitle;       // e.g. "256GB · Natural Titanium · iOS 17"
        public string Category;
        public float Rating;
        public int ReviewCount;
        public string Emoji;          // display emoji / placeholder sprite key

        public List<StoreListing> Listings = new List<StoreListing>();
        public List<PricePoint>   PriceHistory1M = new List<PricePoint>();
        public List<PricePoint>   PriceHistory3M = new List<PricePoint>();
        public List<PricePoint>   PriceHistory6M = new List<PricePoint>();
        public List<PricePoint>   PriceHistory1Y = new List<PricePoint>();

        // Computed helpers
        public float CurrentBestPrice
        {
            get
            {
                float best = float.MaxValue;
                foreach (var l in Listings)
                    if (l.Price < best) best = l.Price;
                return best == float.MaxValue ? 0 : best;
            }
        }

        public float AllTimeLow
        {
            get
            {
                float low = float.MaxValue;
                foreach (var p in PriceHistory1Y)
                    if (p.Price < low) low = p.Price;
                return low == float.MaxValue ? CurrentBestPrice : low;
            }
        }

        public float AllTimeHigh
        {
            get
            {
                float high = 0;
                foreach (var p in PriceHistory1Y)
                    if (p.Price > high) high = p.Price;
                return high;
            }
        }

        public float Average90Day
        {
            get
            {
                if (PriceHistory3M == null || PriceHistory3M.Count == 0) return CurrentBestPrice;
                float sum = 0;
                foreach (var p in PriceHistory3M) sum += p.Price;
                return sum / PriceHistory3M.Count;
            }
        }
    }

    [Serializable]
    public class StoreListing
    {
        public string StoreName;
        public float  Price;
        public bool   IsBest;
        public string StoreColorHex;  // "#ff9900" etc.
        public string StoreEmoji;
    }

    [Serializable]
    public class PricePoint
    {
        public string Label;   // x-axis label ("Jan", "Week 1", etc.)
        public float  Price;
        public DateTime Date;
    }
}
