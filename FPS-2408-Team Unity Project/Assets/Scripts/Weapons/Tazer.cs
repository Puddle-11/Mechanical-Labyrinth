using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class Tazer : Weapon
{
    private GameObject currTazer;
    [SerializeField] private GameObject electricRopePrefab;
    [SerializeField] private GameObject endAnchor;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private Transform shootPos;
    public bool debugShoot;
    private void Update()
    {
        if (debugShoot)
        {
            Attack();
            debugShoot = false;
        }
    }
    public override IEnumerator AttackDelay()
    {
        isAttacking = true;

       currTazer = Instantiate(electricRopePrefab, shootPos.position, Quaternion.identity, transform);
        ElectrifiedRope electricRopeRef;
        GameObject endAnchorInstance = Instantiate(endAnchor, shootPos.position, Quaternion.identity);
        GameObject startAnchor = new GameObject("Start Anchor");
        startAnchor.transform.position = shootPos.position;
        startAnchor.transform.parent = shootPos;
        if(currTazer.TryGetComponent<ElectrifiedRope>(out electricRopeRef))
        {
            electricRopeRef.SetDecay(coolDown);
            electricRopeRef.SetAnchors(new Transform[] {startAnchor.transform, endAnchorInstance.transform});
        }
        Rigidbody tempRef;
        if(endAnchorInstance.TryGetComponent<Rigidbody>(out tempRef)){
            tempRef.velocity = shootPos.forward * projectileSpeed;
        }
        yield return new WaitForSeconds(coolDown);
        isAttacking = false;
    }
    public void OnDisable()
    {
        Destroy(currTazer);
    }


}
