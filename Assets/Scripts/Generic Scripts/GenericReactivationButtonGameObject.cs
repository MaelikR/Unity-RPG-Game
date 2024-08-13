using UnityEngine;

public class GenericReactivationButtonGameObject : MonoBehaviour
{
	public GameObject reaObject;
	[Range(0f, 60f)] 
	public float dlyTime = 20f;
	public void OnButtonClick()
	{
		Invoke("ReactivateGameObject", dlyTime);
	}

	private void ReactivateGameObject()
	{
		reaObject.SetActive(true);
	}
}