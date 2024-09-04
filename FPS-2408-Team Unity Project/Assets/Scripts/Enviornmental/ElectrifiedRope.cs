using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
//====================================
//REWORKED
//====================================
public class ElectrifiedRope : Rope
{
    [SerializeField] private Gradient colorOverLifetime;
    [SerializeField] private float decayTime;
    [SerializeField] private float hangDist;
    [SerializeField] private bool destroyAnchors = false;


    private float timer;

    #region MonoBehavior Methods
    public override void Start()
    {
        timer = decayTime;
        base.Start();
    }
    public override void Update()
    {
        //  \/ Update Timer \/
        timer = Mathf.Clamp(timer - Time.deltaTime, 0, decayTime);

        if(timer == 0)
        {
            if (destroyAnchors)
            {
                for (int i = 0; i < anchors.Length; i++)
                {
                    Destroy(anchors[i].gameObject);
                }
            }
            Destroy(gameObject);
        }


        SetColor(colorOverLifetime.Evaluate((timer / decayTime) * -1 + 1));
        SetRopeLength(hangDist);
        base.Update();
    }
    #endregion

    #region Getters and Setters
    public void SetDecay(float _val) { decayTime = _val; }



    public override void SetRopeLength(float _length)
    {
        float avgDist = 0;
        for (int i = 0; i < currentControlPos.Length; i++)
        {
            if (anchors[i] != null && anchors[i + 1] != null)
            {
                avgDist += Vector3.Distance(anchors[i].position, anchors[i + 1].position);
            }
        }
        base.SetRopeLength(avgDist / anchors.Length  + _length);
    }
    #endregion
}
