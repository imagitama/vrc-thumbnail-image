using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class VRC_Thumbnail_Image : MonoBehaviour
{
    [Tooltip("A 1024x768 image to display as the thumbnail background")]
    public Texture2D imageToUse;
    [Tooltip("Test it out in your editor")]
    public bool testInEditor = false;
    [Tooltip("Test it out in play mode")]
    public bool testInPlay = false;
    [Tooltip("If to set every root game object to inactive")]
    public bool hideBackgroundObjects = true;
    [Tooltip("A list of game objects to exclude")]
    public Transform[] objectsToExclude;
    [Tooltip("Push the camera left or right")]
    public float offsetX = 0f;
    [Tooltip("Push the camera up or down")]
    public float offsetY = 0f;
    [Tooltip("If to override an animator with a new controller (only in play mode)")]
    public bool overrideAnimator = false;
    [Tooltip("The animator to override")]
    public Animator target;
    [Tooltip("The controller to use")]
    public RuntimeAnimatorController controller;

    bool hasCompletedSetup = false;

    bool previousIsTesting = false;
    Camera camera;
    GameObject canvasGameObject;
    GameObject testCamera;

    void Update() {
        if (previousIsTesting != testInEditor) {
            if (testInEditor == false) {
                StopTesting();
            }
            previousIsTesting = testInEditor;
        }

        if (hasCompletedSetup == false) {
            if (Application.isPlaying == true) {
                if (testInPlay && !IsInVrchatPublishMode()) {
                    StartTesting();
                } else {
                    DoIt();
                }
            } else {
                if (testInEditor == true) {
                    StartTesting();
                }
            }
        }

        if (canvasGameObject != null) {
            canvasGameObject.transform.position = new Vector3(offsetX, offsetY, -10);
        }

        if (camera != null) {
            camera.transform.position = new Vector3(offsetX, offsetY, 10);
        }
    }

    bool IsInVrchatPublishMode() {
        return GameObject.Find("/VRCCam") != null;
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

        testInEditor = true;

        CreateTestCamera();
        
        DoIt();
    }

    void StopTesting() {
        Debug.Log("Stopping testing...");

        testInEditor = false;

        ClearOutput();
        RemoveTestCamera();

        hasCompletedSetup = false;
    }

    void DoIt() {
        Debug.Log("Doing it...");

        camera = GetVRCCamera();

        if (camera == null) {
            Debug.Log("VRC camera not detected - stopping");
            return;
        }

        if (testInEditor != true) {
            HideBackgroundObjects();
        }

        ConfigureCamera();

        if (overrideAnimator && Application.isPlaying == true) {
            ConfigureAnimator();
        }

        ShowOutput();

        hasCompletedSetup = true;
    }

    void ConfigureCamera() {
        if (camera == null) {
            return;
        }
        camera.transform.rotation = Quaternion.Euler(0, 180, 0);
        camera.orthographic = true;
        camera.orthographicSize = 1;
        camera.cullingMask = -1; // UI hidden by default
    }

    void ConfigureAnimator() {
        target.runtimeAnimatorController = controller;
    }

    List<GameObject> GetRootGameObjects() {
        List<GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);
        return rootObjects;
    }

    List<GameObject> FilterGameObjectsToHide(List<GameObject> allGameObjects) {
        return allGameObjects.Where(go => go.name != "VRCCam" && go.name != "VRCSDK" && go != this.gameObject && !objectsToExclude.Any(go2 => go == go2.gameObject)).ToList();
    }

    void HideBackgroundObjects() {
        if (!hideBackgroundObjects) {
            return;
        }

        var allRootGameObjects = GetRootGameObjects();

        var gameObjectsToHide = FilterGameObjectsToHide(allRootGameObjects);

        Debug.Log("Hiding " + gameObjectsToHide.Count + " game objects...");

        foreach (GameObject gameObjectToHide in gameObjectsToHide) {
            gameObjectToHide.SetActive(false);
        }
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

    void ClearOutput() {
        Debug.Log("Clearing output...");

        if (canvasGameObject == null) {
            return;
        }
    
        canvasGameObject.SetActive(false);
    }

    void ShowOutput() {
        Debug.Log("Showing output...");

        canvasGameObject = transform.Find("Canvas").gameObject;
        
        if (imageToUse != null) {
            canvasGameObject.transform.Find("RawImage").gameObject.GetComponent<RawImage>().texture = imageToUse;
        }

        canvasGameObject.SetActive(true);
    }
}