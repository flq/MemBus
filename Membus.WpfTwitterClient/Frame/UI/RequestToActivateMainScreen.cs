using System;
using Caliburn.Micro;
using MemBus.Support;

namespace Membus.WpfTwitterClient.Frame.UI
{
    public class RequestToActivateMainScreen
    {
        public RequestToActivateMainScreen(Type typeOfScreen)
        {
            TypeOfScreen = typeOfScreen;
        }

        public Type TypeOfScreen { get; private set; }
        public Screen Screen { get; private set; }

        public void SetScreen(Screen screen)
        {
            if (ScreenAvailable)
                throw new InvalidOperationException("Message already has a screen set of type " + Screen.GetType().Name);
            if (!TypeOfScreen.IsAssignableFrom(screen.GetType()))
                throw new InvalidOperationException("Screen {0} is incompatible with {1}".Fmt(screen.GetType().Name, TypeOfScreen.Name));
            Screen = screen;
        }

        public bool ScreenAvailable
        {
            get { return Screen != null; }
        }
    }
}