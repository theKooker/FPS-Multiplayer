using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigunController : WeaponController
{
    public override void fireShots()
    {
        Instantiate(this.bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        if(this.fireRate < 10)
        {
            this.fireRate += .5f;
        }
    }
}
