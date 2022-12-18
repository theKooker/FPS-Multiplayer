using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationUpdater : MonoBehaviourPunCallbacks
{// TODO fix: https://github.com/Unity-Technologies/com.unity.netcode.gameobjects/issues/2305
    private Animator _animator;
    private readonly static int ANIM_XMOVEMENT = Animator.StringToHash("xMovement");
    private readonly static int ANIM_ZMOVEMENT = Animator.StringToHash("zMovement");
    private readonly static int ANIM_SPEED = Animator.StringToHash("speed");
    private readonly static int ANIM_CROUCH = Animator.StringToHash("crouch");
    private readonly static int ANIM_JUMP = Animator.StringToHash("jump");
    private readonly static int ANIM_GROUNDED = Animator.StringToHash("grounded");

    [SerializeField] private float transitionSpeed;
    private float _animatorSpeed;
    private Vector2 _animatorMovement;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PhotonView pView;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    private void Start() {
        enabled = pView.IsMine;
    }

    private void LateUpdate() {
        UpdateAnimator();
    }

    public override void OnEnable() {
        base.OnEnable();
        _animatorSpeed = 0f;
        _animatorMovement = new Vector2(0,0);
        playerController.OnJump += Jump;
    }

    public override void OnDisable() {
        base.OnDisable();
        playerController.OnJump -= Jump;
    }

    private void UpdateAnimator() {
        Vector3 localVelocity = Quaternion.Inverse(transform.rotation) * playerController.Velocity;
        Vector2 planeVelocity = new Vector2(localVelocity.x, localVelocity.z);
        // Movement Direction
        Vector2 movementChange = (planeVelocity.normalized - _animatorMovement) * Time.deltaTime;
        _animatorMovement += movementChange * transitionSpeed;
        _animatorMovement = Vector3.ClampMagnitude(_animatorMovement, 1f);
        _animator.SetFloat(ANIM_XMOVEMENT, _animatorMovement.x);
        _animator.SetFloat(ANIM_ZMOVEMENT, _animatorMovement.y);

        // Speed Value
        float speedChange = (planeVelocity.magnitude/playerController.MaxSpeed - _animatorSpeed) * Time.deltaTime;
        _animatorSpeed += speedChange * transitionSpeed;
        _animatorSpeed = Mathf.Clamp(_animatorSpeed, 0f, 1f);
        _animator.SetFloat(ANIM_SPEED, _animatorSpeed);

        // Crouch Value
        _animator.SetFloat(ANIM_CROUCH, playerController.Crouched);

        _animator.SetBool(ANIM_GROUNDED, playerController.Grounded);
    }

    private void Jump() {
        _animator.SetTrigger(ANIM_JUMP);
    }
}
