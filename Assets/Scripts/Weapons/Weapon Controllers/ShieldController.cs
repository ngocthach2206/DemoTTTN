using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedShield = Instantiate(weaponData.Prefab);
        spawnedShield.transform.position = transform.position; //assign the position to be the same as this object which is parented to the player
        spawnedShield.transform.parent = transform; //spawn below this object
    }
}
