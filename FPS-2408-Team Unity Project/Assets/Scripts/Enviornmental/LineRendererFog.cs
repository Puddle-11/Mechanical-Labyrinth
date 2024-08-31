using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererFog : MonoBehaviour
{
    [SerializeField] private Gradient colGradient;
    [SerializeField] private float maxDistance;
    private LineRenderer lineRendRef;
    private float currDistance = 1;
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
        if (lineRendRef != null)
        {
            GradientAlphaKey[] gradientAlphaKeys = new GradientAlphaKey[1];
            GradientColorKey[] gradientColorKeys = new GradientColorKey[1];
            gradientColorKeys[0] = new GradientColorKey(colGradient.Evaluate(currDistance/maxDistance),0f);
            gradientAlphaKeys[0] = new GradientAlphaKey(1f,0f);
            Gradient tempGrad = new Gradient();
            tempGrad.SetKeys(gradientColorKeys, gradientAlphaKeys);
            lineRendRef.colorGradient=tempGrad;
        }
    }
}
