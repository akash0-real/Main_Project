using UnityEngine;
using TMPro;
using PriceHawk.Managers;
using PriceHawk.Data;

namespace PriceHawk.UI
{
    /// <summary>
    /// Drives the Dashboard screen (KPI cards).
    /// Attach to the DashboardPanel GameObject.
    /// </summary>
    public class DashboardScreen : MonoBehaviour
    {
        [Header("KPI Labels")]
        public TMP_Text TotalSavedLabel;
        public TMP_Text TotalSavedChangeLabel;
        public TMP_Text ActiveAlertsLabel;
        public TMP_Text AlertsChangeLabel;
        public TMP_Text PriceDropsLabel;
        public TMP_Text PriceDropsChangeLabel;
        public TMP_Text AmazonSpendLabel;
        public TMP_Text AmazonSpendChangeLabel;

        void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            var s = AppManager.Instance.DashStats;

            if (TotalSavedLabel)       TotalSavedLabel.text       = FormatINR(s.TotalSaved);
            if (TotalSavedChangeLabel) TotalSavedChangeLabel.text = "↑ ₹1,200 this month";

            if (ActiveAlertsLabel)     ActiveAlertsLabel.text     = s.ActiveAlerts.ToString();
            if (AlertsChangeLabel)     AlertsChangeLabel.text     = $"{s.AlertsNearTarget} close to target";

            if (PriceDropsLabel)       PriceDropsLabel.text       = s.PriceDropsToday.ToString();
            if (PriceDropsChangeLabel) PriceDropsChangeLabel.text = $"↑ {s.WishlistDrops} on wishlist";

            if (AmazonSpendLabel)      AmazonSpendLabel.text      = FormatINR(s.AmazonSpend);
            if (AmazonSpendChangeLabel)AmazonSpendChangeLabel.text= "This month";
        }

        // ── Helpers ──────────────────────────────────────────────────────────────
        static string FormatINR(float val) =>
            "₹" + val.ToString("N0");
    }
}
