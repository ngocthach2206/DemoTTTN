using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedSpear = Instantiate(weaponData.Prefab);
        spawnedSpear.transform.position = transform.position; //assign the position to be the same as this object which is parented to the player
        spawnedSpear.GetComponent<SpearBehaviour>().DirectionChecker(pm.lastMovedVector); //reference and set the direction
    }
}
