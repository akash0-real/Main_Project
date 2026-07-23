using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PriceHawk.Models;

namespace PriceHawk.UI
{
    /// <summary>
    /// Drives a single active-alert card in the Alerts screen list.
    /// Attach to AlertCardPrefab root.
    /// </summary>
    public class AlertCardView : MonoBehaviour
    {
        [Header("References")]
        public TMP_Text   ProductNameLabel;
        public TMP_Text   TargetPriceLabel;
        public TMP_Text   CurrentPriceLabel;
        public TMP_Text   StatusLabel;
        public Button     DeleteButton;

        public event Action<string> OnDelete;

        string _alertId;

        public void Populate(PriceAlert alert)
        {
            _alertId = alert.Id;

            if (ProductNameLabel)  ProductNameLabel.text  = alert.ProductName;
            if (TargetPriceLabel)  TargetPriceLabel.text  = "Alert below ₹" + alert.TargetPrice.ToString("N0");
            if (CurrentPriceLabel) CurrentPriceLabel.text = "₹" + alert.CurrentPrice.ToString("N0");
            if (StatusLabel)       StatusLabel.text        = alert.Status switch
            {
                AlertStatus.Watching  => "👁 Watching",
                AlertStatus.Triggered => "🔔 Triggered",
                AlertStatus.Paused    => "⏸ Paused",
                _                     => "👁 Watching",
            };

            DeleteButton?.onClick.AddListener(() => OnDelete?.Invoke(_alertId));
        }
    }

    /// <summary>
    /// Drives a single product option row in the left iPhone-selector panel.
    /// </summary>
    public class ProductSelectorItem : MonoBehaviour
    {
        [Header("References")]
        public TMP_Text ProductNameLabel;
        public TMP_Text CurrentPriceLabel;
        public Button   SelectButton;
        public Image    SelectedIndicator;

        public event Action<Models.Product> OnSelected;

        Models.Product _product;

        public void Populate(Models.Product p)
        {
            _product = p;
            if (ProductNameLabel)  ProductNameLabel.text  = p.Name;
            if (CurrentPriceLabel) CurrentPriceLabel.text = "Currently ₹" + p.CurrentBestPrice.ToString("N0");
            SelectButton?.onClick.AddListener(() => OnSelected?.Invoke(_product));
        }

        public void SetSelected(bool selected)
        {
            if (SelectedIndicator != null)
                SelectedIndicator.color = selected
                    ? new Color(0f, 0.898f, 0.627f, 0.3f)
                    : Color.clear;
        }
    }
}
