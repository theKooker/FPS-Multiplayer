using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
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

	PhotonView PV;

	const float maxHealth = 100f;
	float currentHealth = maxHealth;

	PlayerManager playerManager;

	[Header("Crouch")]
	private float _crouch;
    public float Crouched => _crouch;
    [SerializeField] private float crouchSpeed;

    [SerializeField, Min(0f)] private float crouchDistance;
    private Vector3 initialCameraPosition;
    private float initialHeight;
    private float initialCenter;

	// Animation Interface
	public event Action OnJump;
    public bool Grounded => _cc.isGrounded;
	public float MaxSpeed => sprintSpeed;
	public Vector3 Velocity => _cc.velocity;

	[SerializeField] private bool test = false;

	private CharacterController _cc;
	private float _yVelocity;
    private const float GROUNDING_FORCE = -0.01f;

	public override void OnEnable() {
		base.OnEnable();
		_yVelocity = GROUNDING_FORCE;
	}

	void Awake()
	{
		_cc = GetComponent<CharacterController>();
		PV = GetComponent<PhotonView>();
		if (!test)
			playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
	
		// Crouch
		initialCameraPosition = cameraHolder.transform.localPosition;
		initialHeight = _cc.height;
		initialCenter = _cc.center.y;
	}

	void Start()
	{
		if(IsMineProxy())
		{
			EquipItem(0);
		}
		else
		{
			Destroy(GetComponentInChildren<Camera>().gameObject);
			Destroy(ui);
		}
	}

	void Update()
	{
		if(!IsMineProxy())
			return;

		Look();
		Move();
		Jump();
		Crouch();

		for(int i = 0; i < items.Length; i++)
		{
			if(Input.GetKeyDown((i + 1).ToString()))
			{
				EquipItem(i);
				break;
			}
		}

		if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
		{
			if(itemIndex >= items.Length - 1)
			{
				EquipItem(0);
			}
			else
			{
				EquipItem(itemIndex + 1);
			}
		}
		else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
		{
			if(itemIndex <= 0)
			{
				EquipItem(items.Length - 1);
			}
			else
			{
				EquipItem(itemIndex - 1);
			}
		}

		if(Input.GetMouseButtonDown(0))
		{
			if (!test)
				items[itemIndex].Use();
		}

		if(transform.position.y < -10f)
		{
			Die();
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
		if(Input.GetKeyDown(KeyCode.Space) && _cc.isGrounded)
		{
			_yVelocity += jumpForce;
			OnJump?.Invoke();
		}
	}

	private void Crouch() {
		float newCrouch;
        if (Input.GetKey(KeyCode.LeftControl))
            newCrouch = Mathf.Min(_crouch + crouchSpeed * Time.deltaTime, 1f);
        else
            newCrouch = Mathf.Max(_crouch - crouchSpeed * Time.deltaTime, 0f);
        SetCrouch(newCrouch);
	}

	private void SetCrouch(float value) {
        if (value == _crouch)
			return;

		if (test) {
			CrouchUpdate(value, new PhotonMessageInfo());
		}
		else
			photonView.RPC(nameof(CrouchUpdate), RpcTarget.All, value);
    }

    [PunRPC]
    private void CrouchUpdate(float value, PhotonMessageInfo info) {
        CrouchHightAdjustment(_crouch, value);
        _crouch = value;
    }

	private void CrouchHightAdjustment(float oldValue, float newValue) {
        // Move camera
        Vector3 cameraPosition = initialCameraPosition;
        cameraPosition.y = cameraPosition.y - crouchDistance * _crouch;
        cameraHolder.transform.localPosition = cameraPosition;

		// Resize CharacterController
        _cc.height = initialHeight - crouchDistance * _crouch;
        Vector3 center = _cc.center;
        center.y = initialCenter - crouchDistance * _crouch * 0.5f;
        _cc.center = center;
    }

	void EquipItem(int _index)
	{
		if(_index == previousItemIndex)
			return;

		itemIndex = _index;

		items[itemIndex].itemGameObject.SetActive(true);

		if(previousItemIndex != -1)
		{
			items[previousItemIndex].itemGameObject.SetActive(false);
		}

		previousItemIndex = itemIndex;

		if(IsMineProxy())
		{
			Hashtable hash = new Hashtable();
			hash.Add("itemIndex", itemIndex);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
		}
	}

	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if(changedProps.ContainsKey("itemIndex") && !IsMineProxy() && targetPlayer == PV.Owner)
		{
			EquipItem((int)changedProps["itemIndex"]);
		}
	}

	public void SetGroundedState(bool _grounded)
	{
		grounded = _grounded;
	}

	void FixedUpdate()
	{
		if(!IsMineProxy())
			return;
	}

	public void TakeDamage(float damage)
	{
		PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
	}

	[PunRPC]
	void RPC_TakeDamage(float damage, PhotonMessageInfo info)
	{
		currentHealth -= damage;

		healthbarImage.fillAmount = currentHealth / maxHealth;

		if(currentHealth <= 0)
		{
			Die();
			if (!test)
				PlayerManager.Find(info.Sender).GetKill();
		}
	}

	void Die()
	{
		if (!test)
			playerManager.Die();
	}

	private bool IsMineProxy() {
		return test || PV.IsMine;
	}
}