using System;
using UnityEngine;

namespace ReeperKSP.Gui.Window.Buttons
{
    public class BasicTitleBarButton : ITitleBarButton
    {
        private readonly Callback _callback;

        public BasicTitleBarButton(GUIStyle style, Texture texture, Callback callback)
            : this(texture, callback)
        {
            if (style == null) throw new ArgumentNullException("style");

            Style = style;
        }


        public BasicTitleBarButton(Texture texture, Callback callback)
        {
            if (texture == null) throw new ArgumentNullException("texture");
            if (callback == null) throw new ArgumentNullException("callback");

            Texture = texture;
            _callback = callback;
            Enabled = true;
        }

        public void Click()
        {
            _callback();
        }


        public GUIStyle Style { get; set; }
        public bool Enabled { get; set; }
        public Texture Texture { get; set; }
    }
}