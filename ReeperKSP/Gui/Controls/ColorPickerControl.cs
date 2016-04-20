using System;
using ReeperCommon.Extensions;
using UnityEngine;

namespace ReeperKSP.Gui.Controls
{
    public class ColorPickerControl
    {
        private Rect _rect;
        private readonly ColorSaturationSlider _slider;
        private readonly ColorPickerBox _colorPickerBox = new ColorPickerBox(8f, 6f, 3f);

        private Color _selected = Color.white;
        private readonly Texture2D _selectedColor = new Texture2D(1, 1);

        public event Action<Color> OnChanged = delegate { };


        public ColorPickerControl(GUIStyle baseSliderStyle, GUIStyle thumbStyle)
        {
            _selectedColor.wrapMode = TextureWrapMode.Repeat;

            _slider = new ColorSaturationSlider(baseSliderStyle, thumbStyle, 12, 1)
            {
                Lightness = 1f,
                Saturation = 1f
            };

            _slider.OnChanged += f =>
            {
                _colorPickerBox.Saturation = f;
                UpdateSelectedColor();
            };

            _colorPickerBox.OnChanged += UpdateSelectedColor;
        }


        private void UpdateSelectedColor()
        {
            var hsl = _colorPickerBox.Selected.GetHSL();

            _slider.Lightness = hsl.Lightness;
            _slider.Hue = hsl.Hue;

            // some small adjustments to make the crosshair more visible and contrast with selected color
            var adjustCrosshairColor = _colorPickerBox.Selected.GetHSL();

            adjustCrosshairColor.Hue = UtilMath.WrapAround(
                (adjustCrosshairColor.Hue < 0.4f || adjustCrosshairColor.Hue > 0.6f) ?
                1f - adjustCrosshairColor.Hue : adjustCrosshairColor.Hue + 0.5f, 0f, 1f);
            adjustCrosshairColor.Lightness = UtilMath.WrapAround(adjustCrosshairColor.Lightness + 0.5f, 0f, 1f);

            _colorPickerBox.CrosshairColor = adjustCrosshairColor.Color;

            hsl.Saturation = _slider.Saturation;
            Selected = hsl.Color;

            OnChanged(Selected);
        }



        public void Draw(float width, float height)
        {
            var reserved = GUILayoutUtility.GetRect(width, height);
            UpdatePosition(reserved);


            GUILayout.BeginArea(_rect);
            GUILayout.BeginHorizontal();
            {
                // left side: color picker, selected color box
                GUILayout.BeginVertical();
                _colorPickerBox.Draw(GUILayout.ExpandWidth(true),
                    GUILayout.ExpandHeight(true),
                    GUILayout.MaxWidth(_rect.width - _slider.ScrollbarWidth),
                    GUILayout.MaxHeight(_rect.height));


                GUILayout.Label(string.Empty, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MaxWidth(_rect.width - _slider.ScrollbarWidth), GUILayout.MaxHeight(_slider.ScrollbarWidth));
                if (Event.current.type == EventType.Repaint)
                {
                    var r = GUILayoutUtility.GetLastRect();

                    var pt = GUIUtility.GUIToScreenPoint(new Vector2(r.x, r.y));
                    r.x = pt.x;
                    r.y = pt.y;

                    Graphics.DrawTexture(GUIUtility.ScreenToGUIRect(r), _selectedColor);
                }

                GUILayout.EndVertical();
                _slider.Draw();

            }
            GUILayout.EndHorizontal();


            GUILayout.EndArea();
        }


        private void UpdatePosition(Rect position)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            _rect = position;
            _rect.width = _rect.height = Mathf.Min(position.width, position.height);

            _slider.ScrollbarHeight = (int)_rect.height;

        }


        public Color Selected
        {
            get { return _selected; }
            set
            {
                var hsl = value.GetHSL();

                _slider.Saturation = hsl.Saturation;
                _colorPickerBox.Selected = value;

                _selected = value;
                _selectedColor.SetPixel(0, 0, _selected);
                _selectedColor.Apply();
            }

        }
    }
}
