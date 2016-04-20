using ReeperKSP.Serialization;
using UnityEngine;

namespace ReeperKSP.Gui.Window
{
    public interface IWindowComponent : IReeperPersistent
    {
        void OnWindowPreDraw();
        void OnWindowDraw(int winid);
        void OnWindowFinalize(int winid);
        void OnWindowPostDraw();

        void OnUpdate();

        Rect Dimensions { get; set; }
        WindowID Id { get; }
        string Title { get; set; }
        GUISkin Skin { get; set;}
        bool Draggable { get; set; }
        bool Visible { get; set; }

        float Width { get; set; }
        float Height { get; set; }
    }
}
