using ReeperKSP.Serialization;
using UnityEngine;

namespace ReeperKSP.Gui.Window.Decorators
{
// ReSharper disable once UnusedMember.Global
    // note: due the way this works, this should be one of the first decorators if used, if any other decorators
    // make use of GUI.matrix in some way
    public class WindowScale : WindowDecorator
    {
// ReSharper disable once MemberCanBePrivate.Global
        [ReeperPersistent] private Vector3 _scale = Vector3.one;

        public WindowScale(IWindowComponent decoratedComponent, Vector2 initialScale) : base(decoratedComponent)
        {
            Scale = initialScale;
        }


        public override void OnWindowPreDraw()
        {
            base.OnWindowPreDraw();
            GUIUtility.ScaleAroundPivot(_scale, new Vector2(Dimensions.x, Dimensions.y));
        }


        public Vector2 Scale
        {
            get { return _scale; }
            set { _scale = new Vector3(value.x, value.y, 1f); }
        }
    }
}
