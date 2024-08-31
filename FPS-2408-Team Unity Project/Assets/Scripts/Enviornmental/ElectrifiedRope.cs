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

    public override void Start()
    {
        timer = decayTime;
        base.Start();
    }
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
        float avgDist = 0;
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
    public void SetDecay(float _val) { decayTime = _val; }
}
