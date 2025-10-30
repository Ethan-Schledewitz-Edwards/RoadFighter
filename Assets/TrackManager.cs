using System;
using System.Collections;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
	public static TrackManager Instance;

	private float m_countDown = 0.1f;// 3 for 3-2-1 countdown

	public event Action OnRaceBegin;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	void Start()
    {
		StartCoroutine(StartRace());
    }

	public IEnumerator StartRace()
	{
		while(m_countDown > 0)
		{
			m_countDown -= Time.deltaTime;
			yield return null;
		}

		OnRaceBegin?.Invoke();
	}
}
