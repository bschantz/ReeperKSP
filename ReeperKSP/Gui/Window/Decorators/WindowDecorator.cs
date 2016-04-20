using System;
using ReeperKSP.Serialization;
using UnityEngine;

namespace ReeperKSP.Gui.Window.Decorators
{
    public abstract class WindowDecorator : IWindowComponent
    {
        public readonly IWindowComponent Decorated;

        public WindowDecorator(IWindowComponent decoratedComponent)
        {
            if (decoratedComponent == null) throw new ArgumentNullException("decoratedComponent");

            Decorated = decoratedComponent;
        }


        public virtual void OnWindowPreDraw()
        {
            Decorated.OnWindowPreDraw();
        }


        public virtual void OnWindowDraw(int winid)
        {
            Decorated.OnWindowDraw(winid);
        }


        public virtual void OnWindowFinalize(int winid)
        {
            Decorated.OnWindowFinalize(winid);
        }


        public virtual void OnWindowPostDraw()
        {
            Decorated.OnWindowPostDraw();
        }


        public virtual void OnUpdate()
        {
            Decorated.OnUpdate();
        }


        public virtual void DuringSerialize(IConfigNodeSerializer formatter, ConfigNode node)
        {
        }


        public virtual void DuringDeserialize(IConfigNodeSerializer formatter, ConfigNode node)
        {
        }


        public Rect Dimensions
        {
            get
            {
                return Decorated.Dimensions;
            }
            set
            {
                Decorated.Dimensions = value;
            }
        }


        public string Title
        {
            get { return Decorated.Title; }
            set { Decorated.Title = value; }
        }


        public GUISkin Skin
        {
            get { return Decorated.Skin; }
            set { Decorated.Skin = value; }
        }


        public bool Draggable { 
            get { return Decorated.Draggable; }
            set { Decorated.Draggable = value; }
        }


        public virtual bool Visible
        {
            get { return Decorated.Visible; }
            set { Decorated.Visible = value; }
        }


        public WindowID Id { get { return Decorated.Id; } }


        public float Width
        {
            get { return Decorated.Width; }
            set { Decorated.Width = value; }
        }

        public float Height
        {
            get { return Decorated.Height; }
            set { Decorated.Height = value; }
        }
    }
}
