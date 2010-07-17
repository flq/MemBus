namespace MemBus.Support
{
    /// <summary>
    /// Provides a fire method to fire the trigger "manually"
    /// </summary>
    public class ManualTrigger : Trigger
    {
        public void Trigger()
        {
            fire();
        }
    }
}