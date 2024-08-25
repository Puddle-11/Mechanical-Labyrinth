using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class Rope : MonoBehaviour
{
    public Transform[] anchors;
    public float ropeLength;
    private ControlPoint[] currentControlPos;
    [SerializeField] private LineRenderer lnRef;
    [SerializeField] private int lineResolution;
    [SerializeField] private float springConstant;
    [SerializeField] private float dampeningForce;
    [SerializeField] private bool dynamic = true;
    [SerializeField] private float acceptableMargin;
    private Vector3[] Positions;
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
        for (int i = 0; i < currentControlPos.Length; i++)
        {
            currentControlPos[i].absolutePosition = ((anchors[i + 1].position + anchors[i].position) / 2) + Vector3.down * Mathf.Clamp(ropeLength - Vector3.Distance(anchors[i].position, anchors[i + 1].position), 0f, Mathf.Infinity);
        }
    }
    private Vector3 CalculateCurve(float t, Vector3 p1, Vector3 p2, Vector3 control)
    {
        float filteredT = 1 - t;
        float t2 = t * t;
        float ft2 = filteredT * filteredT;

        Vector3 res = ft2 * p1 + 2 * filteredT * t * control + t2 * p2;
        return res;
    }
  
}
