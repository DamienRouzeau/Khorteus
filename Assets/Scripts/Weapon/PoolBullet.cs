using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBullet : MonoBehaviour
{
    //private static PoolBullet Instance { get; set; }
    //public static PoolBullet instance => Instance;

    private List<BulletBehaviour> bulletsBank = new List<BulletBehaviour>();
    [SerializeField]
    private BulletBehaviour bulletPrefab;
    [SerializeField]
    private Transform bulletLauncher;

    //private void Awake()
    //{
    //    if (Instance != null && Instance != this) Destroy(this);
    //    else Instance = this;
    //}

    public BulletBehaviour GetBullet()
    {
        if(bulletsBank.Count > 0)
        {
            BulletBehaviour bullet = bulletsBank[0];
            bullet.transform.parent = transform;
            bullet.transform.localPosition = Vector3.zero;
            bullet.transform.localRotation = Quaternion.Euler(0,0,0);
            bullet.gameObject.SetActive(true);
            bullet.gameObject.transform.parent = null;
            BulletBehaviour behaviour = bullet.GetComponent<BulletBehaviour>();
            bulletsBank.Remove(bullet);
            return behaviour;
        }
        else
        {
            var bullet = Instantiate(bulletPrefab, bulletLauncher);
            bullet.gameObject.transform.parent = null;
            BulletBehaviour behaviour = bullet.GetComponent<BulletBehaviour>();
            behaviour.SetPool(this);
            return behaviour;
        }
    }

    public void AddBulletToPool(BulletBehaviour bullet)
    {
        bulletsBank.Add(bullet);
        bullet.transform.parent = bulletLauncher;
        bullet.gameObject.SetActive(false);
    }
}
