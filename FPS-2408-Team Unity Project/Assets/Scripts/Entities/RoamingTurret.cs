using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RoamingTurret : BaseEnemy
{
    [Header("ROAMING TURRET")]
    [Header("_______________________________")]
    [Space]
    [SerializeField] private Vector3 shoulderPos;
    [SerializeField] private Leg[] legs;
    [SerializeField] private Transform[] legAnchorPos;
    private Vector3[] legTargetPos;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float groundSearchDist;
    [SerializeField] private float maxDistanceFromTarget;
    [SerializeField] private float hardMaxDistance;
    [SerializeField] private float legMoveSpeed;
    [SerializeField] private AnimationCurve legLiftCurve;
    [SerializeField] private float legLiftHeight;
    [System.Serializable]
    public struct Leg
    {
        public GameObject legObj;
        public Vector3 worldPos;
        public bool isMoving;
    }
    public override void Start()
    {
        base.Start();

     legTargetPos = UpdateAnchorPos();
    }
    public override void Update()
    {
        base.Update();
        UpdateLegs();
    }
    private void UpdateLegs()
    {
        legTargetPos = UpdateAnchorPos();

        for (int i = 0; i < legs.Length; i++)
        {
            int A1 = i >= legs.Length - 1 ? 0 : i + 1;
            int A2 = i <= 0 ? legs.Length - 1 : i - 1;
            if (Vector3.Distance(legs[i].worldPos, legTargetPos[i]) > maxDistanceFromTarget)
            {
                if (!legs[A1].isMoving && !legs[A2].isMoving)
                {

                    StartCoroutine(MoveLeg(i));
                }
                else if (Vector3.Distance(legs[i].worldPos, legTargetPos[i]) > hardMaxDistance)
                {
                    StartCoroutine(MoveLeg(i));

                }
            }
            if (i > legs.Length)
            {
                Debug.LogWarning("Incorrect number of legs assigned to " + gameObject.name);
            }
            else
            {
                legs[i].legObj.transform.position = legs[i].worldPos;
                Quaternion rot = Quaternion.LookRotation(shoulderPos + transform.position - legs[i].legObj.transform.position);
                legs[i].legObj.transform.rotation = rot;
            }
        }
    }
    public IEnumerator MoveLeg(int index)
    {
        if (legs[index].isMoving) yield break;
        legs[index].isMoving = true;
        float timer = 0;
        while (timer < legMoveSpeed)
        {
            Vector3 lerpPos = Vector3.Lerp(legs[index].worldPos, legTargetPos[index], timer / legMoveSpeed);
            legs[index].worldPos = lerpPos + Vector3.up * legLiftCurve.Evaluate(timer / legMoveSpeed) * legLiftHeight;
            timer += Time.deltaTime;
            yield return null;

        }
        legs[index].worldPos = legTargetPos[index];
        legs[index].isMoving = false;
    }
    private Vector3[] UpdateAnchorPos()
    {
        Vector3[] temp = new Vector3[legAnchorPos.Length];
        for (int i = 0; i < legAnchorPos.Length; i++)
        {
            Vector3 rayPos = shoulderPos + new Vector3(legAnchorPos[i].position.x, 0, legAnchorPos[i].position.z);
            RaycastHit hit;
            if (Physics.Raycast(rayPos, Vector3.down, out hit, groundSearchDist, ground))
            {
                temp[i] = hit.point;
            }
            else
            {
                temp[i] = rayPos + Vector3.down * groundSearchDist;
            }
        }
        return temp;
    }
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (legTargetPos != null)
        {
            for (int i = 0; i < legTargetPos.Length; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(legTargetPos[i], 0.2f);

            }
        }
        if (legs != null)
        {
            for (int i = 0; i < legs.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(legs[i].worldPos, 0.1f);

            }
        }
    }
}
