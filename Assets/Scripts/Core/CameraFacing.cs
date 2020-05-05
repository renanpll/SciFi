using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SciFi.Core
{
    public class CameraFacing : MonoBehaviour
    {
        void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
