using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GarrethFlicker : MonoBehaviour
{
    [SerializeField] private GameObject rootObj;
    private Renderer[] allRend;

    [SerializeField] private float initialFlickerSpeed;
    [SerializeField] private float finalFlickerSpeed;
    [SerializeField] private float decayTime;
    [SerializeField] private AnimationCurve flickerSpeedRampUp;
    [SerializeField] private bool debugFlicker;
    private void Start()
    {

        allRend = GetAllRenderers();
    }
    public void Update()
    {
        if (debugFlicker)
        {
            StartCoroutine(RunFlicker());

            debugFlicker = false;
        }

    }
    public Renderer[] GetAllRenderers()
    {
        Debug.Log("got renderers");
        if (rootObj == null)
        {
            Debug.LogWarning("No Root Object found");
            return null;
        }
            return rootObj.GetComponentsInChildren<Renderer>();
    }
    public void SetRendererState(bool _val)
    {
        if (allRend == null || allRend.Length <= 0)
        {
            Debug.LogWarning("Renderers not assigned properly");
            return;
        }
            for (int i = 0; i < allRend.Length; i++)
        {
            allRend[i].enabled = _val;
        }
    }
    public IEnumerator RunFlicker()
    {
        float timer = 0;

        while (timer < decayTime)
        {

            float flickerSpeed = Mathf.Lerp(initialFlickerSpeed, finalFlickerSpeed, flickerSpeedRampUp.Evaluate(timer / decayTime));

            WaitForSeconds s = new WaitForSeconds(flickerSpeed / 2);
            yield return s;
            SetRendererState(false);
            yield return s;
            SetRendererState(true);
            timer = timer +  flickerSpeed;
            Debug.Log(timer);
            yield return null;
        }
        SetRendererState(false);

    }
}
