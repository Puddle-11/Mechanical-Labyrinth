using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class ElectrifiedRope : Rope
{
    [SerializeField] private Gradient colorOverLifetime;
    [SerializeField] private float decayTime;
    private float timer;
    [SerializeField] private float hangDist;
    public void SetDecay(float _val)
    {
        decayTime = _val;
    }
    public override void Start()
    {
        timer = decayTime;
        base.Start();
    }
    // Update is called once per frame
    public override void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            SetColor(colorOverLifetime.Evaluate((timer / decayTime) * -1 + 1));
        }
        else
        {
            timer = decayTime;
            for (int i = 0; i < anchors.Length; i++)
            {
                Destroy(anchors[i].gameObject);
            }
            Destroy(gameObject);
        }
        float avgDist  =0;
        for (int i = 0; i < currentControlPos.Length; i++)
        {
            if (anchors[i] != null && anchors[i + 1] != null)
            {
                avgDist += Vector3.Distance(anchors[i].position, anchors[i + 1].position);
            }
        }
        SetRopeLength(avgDist / anchors.Length  + hangDist);
        base.Update();
    }
    //REMEMBER you are working on setting the colliders to the correct positions and setting the collider bounds to the correcdt positions
    //also the colliders rotation
    //theres a method in ROPE called Update colliders which you must override here
   


}
