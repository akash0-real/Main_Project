using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PriceHawk.Models;

namespace PriceHawk.UI
{
    /// <summary>
    /// Drives a single store comparison row in the Product View.
    /// Attach to the StoreRowPrefab root.
    /// </summary>
    public class StoreRowView : MonoBehaviour
    {
        [Header("References")]
        public TMP_Text StoreNameLabel;
        public TMP_Text StoreEmojiLabel;
        public Image    BarFill;
        public TMP_Text PriceLabel;
        public GameObject BestBadge;
        public Image    RowBackground;  // optional tint for "best" row

        [Header("Colors")]
        public Color BestRowTint   = new Color(0f, 0.898f, 0.627f, 0.05f);
        public Color NormalRowTint = new Color(0.118f, 0.149f, 0.192f, 1f); // surface2

        public void Populate(StoreListing listing, float lowestPrice)
        {
            if (StoreNameLabel)  StoreNameLabel.text  = listing.StoreName;
            if (StoreEmojiLabel) StoreEmojiLabel.text = listing.StoreEmoji;
            if (PriceLabel)      PriceLabel.text      = FormatINR(listing.Price);

            // Bar width proportional to price relative to lowest
            if (BarFill != null)
            {
                float pct = listing.Price > 0 ? lowestPrice / listing.Price : 1f;
                BarFill.fillAmount = Mathf.Clamp01(pct);

                // Parse store color
                if (ColorUtility.TryParseHtmlString(listing.StoreColorHex, out Color c))
                    BarFill.color = c;
            }

            if (BestBadge != null)  BestBadge.SetActive(listing.IsBest);
            if (RowBackground != null)
                RowBackground.color = listing.IsBest ? BestRowTint : NormalRowTint;

            if (PriceLabel != null && listing.IsBest)
                PriceLabel.color = new Color(0f, 0.898f, 0.627f, 1f); // accent green
        }

        static string FormatINR(float v) => "₹" + v.ToString("N0");
    }
}
