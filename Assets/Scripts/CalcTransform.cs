using Photon.Pun;
using UnityEngine;


// this script is attached to the ImageTarget on HL2 side
// it receives the transform of the KinectImageTarget and calculates the transform from Kinect to HL2
public class CalcTransform : MonoBehaviour
{
    private Transform _azureTrans;
    private bool _calibDone = false;
    private Matrix4x4 T_k2hl = new Matrix4x4();
    public GameObject _streamCube;

    public void TakeMeasurement()
    {
        GameObject[] PUNPlayers = GameObject.FindGameObjectsWithTag("PUNPlayer");
        
        foreach (GameObject player in PUNPlayers)
        {
            if (!player.GetComponent<PhotonView>().IsMine)
            {
                _azureTrans = player.transform;
            }
        }

        // mk - marker in kinect space
        Matrix4x4 T_mk = Matrix4x4.TRS(_azureTrans.position, _azureTrans.rotation, new Vector3(1.0f, 1.0f, 1.0f));

        // mh - marker in hololens space
        Matrix4x4 T_mh = Matrix4x4.TRS(this.transform.position, this.transform.rotation, new Vector3(1.0f, 1.0f, 1.0f));

        _calibDone = true;
        GetComponentInChildren<Renderer>().enabled = false;
        T_k2hl = T_mh * T_mk.inverse;
    }

    private void Update()
    {
        if (_calibDone)
        {
            Debug.Log("azure_trans " + _azureTrans.position.ToString());
            Matrix4x4 T_mk = Matrix4x4.TRS(_azureTrans.position, _azureTrans.rotation, new Vector3(1.0f, 1.0f, 1.0f));
            Matrix4x4 T_kInHl = T_k2hl * T_mk;
            Debug.Log("rotation " + T_kInHl.rotation.ToString());
            _streamCube.transform.SetPositionAndRotation(new Vector3(T_kInHl.m03, T_kInHl.m13, T_kInHl.m23), T_kInHl.rotation);
        }
    }

}
