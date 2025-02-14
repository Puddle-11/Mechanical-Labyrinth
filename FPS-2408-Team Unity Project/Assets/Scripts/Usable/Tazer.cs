using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tazer : Weapon
{
    [SerializeField] private GameObject electricRopePrefab;
    [SerializeField] private float lineDecay;
    [SerializeField] private GameObject endAnchor;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private Transform shootPos;

    public override IEnumerator AttackDelay()
    {
        isAttacking = true;

        ElectrifiedRope electricRopeRef;
        GameObject currTazer = Instantiate(electricRopePrefab, Vector3.zero, Quaternion.identity);
        GameObject endAnchorInstance = Instantiate(endAnchor, shootPos.position + shootPos.position * 0.01f, Quaternion.identity);
        GameObject startAnchorInstance = Instantiate(endAnchor, shootPos.position, Quaternion.identity);
        if (currTazer.TryGetComponent<ElectrifiedRope>(out electricRopeRef))
        {
            electricRopeRef.SetDecay(lineDecay);
            electricRopeRef.SetAnchors(new Transform[] { startAnchorInstance.transform, endAnchorInstance.transform });
        }
        Rigidbody tempRef;
        if (endAnchorInstance.TryGetComponent<Rigidbody>(out tempRef)) tempRef.linearVelocity = shootPos.forward * projectileSpeed;

        yield return new WaitForSeconds(coolDown);
        isAttacking = false;
    }



}
