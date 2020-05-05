using GameDevTV.Inventories;
using UnityEngine;

namespace SciFi.Control
{
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        Pickup _pickup;
        private void Awake()
        {
            _pickup = GetComponent<Pickup>();
        }

        public CursorType GetCursorType()
        {
            if (_pickup.CanBePickedUp())
            {
                return CursorType.Pickup;
            }
            else
            {
                return CursorType.FullPickup;
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _pickup.PickupItem();
            }

            return true;
        }

    }

}
