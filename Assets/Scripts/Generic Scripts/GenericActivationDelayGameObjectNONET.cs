using UnityEngine;

public class GenericActivationDelayGameObjectNONET : MonoBehaviour
{
    public GameObject aObject;
    [Range(0f, 60f)]
    public float activedDlyTime = 20f;

    private void Start()
    {
        Invoke("ActivationGameObject", activedDlyTime);
    }

    private void ActivationGameObject()
    {
        aObject.SetActive(true);
    }
}
