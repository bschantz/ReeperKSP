using ReeperCommon.DataObjects;
using ReeperCommon.Extensions;
using UnityEngine;

namespace ReeperKSP.Gui.Controls
{
    public class ColorPickerBox
    {
        private readonly float _crosshairCenterOffset;
        private readonly float _crosshairLineWidth;
        private readonly float _crosshairLength;
        private Texture2D _texture = new Texture2D(1, 1);
        private float _saturationValue = 1f;
        private Vector2 _targetCoordinates = default(Vector2);
        private Color _selected = Color.white;

        public delegate void ChangedDelegate();

        public event ChangedDelegate OnChanged = delegate { };


        public ColorPickerBox(float crosshairLength, float crosshairCenterOffset, float crosshairLineWidth)
        {
            _crosshairCenterOffset = crosshairCenterOffset;
            _crosshairLineWidth = Mathf.Max(0.5f, crosshairLineWidth);
            _crosshairLength = Mathf.Abs(crosshairLength);

            CrosshairColor = Color.black;
        }



        public void UpdateTexture()
        {
            UpdateTexture(_texture.width, _texture.height);
        }



        private void UpdateTexture(int w, int h)
        {
            w = Mathf.Max(1, w);
            h = Mathf.Max(1, h);

            if (_texture != null) Object.Destroy(_texture);

            _texture = new Texture2D(w, h, TextureFormat.ARGB32, false);

            var hsl = new ColorHsl(0f, Saturation, 0f);
            var colors = new Color[w * h];

            for (int y = 0; y < h; ++y)
                for (int x = 0; x < w; ++x)
                {
                    hsl.Hue = CalculateHue(x, y);
                    hsl.Lightness = 1f - CalculateLightness(x, y); // due to coordinates being inverted in screen space

                    colors[y * w + x] = hsl.Color;
                }

            _texture.SetPixels(colors);
            _texture.Apply();
        }



        private float CalculateHue(int x, int y)
        {
            return x / (float)_texture.width;
        }



        private float CalculateLightness(int x, int y)
        {
            return y / (float)_texture.height;
        }



        public void Draw(params GUILayoutOption[] options)
        {
            if (GUILayout.RepeatButton("Hello", options) && Event.current.type == EventType.Repaint)
                OnBoxClicked();


            if (Event.current.type != EventType.Repaint) return;

            // draw color box
            var r = GUILayoutUtility.GetLastRect();

            var pt = GUIUtility.GUIToScreenPoint(new Vector2(r.x, r.y));
            r.x = pt.x;
            r.y = pt.y;

            Graphics.DrawTexture(GUIUtility.ScreenToGUIRect(r), _texture);
            CheckBoxSize(r.width, r.height);

            DrawCrosshair();
        }



        private void DrawCrosshair()
        {
            // coordinates of right line on crosshair
            var p1 = new Vector2(_targetCoordinates.x + _crosshairCenterOffset, _targetCoordinates.y);
            var p2 = new Vector2(p1.x + _crosshairLength, p1.y);

            // note: intentionally avoid storing GUI.matrix here as it is slow. Note that at the 
            // end of the loop, we'll be right-side up again so it's unnecessary
            for (int i = 0; i < 4; ++i)
            {
                GUIUtility.RotateAroundPivot(90f, _targetCoordinates);
                Drawing.DrawLine(p1, p2, CrosshairColor, _crosshairLineWidth, true);
            }
        }



        private void CheckBoxSize(float width, float height)
        {
            if (!Mathf.Approximately(_texture.width, (int)width) || !Mathf.Approximately(_texture.height, (int)height))
                UpdateTexture((int)width, (int)height);
        }



        private void OnBoxClicked()
        {
            var buttonRect = GUILayoutUtility.GetLastRect();

            var pt = GUIUtility.GUIToScreenPoint(new Vector2(buttonRect.x, buttonRect.y));
            var mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

            buttonRect.x = pt.x;
            buttonRect.y = pt.y;

            if (!buttonRect.Contains(mouse))
                return;

            _targetCoordinates.x = Mathf.Clamp(mouse.x - buttonRect.x, 0f, _texture.width);
            _targetCoordinates.y = Mathf.Clamp(mouse.y - buttonRect.y, 0f, _texture.height);

            int x = (int)_targetCoordinates.x;
            int y = (int)_targetCoordinates.y;

            var hsl = new ColorHsl(CalculateHue(x, y), Saturation, CalculateLightness(x, y));

            Selected = hsl.Color;

            OnChanged();
        }



        public float Saturation
        {
            get { return _saturationValue; }
            set
            {
                _saturationValue = value;
                UpdateTexture();
            }
        }



        public Color Selected
        {
            get { return _selected; }
            set
            {
                // calculate target coordinates for given color
                var hsl = value.GetHSL();

                _targetCoordinates.x = hsl.Hue * _texture.width;
                _targetCoordinates.y = hsl.Lightness * _texture.height;

                _selected = value;
            }
        }



        public Color CrosshairColor { get; set; }
    }
}
