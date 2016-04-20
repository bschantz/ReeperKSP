using ReeperCommon.Extensions;
using UnityEngine;

namespace ReeperKSP.Gui.Window.Decorators
{
    public class ClampToScreen : WindowDecorator
    {
        public ClampToScreen(IWindowComponent decoratedComponent) : base(decoratedComponent)
        {
        }


        public override void OnWindowPostDraw()
        {
            base.OnWindowPostDraw();
            Dimensions = KSPUtil.ClampRectToScreen(Dimensions.MultiplyScale(GUI.matrix)).InvertScale(GUI.matrix);
        }
    }
}
