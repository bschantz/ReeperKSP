using System;

namespace ReeperKSP.Gui.Window.Decorators
{
// ReSharper disable once UnusedMember.Global
    // note: something doesn't quite work right with this
    public class HideOnF2 : WindowDecorator
    {
        private bool _interfaceVisible = false;
        private bool _restoreVisibility = false; // because we don't want to accidentally show the window if it was
                                                 // hidden when F2 was pressed (unless the caller explicitly makes it
                                                 // visible during this time)



        public HideOnF2(IWindowComponent decoratedComponent) : base(decoratedComponent)
        {
            throw new NotImplementedException("this class buggy; don't use till fixed");
            GameEvents.onShowUI.Add(Show);
            GameEvents.onHideUI.Add(Hide);
        }



        ~HideOnF2()
        {
            GameEvents.onHideUI.Remove(Hide);
            GameEvents.onShowUI.Remove(Show);
        }



        private void Show()
        {
            _interfaceVisible = true;
            Visible = true;
        }



        private void Hide()
        {
            _interfaceVisible = false;

            base.Visible = false;
        }



        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                _restoreVisibility = value;

                base.Visible = _restoreVisibility && _interfaceVisible; // if UI not visible, 
                            // don't allow window to be explicitly made visible (although the change is 
                            // cached so it will appear when UI is shown again)

            }
        }
    }
}
