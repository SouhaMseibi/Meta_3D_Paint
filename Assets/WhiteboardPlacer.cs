using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine;
using TMPro; 
using Meta.XR.MRUtilityKit;
public class WhietbaordPlacer : MonoBehaviour
{
    [SerializeField]
    public GameObject  anchorPrefab;
    [SerializeField]
    public GameObject   shadowPrefab;

    private GameObject currentPreview;
    public const string NumUuidsPlayerPref = "numUuids";

    private Canvas canvas;
    // private TextMeshProUGUI uuidText;
    // private TextMeshProUGUI savedStatusText;
    private List<OVRSpatialAnchor> anchors = new List<OVRSpatialAnchor>();
    private OVRSpatialAnchor lastCreatedAnchor;
    // private AnchorLoader anchorLoader;
    private bool isInitialized;
    private bool IsCreated ; 

    public void Initialized() 
    {
         isInitialized = true;
         IsCreated = false ; 
    }

    // private void Awake()
    // {
    //     anchorLoader = GetComponent<AnchorLoader>();
    // }
    private void Start() => currentPreview= Instantiate(shadowPrefab);
    void Update()
    {
        if (!isInitialized && IsCreated ) return;

        Vector3 rayOrigin = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Vector3 rayDirection = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;

        if (MRUK.Instance?.GetCurrentRoom()?.Raycast(new Ray(rayOrigin, rayDirection), Mathf.Infinity, out RaycastHit hit, out MRUKAnchor anchorHit) == true  &&  !IsCreated)
        {
            if (anchorHit != null )
            {
                currentPreview.transform.position = hit.point;
                currentPreview.transform.rotation = Quaternion.LookRotation(-hit.normal);
                if (OVRInput.GetDown(OVRInput.Button.One))
                {
                    Quaternion rotation = Quaternion.LookRotation(-hit.normal);
                    CreateSpatialAnchor(hit.point, rotation);
                    
                }
            }
            
        }
    }
    public void CreateSpatialAnchor(Vector3 position , Quaternion rotation )
    {
        GameObject  Keybaord  = Instantiate(anchorPrefab, position , rotation );
        Keybaord.AddComponent<OVRSpatialAnchor>();
        IsCreated = true ; 
    }

}