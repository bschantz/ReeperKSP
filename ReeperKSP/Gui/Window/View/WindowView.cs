using System;
using ReeperCommon.Extensions;
using UnityEngine;

namespace ReeperKSP.Gui.Window.View
{
    // note: do not hide this behind an interface; it will mess with Unity's operator overloads of == and Equals
// ReSharper disable once ClassNeverInstantiated.Global
    public class WindowView : MonoBehaviour
    {
        public IWindowComponent Logic { get; private set; }


// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedMember.Global
        private void OnGUI()
        {
            if (Logic.IsNull() || !Logic.Visible) return;

            Logic.OnWindowPreDraw();

            if (!Logic.Skin.IsNull())
                GUI.skin = Logic.Skin;

            
            Logic.Dimensions = GUILayout.Window(Logic.Id.Value, Logic.Dimensions, DrawWindow,
                Logic.Title);
            
        }


        private void DrawWindow(int winid)
        {
            Logic.OnWindowDraw(winid);
            Logic.OnWindowFinalize(winid);
        }


// ReSharper disable once UnusedMember.Global
        private void Update()
        {
            if (Logic.IsNull()) return;

            Logic.OnUpdate();
        }


        public static WindowView Create(IWindowComponent window, string goName = "WindowView")
        {
            if (window == null) throw new ArgumentNullException("window");

            var view = new GameObject(goName).AddComponent<WindowView>();
            view.Logic = window;

            return view;
        }
    }
}
