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
    private Vector3 dir;
    private bool useEnd = false;

    public void SetPositions(Vector3 _start, Vector3 _end)
    {
       
        transform.position = _start;
        dir = _end;
        Destroy(gameObject, hangTime);
    }
    public void SetDirection(Vector3 _start, Vector3 _direction)
    {
        useEnd = true;
        SetPositions(_start, _direction);
    }
    public void Update()
    {
        if (useEnd)
        {
            transform.position += dir * Time.deltaTime * speed;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, dir, speed * Time.deltaTime);

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
