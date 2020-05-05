using UnityEngine;


namespace SciFi.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        private IAction _currentAction;

        public void StartAction(IAction action)
        {
            if (_currentAction != null)
            {
                if (action != _currentAction)
                {
                    _currentAction.Cancel();
                }
            }

            _currentAction = action;
        }
    }
}