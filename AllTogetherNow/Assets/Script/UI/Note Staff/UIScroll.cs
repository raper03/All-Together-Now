using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Events;
using Shapes;
using System;
using System.Collections;

[ExecuteAlways] public class UIScroll : ImmediateModeShapeDrawer
{
    public LineLength scroll;
    public Transform crosshairTransform;
    
    public override void DrawShapes(Camera cam)
    {
        using(Draw.Command(cam)){
            Draw.ZTest = CompareFunction.Always; 
            Draw.Matrix = crosshairTransform.localToWorldMatrix;
			
			Draw.LineGeometry = LineGeometry.Flat2D;
			Draw.ThicknessSpace = ThicknessSpace.Pixels;
			Draw.Thickness = scroll.thickness; 
            Draw.LineEndCaps = LineEndCap.Round;

            scroll.end.x = scroll.start.x;

            Draw.Line(scroll.start, scroll.end, Color.white);
            
        }
    }

}