using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerController m_playerController;

	[SerializeField] private TextMeshProUGUI m_fuelValueText;
	[SerializeField] private TextMeshProUGUI m_distValueText;

	private void Start()
	{
		m_playerController.OnFuelUpdate += SetFuelText;
		m_playerController.OnDistUpdate += SetDistText;
	}

	private void OnDestroy()
	{
		m_playerController.OnFuelUpdate -= SetFuelText;
		m_playerController.OnDistUpdate -= SetDistText;
	}

	private void SetFuelText(float value)
	{
		m_fuelValueText.text = ((int)value).ToString();
	}

	private void SetDistText(float value)
	{
		m_distValueText.text = ((int)value).ToString();
	}
}
