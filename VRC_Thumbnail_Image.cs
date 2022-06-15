using UnityEngine;

[ExecuteInEditMode]
public class VRC_Thumbnail_Image : MonoBehaviour
{
    public bool isEnabled = true;
    public Material materialToUse;
    public bool isTesting = false;

    bool hasCompletedSetup = false;

    Camera camera;
    GameObject mesh;
    GameObject testCamera;

    void Update() {
        if (isEnabled == false || materialToUse == null) {
            return;
        }

        if (isTesting == false) {
            StopTesting();
        }

        if (hasCompletedSetup == false) {
            if (Application.isPlaying == true) {
                DoIt();
            } else {
                if (isTesting == true) {
                    StartTesting();
                }
            }
        }
    }

    Camera GetVRCCamera() {
        Debug.Log("Getting VRC camera...");

        GameObject cameraObject = GameObject.Find("/VRCCam");

        if (cameraObject == null) {
            return null;
        }

        return cameraObject.GetComponent<Camera>();
    }

    void StartTesting() {
        Debug.Log("Starting testing...");

        isTesting = true;

        CreateTestCamera();
        
        DoIt();
    }

    void StopTesting() {
        Debug.Log("Stopping testing...");

        isTesting = false;

        ClearMesh();
        RemoveTestCamera();

        hasCompletedSetup = false;
    }

    void DoIt() {
        Debug.Log("Doing it...");

        camera = GetVRCCamera();

        if (camera == null) {
            return;
        }

        camera.transform.position = new Vector3(0, 0, -9f);
        camera.transform.rotation = Quaternion.Euler(0, 180, 0);
        camera.orthographic = true;
        camera.orthographicSize = 7.5f;

        StoreMesh();

        hasCompletedSetup = true;
    }

    void CreateTestCamera() {
        Debug.Log("Creating test camera...");

        testCamera = new GameObject("VRCCam");
        testCamera.AddComponent<Camera>();
    }

    void RemoveTestCamera() {
        Debug.Log("Removing test camera...");

        DestroyImmediate(testCamera);
    }

    void ClearMesh() {
        Debug.Log("Clear mesh...");

        if (mesh == null) {
            return;
        }

        mesh.GetComponent<MeshRenderer>().material = null;
        mesh.SetActive(false);
    }

    void StoreMesh() {
        Debug.Log("Storing mesh...");

        mesh = FindObject("Plane");
        mesh.GetComponent<MeshRenderer>().material = materialToUse;
        mesh.SetActive(true);
    }

    public GameObject FindObject(string name)
    {
        Component[] transforms = this.gameObject.GetComponentsInChildren(typeof(Transform), true);

        foreach(Transform transform in transforms ) {
            if (transform.name == name){
                return transform.gameObject;
            }
        }

        return null;
    }
}