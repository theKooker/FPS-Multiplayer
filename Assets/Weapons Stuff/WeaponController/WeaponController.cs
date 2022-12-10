using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class WeaponController : MonoBehaviour
{
    public GameObject bullet;
    public GameObject bulletSpawn;

    public float fireRate;

    private float timeBetweenShots = 0;

    private bool shouldShoot = false;

    public void startShooting()
    {
        this.shouldShoot = true;
    }
    public void stopShooting()
    {
        this.shouldShoot = false;
    }

    public abstract void fireShots();

    private void FixedUpdate()
    {
        if(this.shouldShoot && Time.time > timeBetweenShots)
        {
            timeBetweenShots = Time.time + (1/fireRate);
            fireShots();     
        }
    }
}



/* Shootinputstream wird geraised ruft shoot methode auf welche dann

firstPersonControllerInput.Shoot.Subscribe(input->
        {
    if (Input)
    {
        this.weapon.startShooting();
    }
    else
    {
        this.weapon.stopShooting();
    }


})*/