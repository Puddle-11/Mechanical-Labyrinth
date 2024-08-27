using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Rope : MonoBehaviour
{
    [Header("GENERAL")]
    [Space]
    [SerializeField] protected Transform[] anchors;
    [SerializeField] protected float ropeLength;


    [SerializeField] protected LineRenderer lnRef;
    [SerializeField] protected int lineResolution = 10;
    [Space]
    [Header("PHYSICS")]
    [Space]
    [SerializeField] protected float springConstant = 0.25f;
    [SerializeField] protected float dampeningForce = 5f;
    [SerializeField] protected float acceptableMargin = 0.001f;

    [SerializeField] protected bool dynamic = true;

    [SerializeField] protected float swaySpeed = 0.5f;
    [SerializeField] protected Vector3 swayDir;
    [SerializeField] protected float softCollForce = 0.02f;
    [SerializeField] protected GameObject collisionDetectorPrefab;
    protected Vector3[] Positions;
    protected ControlPoint[] currentControlPos;
    protected float swayTimer;
    [System.Serializable]
    public struct ControlPoint
    {
        public Vector3 absolutePosition;
        public Vector3 currentPosition;
        public Vector3 currenInertia;
        public GameObject inputCollider;
    }

    public virtual void Start()
    {
        SetRope();
    }
    public void SetColor(Color _col)
    {
        GradientAlphaKey[] gradientAlphaKeys = new GradientAlphaKey[1];
        GradientColorKey[] gradientColorKeys = new GradientColorKey[1];
        gradientColorKeys[0] = new GradientColorKey(_col, 0f);
        gradientAlphaKeys[0] = new GradientAlphaKey(1f, 0f);
        Gradient tempGrad = new Gradient();
        tempGrad.SetKeys(gradientColorKeys, gradientAlphaKeys);
        lnRef.colorGradient = tempGrad;
    }
    public void RopeInput(Vector3 _pos, int _index)
    {
        Vector3 normalizedDir = (currentControlPos[_index].currentPosition - new Vector3(_pos.x, currentControlPos[_index].currentPosition.y, _pos.z)).normalized;
        currentControlPos[_index].currenInertia += normalizedDir * softCollForce * Vector3.Distance(anchors[_index].position, anchors[_index + 1].position);
   
    
    }
    public void SetRope()
    {
        swayTimer = Random.Range(0f, Mathf.PI);
        currentControlPos = new ControlPoint[anchors.Length - 1];


        UpdateCurveAnchorPoints();
        GenerateColliders();


        for (int i = 0; i < currentControlPos.Length; i++)
        {
            
               
            currentControlPos[i].currentPosition = currentControlPos[i].absolutePosition;
            currentControlPos[i].currenInertia = Vector3.zero;
        }
        Positions = new Vector3[lineResolution * currentControlPos.Length];
        UpdateRopePhysics();
        DrawRope(true);
    }
    public virtual void GenerateColliders()
    {
        if (collisionDetectorPrefab != null)
        {
            for (int i = 0; i < currentControlPos.Length; i++)
            {

                Vector3 instancePos = CalculateCurve(0.5f, anchors[i].position, anchors[i + 1].position, currentControlPos[i].absolutePosition);
                Quaternion lookRot = Quaternion.LookRotation(anchors[i].position - instancePos);
                GameObject trig = Instantiate(collisionDetectorPrefab, instancePos, lookRot, transform);
                RopeInputReceiver ropeinputRef;
                if (trig.TryGetComponent<RopeInputReceiver>(out ropeinputRef))
                {
                    ropeinputRef.SetIndex(i);
                    ropeinputRef.SetRopeRef(this);
                }
                currentControlPos[i].inputCollider = trig;

            }
        }
    }

    public virtual void UpdateColliders()
    {
        if (collisionDetectorPrefab != null)
        {
            for (int i = 0; i < currentControlPos.Length; i++)
            {
                Vector3 instancePos = CalculateCurve(0.5f, anchors[i].position, anchors[i + 1].position, currentControlPos[i].absolutePosition);
                Quaternion lookRot = Quaternion.LookRotation(anchors[i].position - instancePos);
                currentControlPos[i].inputCollider.transform.rotation = lookRot;
                currentControlPos[i].inputCollider.transform.position = instancePos;
            }
        }
    }
    public void SetAnchors(Transform[] _transforms)
    {
        anchors = _transforms;
        SetRope();
    }
    public void SetRopeLength(float _length)
    {
        ropeLength = _length;
    }
    public void SetAnchors(Vector3[] _pos)
    {


        Transform[] transforms = new Transform[_pos.Length];


        for (int i = 0; i < transforms.Length; i++)
        {
            GameObject temp = new GameObject("Anchor" + gameObject.name +  " Pos " + i);
            temp.transform.position = _pos[i];
            temp.transform.parent = transform;
            transforms[i] = temp.transform;
        }
        SetAnchors(transforms);
    }
    // Update is called once per frame
    public virtual void Update()
    {
        if (dynamic)
        {
            UpdateRopePhysics();
            DrawRope(false);
            UpdateColliders();
        }
    }
    private void UpdateRopePhysics()
    {
        //Update sway timer
   
        UpdateCurveAnchorPoints();

        for (int i = 0; i < currentControlPos.Length; i++)
        {
            float currForce = Vector3.Distance(currentControlPos[i].currentPosition, currentControlPos[i].absolutePosition) * springConstant;
            Vector3 direction = (currentControlPos[i].absolutePosition - currentControlPos[i].currentPosition).normalized;
            currentControlPos[i].currenInertia += direction * currForce * Time.deltaTime;
            currentControlPos[i].currenInertia /= 1 + dampeningForce * Time.deltaTime;
            currentControlPos[i].currentPosition += currentControlPos[i].currenInertia;
        }
    }
    public void DrawRope(bool _override)
    {
        for (int i = 0; i < currentControlPos.Length; i++)
        {
            if (currentControlPos[i].currenInertia.magnitude > acceptableMargin || _override)
            {
                for (int j = 0; j < lineResolution; j++)
                {
                    Positions[(i * lineResolution) + j] = CalculateCurve(j / (float)(lineResolution - 1), anchors[i].position, anchors[i + 1].position, currentControlPos[i].currentPosition);
                }

            }
        }
        lnRef.positionCount = Positions.Length;
        lnRef.SetPositions(Positions);
    }
    private void UpdateCurveAnchorPoints()
    {
        if (swayTimer > Mathf.PI * 2)
        {
            swayTimer = 0;
        }
        else
        {
            swayTimer += Time.deltaTime * swaySpeed;
        }
        for (int i = 0; i < currentControlPos.Length; i++)
        {
            currentControlPos[i].absolutePosition = Mathf.Sin(swayTimer) * swayDir + ((anchors[i + 1].position + anchors[i].position) / 2) + Vector3.down * Mathf.Clamp(ropeLength - Vector3.Distance(anchors[i].position, anchors[i + 1].position), 0f, Mathf.Infinity);
        }
    }
    protected Vector3 CalculateCurve(float t, Vector3 p1, Vector3 p2, Vector3 control)
    {
        return Mathf.Pow((1-t),2) * p1 + 2 * (1 - t) * t * control + Mathf.Pow(t, 2) * p2;
    }
    private void OnDrawGizmos()
    {
        if (currentControlPos == null) return;
        for (int i = 0; i < currentControlPos.Length; i++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(currentControlPos[i].absolutePosition, 0.2f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(currentControlPos[i].currentPosition, 0.2f);
        }
    }

}
