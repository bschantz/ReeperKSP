using System;
using ReeperCommon.DataObjects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReeperKSP.Gui.Controls
{
    public class ColorSaturationSlider
    {
        private readonly GUIStyle _thumbStyle;
        private float _sliderThumbPosition = 100f;
        private readonly GUIStyle _style;
        private Texture2D _sliderTexture;
        private int _scrollbarHeight = 0;
        private float _hue = 1f;
        private float _lightness = 1f;

        public event Action<float> OnChanged = delegate { };

        public ColorSaturationSlider(GUIStyle baseScrollbarStyle, GUIStyle thumbStyle, int initialWidth,
            int initialHeight)
        {
            _thumbStyle = thumbStyle;
            if (baseScrollbarStyle == null) throw new ArgumentNullException("baseScrollbarStyle");
            if (thumbStyle == null) throw new ArgumentNullException("thumbStyle");

            _style = new GUIStyle(baseScrollbarStyle);
            ScrollbarWidth = initialWidth;
            ScrollbarHeight = initialHeight;
        }

        private void UpdateSliderTexture()
        {
            if (_sliderTexture != null) Object.Destroy(_sliderTexture);
            if (ScrollbarHeight < 1) return;

            var colors = new Color[ScrollbarHeight];
            var hsl = new ColorHsl(Hue, Saturation, Lightness);

            for (int y = 0; y < colors.Length; ++y)
            {
                hsl.Lightness = (y / (float)ScrollbarHeight);
                colors[y] = hsl.Color;
            }

            _sliderTexture = new Texture2D(1, colors.Length, TextureFormat.ARGB32, false);
            _sliderTexture.SetPixels(colors);
            _sliderTexture.Apply();

            _style.normal.background = _sliderTexture;
        }


        public int ScrollbarWidth { get; set; }

        public int ScrollbarHeight
        {
            get { return _scrollbarHeight; }
            set
            {
                int prevHeight = _scrollbarHeight;
                _scrollbarHeight = Math.Max(0, value);

                if (_scrollbarHeight != prevHeight)
                    UpdateSliderTexture();
            }
        }


        public float Saturation
        {
            get { return 1f - _sliderThumbPosition / 100f; }
            set
            {
                bool invoke = Mathf.Approximately(_sliderThumbPosition, value);

                _sliderThumbPosition = (1f - value) * 100f;
                UpdateSliderTexture();

                if (invoke) OnChanged(Saturation);
            }
        }


        public float Hue
        {
            get { return _hue; }
            set
            {
                _hue = value;
                UpdateSliderTexture();
            }
        }


        public float Lightness
        {
            get { return _lightness; }
            set
            {
                _lightness = value;
                UpdateSliderTexture();
            }
        }


        public void Draw()
        {
            float newValue = GUILayout.VerticalSlider(_sliderThumbPosition, 0f, 100f, _style, _thumbStyle, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true),
                GUILayout.MaxHeight(ScrollbarHeight), GUILayout.MaxWidth(ScrollbarWidth), GUILayout.MinWidth(ScrollbarWidth));

            bool changed = !Mathf.Approximately(newValue, _sliderThumbPosition);
            _sliderThumbPosition = newValue;

            if (changed) OnChanged(Saturation);
        }
    }
}
