using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class recognizeImage : MonoBehaviour
{
    public GameObject dolfPrefab, mapPrefab, saintPrefab;
    private ARTrackedImageManager trackedImage;
    private GameObject dolf, map, saint;
    private ARRaycastManager aRRaycastManager;
    public GameObject indicator;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private string currentObject = "";
    // Start is called before the first frame update
    void Start()
    {
        trackedImage = gameObject.GetComponent<ARTrackedImageManager>();
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        trackedImage.trackedImagesChanged += OnImageChanged;

    }
    void Update()
    {
        UpdatePose();
        UpdateIndicator();
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlacePrefab();
        }

    }
    private void UpdatePose()
    {
        Vector3 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);
        placementPoseIsValid = hits.Count > 1;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
    private void UpdateIndicator()
    {
        if (placementPoseIsValid)
        {
            indicator.SetActive(true);
            indicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            indicator.SetActive(false);
        }

    }
    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage img in args.added)
        {
            if (img.referenceImage.name.Equals("Cover"))
            {
                dolf = Instantiate(dolfPrefab, img.transform.position, img.transform.rotation);
                currentObject = "Cover";
            }
            else if (img.referenceImage.name.Equals("Map"))
            {
                map = Instantiate(mapPrefab, img.transform.position, img.transform.rotation);
                currentObject = "Map";
            }
            else if (img.referenceImage.name.Equals("Saint"))
            {
                saint = Instantiate(saintPrefab, img.transform.position, img.transform.rotation);
                currentObject = "Saint";
            }
        }
        foreach (ARTrackedImage img in args.updated)
        {
            if (img.referenceImage.name.Equals("Cover"))
            {
                UpdateTrackedObject(dolf, img);
            }
            else if (img.referenceImage.name.Equals("Map"))
            {
                UpdateTrackedObject(map, img);
            }
            else if (img.referenceImage.name.Equals("Saint"))
            {
                UpdateTrackedObject(saint, img);
            }
        }
    }
    private void UpdateTrackedObject(GameObject trackedObject, ARTrackedImage img)
    {
        if (img.trackingState == TrackingState.Tracking)
        {
            trackedObject.SetActive(true);
            trackedObject.transform.position = img.transform.position;
            trackedObject.transform.rotation = img.transform.rotation;

        }
        else
        {
            trackedObject.SetActive(false);
        }
    }

    private void PlacePrefab()
    {
        if (currentObject.Equals("Cover"))
        {
            Instantiate(dolfPrefab, placementPose.position, placementPose.rotation);
        }
        else if (currentObject.Equals("Map"))
        {
            Instantiate(mapPrefab, placementPose.position, placementPose.rotation);
        }
        else if (currentObject.Equals("Saint"))
        {
            Instantiate(saintPrefab, placementPose.position, placementPose.rotation);
        }
    }
}
