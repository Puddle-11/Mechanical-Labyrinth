using System.Collections;
using UnityEngine;

public class RepairDrone : BaseEnemy
{
    [Header("Repair Drone")]
    [Space]
    [SerializeField] private int healingAmount;
    [SerializeField] private int coolDown;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] private ParticleSystem healingParticles;
    bool healing = false;

    #region Monobehavior Methods

    public override void Update()
    {
        if (healing == true)
        {
            UpdateLineRenderer();
        }
        base.Update();
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject != GameManager.instance.playerRef)
        {
            if (other.gameObject.TryGetComponent(out BaseEnemy enemy))
            {
                if (enemy.GetCurrentHealth() < enemy.GetMaxHealth())
                {
                    StartHeal(other.gameObject);
                }
            }
        }
    }

    public void UpdateLineRenderer()
    {
        if(target == null)
        {
            lineRenderer.enabled = false;
        }
        else
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();

            float lengthOfLineRenderer = Vector3.Distance(target.transform.position, transform.position);
            Vector3[] points = new Vector3[(int)lengthOfLineRenderer];

            points[0] = transform.position;
            points[1] = target.transform.position;        

            lineRenderer.SetPositions(points);
            lineRenderer.enabled = true;

            healingParticles.gameObject.transform.position = target.transform.position;
        }
    }

    public void StartHeal(GameObject _target)
    {
        if (healing == true)
        {
            return;
        }
        SetTarget(_target);
        StartCoroutine(Healing(_target));
    }

    private IEnumerator Healing(GameObject __target)
    {
        healing = true;
        healingParticles.gameObject.SetActive(true);

        if (__target.gameObject.TryGetComponent<IHealth>(out IHealth hp))
        {
            while (hp.GetCurrentHealth() < hp.GetMaxHealth())
            {
                if (target == null)
                {
                    break;
                }
                hp.UpdateHealth(+healingAmount);

                float timer = 0;
                while (timer < coolDown)
                {
                    timer += Time.deltaTime;
                    yield return null;
                }
            }

        }

        healingParticles.gameObject.SetActive(false);
        healing = false;
    }

    protected override void EnemyStatus(ref EnemyState _enemyStateRef)
    {
        if (healing == false) {
            _enemyStateRef = EnemyState.Patrol;
        }
        else
        {
            _enemyStateRef = EnemyState.Persue;
        }
    }
    #endregion
}
