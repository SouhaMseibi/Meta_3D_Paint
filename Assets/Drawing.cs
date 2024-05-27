using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; 

public class Drawing : MonoBehaviour
{
    public GameObject Line;
    // public float BrushSize ;
    private Texture2D whiteboardTexture;
    private Renderer whiteboardRenderer;
    private Collider whiteboardCollider;
    // [SerializeField] protected OVRHand hand ; 
    // [SerializeField] protected OVRSkeleton handSkeleton ;
    private LineRenderer line;
    private Vector3 previousPosition;
    [SerializeField]
    private float minDistance = 0.1f;
    protected float MaxtriggerDistance = 0.0219f ; 
    protected float MintriggerDistance = 0.0211f ;
    
    private bool isDrawing = false;
    // Start is called before the first frame update
    void Start()
    {
        whiteboardRenderer = GetComponent<Renderer>();
        whiteboardCollider = GetComponent<Collider>();
        line = GetComponent<LineRenderer>();
        line.positionCount = 1;
        previousPosition = transform.position;     
        line.positionCount =1 ;   
    }

    void Update()
    
    {

        if (OVRInput.Get(OVRInput.Button.One))
        {
            isDrawing = true;
        }
        else if (OVRInput.GetUp(OVRInput.Button.One))
        {
            isDrawing = false;
        }
        if (isDrawing)
        {

            GameObject gHit = whiteboardCollider.gameObject;
            Transform tHit = gHit.transform;
            Vector3 currentPosition = new Vector3(tHit.position.x,
                                        tHit.position.y,
                                        tHit.position.z
                                        );
            currentPosition.z = 0f;

            if (Vector3.Distance(currentPosition, previousPosition) > minDistance)
            {
                if (previousPosition == transform.position)
                {
                    line.SetPosition(0, currentPosition);
                }
                else
                {
                    line.positionCount++;
                    line.SetPosition(line.positionCount - 1, currentPosition);
                }

                previousPosition = currentPosition;
            }
        }
    }
}
    




// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.IO; 

// public class Paintable_passthrough : MonoBehaviour
// {
//     public GameObject BrushPrefab;
//     public float BrushSize ;
//     private Texture2D whiteboardTexture;
//     private Renderer whiteboardRenderer;
//     private Collider whiteboardCollider;
//     // [SerializeField] protected OVRHand hand ; 
//     // [SerializeField] protected OVRSkeleton handSkeleton ;
    
//     private bool isDrawing = false;
//     // Start is called before the first frame update
//     void Start()
//     {
//         whiteboardRenderer = GetComponent<Renderer>();
//         whiteboardCollider = GetComponent<Collider>();
//         // Initialize the whiteboard texture
//         CreateTexture();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (OVRInput.Get(OVRInput.Button.One))
//         {
//             isDrawing = true;
//         }
//         else if (OVRInput.GetUp(OVRInput.Button.One))
//         {
//             isDrawing = false;
//         }
//         if ( isDrawing ) 
//         {
//             Vector3 rayOrigin = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
//             Vector3 rayDirection = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;
//             Ray ray = new Ray(rayOrigin, rayDirection) ; 
//             // Ray ray = new Ray(Rightcontroller.position, Rightcontroller.forward);
//             RaycastHit hit;
//             if (Physics.Raycast(ray, out hit))
//             {   
                   
//                     InstantiateBrush(hit.point);

//                     // Draw on the texture
//                     Vector2 uv;
//                     if (GetTextureCoord(hit.point, out uv))
//                     {
//                         DrawOnTexture(uv);
//                     }
                
//             }
//         }
//     }
//     public void CreateTexture()
//     {
//         whiteboardTexture = new Texture2D(1024, 1024);
//         for (int y = 0; y < whiteboardTexture.height; y++)
//         {
//             for (int x = 0; x < whiteboardTexture.width; x++)
//             {
//                 whiteboardTexture.SetPixel(x, y, Color.white);
//             }
//         }
//         whiteboardTexture.Apply();
//         whiteboardRenderer.material.mainTexture = whiteboardTexture;
//     }
//     public bool GetTextureCoord(Vector3 fingerPos, out Vector2 uv)
//     {
//         uv = Vector2.zero;

//         Vector3 localPoint = transform.InverseTransformPoint(fingerPos);
//         float halfWidth = whiteboardCollider.bounds.size.x / 2f;
//         float halfHeight = whiteboardCollider.bounds.size.y / 2f;

//         uv.x = (localPoint.x + halfWidth) / (2f * halfWidth);
//         uv.y = (localPoint.y + halfHeight) / (2f * halfHeight);

//         return uv.x >= 0f && uv.x <= 1f && uv.y >= 0f && uv.y <= 1f;
//     }
//     public void DrawOnTexture(Vector2 uv)
//     {
//         int x = (int)(uv.x * whiteboardTexture.width);
//         int y = (int)(uv.y * whiteboardTexture.height);

//         whiteboardTexture.SetPixel(x, y, Color.black);
//         whiteboardTexture.Apply();
//     }
//     public void InstantiateBrush(Vector3 position)
//     {
//         // Adjust position to be on the surface of the whiteboard
//         // Vector3 localPoint = transform.InverseTransformPoint(position);
//         // localPoint.z = whiteboardCollider.transform.position.z; // Ensure the brush is on the whiteboard surface
//         // Vector3 worldPoint = transform.TransformPoint(localPoint);
//         var brush = Instantiate(BrushPrefab, position + Vector3.up * 0.1f, Quaternion.identity, transform);
//         brush.transform.localScale = Vector3.one * BrushSize;
//     }
// }


