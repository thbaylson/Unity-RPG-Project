using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        IAction lastAction;

        public void StartAction(IAction action)
        {
            // If we're doing the same action, then just keep doing it
            if (lastAction == action) return;
            
            // If the action has changed, cancel the lastAction
            if(lastAction != null)
            {
                lastAction.Cancel();
            }

            lastAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}
