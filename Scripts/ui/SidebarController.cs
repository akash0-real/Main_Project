using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PriceHawk.Managers;

namespace PriceHawk.UI
{
    /// <summary>
    /// Drives the left sidebar nav items and watchlist display.
    /// </summary>
    public class SidebarController : MonoBehaviour
    {
        [Header("Nav Items")]
        public List<SidebarNavItem> NavItems = new List<SidebarNavItem>();

        [Header("Watchlist Container")]
        public Transform  WatchlistContainer;
        public GameObject WatchlistItemPrefab;

        void Awake()
        {
            foreach (var item in NavItems)
            {
                string id = item.ScreenId;
                item.Button?.onClick.AddListener(() => AppManager.Instance.NavigateTo(id));
            }

            AppManager.Instance.OnScreenChanged += HighlightNav;
        }

        void OnDestroy()
        {
            if (AppManager.Instance != null)
                AppManager.Instance.OnScreenChanged -= HighlightNav;
        }

        void Start()
        {
            RefreshWatchlist();
        }

        void HighlightNav(string screenId)
        {
            foreach (var item in NavItems)
                item.SetActive(item.ScreenId == screenId);
        }

        public void RefreshWatchlist()
        {
            if (WatchlistContainer == null || WatchlistItemPrefab == null) return;
            foreach (Transform c in WatchlistContainer) Destroy(c.gameObject);

            // Show static default watchlist items (mirrors HTML sidebar)
            var defaultItems = new (string emoji, string name, string productId)[]
            {
                ("📱", "iPhone 15 Pro Max", "iphone15promax"),
                ("🎧", "AirPods Pro 2",     ""),
                ("⌚", "Apple Watch Ultra", ""),
            };

            foreach (var (emoji, name, productId) in defaultItems)
            {
                var go  = Instantiate(WatchlistItemPrefab, WatchlistContainer);
                var lbl = go.GetComponentInChildren<TMP_Text>();
                if (lbl != null) lbl.text = emoji + " " + name;

                var btn = go.GetComponentInChildren<Button>();
                if (btn != null && !string.IsNullOrEmpty(productId))
                {
                    string pid = productId;
                    btn.onClick.AddListener(() =>
                    {
                        AppManager.Instance.SetActiveProductById(pid);
                        AppManager.Instance.NavigateTo("product");
                    });
                }
            }
        }
    }

    [System.Serializable]
    public class SidebarNavItem
    {
        public string ScreenId;
        public Button Button;
        public Image  ActiveBar;   // left-side colored strip

        public void SetActive(bool active)
        {
            if (ActiveBar != null)
                ActiveBar.color = active
                    ? new Color(0f, 0.898f, 0.627f, 1f)
                    : Color.clear;
        }
    }
}
