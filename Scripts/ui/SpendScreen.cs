using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PriceHawk.Models;
using PriceHawk.Managers;

namespace PriceHawk.UI
{
    /// <summary>
    /// Drives the Spend Lens screen.
    /// </summary>
    public class SpendScreen : MonoBehaviour
    {
        // ── Summary Row ──────────────────────────────────────────────────────────
        [Header("Summary")]
        public TMP_Text TotalSpendLabel;
        public TMP_Text TotalOrdersLabel;
        public TMP_Text TotalSavedLabel;
        public TMP_Text AvgOrderLabel;

        // ── Store Breakdown Bars ─────────────────────────────────────────────────
        [Header("Store Breakdown")]
        public Transform SpendBarsContainer;
        public GameObject SpendBarRowPrefab;

        // ── Budget Tracker ────────────────────────────────────────────────────────
        [Header("Budget Tracker")]
        public TMP_Text   BudgetSpentLabel;
        public TMP_Text   BudgetTotalLabel;
        public TMP_Text   BudgetRemainingLabel;
        public Slider     BudgetSlider;
        public TMP_Text   BudgetFootnoteLabel;

        // ── Recent Orders ─────────────────────────────────────────────────────────
        [Header("Recent Orders")]
        public Transform OrderListContainer;
        public GameObject OrderItemPrefab;

        // ── Monthly Bars Chart ────────────────────────────────────────────────────
        [Header("Monthly Bar Chart")]
        public Transform MonthlyBarsContainer;
        public GameObject MonthlyBarPrefab;

        void OnEnable()
        {
            PopulateSummary();
            PopulateStoreBars();
            PopulateBudget();
            PopulateRecentOrders();
            PopulateMonthlyBars();
        }

        void PopulateSummary()
        {
            // Derived from mock orders
            var orders = AppManager.Instance.RecentOrders;
            float total = 0, saved = 0;
            foreach (var o in orders) { total += o.Price; saved += o.AmountSaved; }

            SetText(TotalSpendLabel,  "₹" + total.ToString("N0"));
            SetText(TotalOrdersLabel, orders.Count.ToString());
            SetText(TotalSavedLabel,  "₹" + saved.ToString("N0"));
            SetText(AvgOrderLabel,    orders.Count > 0 ? "₹" + (total / orders.Count).ToString("N0") : "—");
        }

        void PopulateStoreBars()
        {
            if (SpendBarsContainer == null || SpendBarRowPrefab == null) return;
            foreach (Transform c in SpendBarsContainer) Destroy(c.gameObject);

            var storeData = new List<(string name, float amount, string colorHex)>
            {
                ("Amazon",   8400f, "#ff9900"),
                ("Flipkart", 2100f, "#2874f0"),
                ("Croma",    1500f, "#9b59b6"),
            };

            float maxAmt = 0;
            foreach (var s in storeData) if (s.amount > maxAmt) maxAmt = s.amount;

            foreach (var s in storeData)
            {
                var go  = Instantiate(SpendBarRowPrefab, SpendBarsContainer);
                var row = go.GetComponent<SpendBarRowView>();
                if (row != null)
                    row.Populate(s.name, s.amount, s.amount / maxAmt, s.colorHex, maxAmt);
            }
        }

        void PopulateBudget()
        {
            float spent     = 12000f;
            float budgetCap = 15000f;
            float remaining = budgetCap - spent;
            float pct       = spent / budgetCap;

            SetText(BudgetSpentLabel,     "₹" + spent.ToString("N0"));
            SetText(BudgetTotalLabel,     "₹" + budgetCap.ToString("N0"));
            SetText(BudgetRemainingLabel, "₹" + remaining.ToString("N0"));
            SetText(BudgetFootnoteLabel,  $"{pct*100:0}% of budget used · 12 days remaining");

            if (BudgetSlider != null) BudgetSlider.value = pct;
        }

        void PopulateRecentOrders()
        {
            if (OrderListContainer == null || OrderItemPrefab == null) return;
            foreach (Transform c in OrderListContainer) Destroy(c.gameObject);

            foreach (var order in AppManager.Instance.RecentOrders)
            {
                var go   = Instantiate(OrderItemPrefab, OrderListContainer);
                var view = go.GetComponent<OrderItemView>();
                if (view != null) view.Populate(order);
            }
        }

        void PopulateMonthlyBars()
        {
            if (MonthlyBarsContainer == null || MonthlyBarPrefab == null) return;
            foreach (Transform c in MonthlyBarsContainer) Destroy(c.gameObject);

            var data  = AppManager.Instance.MonthlySpend;
            float max = 0;
            foreach (var d in data) if (d.Amount > max) max = d.Amount;

            for (int i = 0; i < data.Count; i++)
            {
                var  d       = data[i];
                bool isLast  = i == data.Count - 1;
                float pct    = d.Amount / max;

                var go  = Instantiate(MonthlyBarPrefab, MonthlyBarsContainer);
                var bar = go.GetComponent<MonthlyBarView>();
                if (bar != null) bar.Populate(d.Label, d.Amount, pct, isLast);
            }
        }

        static void SetText(TMP_Text lbl, string text)
        {
            if (lbl != null) lbl.text = text;
        }
    }

    // ── Small view components ────────────────────────────────────────────────────

    public class SpendBarRowView : MonoBehaviour
    {
        public TMP_Text StoreName;
        public Image    BarFill;
        public TMP_Text AmountLabel;
        public TMP_Text PctLabel;

        public void Populate(string name, float amount, float fillPct, string colorHex, float maxAmount)
        {
            if (StoreName)   StoreName.text   = name;
            if (AmountLabel) AmountLabel.text = "₹" + amount.ToString("N0");
            if (PctLabel)    PctLabel.text    = Mathf.RoundToInt(fillPct * 100) + "%";

            if (BarFill != null)
            {
                BarFill.fillAmount = Mathf.Clamp01(fillPct);
                if (ColorUtility.TryParseHtmlString(colorHex, out Color c))
                    BarFill.color = c;
            }
        }
    }

    public class OrderItemView : MonoBehaviour
    {
        public TMP_Text EmojiLabel;
        public TMP_Text NameLabel;
        public TMP_Text DateLabel;
        public TMP_Text PriceLabel;

        public void Populate(Order o)
        {
            if (EmojiLabel) EmojiLabel.text = o.Emoji;
            if (NameLabel)  NameLabel.text  = o.Name;
            if (DateLabel)  DateLabel.text  = o.DateLabel;
            if (PriceLabel) PriceLabel.text = "₹" + o.Price.ToString("N0");
        }
    }

    public class MonthlyBarView : MonoBehaviour
    {
        public TMP_Text  MonthLabel;
        public TMP_Text  ValueLabel;
        public RectTransform BarRect;  // height driven by fillPct
        public Image     BarImage;

        [Header("Colors")]
        public Color LatestColor  = new Color(1f, 0.6f, 0f, 1f);    // amazon orange
        public Color DefaultColor = new Color(0.118f, 0.149f, 0.192f, 1f);

        public void Populate(string label, float amount, float fillPct, bool isLatest)
        {
            if (MonthLabel) MonthLabel.text = label;
            if (ValueLabel) ValueLabel.text = "₹" + (amount / 1000f).ToString("0.0") + "k";

            if (BarImage)
                BarImage.color = isLatest ? LatestColor : DefaultColor;

            if (BarRect != null)
            {
                var sd = BarRect.sizeDelta;
                sd.y   = Mathf.Max(4f, fillPct * 120f);  // max height 120px
                BarRect.sizeDelta = sd;
            }
        }
    }
}
