using System;
using UnityEngine;

namespace ReeperKSP.Gui.Window.Decorators
{
// ReSharper disable once UnusedMember.Global
    public class AdjustableScale : Resizable
    {
        private readonly WindowScale _scale;
        private readonly float _minScalar;
        private readonly float _maxScalar;

        public AdjustableScale(
            WindowScale decoratedComponent,
            Vector2 hotzoneSize, 
            Vector2 minSize,
            float minScalar,
            float maxScalar,
            Texture2D hintTexture, 
            float hintPopupDelay, 
            Vector2 hintScale,
            Func<bool> allowScaling) : base(decoratedComponent, hotzoneSize, minSize, hintTexture, hintPopupDelay, hintScale, allowScaling)
        {
            _scale = decoratedComponent;
            _minScalar = minScalar;
            _maxScalar = maxScalar;
        }


        protected override void OnDragUpdate(Matrix4x4 guiMatrix)
        {
            // it's a bit simpler to pretend the window is at 0,0 and that we want to match width to the
            // mouse's (x or y or both) position
            var adjustedMousePos = GetScreenPositionOfMouse() - new Vector2(Dimensions.x, Dimensions.y);
            
            var scalar = 0f;

            switch (Mode)
            {
                case ActiveMode.Both: // use the greater of the two
                    scalar = Mathf.Max(adjustedMousePos.x / Dimensions.width, adjustedMousePos.y / Dimensions.height);
                    break;

                case ActiveMode.Bottom: // height only
                    scalar = adjustedMousePos.y / Dimensions.height;
                    break;

                case ActiveMode.Right: // width only
                    scalar = adjustedMousePos.x / Dimensions.width; 
                    break;
            }

            scalar = Mathf.Clamp(scalar, _minScalar, _maxScalar);
            _scale.Scale = new Vector2(scalar, scalar);
        }

    }
}
