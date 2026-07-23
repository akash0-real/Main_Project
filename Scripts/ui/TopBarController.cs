using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PriceHawk.Managers;

namespace PriceHawk.UI
{
    /// <summary>
    /// Drives the sticky top bar: logo, search input, nav buttons.
    /// </summary>
    public class TopBarController : MonoBehaviour
    {
        [Header("Search")]
        public TMP_InputField SearchInput;
        public Button         SearchButton;

        [Header("Nav Buttons")]
        public Button DashboardButton;
        public Button ProductButton;
        public Button ScannerButton;
        public Button AlertsButton;
        public Button SpendButton;

        void Awake()
        {
            SearchButton?.onClick.AddListener(DoSearch);
            if (SearchInput != null)
                SearchInput.onSubmit.AddListener(_ => DoSearch());

            DashboardButton?.onClick.AddListener(() => Nav("dashboard"));
            ProductButton  ?.onClick.AddListener(() => Nav("product"));
            ScannerButton  ?.onClick.AddListener(() => Nav("scanner"));
            AlertsButton   ?.onClick.AddListener(() => Nav("alerts"));
            SpendButton    ?.onClick.AddListener(() => Nav("spend"));

            AppManager.Instance.OnScreenChanged += RefreshNavButtons;
        }

        void OnDestroy()
        {
            if (AppManager.Instance != null)
                AppManager.Instance.OnScreenChanged -= RefreshNavButtons;
        }

        void DoSearch()
        {
            string query = SearchInput != null ? SearchInput.text.Trim() : "";
            if (string.IsNullOrEmpty(query)) return;

            AppManager.Instance.ShowToast($"🔍 Searching for \"{query}\"…");

            // Find and activate best match
            var results = AppManager.Instance.Search(query);
            if (results.Count > 0)
            {
                AppManager.Instance.SetActiveProduct(results[0]);
                Nav("product");
            }
        }

        void Nav(string screenId) => AppManager.Instance.NavigateTo(screenId);

        void RefreshNavButtons(string screenId)
        {
            // You can tint/underline the active button here
            // Example: highlight ProductButton when screenId == "product"
        }
    }
}
