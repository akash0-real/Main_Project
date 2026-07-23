using System;
using System.Collections.Generic;
using PriceHawk.Models;

namespace PriceHawk.Data
{
    /// <summary>
    /// Provides all mock/static data that mirrors pricehawk-v3.html.
    /// Replace the bodies of these methods with real API calls when integrating a backend.
    /// </summary>
    public static class MockDataProvider
    {
        // ── PRODUCTS ────────────────────────────────────────────────────────────
        public static List<Product> GetProducts()
        {
            return new List<Product>
            {
                BuildIPhone15ProMax(),
                BuildIPhone15Pro(),
                BuildIPhone15Plus(),
                BuildIPhone15(),
            };
        }

        static Product BuildIPhone15ProMax()
        {
            var p = new Product
            {
                Id          = "iphone15promax",
                Name        = "Apple iPhone 15 Pro Max",
                Subtitle    = "256GB · Natural Titanium · iOS 17",
                Category    = "Smartphones",
                Rating      = 4.8f,
                ReviewCount = 12400,
                Emoji       = "📱",
            };

            p.Listings = new List<StoreListing>
            {
                new StoreListing { StoreName="Amazon",      Price=134900, IsBest=true,  StoreColorHex="#ff9900", StoreEmoji="🟠" },
                new StoreListing { StoreName="Flipkart",    Price=138000, IsBest=false, StoreColorHex="#2874f0", StoreEmoji="🔵" },
                new StoreListing { StoreName="Apple Store", Price=159900, IsBest=false, StoreColorHex="#888888", StoreEmoji="🍎" },
                new StoreListing { StoreName="Croma",       Price=154990, IsBest=false, StoreColorHex="#9b59b6", StoreEmoji="📦" },
                new StoreListing { StoreName="Reliance",    Price=151900, IsBest=false, StoreColorHex="#e74c3c", StoreEmoji="🏪" },
            };

            // 1M price history (matches HTML priceData['1M'])
            p.PriceHistory1M = BuildPricePoints(
                new float[]{ 142000,139000,141000,138500,136000,137000,134900 },
                new string[]{ "Jul 1","Jul 5","Jul 9","Jul 13","Jul 17","Jul 21","Jul 23" });

            // 3M price history
            p.PriceHistory3M = BuildPricePoints(
                new float[]{ 149000,146000,143000,141000,139000,137000,136000,134900 },
                new string[]{ "May","May-2","Jun","Jun-2","Jul","Jul-2","Jul-3","Now" });

            // 6M price history
            p.PriceHistory6M = BuildPricePoints(
                new float[]{ 159900,155000,150000,146000,142000,139000,136000,134900 },
                new string[]{ "Jan","Feb","Mar","Apr","May","Jun","Jul","Now" });

            // 1Y price history
            p.PriceHistory1Y = BuildPricePoints(
                new float[]{ 159900,158000,155000,150000,148000,145000,142000,140000,138000,136000,135000,134900 },
                new string[]{ "Aug","Sep","Oct","Nov","Dec","Jan","Feb","Mar","Apr","May","Jun","Jul" });

            return p;
        }

        static Product BuildIPhone15Pro()
        {
            var p = new Product { Id="iphone15pro", Name="iPhone 15 Pro", Subtitle="128GB · Black Titanium · iOS 17",
                                  Category="Smartphones", Rating=4.7f, ReviewCount=9200, Emoji="📱" };
            p.Listings = new List<StoreListing>
            {
                new StoreListing { StoreName="Amazon",   Price=119900, IsBest=true,  StoreColorHex="#ff9900", StoreEmoji="🟠" },
                new StoreListing { StoreName="Flipkart", Price=121900, IsBest=false, StoreColorHex="#2874f0", StoreEmoji="🔵" },
                new StoreListing { StoreName="Croma",    Price=124990, IsBest=false, StoreColorHex="#9b59b6", StoreEmoji="📦" },
            };
            p.PriceHistory1M = BuildPricePoints(
                new float[]{ 134900,132000,130000,127000,122000,120000,119900 },
                new string[]{ "Jul 1","Jul 5","Jul 9","Jul 13","Jul 17","Jul 21","Jul 23" });
            p.PriceHistory1Y = p.PriceHistory1M; // simplified
            return p;
        }

        static Product BuildIPhone15Plus()
        {
            var p = new Product { Id="iphone15plus", Name="iPhone 15 Plus", Subtitle="128GB · Pink · iOS 17",
                                  Category="Smartphones", Rating=4.6f, ReviewCount=5400, Emoji="📱" };
            p.Listings = new List<StoreListing>
            {
                new StoreListing { StoreName="Amazon",   Price=79900, IsBest=true,  StoreColorHex="#ff9900", StoreEmoji="🟠" },
                new StoreListing { StoreName="Flipkart", Price=81900, IsBest=false, StoreColorHex="#2874f0", StoreEmoji="🔵" },
            };
            p.PriceHistory1M = BuildPricePoints(
                new float[]{ 89900,87000,85000,83000,81000,80000,79900 },
                new string[]{ "Jul 1","Jul 5","Jul 9","Jul 13","Jul 17","Jul 21","Jul 23" });
            p.PriceHistory1Y = p.PriceHistory1M;
            return p;
        }

        static Product BuildIPhone15()
        {
            var p = new Product { Id="iphone15", Name="iPhone 15", Subtitle="128GB · Blue · iOS 17",
                                  Category="Smartphones", Rating=4.5f, ReviewCount=18000, Emoji="📱" };
            p.Listings = new List<StoreListing>
            {
                new StoreListing { StoreName="Amazon",   Price=69900, IsBest=true,  StoreColorHex="#ff9900", StoreEmoji="🟠" },
                new StoreListing { StoreName="Flipkart", Price=71900, IsBest=false, StoreColorHex="#2874f0", StoreEmoji="🔵" },
            };
            p.PriceHistory1M = BuildPricePoints(
                new float[]{ 79900,77000,75000,73000,71000,70000,69900 },
                new string[]{ "Jul 1","Jul 5","Jul 9","Jul 13","Jul 17","Jul 21","Jul 23" });
            p.PriceHistory1Y = p.PriceHistory1M;
            return p;
        }

        static List<PricePoint> BuildPricePoints(float[] prices, string[] labels)
        {
            var list = new List<PricePoint>();
            for (int i = 0; i < prices.Length; i++)
                list.Add(new PricePoint { Price = prices[i], Label = labels[i], Date = DateTime.Now });
            return list;
        }

        // ── BARCODE DATABASE ────────────────────────────────────────────────────
        public static Dictionary<string, ScanResult> GetBarcodeDatabase()
        {
            return new Dictionary<string, ScanResult>
            {
                ["8901234567890"] = new ScanResult
                {
                    Barcode="8901234567890", ProductName="Samsung Galaxy Buds2 Pro",
                    Category="Electronics · Wireless Audio", Price=11999,
                    AmazonPrice=11999, FlipkartPrice=12499, CromaPrice=13500,
                    Verdict="✅ Good Price", TimingNote="Price stable for 2 weeks", TimingIcon="🕐"
                },
                ["4006381333931"] = new ScanResult
                {
                    Barcode="4006381333931", ProductName="boAt Airdopes 141",
                    Category="Electronics · TWS Earbuds", Price=1299,
                    AmazonPrice=1299, FlipkartPrice=1399, CromaPrice=1499,
                    Verdict="🔥 All-Time Low!", TimingNote="Best price ever seen", TimingIcon="🎯"
                },
                ["5901234123457"] = new ScanResult
                {
                    Barcode="5901234123457", ProductName="Sony WH-1000XM5",
                    Category="Electronics · Headphones", Price=24990,
                    AmazonPrice=24990, FlipkartPrice=25999, CromaPrice=27990,
                    Verdict="⚠️ Slightly above avg", TimingNote="Wait for sale", TimingIcon="⏳"
                },
            };
        }

        // ── ACTIVE ALERTS ────────────────────────────────────────────────────────
        public static List<PriceAlert> GetDefaultAlerts()
        {
            return new List<PriceAlert>
            {
                new PriceAlert { Id="a1", ProductId="iphone15promax", ProductName="iPhone 15 Pro Max",
                    CurrentPrice=134900, TargetPrice=120000, Status=AlertStatus.Watching, CreatedAt=DateTime.Now },
                new PriceAlert { Id="a2", ProductId="iphone15pro",    ProductName="iPhone 15 Pro",
                    CurrentPrice=119900, TargetPrice=109900, Status=AlertStatus.Watching, CreatedAt=DateTime.Now },
                new PriceAlert { Id="a3", ProductId="iphone15",       ProductName="iPhone 15",
                    CurrentPrice=69900,  TargetPrice=64900,  Status=AlertStatus.Watching, CreatedAt=DateTime.Now },
                new PriceAlert { Id="a4", ProductId="iphone15plus",   ProductName="iPhone 15 Plus",
                    CurrentPrice=79900,  TargetPrice=74900,  Status=AlertStatus.Watching, CreatedAt=DateTime.Now },
            };
        }

        // ── RECENT ORDERS ────────────────────────────────────────────────────────
        public static List<Order> GetRecentOrders()
        {
            return new List<Order>
            {
                new Order { Emoji="📚", Name="NCERT Physics Class 12",        DateLabel="Jul 10 · Prime Delivery", Price=280,  AmountSaved=0    },
                new Order { Emoji="🎧", Name="boAt Bassheads 100 Wired",      DateLabel="Jul 7 · Saved ₹120",      Price=349,  AmountSaved=120  },
                new Order { Emoji="💡", Name="Syska 9W LED Bulb (6-pack)",    DateLabel="Jul 5 · Hostel room",     Price=399,  AmountSaved=0    },
                new Order { Emoji="🖊️", Name="Classmate Notebook (10-pack)", DateLabel="Jul 3 · Saved ₹80",       Price=420,  AmountSaved=80   },
                new Order { Emoji="📱", Name="Phone Case for Redmi Note 12",  DateLabel="Jul 1 · Lightning Deal",  Price=199,  AmountSaved=0    },
                new Order { Emoji="⌨️", Name="Logitech K380 Keyboard",       DateLabel="Jun 28 · Saved ₹1,200",   Price=2499, AmountSaved=1200 },
            };
        }

        // ── MONTHLY SPEND ────────────────────────────────────────────────────────
        public static List<SpendData> GetMonthlySpend()
        {
            return new List<SpendData>
            {
                new SpendData { Label="Feb", Amount=7200  },
                new SpendData { Label="Mar", Amount=9400  },
                new SpendData { Label="Apr", Amount=6800  },
                new SpendData { Label="May", Amount=11200 },
                new SpendData { Label="Jun", Amount=9600  },
                new SpendData { Label="Jul", Amount=12000 },
            };
        }

        // ── DASHBOARD KPIs ───────────────────────────────────────────────────────
        public static DashboardStats GetDashboardStats()
        {
            return new DashboardStats
            {
                TotalSaved      = 6240,
                ActiveAlerts    = 4,
                PriceDropsToday = 12,
                WishlistDrops   = 3,
                AmazonSpend     = 12000,
                AlertsNearTarget= 1,
            };
        }
    }

    [Serializable]
    public class DashboardStats
    {
        public float TotalSaved;
        public int   ActiveAlerts;
        public int   PriceDropsToday;
        public int   WishlistDrops;
        public float AmazonSpend;
        public int   AlertsNearTarget;
    }
}
