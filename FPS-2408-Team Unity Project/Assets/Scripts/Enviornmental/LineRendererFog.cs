using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//====================================
//REWORKED
//====================================
public class LineRendererFog : MonoBehaviour
{
    [SerializeField] private Gradient colGradient;
    [SerializeField] private float maxDistance;

    private LineRenderer lineRendRef;
    private float currDistance = 1;

    #region MonoBehavior Methods
    private void Awake()
    {
        if(!TryGetComponent<LineRenderer>(out lineRendRef))
        {
            Debug.Log("Failed to find line renderer on " + gameObject.name);
        }
    }
    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.playerRef != null)
        {
            currDistance = Vector3.Distance(GameManager.instance.playerRef.transform.position, transform.position);
        }
        SetLineColor(colGradient.Evaluate(currDistance / maxDistance));
    }
    #endregion

    #region Getters and Setters
    public void SetLineColor(Color _col)
    {
        if (lineRendRef == null) return;
        Gradient tempGrad = new Gradient();
        tempGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(colGradient.Evaluate(currDistance / maxDistance), 0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f) });
        lineRendRef.colorGradient = tempGrad;
    }
    #endregion
}
