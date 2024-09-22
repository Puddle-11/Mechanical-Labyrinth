using UnityEngine;
//====================================
//REWORKED
//====================================
public class SharedEnemyBehavior : BaseEntity
{

    [Space]
    [Header("SHARED VARIABLES")]
    [Header("_______________________________")]
    [Space]
    [SerializeField] protected Transform headPos;
    [SerializeField] protected int sneakDamageMultiplyer = 2;
    [Space]
    [SerializeField] protected float stoppingDistance = 7;
    [SerializeField] protected float rotationSpeed = 180;
    [SerializeField] protected float roamTimer = 2;
    [SerializeField] protected float roamingDistance = 15;

    [Header("DETECTION")]
    [Space]
    [SerializeField] protected LayerMask sightMask;
    [SerializeField] protected float attackRange = 7;
    [SerializeField] protected float sightRange = 13.5f;
    [SerializeField] protected float hearingRange = 20;
    [Space]
    [SerializeField] protected float attackAngle = 50;
    [SerializeField] protected float sightAngle = 90;
    [Space]
    [SerializeField] protected float cullingDist;
    [SerializeField] protected GameObject target;
    public bool InCullingDist()
    {
        if (target == null) return false;
        return Vector3.Distance(target.transform.position, gameObject.transform.position) < cullingDist;
    }
    public Vector3 DirectionToTarget()
    {
        if (target == null) return Vector3.zero;
        Vector3 targetDir = (target.transform.position - (headPos != null ? headPos.transform.position : transform.transform.position)).normalized;
        return targetDir;
    }

    public void SetTarget(GameObject _target)
    {
        target = _target;
    }

    public bool InAngleRange(float _range)
    {
        if (target == null) return false;
        //Checks if target is in range, Gets the direction to the target and checks the angle from transform.forward
        //if less than sight radius, target is in sight
        if (IsInRange(sightRange) && GetAngle() < _range / 2)
        {
            return true;
        }
        return false;
    }
    public float GetAngle()
    {
        if (target == null) return 0;
        Vector3 dPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        Vector3 targetDir = (dPos - transform.position).normalized;
        return Vector3.Angle(targetDir, transform.forward);
    }
    public bool InAngleRange()
    {
        return InAngleRange(sightAngle);
    }
    protected bool IsInRange() { return IsInRange(hearingRange); }
    protected bool IsInRange(out Vector3 _dirToTarget) { return IsInRange(out _dirToTarget, hearingRange); }
    public bool IsInRange(float _dist)
    {
        Vector3 _dir;
        return IsInRange(out _dir, _dist);
    }
    public void FaceTarget()
    {
        Vector3 targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

        Quaternion rot = Quaternion.LookRotation(targetPos - transform.position);

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotationSpeed);
    }
    public bool IsInRange(out Vector3 _dirToTarget, float _dist)
    {
        _dirToTarget = DirectionToTarget();
            Debug.DrawRay(headPos != null ? headPos.transform.position : transform.position, _dirToTarget * _dist);
        Debug.Log("hit in range");
        if (DistanceToTarget() < _dist)
        {
            RaycastHit hit;
            if (Physics.Raycast(headPos != null ? headPos.transform.position: transform.position, _dirToTarget, out hit, _dist, ~sightMask))
            {
                Debug.Log("Hit Object: " + hit.collider.name);

                if (hit.collider.gameObject == target)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public float DistanceToTarget()
    {
        if (target == null) return 0; 
        return Vector3.Distance(target.transform.position, transform.position);
    }
}
