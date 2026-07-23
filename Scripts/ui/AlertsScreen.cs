using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PriceHawk.Models;
using PriceHawk.Managers;

namespace PriceHawk.UI
{
    /// <summary>
    /// Drives the Price Alerts screen.
    /// </summary>
    public class AlertsScreen : MonoBehaviour
    {
        // ── Left panel: iPhone selector ──────────────────────────────────────────
        [Header("Product Selector")]
        public Transform         ProductSelectorContainer;
        public GameObject        ProductSelectorItemPrefab;

        // ── Right panel: Alert config ─────────────────────────────────────────────
        [Header("Alert Config")]
        public TMP_Text          SelectedProductNameLabel;
        public TMP_Text          CurrentPriceLabel;
        public TMP_InputField    TargetPriceInput;
        public TMP_InputField    EmailInput;
        public Transform         QuickTargetsContainer;
        public GameObject        QuickTargetButtonPrefab;
        public Button            SaveAlertButton;

        // ── Active alerts list ────────────────────────────────────────────────────
        [Header("Active Alerts List")]
        public Transform         AlertsListContainer;
        public GameObject        AlertCardPrefab;

        // ── State ─────────────────────────────────────────────────────────────────
        Product _selectedProduct;
        readonly List<GameObject> _alertCards = new List<GameObject>();

        void Awake()
        {
            SaveAlertButton?.onClick.AddListener(OnSaveAlert);
            AppManager.Instance.OnAlertAdded   += OnAlertAdded;
            AppManager.Instance.OnAlertRemoved += OnAlertRemoved;
        }

        void OnDestroy()
        {
            if (AppManager.Instance != null)
            {
                AppManager.Instance.OnAlertAdded   -= OnAlertAdded;
                AppManager.Instance.OnAlertRemoved -= OnAlertRemoved;
            }
        }

        void OnEnable()
        {
            PopulateProductSelector();
            RefreshAlertsList();
        }

        // ── Product selector ──────────────────────────────────────────────────────
        void PopulateProductSelector()
        {
            if (ProductSelectorContainer == null || ProductSelectorItemPrefab == null) return;

            foreach (Transform child in ProductSelectorContainer) Destroy(child.gameObject);

            foreach (var product in AppManager.Instance.Products)
            {
                var go   = Instantiate(ProductSelectorItemPrefab, ProductSelectorContainer);
                var item = go.GetComponent<ProductSelectorItem>();
                if (item != null)
                {
                    item.Populate(product);
                    item.OnSelected += SelectProduct;
                }
            }

            // Auto-select first
            if (AppManager.Instance.Products.Count > 0)
                SelectProduct(AppManager.Instance.Products[0]);
        }

        void SelectProduct(Product p)
        {
            _selectedProduct = p;

            if (SelectedProductNameLabel) SelectedProductNameLabel.text = p.Name;
            if (CurrentPriceLabel)        CurrentPriceLabel.text        = "₹" + p.CurrentBestPrice.ToString("N0");

            PopulateQuickTargets(p.CurrentBestPrice);
        }

        void PopulateQuickTargets(float currentPrice)
        {
            if (QuickTargetsContainer == null || QuickTargetButtonPrefab == null) return;

            foreach (Transform child in QuickTargetsContainer) Destroy(child.gameObject);

            float[] offsets = { -5000f, -10000f, -15000f, -20000f, -25000f };
            foreach (float offset in offsets)
            {
                float target = currentPrice + offset;
                if (target <= 0) continue;

                var go  = Instantiate(QuickTargetButtonPrefab, QuickTargetsContainer);
                var btn = go.GetComponentInChildren<Button>();
                var lbl = go.GetComponentInChildren<TMP_Text>();

                if (lbl != null) lbl.text = "₹" + target.ToString("N0");
                if (btn != null)
                {
                    float t = target;
                    btn.onClick.AddListener(() =>
                    {
                        if (TargetPriceInput != null)
                            TargetPriceInput.text = "₹" + t.ToString("N0");
                    });
                }
            }
        }

        // ── Save alert ────────────────────────────────────────────────────────────
        void OnSaveAlert()
        {
            if (_selectedProduct == null) return;

            string priceStr = TargetPriceInput != null ? TargetPriceInput.text : "";
            priceStr = priceStr.Replace("₹", "").Replace(",", "").Trim();

            if (!float.TryParse(priceStr, out float targetPrice))
            {
                AppManager.Instance.ShowToast("⚠️ Enter a valid target price first");
                return;
            }

            string email = EmailInput != null ? EmailInput.text : "";

            AppManager.Instance.AddAlert(
                _selectedProduct.Id,
                _selectedProduct.Name,
                _selectedProduct.CurrentBestPrice,
                targetPrice,
                email);
        }

        // ── Alerts list ───────────────────────────────────────────────────────────
        void RefreshAlertsList()
        {
            if (AlertsListContainer == null || AlertCardPrefab == null) return;

            foreach (Transform child in AlertsListContainer) Destroy(child.gameObject);
            _alertCards.Clear();

            foreach (var alert in AppManager.Instance.Alerts)
                SpawnAlertCard(alert);
        }

        void OnAlertAdded(PriceAlert alert)   => SpawnAlertCard(alert);
        void OnAlertRemoved(PriceAlert alert) => RefreshAlertsList();

        void SpawnAlertCard(PriceAlert alert)
        {
            if (AlertsListContainer == null || AlertCardPrefab == null) return;

            var go   = Instantiate(AlertCardPrefab, AlertsListContainer);
            var card = go.GetComponent<AlertCardView>();
            if (card != null)
            {
                card.Populate(alert);
                card.OnDelete += id => AppManager.Instance.RemoveAlert(id);
            }
            _alertCards.Add(go);
        }
    }
}
