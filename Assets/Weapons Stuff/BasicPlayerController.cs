using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPlayerController : MonoBehaviour
{
    public GameObject weapon;    

    private void Awake()
    {
       weapon.GetComponent<WeaponController>().startShooting();
    }
}
