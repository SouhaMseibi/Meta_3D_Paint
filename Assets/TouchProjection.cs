
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO; 
using TMPro; 
using Meta.XR.MRUtilityKit;

public class TouchProjection : MonoBehaviour
{
    public GameObject WhiteBoard;
    public OVRHand Righthand ; 
    public OVRSkeleton RighthandSkeleton ; 
    public OVRHand Lefthand ; 
    public OVRSkeleton LefthandSkeleton ; 
    public  TextMeshProUGUI Message;
    public Material meshMaterial ; 
    public  GameObject line;

    protected MeshFilter planeMeshFilter;
    protected  MeshRenderer planeMeshRenderer;
    protected float width = 0.003f ;
    protected float height = 0.003f ;
    protected float SeuilDistance  = 0.0266f ; 
    protected float SeuilMinPoints  = 0.000f ;
    protected float SeuilMaxPoints  = 0.007f ;
    protected Vector3 currentPoint = Vector3.zero ;
    protected Vector3 previousPoint = Vector3.zero ;
    protected Vector3 currentPointLeft = Vector3.zero ;
    protected Vector3 previousPointLeft = Vector3.zero ;
    protected LineRenderer lineRenderer; 
    protected float updateInterval = 0.014f ; 


    
    


    void Start()
    {
        planeMeshFilter = WhiteBoard.GetComponent<MeshFilter>();
        planeMeshRenderer = WhiteBoard.GetComponent<MeshRenderer>();
        InvokeRepeating("CustomUpdate", 0f, updateInterval);
    }
    void CustomUpdate ()
    {       
            if ( !Righthand ) Righthand = GetComponent<OVRHand>();
            if ( !Lefthand ) Lefthand = GetComponent<OVRHand>();
            if ( !RighthandSkeleton ) RighthandSkeleton = GetComponent<OVRSkeleton>();
            if ( !LefthandSkeleton ) LefthandSkeleton = GetComponent<OVRSkeleton>();
            if (( Righthand.IsTracked )  ) 
            {
                // List<Vector3> FingersPosRight , FingersPosLeft ; 
                // RightFingerTipsPos (  RighthandSkeleton, out FingersPosRight ) ;
                // LeftFingerTipsPos (  LefthandSkeleton, out FingersPosLeft ) ; 
                // Vector3 touchPoint = HandleInput(FingersPosRight , FingersPosLeft ) ;

                Vector3 touchPoint =Vector3.zero;
                foreach ( var bone in RighthandSkeleton.Bones)
                {
                    if ( bone.Id == OVRSkeleton.BoneId.Hand_IndexTip   ) 
                    {
                        touchPoint= bone.Transform.position;
                    }
                } 
                
                Vector3 projectedPoint = ProjectPointOntoPlane(touchPoint);
                
                if ( chechkDistance ( touchPoint , projectedPoint ))
                {
                    
                    Quaternion rotation = WhiteBoard.transform.rotation;
                    Vector3[] rectangleVertices = GetRectangleOnPlane(projectedPoint,rotation , width, height); // Example: 1x1 rectangle
                    CreateMeshRight(rectangleVertices , projectedPoint ) ;

                }

                           
            }
             if ( ( Lefthand.IsTracked ) ) 
            {

                Vector3 touchPointLeft =Vector3.zero;
                foreach ( var bone in LefthandSkeleton.Bones)
                {
                    if ( bone.Id == OVRSkeleton.BoneId.Hand_IndexTip   ) 
                    {
                        touchPointLeft= bone.Transform.position;
                    }
                } 
                
                Vector3 projectedPointLeft = ProjectPointOntoPlane(touchPointLeft);
                
                if ( chechkDistance ( touchPointLeft , projectedPointLeft ))
                {
                    
                    Quaternion rotation = WhiteBoard.transform.rotation;
                    Vector3[] rectangleVertices = GetRectangleOnPlane(projectedPointLeft,rotation , width, height); // Example: 1x1 rectangle
                    CreateMeshLeft(rectangleVertices , projectedPointLeft ) ;
                    
                }
                
                           
             }
    }

   
    bool chechkDistance ( Vector3 touchPoint  , Vector3  projectedPoint)
    {
        
        float distance = Vector3.Distance(touchPoint , projectedPoint);
        Message.text = "Distance "+ distance.ToString();
        if ( distance < SeuilDistance  )
        {
            return true ;
        }
        return false ; 
    }
    Vector3 ProjectPointOntoPlane(Vector3 point )
    {
        
        
        
        Vector3 normal =planeMeshFilter.transform.TransformDirection (planeMeshFilter.mesh.normals[0]); 
        Plane plane = new Plane(normal, WhiteBoard.transform.position);
       
        Vector3 projectedPoint = plane.ClosestPointOnPlane(point);
        return projectedPoint ; 
    }

    Vector3[] GetRectangleOnPlane(Vector3 center,  Quaternion rotation, float width, float height)
    {
        Vector3[] vertices = new Vector3[4];

        Vector3 up = WhiteBoard.transform.up;
        Vector3 right = WhiteBoard.transform.right;
        Vector3 forward = WhiteBoard.transform.forward;

        vertices[0] = center - right * width / 2 - up * height / 2; // Bottom-left
        vertices[1] = center + right * width / 2 - up * height / 2; // Bottom-right
        vertices[2] = center - right * width / 2 + up * height / 2; // Top-left
        vertices[3] = center + right * width / 2 + up * height / 2; // Top-right 

        return vertices;
    }

    void SetLineWidth( Vector3 vex_0 , Vector3 vex_1  , LineRenderer lineRenderer)
    {
        float initialWidth = Vector3.Distance(vex_0, vex_1);
        lineRenderer.startWidth = initialWidth;
        lineRenderer.endWidth = initialWidth;
    }
    
    void CreateMeshRight(Vector3[] vertices ,  Vector3 center  )
    {
        
        
       
        if ( previousPoint == Vector3.zero   )             // soit its the first points , soit there is no touch 
        {
            previousPoint = center ;  
        }

        currentPoint = center ;
        
        if (( Vector3.Distance ( previousPoint , currentPoint ) > SeuilMinPoints)  && ( Vector3.Distance ( previousPoint , currentPoint ) < SeuilMaxPoints))
        {

            GameObject lineObject = Instantiate(line);
            lineObject.SetActive(false);
            LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, previousPoint);
            lineRenderer.SetPosition(1, currentPoint);
            SetLineWidth(vertices[0] , vertices[2] , lineRenderer ) ;
            lineObject.SetActive(true);
        }
        
        
        
        previousPoint = currentPoint ; 
       
    }
    void CreateMeshLeft(Vector3[] vertices ,  Vector3 center  )
    {
        
        
       
        if ( previousPointLeft == Vector3.zero   )             // soit its the first points , soit there is no touch 
        {
            previousPointLeft = center ;  
        }

        currentPointLeft = center ;
        
        if (( Vector3.Distance ( previousPointLeft , currentPointLeft ) > SeuilMinPoints)  && ( Vector3.Distance ( previousPointLeft , currentPointLeft ) < SeuilMaxPoints))
        {
            
            GameObject lineObject = Instantiate(line);
            lineObject.SetActive(false);
            LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();
            
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, previousPointLeft);
            lineRenderer.SetPosition(1, currentPointLeft);
            SetLineWidth(vertices[0] , vertices[2] , lineRenderer ) ;
            lineObject.SetActive(true);
        }
         
        previousPointLeft = currentPointLeft ; 
       
    }



    public void  RightFingerTipsPos (  OVRSkeleton handSkeleton , out List<Vector3> FingersPosRight  )
    {
        FingersPosRight = new List<Vector3>();
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
    public void  LeftFingerTipsPos (OVRSkeleton handSkeleton , out List<Vector3> FingersPosLeft  )
    {
        FingersPosLeft = new List<Vector3>();
        foreach ( var bone in handSkeleton.Bones)
        {
            if ( bone.Id == OVRSkeleton.BoneId.Hand_ThumbTip   ) 
            {
                FingersPosLeft.Add(bone.Transform.position);
            }
            if ( bone.Id == OVRSkeleton.BoneId.Hand_IndexTip  ) 
            {
                FingersPosLeft.Add(bone.Transform.position);
            }
            if ( bone.Id == OVRSkeleton.BoneId.Hand_MiddleTip ) 
            {
                FingersPosLeft.Add(bone.Transform.position);
            }
            if ( bone.Id == OVRSkeleton.BoneId.Hand_RingTip) 
            {
                FingersPosLeft.Add(bone.Transform.position);
            }
            if ( bone.Id == OVRSkeleton.BoneId.Hand_PinkyTip ) 
            {
                FingersPosLeft.Add(bone.Transform.position);
            }
        }
            
        
    }
    

    // Vector3 HandleInput (List<Vector3> FingersPosRight , List<Vector3> FingersPosLeft  )
    // {
    //     Vector3 normal =planeMeshFilter.transform.TransformDirection (planeMeshFilter.mesh.normals[0]); 
    //     Plane plane = new Plane(normal, WhiteBoard.transform.position);

        
    //     float minimumDistance =0f;
    //     List<Vector3> rightfingerpos = new List<Vector3>() ;
    //     List<Vector3> leftfingerpos = new List<Vector3>() ;
        
        
    //     foreach ( var finger in FingersPosRight)
    //     {
    //         if ( plane.GetDistanceToPoint(finger) <= SeuilDistance )
    //         {
    //             rightfingerpos.Add(finger);
    //         }  
    //     }
    //     foreach ( var finger in FingersPosLeft)
    //     {
    //         if ( plane.GetDistanceToPoint(finger) <= SeuilDistance )
    //         {
    //             leftfingerpos.Add(finger);
    //         }  
    //     }

    //     if ( rightfingerpos.Count >= 1)
    //     {   
    //         return rightfingerpos[0]; 
    //         MessageTextDebug.text= "more than one right finger ";
    //     }
        
    //     if ( leftfingerpos.Count >= 1)
    //     { 
    //         return leftfingerpos[0]; 
    //         MessageTextDebug.text= "more than one left finger ";
    //     }
    //     return Vector3.zero ; 
     
    // }
   

}

