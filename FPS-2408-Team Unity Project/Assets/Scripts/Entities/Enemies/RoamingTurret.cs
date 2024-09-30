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
    [SerializeField] protected Vector3 shoulderPos;
    [SerializeField] protected float legDistance;
    [SerializeField] protected Leg[] legs;
    [SerializeField] protected LayerMask ground;
    [SerializeField] protected float groundSearchDist;
    [SerializeField] protected float maxDistanceFromTarget;
    [SerializeField] protected float hardMaxDistance;
    [SerializeField] protected float legMoveSpeed;
    [SerializeField] protected float legLiftHeight;
    [SerializeField] protected AnimationCurve legLiftCurve;
    [SerializeField] protected AudioClip walkingSound;
    [SerializeField] protected AudioSource audioSource;
    [Range(0, 1)][SerializeField] protected float walkingVolumeSound;

    #region Custom Structs and Enum
    [System.Serializable]
    public struct Leg
    {
        public Transform anchor;
       [HideInInspector] public Vector3 absolutePos;
        [HideInInspector] public Vector3 currPos;
        public Transform target;
         public bool isMoving;
        public float DistanceToAbsolute()
        {
            return Vector3.Distance(currPos, absolutePos);
        }
    }
    #endregion

    #region MonoBehavior Methods
    public override void Start()
    {
        base.Start();
        UpdateAnchors();
        UpdateAbsolutePos();
    }
    public override void Update()
    {
      
        base.Update();
        UpdateLegs();
    }
    #endregion
    private void UpdateLegs()
    {
        UpdateAbsolutePos();
        for (int i = 0; i < legs.Length; i++)
        {
            int A1 = i >= legs.Length - 1 ? 0 : i + 1;
            int A2 = i <= 0 ? legs.Length - 1 : i - 1;


            if (legs[i].DistanceToAbsolute() > maxDistanceFromTarget)
            {
                if (!legs[A1].isMoving && !legs[A2].isMoving)
                {
                    StartCoroutine(MoveLeg(i));
                    StartCoroutine(MoveLeg((i + 2) % legs.Length));
                }
            }
            if (legs[i].DistanceToAbsolute() > hardMaxDistance)
            {
                StartCoroutine(MoveLeg(i, true));
            }
            if (legs[i].target != null) legs[i].target.position = legs[i].currPos;
        }
    
    }
    private void UpdateAnchors()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            legs[i].anchor.localPosition = legs[i].anchor.localPosition.normalized * legDistance;
        }
    }
    public IEnumerator MoveLeg(int index, bool _override = false)
    {
        if (!_override && legs[index].isMoving) yield break;
        legs[index].isMoving = true;
        float timer = 0;
        while (timer < legMoveSpeed)
        {
            Vector3 lerpPos = Vector3.Lerp(legs[index].currPos, legs[index].absolutePos, timer / legMoveSpeed);
            legs[index].currPos = lerpPos + Vector3.up * legLiftCurve.Evaluate(timer / legMoveSpeed) * legLiftHeight;
            timer += Time.deltaTime;
            yield return null;
        }

        if (audioSource != null) 
        {
            audioSource.PlayOneShot(walkingSound, walkingVolumeSound);
        }

        legs[index].currPos = legs[index].absolutePos; //snaps leg to final position
        legs[index].isMoving = false;
    }
    private void UpdateAbsolutePos()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            Vector3 rayPos = shoulderPos + new Vector3(legs[i].anchor.position.x, transform.position.y, legs[i].anchor.position.z);

            RaycastHit hit;
            if (Physics.Raycast(rayPos, Vector3.down, out hit, groundSearchDist, ground))
                legs[i].absolutePos = hit.point;
            else 
               legs[i].absolutePos = rayPos + Vector3.down * groundSearchDist;
        }
    }


}
