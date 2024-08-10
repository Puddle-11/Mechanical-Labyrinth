using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    //[SerializeField] private AnimationCurve lengthOverLife;
    //[SerializeField] private LineRenderer trRef;
    //[SerializeField] private float lengthFactor;
    //private float timer;
    [SerializeField] private float hangTime;
    [SerializeField] private float speed;
    private Vector3 target;


    public void SetPositions(Vector3 _start, Vector3 _end)
    {

        transform.position = _start;
        target = _end;
            Destroy(gameObject, hangTime);
    }

    public void Update()
    {

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            Destroy(gameObject);

        }
        //if (trRef != null)
        //{
        //    if (timer >= hangTime || Vector3.Distance(bulletPos, endPos) < 0.01f)
        //    {
        //        Destroy(gameObject);
        //    }
        //    else
        //    {
        //        Vector3 dir = (startPos - endPos).normalized;
        //        float currLength = lengthOverLife.Evaluate(timer/hangTime);
        //        Vector3[] segmentPos = new Vector3[trRef.positionCount];
        //        segmentPos[0] = bulletPos;
        //        for (int i = 1; i < segmentPos.Length; i++)
        //        {
        //            segmentPos[i] = segmentPos[0] + dir * ((currLength *lengthFactor )/ (float)segmentPos.Length) * i;
        //        }
        //        trRef.SetPositions(segmentPos);
        //    }
        //    timer += Time.deltaTime;
        //}
    }
}
