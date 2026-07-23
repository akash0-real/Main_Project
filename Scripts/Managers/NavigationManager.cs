using System.Collections.Generic;
using UnityEngine;

namespace PriceHawk.Managers
{
    /// <summary>
    /// Handles show/hide of top-level screen GameObjects.
    /// Each screen GameObject should have a tag matching its screenId
    /// OR be registered via the inspector list below.
    /// </summary>
    public class NavigationManager : MonoBehaviour
    {
        [System.Serializable]
        public class ScreenEntry
        {
            public string     ScreenId;
            public GameObject Panel;
        }

        [Header("Screen Panels (assign in Inspector)")]
        public List<ScreenEntry> Screens = new List<ScreenEntry>();

        [Header("Sidebar / TopBar Buttons (optional highlight)")]
        public List<SidebarButtonEntry> SidebarButtons = new List<SidebarButtonEntry>();

        string _currentScreenId;

        void Awake()
        {
            AppManager.Instance.OnScreenChanged += ShowScreen;
        }

        void OnDestroy()
        {
            if (AppManager.Instance != null)
                AppManager.Instance.OnScreenChanged -= ShowScreen;
        }

        void Start()
        {
            // Default to product screen (mirrors HTML: nav('product'))
            ShowScreen("product");
        }

        public void ShowScreen(string screenId)
        {
            _currentScreenId = screenId;

            foreach (var s in Screens)
                s.Panel.SetActive(s.ScreenId == screenId);

            foreach (var b in SidebarButtons)
                b.SetActive(b.ScreenId == screenId);
        }

        public string CurrentScreen => _currentScreenId;
    }

    [System.Serializable]
    public class SidebarButtonEntry
    {
        public string         ScreenId;
        public UnityEngine.UI.Button Button;
        public UnityEngine.UI.Image  ActiveIndicator; // optional highlight strip

        public void SetActive(bool active)
        {
            if (ActiveIndicator != null) ActiveIndicator.enabled = active;
        }
    }
}
