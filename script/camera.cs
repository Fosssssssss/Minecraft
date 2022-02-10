using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : ScriptableObject
{
    private float mouseHorizontal;
    private float mouseVertical;

    private void Update() {
        mouseUpdate();
    }

    private void mouseUpdate()
    {


        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");
        if (mouseVertical > 100)
            mouseVertical = 50;
        else
            mouseVertical = 30;

    }

}
