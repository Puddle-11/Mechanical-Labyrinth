using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerCamper : BaseEnemy
{
    [SerializeField] private IKSolver[] Arms;
    [SerializeField] private float maxSearchDist;
    [SerializeField] private LayerMask searchMask;
    public override  void Start()
    {
        for (int i = 0; i < Arms.Length; i++)
        {
            RaycastHit hit;
            int j = 0;
            while (j < 100)
            {
                Vector3 dir = Random.insideUnitSphere;
                if (Physics.Raycast(transform.position, dir, out hit, Arms[i].GetTotalLength(), searchMask))
                {
                    if (hit.distance > Arms[i].GetTotalLength() / 2)
                    {
                        Arms[i].SetTarget(hit.point);
                        break;
                    }
                }

                //attempt to find hit in area


            }
            if(j == 100)
            {
                Destroy(Arms[i]);
            }

        }

        base.Start();
    }
}
