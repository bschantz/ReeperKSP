using System;
using System.Collections.Generic;
using ReeperCommon.Extensions;
using ReeperKSP.Gui.Window.Buttons;
using UnityEngine;

namespace ReeperKSP.Gui.Window.Decorators
{
    public class TitleBarButtons : WindowDecorator
    {
        private readonly List<ITitleBarButton> _buttons = new List<ITitleBarButton>();
        private readonly ButtonAlignment _alignment;
        private readonly Vector2 _offset = Vector2.zero;

        public enum ButtonAlignment
        {
            Center,
            Left,
            Right
        }


        public TitleBarButtons(
            IWindowComponent window,
            ButtonAlignment alignment = ButtonAlignment.Right,
            Vector2 offset = default(Vector2)) : base(window)
        {
            if (window == null) throw new ArgumentNullException("window");

            _alignment = alignment;
            _offset = offset;
        }


        public override void OnWindowDraw(int winid)
        {
            base.OnWindowDraw(winid);
            DrawTitleBarButtons();
        }


        private void DrawTitleBarButtons()
        {
            GUILayout.BeginArea(new Rect(_offset.x, _offset.y, Dimensions.width - _offset.x * 2f, Dimensions.height));
                GUILayout.BeginHorizontal();
                {
                    if (_alignment != ButtonAlignment.Left)
                        GUILayout.FlexibleSpace();

                    _buttons.ForEach(DrawButton);

                    if (_alignment != ButtonAlignment.Right)
                        GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            GUILayout.EndArea();

        }


        private void DrawButton(ITitleBarButton button)
        {
            if (!button.Enabled) return;

            if (GUILayout.Button(button.Texture, button.Style.IsNull() ? GUI.skin.button : button.Style,
                GUILayout.ExpandWidth(false),
                GUILayout.ExpandHeight(false)))
                button.Click();
        }


        public void AddButton(ITitleBarButton button)
        {
            if (button == null) throw new ArgumentNullException("button");

            if (_buttons.Contains(button))
                throw new InvalidOperationException("TitleBar already contains button");

            _buttons.Add(button);
        }

    }
}
