using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PriceHawk.Models;

namespace PriceHawk.UI
{
    /// <summary>
    /// Renders price history as a polyline on a RawImage RenderTexture,
    /// or via a series of UI Images if using the sprite-based approach.
    ///
    /// For simplicity this version draws using Unity's GL in OnPostRender
    /// attached to a Camera that renders to a RenderTexture displayed in
    /// the chart RawImage.
    ///
    /// SETUP:
    ///   1. Create a child Camera (ChartCamera) with Clear Flags=Solid Color
    ///      targeting a RenderTexture asset.
    ///   2. Assign that RenderTexture to ChartRawImage.
    ///   3. Assign ChartCamera to this component.
    ///   4. Create a simple unlit material (ChartLineMaterial) with
    ///      Shader "Hidden/Internal-Colored".
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class PriceChartRenderer : MonoBehaviour
    {
        [Header("Chart Camera + Output")]
        public Camera     ChartCamera;
        public RawImage   ChartRawImage;
        public Material   ChartLineMaterial;  // Shader: Hidden/Internal-Colored

        [Header("Y-Axis Labels")]
        public TMP_Text YAxisTopLabel;
        public TMP_Text YAxisMidLabel;
        public TMP_Text YAxisBotLabel;

        [Header("X-Axis Labels Container")]
        public Transform XAxisContainer;     // child TMP_Text objects get labels
        public GameObject XAxisLabelPrefab;  // prefab with TMP_Text

        [Header("Colors")]
        public Color LineColor  = new Color(0f,   0.898f, 0.627f, 1f);  // #00e5a0
        public Color AreaColor  = new Color(0f,   0.898f, 0.627f, 0.18f);
        public Color GridColor  = new Color(0.165f,0.2f,  0.278f, 1f);   // #2a3347

        List<PricePoint> _data;
        float _minPrice, _maxPrice;

        public void Draw(List<PricePoint> data)
        {
            _data = data;
            if (_data == null || _data.Count < 2) return;

            ComputeMinMax();
            UpdateYAxisLabels();
            UpdateXAxisLabels();
            // GL drawing happens in OnPostRender
        }

        void ComputeMinMax()
        {
            _minPrice = float.MaxValue;
            _maxPrice = float.MinValue;
            foreach (var p in _data)
            {
                if (p.Price < _minPrice) _minPrice = p.Price;
                if (p.Price > _maxPrice) _maxPrice = p.Price;
            }
            _minPrice -= 3000f;
            _maxPrice += 3000f;
        }

        void UpdateYAxisLabels()
        {
            float mid = (_minPrice + _maxPrice) * 0.5f;
            if (YAxisTopLabel) YAxisTopLabel.text = FormatK(_maxPrice - 3000f);
            if (YAxisMidLabel) YAxisMidLabel.text = FormatK(mid);
            if (YAxisBotLabel) YAxisBotLabel.text = FormatK(_minPrice + 3000f);
        }

        void UpdateXAxisLabels()
        {
            if (XAxisContainer == null || _data == null) return;

            // Clear existing
            foreach (Transform child in XAxisContainer)
                Destroy(child.gameObject);

            foreach (var p in _data)
            {
                if (XAxisLabelPrefab == null) break;
                var go  = Instantiate(XAxisLabelPrefab, XAxisContainer);
                var lbl = go.GetComponentInChildren<TMP_Text>();
                if (lbl != null) lbl.text = p.Label;
            }
        }

        // Called by Unity after ChartCamera renders
        void OnPostRender()
        {
            if (_data == null || _data.Count < 2 || ChartLineMaterial == null) return;

            ChartLineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.LoadOrtho();  // 0-1 normalized

            float count = _data.Count - 1;

            // Helper: map data to GL coords (0-1)
            System.Func<int,float> xOf = i => (float)i / count;
            System.Func<float,float> yOf = v =>
                (v - _minPrice) / (_maxPrice - _minPrice);

            // ── Grid lines ──────────────────────────────────────────────────────
            GL.Begin(GL.LINES);
            GL.Color(GridColor);
            foreach (float f in new float[]{ 0.25f, 0.5f, 0.75f })
            {
                GL.Vertex3(0f, f, 0f);
                GL.Vertex3(1f, f, 0f);
            }
            GL.End();

            // ── Area fill ───────────────────────────────────────────────────────
            GL.Begin(GL.TRIANGLE_STRIP);
            for (int i = 0; i < _data.Count; i++)
            {
                float x = xOf(i);
                float y = yOf(_data[i].Price);
                GL.Color(AreaColor);
                GL.Vertex3(x, 0f, 0f);
                GL.Vertex3(x, y,  0f);
            }
            GL.End();

            // ── Line ────────────────────────────────────────────────────────────
            GL.Begin(GL.LINE_STRIP);
            GL.Color(LineColor);
            for (int i = 0; i < _data.Count; i++)
                GL.Vertex3(xOf(i), yOf(_data[i].Price), 0f);
            GL.End();

            GL.PopMatrix();
        }

        static string FormatK(float v) => "₹" + (v / 1000f).ToString("0") + "k";
    }
}
