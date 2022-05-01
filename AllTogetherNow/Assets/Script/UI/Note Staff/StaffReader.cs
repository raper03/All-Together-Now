using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Reads essential information from the "Staff View"
/// </summary>
public class StaffReader : MonoBehaviour
{
    
    private StaffEditor staff;
    private ScrollEditor scroll;
    

    public const int TOTALNOTES = 70;

    [HideInInspector]
    public int BPM = 120;
    [HideInInspector]
    public int bars = 16;
    public float currentPosition {get; private set;}
    public Vector4 staffLength;
    public Rect frame;
    public Action FinishInit;
    
    public void Awake()
    {

        staff = GetComponent<StaffEditor>();
        scroll = GetComponentInChildren<ScrollEditor>();
        staff.FinishInit += () =>
        {
            BPM = 120;
            bars = 16;
            scroll.ScrollPositionChanged += (float position) => currentPosition = position;
            staffLength = staff.frame_size;
            frame = staff.actual_frame_size;
            FinishInit?.Invoke();
        };


    }

   
}