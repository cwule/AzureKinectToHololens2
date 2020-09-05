using UnityEngine;
using Photon.Pun;

// Attached to an instantiated player
// Searches for the Kinect Image Target and copies its transform

public class CopyTransform : MonoBehaviourPunCallbacks
{
    // The azure kinect image target
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
