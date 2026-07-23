using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PriceHawk.Models;
using PriceHawk.Managers;

namespace PriceHawk.UI
{
    /// <summary>
    /// Drives the Barcode Scanner screen.
    /// Camera feed is shown via WebCamTexture on RawImage.
    ///
    /// Real barcode decoding requires a plugin (e.g. ZXing.Net or
    /// Barcode Scanner SDK). This script provides the full integration
    /// surface — swap in your decode call where marked.
    /// </summary>
    public class ScannerScreen : MonoBehaviour
    {
        // ── Camera ────────────────────────────────────────────────────────────────
        [Header("Camera View")]
        public RawImage  CameraFeedImage;
        public GameObject IdlePanel;
        public GameObject ScanningOverlay;
        public GameObject SuccessFlash;
        public TMP_Text  SuccessBarcodeLabel;
        public TMP_Text  StatusLabel;
        public Button    StartStopButton;
        public TMP_Text  StartStopButtonLabel;
        public Button    DemoScanButton;

        // ── Result Panel ─────────────────────────────────────────────────────────
        [Header("Result Panel")]
        public GameObject  EmptyResultPanel;
        public GameObject  ResultPanel;
        public TMP_Text    ResultBarcodeLabel;
        public TMP_Text    ResultNameLabel;
        public TMP_Text    ResultCategoryLabel;
        public TMP_Text    ResultPriceLabel;
        public TMP_Text    ResultVerdictLabel;
        public TMP_Text    ResultTimingLabel;
        public TMP_Text    AmazonPriceLabel;
        public TMP_Text    FlipkartPriceLabel;
        public TMP_Text    CromaPriceLabel;
        public Button      AddToWatchlistButton;

        // ── Scan History ─────────────────────────────────────────────────────────
        [Header("Scan History")]
        public Transform   HistoryContainer;
        public GameObject  HistoryItemPrefab;

        // ── State ─────────────────────────────────────────────────────────────────
        WebCamTexture _webcam;
        bool          _isScanning;
        Coroutine     _decodeLoop;

        static readonly string[] DemoBarcodes = { "8901234567890", "4006381333931", "5901234123457" };
        int _demoIndex;

        void Awake()
        {
            StartStopButton?.onClick.AddListener(ToggleCamera);
            DemoScanButton?.onClick.AddListener(SimulateScan);
            AddToWatchlistButton?.onClick.AddListener(AddScannedToWatchlist);
            AppManager.Instance.OnBarcodeScanned += OnBarcodeScanned;
        }

        void OnDestroy()
        {
            StopCamera();
            if (AppManager.Instance != null)
                AppManager.Instance.OnBarcodeScanned -= OnBarcodeScanned;
        }

        void OnEnable()
        {
            ShowIdleState();
            RefreshHistory();
        }

        void OnDisable() => StopCamera();

        // ── Camera ────────────────────────────────────────────────────────────────
        void ToggleCamera()
        {
            if (_isScanning) StopCamera();
            else             StartCamera();
        }

        void StartCamera()
        {
            // Request camera permission (Android/iOS)
            // On platforms without webcam this gracefully falls back
            WebCamDevice[] devices = WebCamTexture.devices;
            if (devices.Length == 0)
            {
                AppManager.Instance.ShowToast("❌ No camera found");
                return;
            }

            // Prefer back camera
            string camName = devices[0].name;
            foreach (var d in devices)
                if (!d.isFrontFacing) { camName = d.name; break; }

            _webcam = new WebCamTexture(camName, 1280, 720);
            _webcam.Play();

            if (CameraFeedImage != null)
            {
                CameraFeedImage.texture = _webcam;
                CameraFeedImage.gameObject.SetActive(true);
            }

            _isScanning = true;
            SetStatus("📷 Scanning… align barcode in frame");
            IdlePanel?.SetActive(false);
            ScanningOverlay?.SetActive(true);
            if (StartStopButtonLabel) StartStopButtonLabel.text = "⏹ Stop Camera";

            // Start decode loop — replace body with real decode call
            _decodeLoop = StartCoroutine(DecodingLoop());
        }

        void StopCamera()
        {
            if (_decodeLoop != null) { StopCoroutine(_decodeLoop); _decodeLoop = null; }
            if (_webcam != null && _webcam.isPlaying) _webcam.Stop();
            _webcam     = null;
            _isScanning = false;
            ShowIdleState();
            if (StartStopButtonLabel) StartStopButtonLabel.text = "📷 Start Camera";
        }

        void ShowIdleState()
        {
            IdlePanel?.SetActive(true);
            ScanningOverlay?.SetActive(false);
            SuccessFlash?.SetActive(false);
            SetStatus("Camera inactive · Click Start to begin");
        }

        void SetStatus(string msg)
        {
            if (StatusLabel != null) StatusLabel.text = msg;
        }

        // ── Decode Loop ───────────────────────────────────────────────────────────
        IEnumerator DecodingLoop()
        {
            while (_isScanning)
            {
                yield return new WaitForSeconds(0.5f);

                if (_webcam == null || !_webcam.isPlaying) yield break;

                // ── INTEGRATION POINT ──────────────────────────────────────────────
                // Replace the block below with a real ZXing / barcode SDK decode:
                //
                //   var pixels = _webcam.GetPixels32();
                //   var barcodeReader = new ZXing.BarcodeReader();
                //   var result = barcodeReader.Decode(pixels, _webcam.width, _webcam.height);
                //   if (result != null) HandleBarcode(result.Text);
                // ──────────────────────────────────────────────────────────────────
            }
        }

        // ── Demo Scan ─────────────────────────────────────────────────────────────
        public void SimulateScan()
        {
            string barcode = DemoBarcodes[_demoIndex % DemoBarcodes.Length];
            _demoIndex++;
            HandleBarcode(barcode);
        }

        void HandleBarcode(string barcode)
        {
            // Flash success state
            StartCoroutine(ShowSuccessFlash(barcode));
            AppManager.Instance.TryLookupBarcode(barcode, out _);
        }

        IEnumerator ShowSuccessFlash(string barcode)
        {
            ScanningOverlay?.SetActive(false);
            if (SuccessFlash) SuccessFlash.SetActive(true);
            if (SuccessBarcodeLabel) SuccessBarcodeLabel.text = barcode;
            yield return new WaitForSeconds(1.2f);
            SuccessFlash?.SetActive(false);
            if (_isScanning) ScanningOverlay?.SetActive(true);
        }

        // ── Result ────────────────────────────────────────────────────────────────
        void OnBarcodeScanned(ScanResult result)
        {
            EmptyResultPanel?.SetActive(false);
            ResultPanel?.SetActive(true);

            SetLabel(ResultBarcodeLabel,   "EAN-13: " + result.Barcode);
            SetLabel(ResultNameLabel,      result.ProductName);
            SetLabel(ResultCategoryLabel,  result.Category);
            SetLabel(ResultPriceLabel,     "₹" + result.Price.ToString("N0"));
            SetLabel(ResultVerdictLabel,   result.Verdict);
            SetLabel(ResultTimingLabel,    result.TimingNote);
            SetLabel(AmazonPriceLabel,     "₹" + result.AmazonPrice.ToString("N0"));
            SetLabel(FlipkartPriceLabel,   "₹" + result.FlipkartPrice.ToString("N0"));
            SetLabel(CromaPriceLabel,      "₹" + result.CromaPrice.ToString("N0"));

            RefreshHistory();
        }

        void AddScannedToWatchlist()
        {
            if (ResultNameLabel == null || string.IsNullOrEmpty(ResultNameLabel.text)) return;
            AppManager.Instance.ShowToast("✅ " + ResultNameLabel.text + " added to Watchlist!");
        }

        void RefreshHistory()
        {
            if (HistoryContainer == null || HistoryItemPrefab == null) return;
            foreach (Transform child in HistoryContainer) Destroy(child.gameObject);

            foreach (var s in AppManager.Instance.ScanHistory)
            {
                var go  = Instantiate(HistoryItemPrefab, HistoryContainer);
                var lbl = go.GetComponentInChildren<TMP_Text>();
                if (lbl != null)
                    lbl.text = $"{s.ProductName}  ₹{s.Price:N0}  {s.ScannedAt:HH:mm}";

                var btn = go.GetComponentInChildren<Button>();
                if (btn != null)
                {
                    string bc = s.Barcode;
                    btn.onClick.AddListener(() => HandleBarcode(bc));
                }
            }
        }

        static void SetLabel(TMP_Text lbl, string text)
        {
            if (lbl != null) lbl.text = text;
        }
    }
}
