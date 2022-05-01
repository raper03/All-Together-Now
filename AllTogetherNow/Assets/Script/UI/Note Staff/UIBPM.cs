using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
[ExecuteAlways] public class UIBPM : ImmediateModeShapeDrawer
{
    public Transform crosshairTransform;
    public Vector3 BPMFloatPosition;
    public Vector3 BPMTextPosition;
    
    public float BPM;

    public override void DrawShapes(Camera cam)
    {
        using ( Draw.Command(cam) )
        {
            Draw.FontSize = 0.2f;
            Draw.Text(BPMTextPosition ,"BPM", TextAlign.Center);
            Draw.Text(BPMFloatPosition, BPM.ToString(), TextAlign.Right);
        }
    }
}
