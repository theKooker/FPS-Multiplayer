using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float walkSpeed;
    //public float WalkSpeed => walkSpeed;
    [SerializeField] private float runSpeed;
    public float MaxSpeed => runSpeed;
    public Vector3 Velocity => _characterController.velocity;

    private CharacterController _characterController;
    private PlayerInput _input;
    private InputAction _walkAction;
    private InputAction _runAction;
    private InputAction _lookAction;
    private InputAction _crouchAction;

    [SerializeField] private float mouseSensitivity;
    private float _xRot;
    [SerializeField] private Transform firstPersonCamera;

    [SerializeField] private float jumpHeight;
    private float _jumpForce;
    private float _yVelocity;
    private const float GROUNDING_FORCE = -0.01f;

    private NetworkVariable<float> _crouch = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public float Crouched => _crouch.Value;
    [SerializeField] private float crouchSpeed;

    [SerializeField, Min(0f)] private float crouchDistance;
    private Vector3 initialCameraPosition;
    private float initialCharacterControllerHeight;
    private float initialCharacterControllerCenter;

    public event Action OnJump;
    public bool Grounded => _characterController.isGrounded;

    private void Awake() {
        _characterController = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInput>();
        initialCameraPosition = firstPersonCamera.localPosition;
        initialCharacterControllerHeight = _characterController.height;
        initialCharacterControllerCenter = _characterController.center.y;
        CalculateJumpForce();
        _crouch.OnValueChanged += CrouchHightAdjustment;// TODO unsubscribe
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        if (!IsOwner)
            return;
        
        _input.enabled = true;
        firstPersonCamera.gameObject.SetActive(true);

        _walkAction = _input.actions["Move"];
        _runAction = _input.actions["Run"];
        _lookAction = _input.actions["Look"];
        _crouchAction = _input.actions["Crouch"];
        _input.actions["Jump"].performed += Jump;
    }

    private void OnEnable() {
        _xRot = 0f;
        _yVelocity = GROUNDING_FORCE;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _crouch.Value = 0f;
    }

    private void Update() {
        if (!IsOwner)
            return;

        // Gravity
        if (_characterController.isGrounded && _yVelocity < 0f)
        {
            _yVelocity = GROUNDING_FORCE;
        }
        _yVelocity += Physics.gravity.y * Time.deltaTime;

        // Movement
        Vector2 movementInput = _walkAction.ReadValue<Vector2>();
        float speed = _runAction.IsPressed() ? runSpeed : walkSpeed;
        Vector3 movement = Quaternion.LookRotation(transform.forward, Vector3.up) * new Vector3(movementInput.x, 0f, movementInput.y) * speed;
        // Adjust movement when walking down slopes to prevent bouncing
        var adjustedMovement = Quaternion.FromToRotation(Vector3.up, GetGroundNormal()) * movement;
        movement = adjustedMovement.y < 0f ? adjustedMovement : movement;
        _characterController.Move((movement + new Vector3(0f, _yVelocity, 0f)) * Time.deltaTime);

        // Camera Rotation
        Vector2 rotInput = _lookAction.ReadValue<Vector2>() * Time.deltaTime * mouseSensitivity;
        transform.Rotate(Vector3.up * rotInput.x);
        _xRot += rotInput.y;
        _xRot = Mathf.Clamp(_xRot, -90f, 90f);
        firstPersonCamera.localRotation = Quaternion.Euler(-_xRot, 0f, 0f);

        // Crouch
        if (_crouchAction.IsPressed())
            _crouch.Value = Mathf.Min(_crouch.Value + crouchSpeed * Time.deltaTime, 1f);
        else
            _crouch.Value = Mathf.Max(_crouch.Value - crouchSpeed * Time.deltaTime, 0f);
    }

    private Vector3 GetGroundNormal()
    {
        var ray = new Ray(transform.position, Vector3.down);// TODO add layer mask
        if (Physics.Raycast(ray, out var hit, 0.2f))
            return hit.normal;
        Debug.Log("GroundNormal could not be determined");
        return -Physics.gravity.normalized;
    }

    private void CalculateJumpForce()
    {
        _jumpForce = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y - GROUNDING_FORCE);
    }

    private void OnValidate()
    {
        CalculateJumpForce();
    }

    private void Jump(InputAction.CallbackContext context) {
        if (!_characterController.isGrounded)
            return;
        _yVelocity += _jumpForce;
        OnJump?.Invoke();
    }

    private void OnDrawGizmosSelected() {
        if (firstPersonCamera) {
            Vector3 transformedInitialPos = transform.position + initialCameraPosition;
            Vector3 crouchPos = transformedInitialPos - Vector3.up * crouchDistance;
            Vector3 lineOffset = transform.forward * 0.5f;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(crouchPos, Vector3.one * 0.1f);
            Gizmos.DrawLine(crouchPos, crouchPos + lineOffset);

            Gizmos.DrawWireCube(transformedInitialPos, Vector3.one * 0.1f);
            Gizmos.DrawLine(transformedInitialPos, transformedInitialPos + lineOffset);
        }
    }

    private void CrouchHightAdjustment(float oldValue, float newValue) {
        // Move camera
        Vector3 cameraPosition = initialCameraPosition;
        cameraPosition.y = cameraPosition.y - crouchDistance * _crouch.Value;
        firstPersonCamera.localPosition = cameraPosition;

        // Resize CharacterController
        _characterController.height = initialCharacterControllerHeight - crouchDistance * _crouch.Value;
        Vector3 center = _characterController.center;
        center.y = initialCharacterControllerCenter - crouchDistance * _crouch.Value * 0.5f;
        _characterController.center = center;
    }
}
