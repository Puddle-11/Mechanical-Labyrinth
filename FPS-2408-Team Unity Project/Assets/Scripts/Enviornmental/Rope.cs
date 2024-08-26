using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Rope : MonoBehaviour
{
    [Header("GENERAL")]
    [Space]
    [SerializeField] private Transform[] anchors;
    [SerializeField] private float ropeLength;


    [SerializeField] private LineRenderer lnRef;
    [SerializeField] private int lineResolution;
    [Space]
    [Header("PHYSICS")]
    [Space]
    [SerializeField] private float springConstant;
    [SerializeField] private float dampeningForce;
    [SerializeField] private float acceptableMargin;

    [SerializeField] private bool dynamic = true;

    [SerializeField] private float swaySpeed;
    [SerializeField] private Vector3 swayDir;


    private Vector3[] Positions;
    private ControlPoint[] currentControlPos;
    private float swayTimer;
    [System.Serializable]
    public struct ControlPoint
    {
        public Vector3 absolutePosition;
        public Vector3 currentPosition;
        public Vector3 currenInertia;
    }

    private void Start()
    {
        SetRope();
    }
    public void SetRope()
    {
        swayTimer = Random.Range(0f, Mathf.PI);
        currentControlPos = new ControlPoint[anchors.Length - 1];
        UpdateCurveAnchorPoints();
        for (int i = 0; i < currentControlPos.Length; i++)
        {
            currentControlPos[i].currentPosition = currentControlPos[i].absolutePosition;
            currentControlPos[i].currenInertia = Vector3.zero;
        }
        Positions = new Vector3[lineResolution * currentControlPos.Length];
        UpdateRopePhysics();
        DrawRope(true);
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
    void Update()
    {
        if (dynamic)
        {
            UpdateRopePhysics();
            DrawRope();
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
    public void DrawRope(bool _override = false)
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
    private Vector3 CalculateCurve(float t, Vector3 p1, Vector3 p2, Vector3 control)
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
