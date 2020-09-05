using Photon.Pun;
using UnityEngine;


// this script is attached to the ImageTarget on HL2 side
// it receives the transform of the KinectImageTarget and calculates the transform from Kinect to HL2
public class CalcTransform : MonoBehaviour
{
    // The cube on the HL2 that will be updated with the AzureKinect ImageTarget transformed to HL2 space
    public GameObject _streamCube;

    // The Image target transform in Azure Kinect space
    private Transform _azureTrans;

    // check whether the calibration is already done
    private bool _calibDone = false;

    // The transformation matrix from kinect to HL2 space
    private Matrix4x4 T_k2hl = new Matrix4x4();
    
    // called eg. via speech command. Takes transform of Imagetarget in Kinect space and Image Target in HL2 space and calculates the transformation from kinect to HL2 space
    public void TakeMeasurement()
    {
        // find the not-local player that contains the kinect transform 
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

        // turn of renderer of HL2 Image Target
        GetComponentInChildren<Renderer>().enabled = false;

        // Calculate Kinect space to HL2 space transform
        T_k2hl = T_mh * T_mk.inverse;
    }

    // every frame update Kinect ImageTarget transform to HL2 space
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
