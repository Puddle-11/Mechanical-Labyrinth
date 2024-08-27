using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ElectrifiedRope : Rope
{
    [SerializeField] private Gradient colorOverLifetime;
    [SerializeField] private float decayTime;
    private float timer;
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
        UpdateColliders();
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            SetColor(colorOverLifetime.Evaluate((timer / decayTime) * -1 + 1));
        }
        else
        {
            for (int i = 0; i < anchors.Length; i++)
            {
                Destroy(anchors[i].gameObject);
            }
            Destroy(gameObject);
        }
        float avgDist  =0;
        for (int i = 0; i < currentControlPos.Length; i++)
        {

            avgDist += Vector3.Distance(anchors[i].position, anchors[i + 1].position);
        }
        SetRopeLength(avgDist / anchors.Length);
        base.Update();
    }
    //REMEMBER you are working on setting the colliders to the correct positions and setting the collider bounds to the correcdt positions
    //also the colliders rotation
    //theres a method in ROPE called Update colliders which you must override here
    public override void UpdateColliders()
    {
        if (collisionDetectorPrefab != null)
        {
            for (int i = 0; i < currentControlPos.Length; i++)
            {
                Vector3 instancePos = CalculateCurve(0.5f, anchors[i].position, anchors[i + 1].position, currentControlPos[i].absolutePosition);
                instancePos.y = (anchors[i].position.y + anchors[i + 1].position.y)/2;
                Quaternion lookRot = Quaternion.LookRotation(new Vector3(anchors[i].position.x, instancePos.y, anchors[i].position.z) - instancePos);
                currentControlPos[i].inputCollider.transform.rotation = lookRot;
                currentControlPos[i].inputCollider.transform.position = instancePos;
                BoxCollider boxRef;
                if (currentControlPos[i].inputCollider.TryGetComponent<BoxCollider>(out boxRef))
                {
                    float distz = Vector3.Distance(anchors[i].position, new Vector3(anchors[i + 1].position.x, anchors[i].position.y, anchors[i + 1].position.z));
                    float disty = Mathf.Abs(anchors[i].position.y - anchors[i + 1].position.y);
                    boxRef.size = new Vector3(boxRef.size.x, disty, distz);
                    
                }
            }
        }
      
    }


}
