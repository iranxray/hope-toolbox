namespace Xray.Hope.Web.Client.Services.State
{
    public class StateStore
    {
        public event Func<StateEvent, Task> OnStateEventRaised;

        public void Publish(StateEvent stateEvent)
        {
            if (stateEvent != null)
            {
                OnStateEventRaised?.Invoke(stateEvent);
            }
        }
    }
}
