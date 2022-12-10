using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMGController : WeaponController
{
    public override void fireShots()
    {
        GameObject bullet = Instantiate(this.bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation); 
        bullet.transform.Rotate(new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), 0));
    }
}
