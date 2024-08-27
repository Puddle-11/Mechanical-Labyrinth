using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.InspectorCurveEditor;
using static UnityEngine.GraphicsBuffer;

public class Bomber : BaseEnemy
{
    protected override void StateHandler()
    {
        if (agent == null) return;
        EnemyStatus(ref currState);
        switch (currState)
        {
            case EnemyState.Investigate:
                agent.stoppingDistance = 0;
                break;
            case EnemyState.Persue:
            case EnemyState.Attack:
                SetNavmeshTarget(target.transform.position);
                agent.stoppingDistance = stoppingDistance;
                if (agent.remainingDistance <= stoppingDistance) FacePlayer();
                break;
        }
        if (currState == EnemyState.Attack)
        {
            weaponScr?.Attack();
        }
    }


}
