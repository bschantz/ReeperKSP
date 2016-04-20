using UnityEngine;

namespace ReeperKSP.Gui.Window.Buttons
{
    public interface ITitleBarButton
    {
        void Click();

        bool Enabled { get; }
        Texture Texture { get; }
        GUIStyle Style { get; }
    }
}
