using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimation : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Transform playerTransform;
    private Vector3 initialPosition, idleHighPosition, idleLowPosition, runningHighPosition, runningLowPosition;
    [SerializeField] private float idleSpeed = 1.0f;
    [SerializeField] private float runningSpeed = 2.0f;
    private float value = 0;
    private bool wasRunning = false;
    private bool wasWalking = false;
    void Start()
    {
        initialPosition = transform.localPosition;
 

        idleHighPosition = transform.position +  new Vector3(0, 0.05f, 0);
        idleLowPosition = transform.position + new Vector3(0, -0.05f, 0);
        runningHighPosition = transform.position + new Vector3(0, 0.1f, 0);
        runningLowPosition = transform.position + new Vector3(0, -0.1f, 0);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
         {
            if(wasWalking)
            {
                transform.localPosition = initialPosition;
                wasWalking = false;
            }
            value = Mathf.Lerp(-0.001f, 0.001f, Mathf.PingPong(Time.time * runningSpeed, 1.0f));

            wasRunning = true;
             transform.position = new Vector3(transform.position.x, transform.position.y + value, transform.position.z);

         }
         else
         {
            if(wasRunning)
            {
                transform.localPosition = initialPosition;
                wasRunning = false;

            }
            wasWalking = true ;
            value = Mathf.Lerp(-0.0001f, 0.0001f, Mathf.PingPong(Time.time * runningSpeed, 1.0f));

            transform.position = new Vector3(transform.position.x, transform.position.y + value, transform.position.z);

         }

    }
}
