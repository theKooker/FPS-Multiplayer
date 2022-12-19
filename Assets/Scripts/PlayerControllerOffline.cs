using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerOffline : MonoBehaviour
{
    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;

    [SerializeField] GameObject cameraHolder;
    [SerializeField] Animator animator;

    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    [SerializeField] Item[] items;

    int itemIndex;
    int previousItemIndex = -1;

    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;


    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    PlayerManager playerManager;



    [SerializeField, Min(0f)] private float crouchDistance;
    private Vector3 initialCameraPosition;
    private float initialHeight;
    private float initialCenter;

    // Animation Interface
    public event Action OnJump;
    public bool Grounded => _cc.isGrounded;
    public float MaxSpeed => sprintSpeed;
    public Vector3 Velocity => _velocity;
    private Vector3 _velocity;
    private Vector3 _oldPosition;

    [SerializeField] private bool test = false;

    private CharacterController _cc;
    private float _yVelocity;
    private const float GROUNDING_FORCE = -0.01f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Look();
        Move();
        Jump();
        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();
        }
    }
    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }
    void Move()
    {
        // Gravity
        if (_cc.isGrounded && _yVelocity < 0f)
            _yVelocity = GROUNDING_FORCE;
        _yVelocity += Physics.gravity.y * Time.deltaTime;

        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);

        _cc.Move((transform.rotation * moveAmount + new Vector3(0f, _yVelocity, 0f)) * Time.deltaTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _cc.isGrounded)
        {
            _yVelocity += jumpForce;
            OnJump?.Invoke();
        }
    }
    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;

        itemIndex = _index;

        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;


    }
}
