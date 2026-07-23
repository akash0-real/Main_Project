using System;
using System.Collections.Generic;
using UnityEngine;
using PriceHawk.Models;
using PriceHawk.Data;

namespace PriceHawk.Managers
{
    /// <summary>
    /// Singleton that owns all runtime state.
    /// Persists across scenes (DontDestroyOnLoad).
    /// </summary>
    public class AppManager : MonoBehaviour
    {
        public static AppManager Instance { get; private set; }

        // ── State ────────────────────────────────────────────────────────────────
        public List<Product>    Products     { get; private set; }
        public List<PriceAlert> Alerts       { get; private set; }
        public List<Order>      RecentOrders { get; private set; }
        public List<SpendData>  MonthlySpend { get; private set; }
        public DashboardStats   DashStats    { get; private set; }

        public Dictionary<string, ScanResult> BarcodeDB { get; private set; }

        // Currently viewed product
        public Product ActiveProduct { get; private set; }

        // Watchlist (product ids)
        public HashSet<string> Watchlist { get; private set; } = new HashSet<string>();

        // Scan history (max 5)
        public List<ScanResult> ScanHistory { get; private set; } = new List<ScanResult>();

        // ── Events ───────────────────────────────────────────────────────────────
        public event Action<Product>     OnActiveProductChanged;
        public event Action<PriceAlert>  OnAlertAdded;
        public event Action<PriceAlert>  OnAlertRemoved;
        public event Action<ScanResult>  OnBarcodeScanned;
        public event Action<string>      OnToastRequested;  // message string
        public event Action<string>      OnScreenChanged;   // screen id

        // ── Lifecycle ────────────────────────────────────────────────────────────
        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }

        void LoadData()
        {
            Products     = MockDataProvider.GetProducts();
            Alerts       = MockDataProvider.GetDefaultAlerts();
            RecentOrders = MockDataProvider.GetRecentOrders();
            MonthlySpend = MockDataProvider.GetMonthlySpend();
            DashStats    = MockDataProvider.GetDashboardStats();
            BarcodeDB    = MockDataProvider.GetBarcodeDatabase();

            if (Products.Count > 0)
                ActiveProduct = Products[0];
        }

        // ── Product ──────────────────────────────────────────────────────────────
        public void SetActiveProduct(Product p)
        {
            ActiveProduct = p;
            OnActiveProductChanged?.Invoke(p);
        }

        public void SetActiveProductById(string id)
        {
            var p = Products.Find(x => x.Id == id);
            if (p != null) SetActiveProduct(p);
        }

        // ── Watchlist ────────────────────────────────────────────────────────────
        public bool ToggleWatchlist(string productId)
        {
            if (Watchlist.Contains(productId))
            {
                Watchlist.Remove(productId);
                ShowToast("Removed from Watchlist");
                return false;
            }
            Watchlist.Add(productId);
            ShowToast($"Added to Watchlist!");
            return true;
        }

        // ── Alerts ───────────────────────────────────────────────────────────────
        public PriceAlert AddAlert(string productId, string productName, float currentPrice, float targetPrice, string email = "")
        {
            var alert = new PriceAlert
            {
                Id           = Guid.NewGuid().ToString(),
                ProductId    = productId,
                ProductName  = productName,
                CurrentPrice = currentPrice,
                TargetPrice  = targetPrice,
                NotifyEmail  = email,
                Status       = AlertStatus.Watching,
                CreatedAt    = DateTime.Now,
            };
            Alerts.Add(alert);
            DashStats.ActiveAlerts = Alerts.Count;
            OnAlertAdded?.Invoke(alert);
            ShowToast($"🔔 Alert set for {productName} at ₹{targetPrice:N0}");
            return alert;
        }

        public void RemoveAlert(string alertId)
        {
            var alert = Alerts.Find(a => a.Id == alertId);
            if (alert == null) return;
            Alerts.Remove(alert);
            DashStats.ActiveAlerts = Alerts.Count;
            OnAlertRemoved?.Invoke(alert);
            ShowToast("🗑️ Alert removed");
        }

        // ── Scanner ──────────────────────────────────────────────────────────────
        public bool TryLookupBarcode(string barcode, out ScanResult result)
        {
            if (BarcodeDB.TryGetValue(barcode, out result))
            {
                result.ScannedAt = DateTime.Now;
                AddToScanHistory(result);
                OnBarcodeScanned?.Invoke(result);
                ShowToast($"✅ {result.ProductName} found!");
                return true;
            }
            ShowToast("❌ Product not found in database");
            return false;
        }

        void AddToScanHistory(ScanResult r)
        {
            ScanHistory.RemoveAll(s => s.Barcode == r.Barcode);
            ScanHistory.Insert(0, r);
            if (ScanHistory.Count > 5) ScanHistory.RemoveAt(ScanHistory.Count - 1);
        }

        // ── Toast ────────────────────────────────────────────────────────────────
        public void ShowToast(string message) => OnToastRequested?.Invoke(message);

        // ── Navigation ───────────────────────────────────────────────────────────
        public void NavigateTo(string screenId) => OnScreenChanged?.Invoke(screenId);

        // ── Search ───────────────────────────────────────────────────────────────
        public List<Product> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return Products;
            query = query.ToLower();
            return Products.FindAll(p =>
                p.Name.ToLower().Contains(query) ||
                p.Category.ToLower().Contains(query));
        }
    }
}
