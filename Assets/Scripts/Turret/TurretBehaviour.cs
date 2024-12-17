using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{
    [Header("Cost")]
    [SerializeField]
    private float additionnalDrain;
    private bool isActive;
    private GeneratorBehaviour generator = GeneratorBehaviour.instance;
    
    [Header("Shot")]
    [SerializeField]
    private float couldownShot;
    private float timeSinceLastShot;
    [SerializeField]
    private PoolBullet[] poolBullets;
    private int shotIndex = 0;
    [SerializeField]
    private Animator[] turretAnim;

    [Header("In range")]
    private EnemyBehaviour target;
    private List<EnemyBehaviour> enemiesInRange = new List<EnemyBehaviour>();

    private void Start()
    {
        timeSinceLastShot = couldownShot;
        generator.SubOutOfPower(RanOutOfPower);

        if (generator.GetEnergy() > 0) isActive = true;
        generator.AddDrain(additionnalDrain);
    }
    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
        if (isActive)
        {
            if (target != null)
            {
                transform.LookAt(GetYAxisOnly(target.transform));
            }
            if (timeSinceLastShot >= couldownShot && enemiesInRange.Count > 0)
            {
                OnShot();
            }
        }
    }

    private Transform GetYAxisOnly(Transform _transform)
    {
        _transform.position = new Vector3(_transform.position.x, transform.position.y, _transform.position.z);
        return _transform;
    }

    private void OnShot()
    {
        target = GetClosestEnemy();
        if (timeSinceLastShot >= couldownShot)
        {
            BulletBehaviour bullet = poolBullets[shotIndex].GetBullet();
            shotIndex++;
            if (shotIndex > poolBullets.Length - 1) shotIndex = 0;
            bullet.Launch();
            turretAnim[shotIndex].SetTrigger("Shot");
            timeSinceLastShot = 0;
        }
    }

    #region manage target(s)
    public void RemoveDeadEnnemy()
    {
        target = GetClosestEnemy();
    }

    private EnemyBehaviour GetClosestEnemy()
    {
        enemiesInRange.RemoveAll(item => item == null);
        if (target != null) target.UnsubDie(RemoveDeadEnnemy);
        if (enemiesInRange == null || enemiesInRange.Count <= 0) return null;
        EnemyBehaviour _closest = enemiesInRange[0];
        foreach (EnemyBehaviour _enemy in enemiesInRange)
        {
            if (Vector3.Distance(_enemy.transform.position, transform.position) < Vector3.Distance(_closest.transform.position, transform.position))
            {
                _closest = _enemy;
            }
        }
        _closest.SubDie(RemoveDeadEnnemy);
        return _closest;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("monster"))
        {
            enemiesInRange.Add(other.gameObject.GetComponent<EnemyBehaviour>());
            target = GetClosestEnemy();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("monster"))
        {
            enemiesInRange.Remove(other.gameObject.GetComponent<EnemyBehaviour>());
            if (enemiesInRange.Count > 0) target = GetClosestEnemy();
        }
    }

    #endregion

    public void RanOutOfPower()
    {
        isActive = false;
    }
}
    
   
