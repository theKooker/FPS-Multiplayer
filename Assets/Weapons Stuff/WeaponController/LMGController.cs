using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LMGController : WeaponController
{
    public override void fireShots()
    {
        Instantiate(this.bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
    }
}
