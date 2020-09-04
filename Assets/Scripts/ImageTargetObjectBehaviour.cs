using UnityEngine;

public class ImageTargetObjectBehaviour : MonoBehaviour
{
    public void ToggleStatus(bool status)
    {
        Debug.Log("Tracking " + status.ToString());
        gameObject.SetActive(status);
    }
}
