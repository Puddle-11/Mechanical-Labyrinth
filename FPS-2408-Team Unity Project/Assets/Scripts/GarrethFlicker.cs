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
            timer = timer + flickerSpeed;
            Debug.Log(timer);
            yield return null;
        }
        SetRendererState(false);

    }

    //public void NotifyNearbyGuards(/*AISharedContext context,*/ LayerMask _hitMask, Vector3 _hemisphereAxis /*this is the normal of the plane splitting the hemisphere*/)
    //{
    //    Collider[] results = new Collider[10]; //max collision count of 10
    //    Vector3 planePoint = /*context.Npc.*/transform.position;
    //    var size = Physics.OverlapSphereNonAlloc(/*context.Npc.*/transform.position, /*guardNotificationRange*/10, results, _hitMask);

    //    for (int i = 0; i < size; i++)
    //    {
    //        if (Vector3.Dot((planePoint - results[i].transform.position).normalized, _hemisphereAxis) > 0)
    //        {
    //            //in upper half of sphere
    //        }
    //        else
    //        {
    //            //in lower half of sphere
    //        }


    //        if (results[i].TryGetComponent(out NpcController controllerRef))
    //        {
    //            AISharedContext guardContext = controllerRef.context;

    //            if (guardContext != context)
    //            {
    //                guardContext.Npc.GetComponent<StateMachine>().ChangeState(new AttackState(guardContext));
    //            }
    //        }
    //    }

    //}



}

