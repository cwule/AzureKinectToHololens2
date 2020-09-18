using UnityEngine;
using Photon.Pun;

public class CopyTransform : MonoBehaviourPunCallbacks
{
    public Transform _imgTarg;
    
    private void Update()
    {
        if (_imgTarg != null)
        {
            this.transform.position = _imgTarg.position;
            this.transform.rotation = _imgTarg.rotation;
        }
    }


}
