using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
[RequireComponent(typeof(DecalProjector))]
public class BulletHole : MonoBehaviour
{
    [SerializeField] private float decayTime;
    private DecalProjector projector;
    [SerializeField] private AnimationCurve opacityOverTime;
    private void Start()
    {
        projector = GetComponent<DecalProjector>();
        StartCoroutine(Decay());
    }
    public IEnumerator Decay()
    {
        float timer = 0;
        while (timer < decayTime)
        {
          
            projector.fadeFactor = opacityOverTime.Evaluate(timer/decayTime);
            yield return null;
            timer+= Time.deltaTime; 
        }
    }
}
