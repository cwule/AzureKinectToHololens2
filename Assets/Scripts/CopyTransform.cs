using UnityEngine;
using Photon.Pun;

public class CopyTransform : MonoBehaviourPunCallbacks
{
    private Transform _imgTarg;
    void Start()
    {
        if (GameObject.Find("KinectImageTarget") != null)
        {
            if (photonView.IsMine)
                _imgTarg = GameObject.Find("KinectImageTarget").transform;
        }
    }

    private void Update()
    {
        if (_imgTarg != null)
        {
            this.transform.position = _imgTarg.position;
            this.transform.rotation = _imgTarg.rotation;
        }
    }


}
