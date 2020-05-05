using UnityEngine;

namespace SciFi.Core
{
    public class FollowCamera : MonoBehaviour
    {

        // Update is called once per frame
        void Update()
        {
            RotateCamera();
        }

        private void RotateCamera()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.Rotate(0f, 90.0f, 0.0f, Space.World);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.Rotate(0.0f, -90.0f, 0.0f, Space.World);
            }
        }
    }

}

