using UnityEngine;

public class GenericDeacGameObject : MonoBehaviour
{
	public GameObject deaObject;
	[Range(0f, 60f)]
	public float dlyTime = 20f;
	private void Start()
	{
		Invoke("DeactivationGameObject", dlyTime);
	}

	private void DeactivationGameObject()
	{
		deaObject.SetActive(false);
	}
}