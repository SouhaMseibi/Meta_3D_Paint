using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; 
using TMPro; 
public class Paintable_passthrough : MonoBehaviour
{
    public GameObject BrushPrefab;
    public float BrushSize ;
    private Texture2D whiteboardTexture;
    private Renderer whiteboardRenderer;
    private Collider whiteboardCollider;
    [SerializeField] protected OVRHand hand ; 
    [SerializeField] protected OVRSkeleton handSkeleton ;
    [SerializeField] protected GameObject whiteboard; 
    [SerializeField] public TextMeshProUGUI MessageText ;
    protected float MaxtriggerDistance = 0.0219f ; 
    protected float MintriggerDistance = 0.0211f ;
    private bool isDrawing = false;
    // Start is called before the first frame update
    void Start()
    {
        hand = GetComponent<OVRHand>();
        handSkeleton = GetComponent<OVRSkeleton>();
        CreateTexture();
    }

    // Update is called once per frame
    void Update()
    {

        if ( !hand ) hand = GetComponent<OVRHand>();
        if ( !handSkeleton ) handSkeleton = GetComponent<OVRSkeleton>();
        if (hand.IsTracked  ) 
        {
            
            Vector3 FingerPos  = HandleInput(FingerTipsPos ( hand , handSkeleton));
            MessageText.text= "Index :" + FingerPos.x.ToString() + "Board : " + whiteboard.transform.position.x.ToString();
            if ( CheckDistance ( FingerPos , whiteboard.transform.position ))
            {
                isDrawing = true;
            }
            else 
            {
                isDrawing=false ; 
            }
            
            
            if ( isDrawing ) 
            {
                // Vector3 rayOrigin = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
                // Vector3 rayDirection = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;
                // Ray ray = new Ray(rayOrigin, rayDirection) ; 
                // // Ray ray = new Ray(Rightcontroller.position, Rightcontroller.forward);
                // RaycastHit hit;
                // if (Physics.Raycast(ray, out hit))
                // {   
                        Vector3 touchZone  = FingerPos ; 
                        touchZone.z = whiteboard.transform.position.z;
                        InstantiateBrush(touchZone);

                        // Draw on the texture
                        Vector2 uv;
                       
                    
            }
        }
    }
    public List<Vector3> FingerTipsPos ( OVRHand hand , OVRSkeleton handSkeleton )
    {
        List<Vector3> FingersPosRight = new List<Vector3>() ; 
        OVRSkeleton.SkeletonType type = handSkeleton.GetSkeletonType () ; 
        if ( type == OVRSkeleton.SkeletonType.HandRight)
        {
            foreach ( var bone in handSkeleton.Bones)
            {
                if ( bone.Id == OVRSkeleton.BoneId.Hand_ThumbTip   ) 
                {
                    FingersPosRight.Add(bone.Transform.position);
                }
                if ( bone.Id == OVRSkeleton.BoneId.Hand_IndexTip  )
                {
                    FingersPosRight.Add(bone.Transform.position);
                }
                if ( bone.Id == OVRSkeleton.BoneId.Hand_MiddleTip )
                {
                    FingersPosRight.Add(bone.Transform.position);
                }
                if ( bone.Id == OVRSkeleton.BoneId.Hand_RingTip)
                {
                    FingersPosRight.Add(bone.Transform.position);
                }
                if ( bone.Id == OVRSkeleton.BoneId.Hand_PinkyTip )
                {
                    FingersPosRight.Add(bone.Transform.position);
                }
            }
        }
            
        return FingersPosRight ; 
    }
    public bool CheckDistance ( Vector3 buttonPos , Vector3 FingerPos ) 
    {
        if (( MintriggerDistance <= Vector3.Distance( buttonPos , FingerPos )) &&  (Vector3.Distance( buttonPos , FingerPos )<= MaxtriggerDistance ) )
        {    return true ; }
        
        return false ;  
    }
    public Vector3 HandleInput (List<Vector3> FingersPosRight  )
    {
        if ( FingersPosRight.Count > 1)
        { 
            return  FingersPosRight[0]; 
        }
        else 
        {   return FingersPosRight[0] ;   }
    }

    public void CreateTexture()
    {
        whiteboardTexture = new Texture2D(1024, 1024);
        for (int y = 0; y < whiteboardTexture.height; y++)
        {
            for (int x = 0; x < whiteboardTexture.width; x++)
            {
                whiteboardTexture.SetPixel(x, y, Color.white);
            }
        }
        whiteboardTexture.Apply();
        whiteboardRenderer.material.mainTexture = whiteboardTexture;
    }
    public bool GetTextureCoord(Vector3 fingerPos, out Vector2 uv)
    {
        uv = Vector2.zero;

        Vector3 localPoint = transform.InverseTransformPoint(fingerPos);
        float halfWidth = whiteboardCollider.bounds.size.x / 2f;
        float halfHeight = whiteboardCollider.bounds.size.y / 2f;

        uv.x = (localPoint.x + halfWidth) / (2f * halfWidth);
        uv.y = (localPoint.y + halfHeight) / (2f * halfHeight);

        return uv.x >= 0f && uv.x <= 1f && uv.y >= 0f && uv.y <= 1f;
    }
    public void DrawOnTexture(Vector2 uv)
    {
        int x = (int)(uv.x * whiteboardTexture.width);
        int y = (int)(uv.y * whiteboardTexture.height);

        whiteboardTexture.SetPixel(x, y, Color.black);
        whiteboardTexture.Apply();
    }
    public void InstantiateBrush(Vector3 position)
    {
        var brush = Instantiate(BrushPrefab, position + Vector3.up * 0.1f, Quaternion.identity, transform);
        brush.transform.localScale = Vector3.one * BrushSize;
    }
}

