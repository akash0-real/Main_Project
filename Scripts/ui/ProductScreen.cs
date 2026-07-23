using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PriceHawk.Models;
using PriceHawk.Managers;

namespace PriceHawk.UI
{
    /// <summary>
    /// Drives the Product View screen.
    /// Wire all references in the Inspector.
    /// </summary>
    public class ProductScreen : MonoBehaviour
    {
        // ── Hero ─────────────────────────────────────────────────────────────────
        [Header("Hero")]
        public TMP_Text ProductTitleLabel;
        public TMP_Text ProductSubtitleLabel;
        public TMP_Text RatingLabel;
        public TMP_Text ReviewCountLabel;
        public TMP_Text CurrentPriceLabel;
        public TMP_Text OriginalPriceLabel;
        public TMP_Text DiscountPctLabel;
        public TMP_Text VerdictLabel;

        // ── Model Selector ───────────────────────────────────────────────────────
        [Header("Model Selector")]
        public List<ModelChipButton> ModelChips = new List<ModelChipButton>();

        // ── Stat Cards ───────────────────────────────────────────────────────────
        [Header("Stat Cards")]
        public TMP_Text AllTimeLowLabel;
        public TMP_Text AllTimeLowSubLabel;
        public TMP_Text AllTimeHighLabel;
        public TMP_Text AllTimeHighSubLabel;
        public TMP_Text Avg90DayLabel;
        public TMP_Text Avg90DaySubLabel;

        // ── Chart ────────────────────────────────────────────────────────────────
        [Header("Price History Chart")]
        public PriceChartRenderer ChartRenderer;
        public List<ChartPillButton> ChartPills = new List<ChartPillButton>();

        // ── Store Compare ─────────────────────────────────────────────────────────
        [Header("Store Compare")]
        public Transform StoreListContainer;
        public GameObject StoreRowPrefab;

        // ── Buttons ───────────────────────────────────────────────────────────────
        [Header("Action Buttons")]
        public Button BuyOnAmazonButton;
        public Button SetAlertButton;
        public Button ShareButton;
        public Button WatchlistToggleButton;
        public TMP_Text WatchlistButtonLabel;

        // ── State ─────────────────────────────────────────────────────────────────
        Product _product;
        string  _chartRange = "1M";

        void Awake()
        {
            AppManager.Instance.OnActiveProductChanged += OnProductChanged;

            BuyOnAmazonButton?.onClick.AddListener(() =>
                AppManager.Instance.ShowToast("🛒 Opening Amazon…"));

            SetAlertButton?.onClick.AddListener(() =>
                AppManager.Instance.NavigateTo("alerts"));

            ShareButton?.onClick.AddListener(() =>
                AppManager.Instance.ShowToast("📤 Link copied!"));

            WatchlistToggleButton?.onClick.AddListener(OnWatchlistToggle);

            // Wire chart pill clicks
            for (int i = 0; i < ChartPills.Count; i++)
            {
                string range = ChartPills[i].Range;
                ChartPills[i].Button.onClick.AddListener(() => SetChartRange(range));
            }

            // Wire model chip clicks
            foreach (var chip in ModelChips)
            {
                string id = chip.ProductId;
                chip.Button.onClick.AddListener(() => AppManager.Instance.SetActiveProductById(id));
            }
        }

        void OnDestroy()
        {
            if (AppManager.Instance != null)
                AppManager.Instance.OnActiveProductChanged -= OnProductChanged;
        }

        void OnEnable()
        {
            _product = AppManager.Instance.ActiveProduct;
            PopulateAll();
        }

        void OnProductChanged(Product p)
        {
            _product = p;
            PopulateAll();
        }

        // ── Populate ──────────────────────────────────────────────────────────────
        void PopulateAll()
        {
            if (_product == null) return;

            // Hero
            float bestPrice = _product.CurrentBestPrice;
            float origPrice = bestPrice + 25000f;          // mirrors HTML logic
            float discPct   = Mathf.Round((origPrice - bestPrice) / origPrice * 100f);

            SetText(ProductTitleLabel,    _product.Name);
            SetText(ProductSubtitleLabel, _product.Subtitle);
            SetText(RatingLabel,          $"★ {_product.Rating:F1} / 5");
            SetText(ReviewCountLabel,     $"{_product.ReviewCount:N0} reviews");
            SetText(CurrentPriceLabel,    FormatINR(bestPrice));
            SetText(OriginalPriceLabel,   FormatINR(origPrice));
            SetText(DiscountPctLabel,     $"-{discPct:0}%");
            SetText(VerdictLabel,         "✅ Near All-Time Low — Good time to buy");

            // Model chip highlight
            foreach (var chip in ModelChips)
                chip.SetSelected(chip.ProductId == _product.Id);

            // Stat cards
            SetText(AllTimeLowLabel,    FormatINR(_product.AllTimeLow));
            SetText(AllTimeLowSubLabel, "Flipkart · Dec 2023");
            SetText(AllTimeHighLabel,   FormatINR(_product.AllTimeHigh));
            SetText(AllTimeHighSubLabel,"Launch Price · Sep 2023");
            SetText(Avg90DayLabel,      FormatINR(_product.Average90Day));
            SetText(Avg90DaySubLabel,   $"Currently {Mathf.Abs((_product.CurrentBestPrice - _product.Average90Day) / _product.Average90Day * 100f):0}% below avg");

            // Chart
            RedrawChart();

            // Store compare rows
            PopulateStoreRows();

            // Watchlist button
            RefreshWatchlistButton();
        }

        void SetChartRange(string range)
        {
            _chartRange = range;

            // Update pill active state
            foreach (var pill in ChartPills)
                pill.SetActive(pill.Range == range);

            RedrawChart();
        }

        void RedrawChart()
        {
            if (ChartRenderer == null || _product == null) return;

            List<PricePoint> data = _chartRange switch
            {
                "3M" => _product.PriceHistory3M,
                "6M" => _product.PriceHistory6M,
                "1Y" => _product.PriceHistory1Y,
                _    => _product.PriceHistory1M,
            };
            ChartRenderer.Draw(data);
        }

        void PopulateStoreRows()
        {
            if (StoreListContainer == null || StoreRowPrefab == null) return;

            foreach (Transform child in StoreListContainer)
                Destroy(child.gameObject);

            float bestPrice = _product.CurrentBestPrice;

            foreach (var listing in _product.Listings)
            {
                var go   = Instantiate(StoreRowPrefab, StoreListContainer);
                var row  = go.GetComponent<StoreRowView>();
                if (row != null)
                    row.Populate(listing, bestPrice);
            }
        }

        void OnWatchlistToggle()
        {
            if (_product == null) return;
            AppManager.Instance.ToggleWatchlist(_product.Id);
            RefreshWatchlistButton();
        }

        void RefreshWatchlistButton()
        {
            if (_product == null || WatchlistButtonLabel == null) return;
            bool inWatchlist = AppManager.Instance.Watchlist.Contains(_product.Id);
            WatchlistButtonLabel.text = inWatchlist ? "★ In Watchlist" : "☆ Add to Watchlist";
        }

        // ── Helpers ───────────────────────────────────────────────────────────────
        static void SetText(TMP_Text label, string text)
        {
            if (label != null) label.text = text;
        }

        static string FormatINR(float val) => "₹" + val.ToString("N0");
    }

    // ── Small helper structs (assign in Inspector via serialized fields) ─────────

    [System.Serializable]
    public class ModelChipButton
    {
        public string    ProductId;   // matches Product.Id
        public Button    Button;
        public Image     ActiveBg;    // optional background tint

        public void SetSelected(bool selected)
        {
            if (ActiveBg != null)
                ActiveBg.color = selected
                    ? new Color(0f, 0.6f, 1f, 0.15f)   // accent2 tint
                    : Color.clear;
        }
    }

    [System.Serializable]
    public class ChartPillButton
    {
        public string    Range;       // "1M", "3M", "6M", "1Y"
        public Button    Button;
        public Image     ActiveBg;

        public void SetActive(bool active)
        {
            if (ActiveBg != null)
                ActiveBg.color = active
                    ? new Color(0f, 0.898f, 0.627f, 0.15f)
                    : Color.clear;
        }
    }
}
