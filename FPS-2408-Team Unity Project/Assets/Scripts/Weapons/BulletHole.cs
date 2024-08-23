using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
[RequireComponent(typeof(DecalProjector))]
public class BulletHole : MonoBehaviour
{
    [SerializeField] private float decayTime;
    [SerializeField] private DecalProjector projector;
    [SerializeField] private AnimationCurve opacityOverTime;
    [SerializeField] private ParticleSystem impactEffect;
    private void Start()
    {
        impactEffect.Play();
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
    public void SetMat(Material _mat)
    {
        projector.material = _mat;
    }
}
