using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class PoolTurret : MonoBehaviour
{
    private static PoolTurret Instance { get; set; }
    public static PoolTurret instance => Instance;

    private List<TurretBehaviour> sniperBank = new List<TurretBehaviour>();
    private List<TurretBehaviour> machineGunBank = new List<TurretBehaviour>();
    [SerializeField]
    private TurretBehaviour sniperPrefab;
    [SerializeField]
    private TurretBehaviour machineGunPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public TurretBehaviour GetTurret(Vector3 pos, TurretType type)
    {
        List<TurretBehaviour> turretsBank;
        TurretBehaviour turretPrefab;
        switch (type)
        {
            case TurretType.sniper:
                turretsBank = sniperBank;
                turretPrefab = sniperPrefab;
                break;
            case TurretType.mahineGun:
                turretsBank = machineGunBank;
                turretPrefab = machineGunPrefab;
                break;
            default:
                turretsBank = machineGunBank;
                turretPrefab = machineGunPrefab;
                break;
        }

        if (turretsBank.Count > 0)
        {
            TurretBehaviour turret = turretsBank[0];
            turret.transform.position = pos;
            turret.gameObject.SetActive(true);
            turret.Init();
            TurretBehaviour behaviour = turret.GetComponent<TurretBehaviour>();
            turretsBank.Remove(turret);
            return behaviour;
        }
        else
        {
            var turret = Instantiate(turretPrefab);
            TurretBehaviour behaviour = turret.GetComponent<TurretBehaviour>();
            behaviour.Init();
            behaviour.SetPool(this);
            return behaviour;
        }
    }

    public void AddTurretToPool(TurretBehaviour turret)
    {
        List<TurretBehaviour> turretsBank;
        switch (turret.GetType())
        {
            case TurretType.sniper:
                turretsBank = sniperBank;
                break;
            case TurretType.mahineGun:
                turretsBank = machineGunBank;
                break;
            default:
                turretsBank = machineGunBank;
                break;
        }
        GeneratorBehaviour.instance.RemoveDrain(turret.GetDrain());
        turretsBank.Add(turret);
        turret.gameObject.SetActive(false);
    }
}
