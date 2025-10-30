using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	private float m_xInputRaw;
	private float m_yInputRaw;

	private float m_driveSpeed = 20;
	private float m_rotSpeed = 60;

	private float m_fuel;
	private float m_dist;

	// Events
	public event Action<float> OnFuelUpdate;
	public event Action<float> OnDistUpdate;

	// Components
	private Rigidbody rb;

	public enum EPlayerStates
	{
		None,
		Driving,
		Crashing,
		Empty
	}
	public EPlayerStates m_CurrentState { get; private set; }

	#region Initialization

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		m_fuel = 100;
		m_dist = 0;

		SubscribeInput();

		TrackManager.Instance.OnRaceBegin += OnRaceBegin;
	}

	private void OnDestroy()
	{
		UnSubscribeInput();
	}
	#endregion

	#region Input

	private void SubscribeInput()
	{
		// Observer
		InputManager.Controls.Player.Movement.performed += OnMoveInput;
	}

	private void UnSubscribeInput()
	{
		InputManager.Controls.Player.Movement.performed -= OnMoveInput;
	}

	private void OnMoveInput(InputAction.CallbackContext context)
	{
		m_xInputRaw = context.ReadValue<Vector2>().x;
		m_yInputRaw = context.ReadValue<Vector2>().y;
	}
	#endregion

	#region State

	public void SetState(EPlayerStates newState)
	{
		if (newState == m_CurrentState) return;

		m_CurrentState = newState;
		Debug.Log($"State Changed: {newState}");
	}

	private void OnRaceBegin()
	{
		SetState(EPlayerStates.Driving);
	}

	private void Drive(float deltaTime)
	{
		Vector3 currentPos = transform.position;
		Vector3 wishDir = (Vector3.right * m_xInputRaw * m_driveSpeed * deltaTime);

		rb.MovePosition(currentPos + wishDir);
	}

	private void Crashing(float deltaTime)
	{
		transform.Rotate(Vector3.up * m_rotSpeed * Time.deltaTime);
	}

	private void OutOfFuel()
	{

	}
	#endregion

	void Update()
	{
		if (m_CurrentState == EPlayerStates.Driving)
		{
			SetFuel(m_fuel - Time.deltaTime);
			SetDist(m_dist + Time.deltaTime);

			Drive(Time.deltaTime);
		}
		else if (m_CurrentState == EPlayerStates.Crashing)
		{
			Crashing(Time.deltaTime);
		}

		if(m_fuel <= 0)
		{
			SetState(EPlayerStates.Empty);
			OutOfFuel();
		}
	}

	#region Values

	private void SetFuel(float value)
	{
		m_fuel = value;
		OnFuelUpdate?.Invoke(m_fuel);
	}

	private void SetDist(float value)
	{
		m_dist = value;
		OnDistUpdate?.Invoke(m_dist);
	}

	#endregion

	public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Car"))
		{
			SetState(EPlayerStates.Crashing);
			rb.velocity = Vector3.zero;
		}
	}
}
