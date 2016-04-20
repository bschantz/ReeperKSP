using System;
using System.Collections;
using ReeperCommon.Extensions;
using UnityEngine;

namespace ReeperKSP.Gui.Window.Decorators
{
    public class Resizable : WindowDecorator
    {
        private readonly Func<bool> _cbAllowResizing;

        [Flags]
        protected enum ActiveMode
        {
            None = 0,
            Right = 1 << 0,
            Bottom = 1 << 1,
            Both = Right | Bottom
        }


        public Vector2 HotzoneSize { get; set; }
        public Vector2 MinSize { get; set; }

        public Texture2D HintTexture
        {
            get { return _hintTexture; }
            set
            {
                if (value == null) throw new ArgumentException("Texture cannot be null");
                
                _hintTexture = value;
                _hintScreenRect.Set(0f, 0f, _hintTexture.width, _hintTexture.height);
            }
        }

        public float HintPopupDelay { get; set; }
        public Vector2 HintScale { get; set; }

        public event Callback DragBegin = delegate { };
        public event Callback DragEnd = delegate { };

        protected ActiveMode Mode { get; private set; }
        private Rect _rightRect = default(Rect);        // hotzone for changing width
        private Rect _bottomRect = default(Rect);       // hotzone for changing height

        private Texture2D _hintTexture;
        private Rect _hintScreenRect = default(Rect); 
        private IEnumerator _dragging;
        private float _delayAccumulator = 0f;
        

        public Resizable(
            IWindowComponent decoratedComponent, 
            Vector2 hotzoneSize, 
            Vector2 minSize, 
            Texture2D hintTexture, 
            float hintPopupDelay,
            Vector2 hintScale,
            Func<bool> cbAllowResizing) : base(decoratedComponent)
        {
            if (decoratedComponent == null) throw new ArgumentNullException("decoratedComponent");
            if (hintTexture == null) throw new ArgumentNullException("hintTexture");
            if (cbAllowResizing == null) throw new ArgumentNullException("cbAllowResizing");

            HotzoneSize = hotzoneSize;
            MinSize = minSize;
            HintTexture = hintTexture;
            HintPopupDelay = hintPopupDelay;
            HintScale = hintScale;
            _cbAllowResizing = cbAllowResizing;
        }


        public Resizable(
            IWindowComponent decoratedComponent,
            Vector2 hotzoneSize,
            Vector2 minSize,
            Texture2D hintTexture,
            float hintPopupDelay = 0.25f)
            : this(decoratedComponent, hotzoneSize, minSize, hintTexture, hintPopupDelay, Vector2.one, AllowResizingDefault)
        {
        }


        private static bool AllowResizingDefault()
        {
            return true;
        }

        public override void OnWindowDraw(int winid)
        {
            base.OnWindowDraw(winid);
            HandleSupportedEvents();
        }


        protected virtual void HandleSupportedEvents()
        {
            if (!_cbAllowResizing()) return;

            switch (Event.current.type)
            {
                case EventType.Repaint:
                    OnRepaint();
                    break;

                case EventType.MouseDown:
                    OnMouseDown();
                    break;

                case EventType.MouseDrag:
                    OnMouseDrag();
                    break;
            }
        }


        private void UpdateHotzoneRects()
        {
            _rightRect = new Rect(Dimensions.width - HotzoneSize.x,
                0f,
                HotzoneSize.x,
                Dimensions.height);

            _bottomRect = new Rect(0f,
                Dimensions.height - HotzoneSize.y,
                Dimensions.width,
                HotzoneSize.y);
        }


        private void OnMouseDown()
        {
            UpdateHotzoneRects();

            if ((Mode = GetMouseMode()) != ActiveMode.None)
                Event.current.Use();
        }


        private ActiveMode GetMouseMode()
        {
            var m = ActiveMode.None;

            if (_bottomRect.Contains(Event.current.mousePosition))
                m = ActiveMode.Bottom;

            if (_rightRect.Contains(Event.current.mousePosition))
                m |= ActiveMode.Right;

            return m;
        }


        private void OnMouseDrag()
        {
            if (Mode == ActiveMode.None) return;

            UpdateHotzoneRects();

            _dragging = UpdateMouseDrag(GUI.matrix);
            Event.current.Use();
        }


        private void OnRepaint()
        {
            UpdateHotzoneRects();

            if (!ShouldShowCursor())
            {
                _delayAccumulator = 0f;
                return;
            }

            _delayAccumulator += Time.deltaTime;
            if (_delayAccumulator < HintPopupDelay)
                return;

            var originalMatrix = GUI.matrix;
            var cursorAngle = GetCursorAngle();
            var pos = GetScreenPositionOfMouse();

            GUIUtility.RotateAroundPivot(cursorAngle, pos);
            GUIUtility.ScaleAroundPivot(HintScale, pos);

            _hintScreenRect.center = GUIUtility.ScreenToGUIPoint(pos);

            // note: this is a fix for blur caused by rotation > 0
            if (!Mathf.Approximately(0f, cursorAngle))
            {
                var mat = GUI.matrix;

                var m = mat.GetRow(0);
                m.w += 0.5f;
                mat.SetRow(0, m);

                m = mat.GetRow(1);
                m.w += 0.5f;
                mat.SetRow(1, m);

                GUI.matrix = mat;
            }

            Graphics.DrawTexture(_hintScreenRect, HintTexture);

            GUI.matrix = originalMatrix;
        }


        private float GetCursorAngle()
        {
            var currentMode = _dragging != null ? Mode : GetMouseMode();

            switch (currentMode)
            {
                case ActiveMode.Bottom:
                    return 90f;

                case ActiveMode.Right:
                    return 0f;

                case ActiveMode.Both:
                    return 45f;

                default:
                    return 0f;
            }
        }


        private bool ShouldShowCursor()
        {
            return _dragging != null || GetMouseMode() != ActiveMode.None;
        }


        public override void OnUpdate()
        {
            base.OnUpdate();
            if (_dragging != null)
                _dragging.MoveNext();
        }


        private IEnumerator UpdateMouseDrag(Matrix4x4 guiMatrix) // note: should GUI.matrix's scaling change while dragging, drag will break. So don't do that
        {
            OnDragBegin(guiMatrix);

            do
            {
                OnDragUpdate(guiMatrix);

                yield return 0;
            } while (Input.GetMouseButton(0) && !Input.GetKeyDown(KeyCode.Escape));

            Mode = ActiveMode.None;
            _dragging = null;

            OnDragEnd(guiMatrix);
        }

        protected virtual void OnDragBegin(Matrix4x4 guiMatrix)
        {
            DragBegin();
        }

        protected virtual void OnDragUpdate(Matrix4x4 guiMatrix)
        {
            // note: the user is dragging the scaled dimensions of the rect. We'll treat the current
            // coordinates as though they're dragging that scaled version and work backwards to come up
            // with a set of dimensions that would result in that size when scaled by GUI.matrix

            var mousePos = GetScreenPositionOfMouse();
            var visibleDimensions = Dimensions.Multiply(guiMatrix);

            var newWidth = (Mode & ActiveMode.Right) != 0 ? mousePos.x - visibleDimensions.x : visibleDimensions.width;
            var newHeight = (Mode & ActiveMode.Bottom) != 0 ? mousePos.y - visibleDimensions.y : visibleDimensions.height;

            Dimensions = new Rect(
                Dimensions.x,
                Dimensions.y,
                Mathf.Max(MinSize.x, newWidth / guiMatrix.m00),
                Mathf.Max(MinSize.y, newHeight / guiMatrix.m11));
        }


        protected virtual void OnDragEnd(Matrix4x4 guiMatrix)
        {
            DragEnd();
        }

        
        // in screen space (inverted y)
        protected static Vector2 GetScreenPositionOfMouse()
        {
            return new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        }
    }
}
