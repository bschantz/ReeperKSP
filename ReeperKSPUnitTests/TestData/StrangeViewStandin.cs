using ReeperKSP.Gui;
using ReeperKSP.Gui.Window;
using ReeperKSP.Serialization;
using UnityEngine;

namespace ReeperKSPUnitTests.TestData
{
    public abstract class StrangeViewStandin : IWindowComponent
    {
        [ReeperPersistent]
        private Rect _windowRect = new Rect(0f, 0f, 200f, 300f);
        [ReeperPersistent]
        private WindowID _id = new WindowID();
        [ReeperPersistent]
        private string _title = string.Empty;
        [ReeperPersistent]
        private bool _draggable = false;
        [ReeperPersistent]
        private bool _visible = true;

        #region IWindowComponent

        public virtual void OnWindowPreDraw()
        {

        }

        public virtual void OnWindowDraw(int winid)
        {

        }


        public virtual void OnWindowFinalize(int winid)
        {
  
        }


        public virtual void OnWindowPostDraw()
        {
            
        }


        public virtual void OnUpdate()
        {

        }

        public virtual void DuringSerialize(IConfigNodeSerializer formatter, ConfigNode node)
        {
        }


        public virtual void DuringDeserialize(IConfigNodeSerializer formatter, ConfigNode node)
        {
        }


        public Rect Dimensions
        {
            get { return _windowRect; }
            set { _windowRect = value; }
        }


        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }


        public GUISkin Skin { get; set; }


        public bool Draggable
        {
            get { return _draggable; }
            set { _draggable = value; }
        }


        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }


        public WindowID Id
        {
            get { return _id; }
            // ReSharper disable once UnusedMember.Global
            set { _id = value; }
        }


        public float Width
        {
            get { return _windowRect.width; }
            set { _windowRect.width = value; }
        }

        public float Height
        {
            get { return _windowRect.height; }
            set { _windowRect.height = value; }
        }

        #endregion
    }
}