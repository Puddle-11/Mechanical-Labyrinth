using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using System;
public class Rope : MonoBehaviour
{
    [Header("GENERAL")]
    [Space]
    [SerializeField] protected Transform[] anchors;
    [SerializeField] protected float ropeLength;

    [SerializeField] protected int lineResolution = 10;
    [SerializeField] protected int collisionSpacing = 1;

    [Space]
    [Header("PHYSICS")]
    [Space]

    [SerializeField] protected float springConstant = 0.25f;
    [SerializeField] protected float dampeningForce = 5f;
    [SerializeField] protected float acceptableMargin = 0.001f;
    [Space]
    [SerializeField] protected PhysType physicsType;
    [SerializeField] private float colliderSize;


    private LineRenderer lnRef;
    protected LinePositions[] Positions;
    public ControlPoint[] currentControlPos;


    #region Custom Structs and Enums
    public enum PhysType
    {
        Static,
        Dynamic,
        Interactable,
    }

    [System.Serializable]
    public struct LinePositions
    {
        public Vector3 position;
        public SphereCollider coll;
        public void Clear()
        {
            position = Vector3.zero;
            Destroy(coll);
        }
        public void SetPoint(Vector3 _pos, float _size, GameObject _obj, PhysType _type = PhysType.Dynamic)
        {
            position = _pos;
            if (_type == PhysType.Interactable)
            {
                if (coll == null)
                {
                    SphereCollider sphereCollider = _obj.AddComponent<SphereCollider>();
                    coll = sphereCollider;
                }
                coll.radius = _size;
                coll.center = _pos - _obj.transform.position;
            }
            else
            {
                if (coll != null) Destroy(coll);
            }
        }

    }
    [System.Serializable]
    public struct ControlPoint
    {
        public Vector3 absolutePosition;
        public Vector3 currentPosition;
        public Vector3 currenInertia;
    }
    #endregion

    #region MonoBehavior Methods
    public virtual void Awake()
    {
        if(!TryGetComponent<LineRenderer>(out lnRef))
        {
            Debug.LogWarning("No line renderer found on " + gameObject.name);
        }
    }
    public virtual void Start()
    {
        InitializeRope();
    }
    public virtual void Update()
    {
        if (physicsType != PhysType.Static)
        {
            UpdateRope();
        }
    }
    public virtual void UpdateRope()
    {
        if (anchors == null || anchors.Length <= 0)
        {
            Debug.LogWarning("No Anchors assigned to " + gameObject.name);
            return;
        }
        UpdateRopePhysics();
        DrawRope();

    }
    #endregion

    #region Getters and Setters
    public virtual Vector3[] GetPositions()
    {
        Vector3[] posTemp = new Vector3[Positions.Length];
        for (int i = 0; i < posTemp.Length; i++)
        {
            posTemp[i] = Positions[i].position;
        }
        return posTemp;
    }
    public virtual void SetPositions(Vector3[] _positions)
    {
        for (int i = 0; i < Positions.Length; i++)
        {
            if (i >= _positions.Length) break;
            Positions[i].position = _positions[i];
        }
    }
    public virtual void ResetPositions(int _length)
    {
        Positions = new LinePositions[_length];
        for (int i = 0; i < _length; i++)
        {
            Positions[i] = new LinePositions();
        }
    }



    public virtual void SetIsTrigger(bool _val)
    {
        for (int i = 0; i < Positions.Length; i++)
        {
            if (Positions[i].coll != null) Positions[i].coll.isTrigger = _val;
        }
    }
    public virtual void SetColliderSize(float _size)
    {
        for (int i = 0; i < Positions.Length; i++)
        {
            Positions[i].coll.radius = _size;
        }
    }


    public virtual void SetAnchors(Transform[] _transforms)
    {
        anchors = _transforms;
    }
    public virtual void SetAnchors(Vector3[] _pos)
    {
        Transform[] transforms = new Transform[_pos.Length];

        for (int i = 0; i < transforms.Length; i++)
        {
            GameObject temp = new GameObject("Anchor" + gameObject.name + " Pos " + i);
            temp.transform.position = _pos[i];
            temp.transform.parent = transform;
            transforms[i] = temp.transform;
        }
        SetAnchors(transforms);
    }


    public virtual void SetAbsolutePosition(int _index)
    {
        if (anchors[_index] != null && anchors[_index + 1] != null)
        {
            float anchorDist = Vector3.Distance(anchors[_index].position, anchors[_index + 1].position);
            float hangDist = Mathf.Clamp(ropeLength - anchorDist, 0f, Mathf.Infinity);
            Vector3 avgPos = (anchors[_index + 1].position + anchors[_index].position) / 2;

            currentControlPos[_index].absolutePosition = avgPos + Vector3.down * hangDist;
        }
    }
    public virtual void SetRopeLength(float _length) { ropeLength = _length;}

    public virtual void SetColor(Color _col)
    {
        Gradient tempGrad = new Gradient();
        tempGrad.SetKeys(new GradientColorKey[] {new GradientColorKey(_col, 0f)}, new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f) });
        if(lnRef != null) lnRef.colorGradient = tempGrad;
    }

    #endregion

    public void InitializeRope()
    {
        if (anchors == null || anchors.Length <= 0)
        {
            Debug.LogWarning("No Anchors assigned to " + gameObject.name);
            return;
        }
        currentControlPos = new ControlPoint[anchors.Length - 1];

        for (int i = 0; i < currentControlPos.Length; i++)
        {
            SetAbsolutePosition(i);
            currentControlPos[i].currentPosition = currentControlPos[i].absolutePosition;
            currentControlPos[i].currenInertia = Vector3.zero;
        }
        ResetPositions(lineResolution * currentControlPos.Length);
        UpdateRopePhysics();
        DrawRope(true);
        SetIsTrigger(true);
    }

    private void UpdateRopePhysics()
    {
        
        for (int i = 0; i < currentControlPos.Length; i++)
        {
            SetAbsolutePosition(i);
            float currForce = Vector3.Distance(currentControlPos[i].currentPosition, currentControlPos[i].absolutePosition) * springConstant;
            Vector3 direction = (currentControlPos[i].absolutePosition - currentControlPos[i].currentPosition).normalized;
            currentControlPos[i].currenInertia += direction * currForce * Time.unscaledDeltaTime;
            currentControlPos[i].currenInertia /= 1 + dampeningForce * Time.unscaledDeltaTime;
            currentControlPos[i].currentPosition += currentControlPos[i].currenInertia;
        }
    }
    public void DrawRope(bool _override = false)
    {
        for (int i = 0; i < currentControlPos.Length; i++)
        {
            if (currentControlPos[i].currenInertia.magnitude <= acceptableMargin && !_override) continue;

            for (int j = 0; j < lineResolution; j++)
            {
                if (anchors[i] != null && anchors[i + 1] != null)
                {
                    Vector3 curvePos = CalculateCurve(j / (float)(lineResolution - 1), anchors[i].position, anchors[i + 1].position, currentControlPos[i].currentPosition);
                    int index = (i * lineResolution) + j;

                    PhysType collType = 
                        index % collisionSpacing == 0 ? physicsType : 
                        physicsType != PhysType.Static ? PhysType.Dynamic : PhysType.Static;
          
                    Positions[index].SetPoint(curvePos, colliderSize, gameObject, collType);        
                }
            }
        }
        if (lnRef != null) lnRef.positionCount = Positions.Length;
        if (lnRef != null) lnRef.SetPositions(GetPositions());
    }

    protected Vector3 CalculateCurve(float t, Vector3 p1, Vector3 p2, Vector3 control)
    {
        return Mathf.Pow((1-t),2) * p1 + 2 * (1 - t) * t * control + Mathf.Pow(t, 2) * p2;
    }
    private void OnDrawGizmosSelected()
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
