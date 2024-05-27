using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System;
using System.IO; 
using TMPro; 


public class FPSCounter : MonoBehaviour
{
    public  TextMeshProUGUI Message; // Reference to a UI Text element to display the FPS

    private float deltaTime = 0.0f;

    void Update()
    {
        // Calculate the delta time
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // Calculate the FPS
        float fps = 1.0f / deltaTime;

        
        Message.text = string.Format("{0:0.} FPS", fps);
        
    }
}
