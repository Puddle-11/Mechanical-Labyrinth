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


    [SerializeField] protected LineRenderer lnRef;
    [SerializeField] protected int lineResolution = 10;
    [Space]
    [Header("PHYSICS")]
    [Space]
    [SerializeField] protected float springConstant = 0.25f;
    [SerializeField] protected float dampeningForce = 5f;
    [SerializeField] protected float acceptableMargin = 0.001f;

    [SerializeField] protected bool dynamic = true;
    [SerializeField] protected bool interactable = false;

    [SerializeField] protected float softCollForce = 0.02f;
    [SerializeField] private float colliderSize;
    protected LinePositions[] Positions;
    public ControlPoint[] currentControlPos;
    [System.Serializable]
    public struct LinePositions
    {
        public Vector3 position;
        public SphereCollider coll;
        public int ropeIndex;
        public void Clear()
        {
            position = Vector3.zero;
            Destroy(coll);
        }
        public void SetPoint(Vector3 _pos, float _size, int _index, GameObject _obj)
        {
            position = _pos;
            ropeIndex = _index;
            if (coll == null)
            {
                SphereCollider sphereCollider = _obj.AddComponent<SphereCollider>();
                coll = sphereCollider;
            }
            coll.radius = _size;
            coll.center = _pos - _obj.transform.position;
        }
    }
    [System.Serializable]
    public struct ControlPoint
    {
        public Vector3 absolutePosition;
        public Vector3 currentPosition;
        public Vector3 currenInertia;
        public GameObject inputCollider;
    }
    public Vector3[] GetPositions()
    {
        Vector3[] posTemp = new Vector3[Positions.Length];
        for (int i = 0; i < posTemp.Length; i++)
        {
            posTemp[i] = Positions[i].position;
        }
        return posTemp;
    }
    public void SetPostiions(Vector3[] _positions)
    {
        for (int i = 0; i < Positions.Length; i++)
        {
            if (i >= _positions.Length) break;
            Positions[i].position = _positions[i];
        }
    }
    public void SetPositionLength(int _length)
    {
        if (Positions != null)
        {
            for (int i = 0; i < Positions.Length; i++)
            {
                Positions[i].Clear();
            }
        }
        Positions = new LinePositions[_length];
        for (int i = 0; i < _length; i++)
        {
            Positions[i] = new LinePositions();
        }
    }
    public void SetIsTrigger(bool _val)
    {
        for (int i = 0; i < Positions.Length; i++)
        {
            Positions[i].coll.isTrigger = _val;
        }
    }
    public void SetColliderSize(float _size)
    {
        for (int i = 0; i < Positions.Length; i++)
        {
            Positions[i].coll.radius = _size;
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

    public virtual void Start()
    {
        SetRope();
        SetIsTrigger(true);
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
        currentControlPos = new ControlPoint[anchors.Length - 1];


        UpdateAbsoluteControlPoint();
        for (int i = 0; i < currentControlPos.Length; i++)
        {
            
               
            currentControlPos[i].currentPosition = currentControlPos[i].absolutePosition;
            currentControlPos[i].currenInertia = Vector3.zero;
        }
        SetPositionLength(lineResolution * currentControlPos.Length);
        UpdateRopePhysics();
        DrawRope(true);

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
        }
    }

    private void UpdateRopePhysics()
    {
        UpdateAbsoluteControlPoint();

        for (int i = 0; i < currentControlPos.Length; i++)
        {
            float currForce = Vector3.Distance(currentControlPos[i].currentPosition, currentControlPos[i].absolutePosition) * springConstant;
            Vector3 direction = (currentControlPos[i].absolutePosition - currentControlPos[i].currentPosition).normalized;
            currentControlPos[i].currenInertia += direction * currForce * Time.unscaledDeltaTime;
            currentControlPos[i].currenInertia /= 1 + dampeningForce * Time.unscaledDeltaTime;
            currentControlPos[i].currentPosition += currentControlPos[i].currenInertia;
        }
    }
    public void DrawRope(bool _override)
    {
        for (int i = 0; i < currentControlPos.Length; i++)
        {
            if (currentControlPos[i].currenInertia.magnitude <= acceptableMargin && !_override) continue;

            for (int j = 0; j < lineResolution; j++)
            {

                if (anchors[i] != null&& anchors[i+i] != null) Positions[(i * lineResolution) + j].SetPoint(CalculateCurve(j / (float)(lineResolution - 1), anchors[i].position, anchors[i + 1].position, currentControlPos[i].currentPosition), colliderSize, i, gameObject);
            }
        }
        lnRef.positionCount = Positions.Length;
        lnRef.SetPositions(GetPositions());
    }

    private void UpdateAbsoluteControlPoint()
    {
        for (int i = 0; i < currentControlPos.Length; i++)
        {
            if (anchors[i] != null && anchors[i+1] != null) currentControlPos[i].absolutePosition = ((anchors[i + 1].position + anchors[i].position) / 2) + Vector3.down * Mathf.Clamp(ropeLength - Vector3.Distance(anchors[i].position, anchors[i + 1].position), 0f, Mathf.Infinity);
        }
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
