# AzureKinectToHololens2
Transform Azure Kinect space to Hololens 2 space using Unity and Vuforia

Unity 2019.4 LTS

* [Get Photon account](https://www.photonengine.com/), create and App ID in your dashboard, add App ID to PhotonServerSettings

* Get Vuforia account, create license key and add key to Vuforia settings

* Deploy scene "VuforiaScene_HL2" to HL2.

* Run scene "VuforiaScene_AzureKinect" on PC with Azure Kinect plugged in. 
### Important!
When running the scene "VuforiaScene_AzureKinect" on the PC, change the Virtual Scene Scale Factor in the Vuforia settings to 0.5 (no idea why Vuforia has the scaling completely wrong for the Azure Kinect).
