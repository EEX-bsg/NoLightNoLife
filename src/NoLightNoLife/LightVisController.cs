using System;
using System.Collections.Generic;
using Modding;
using UnityEngine;
using NLNL;


namespace NLNL
{
    class LightVisController : MonoBehaviour
    {
        void LateUpdate()
        {
            Transform camera = Camera.main.transform;
            transform.rotation = camera.rotation;
        }
    }
}
