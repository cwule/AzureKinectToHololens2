using System.IO;
using UnityEngine;


// this script is attached to the ImageTarget on HL2 side
// it receives the transform of the KinectImageTarget and calculates the transform from Kinect to HL2
public class CalibCalc : MonoBehaviour
{
    // The cube on the HL2 that will be updated with the AzureKinect ImageTarget transformed to HL2 space
    public GameObject _streamCube;

    // The Marker 1 Image target transform in Azure Kinect space
    public Transform _azureTrans;

    // The Marker 2 Image target transform in Azure Kinect space
    public Transform _azureTrans2;

    // The Marker 1 Image target transform in HL2 space
    public Transform _HLTrans;

    // The Hololens camera transform
    public Transform _HLCamTrans;

    // check whether the calibration is already done
    private bool _calibDone = false;

    // The transformation matrix from kinect to HL2 space
    private Matrix4x4 T_kToHl = new Matrix4x4();

    // The transformation matrix from marker 2 to HL2 space
    private Matrix4x4 T_m2Hl = new Matrix4x4();

    // The transformation matrix of m1 in HL space with Hololens being only recorded via m2
    private Matrix4x4 T_m1Hlnew = new Matrix4x4();

    // The transformation matrix of Hololens camera to M2 in kinect space
    private Matrix4x4 T_HlcToM2k = new Matrix4x4();

    private void Start()
    {

        // check if there is already a prior calibration matrix (T_m2Hl) and read it
        try
        {
            Matrix4x4 test = ReadMatrix();
        }
        catch
        {
            Debug.Log("No prior TM2HL calibration found");
        }
    }
    // called eg. via speech command. Takes transform of Imagetarget in Kinect space and Image Target in HL2 space and calculates the transformation from kinect to HL2 space
    public void CalibrateTm2hl()
    {
        //// find the not-local player that contains the kinect transform 
        //GameObject[] PUNPlayers = GameObject.FindGameObjectsWithTag("PUNPlayer");

        //foreach (GameObject player in PUNPlayers)
        //{
        //    if (!player.GetComponent<PhotonView>().IsMine)
        //    {
        //        _azureTrans = player.transform;
        //    }
        //}

        // m1k - marker 1 in kinect space
        Matrix4x4 T_m1k = Matrix4x4.TRS(_azureTrans.position, _azureTrans.rotation, new Vector3(1.0f, 1.0f, 1.0f));

        // m1h - marker 1 in hololens space
        Matrix4x4 T_m1h = Matrix4x4.TRS(_HLTrans.transform.position, _HLTrans.transform.rotation, new Vector3(1.0f, 1.0f, 1.0f));

        // Calculate Kinect space to HL2 space transform
        T_kToHl = T_m1h * T_m1k.inverse;

        // m2k - marker 2 in kinect space
        Matrix4x4 T_m2k = Matrix4x4.TRS(_azureTrans2.position, _azureTrans2.rotation, new Vector3(1.0f, 1.0f, 1.0f));

        // Calculate marker 2 to HL2 space transform
        T_m2Hl = T_kToHl * T_m2k.inverse;

        WriteMatrix(T_m2Hl);
    }

    // get the transformation from marker 2 in Azure Kinect space to the Hololens camera 
    public void CalibrateHlcToM2k()
    {
        // not needed: m1k - marker 1 in kinect space
        //Matrix4x4 T_m1k = Matrix4x4.TRS(_azureTrans.position, _azureTrans.rotation, new Vector3(1.0f, 1.0f, 1.0f));

        // m2k - marker 2 in kinect space
        Matrix4x4 T_m2k = Matrix4x4.TRS(_azureTrans2.position, _azureTrans2.rotation, new Vector3(1.0f, 1.0f, 1.0f));

        // hlc - hololens camera
        Matrix4x4 T_Hlc = Matrix4x4.TRS(_HLCamTrans.position, _HLCamTrans.rotation, new Vector3(1.0f, 1.0f, 1.0f));

        // not needed: marker 1 in HL space when m2 is tracked by kinect
        //Matrix4x4 T_m1HlnewTmp = T_m2Hl * T_m2k * T_m1k;

        T_HlcToM2k = T_Hlc.inverse * T_m2k;

        _calibDone = true;
    }


    private void Update()
    {
        if (_calibDone == true)
        {
            Matrix4x4 T_m1k = Matrix4x4.TRS(_azureTrans.position, _azureTrans.rotation, new Vector3(1.0f, 1.0f, 1.0f));
            Matrix4x4 T_Hlc = Matrix4x4.TRS(_HLCamTrans.position, _HLCamTrans.rotation, new Vector3(1.0f, 1.0f, 1.0f)); 
            Matrix4x4 T_m1hlnew = T_m2Hl * T_Hlc * T_HlcToM2k * T_m1k;
            _streamCube.transform.SetPositionAndRotation(new Vector3(T_m1hlnew.m03, T_m1hlnew.m13, T_m1hlnew.m23), T_m1hlnew.rotation);
        }
    }


    private void WriteMatrix(Matrix4x4 savMat)
    {
        TextWriter tw = new StreamWriter("Assets/Resources/M2ToHL.txt");
        for (int j = 0; j < 4; j++) // Column
        {
            for (int i = 0; i < 4; i++) // Row
            {
                tw.WriteLine(savMat[i, j] + " ");
            }
        }
        tw.Close();

        Debug.Log(T_m2Hl.ToString());
    }

    private Matrix4x4 ReadMatrix()
    {
        string path = "Assets/Resources/M2ToHL.txt";
        var matString = System.IO.File.ReadAllText(path);
        matString = matString.Replace("\n","");
        string[] matStringSplit = matString.Split('\r');

        Matrix4x4 newMat = new Matrix4x4();

        float.TryParse(matStringSplit[0], out newMat.m00);
        float.TryParse(matStringSplit[1], out newMat.m10);
        float.TryParse(matStringSplit[2], out newMat.m20);
        float.TryParse(matStringSplit[3], out newMat.m30);
        float.TryParse(matStringSplit[4], out newMat.m01);
        float.TryParse(matStringSplit[5], out newMat.m11);
        float.TryParse(matStringSplit[6], out newMat.m21);
        float.TryParse(matStringSplit[7], out newMat.m31);
        float.TryParse(matStringSplit[8], out newMat.m02);
        float.TryParse(matStringSplit[9], out newMat.m12);
        float.TryParse(matStringSplit[10], out newMat.m22);
        float.TryParse(matStringSplit[11], out newMat.m32);
        float.TryParse(matStringSplit[12], out newMat.m03);
        float.TryParse(matStringSplit[13], out newMat.m13);
        float.TryParse(matStringSplit[14], out newMat.m23);
        float.TryParse(matStringSplit[15], out newMat.m33);

        return newMat;
    }

}
