using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GlobalMethods : MonoBehaviour
{
    public static RaycastHit RaycastFromCam(LayerMask _ignoreMask, float _dist)
    {
        RaycastHit res;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out res, _dist, ~_ignoreMask);

        return res;
    }
    public static RaycastHit RaycastFromCam(LayerMask _ignoreMask)
    {
        return RaycastFromCam(_ignoreMask, 50);
    }
}
