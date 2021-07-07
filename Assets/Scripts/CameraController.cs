using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : GenericSingleton<CameraController>
{
    public float Width
    {
        get 
        { 
            Vector3 screenWidth = new Vector3(Screen.width, 0);
            return Camera.main.ScreenToWorldPoint(screenWidth).x * 2;
        }
    }
    public float Height
    {
        get
        {
            Vector3 screenHeight = new Vector3(Screen.height, 0);
            return Camera.main.ScreenToWorldPoint(screenHeight).y * 2;
        }
    }
}
