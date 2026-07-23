using System;

namespace PriceHawk.Models
{
    public enum AlertStatus { Watching, Triggered, Paused }

    [Serializable]
    public class PriceAlert
    {
        public string      Id;
        public string      ProductId;
        public string      ProductName;
        public float       CurrentPrice;
        public float       TargetPrice;
        public string      NotifyEmail;
        public AlertStatus Status;
        public DateTime    CreatedAt;

        public bool IsTriggered(float livePrice) => livePrice <= TargetPrice;
    }

    [Serializable]
    public class Order
    {
        public string   Emoji;
        public string   Name;
        public string   DateLabel;   // "Jul 10 · Prime Delivery"
        public float    Price;
        public float    AmountSaved; // 0 if no saving
    }

    [Serializable]
    public class SpendData
    {
        public string Label;   // "Feb", "Mar" …
        public float  Amount;
    }
}
