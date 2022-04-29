using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Jobs;
using Unity.Collections;
public class ScrollEditor : MonoBehaviour, IHandleClick
{
    [SerializeField]
    private UIScroll scroller;
    private StaffReader staff_reader;
    public UnityAction<float> ScrollPositionChanged;
    public Vector2 beginning_position;

    public void Start()
    {
        staff_reader = GetComponentInParent<StaffReader>();
        scroller = GetComponent<UIScroll>();
        beginning_position = scroller.scroll.start;

    }
    public InputLockMode HandleClick(RaycastHit hit, Vector3 mousePosition, int button)
    {
        Vector3 localHitPoint = hit.collider.transform.InverseTransformPoint(hit.point);
        
        scroller.scroll.start = new Vector2(localHitPoint.x, scroller.scroll.start.y);
        return InputLockMode.Unlocked;
    }

    public InputLockMode HandleClickOff(RaycastHit hit, Vector3 mousePosition, int button)
    {
        Debug.Log("scroller is now on second: " + (scroller.scroll.start.x - beginning_position.x) / (0.0625f * 2));
        ScrollPositionChanged.Invoke((scroller.scroll.start.x - beginning_position.x) / (0.0625f * 2));

        return InputLockMode.Unlocked;
    }

    public InputLockMode HandlePersistentClick(RaycastHit hit, Vector3 mousePosition, int button)
    {
        Vector3 localHitPoint = hit.collider.transform.InverseTransformPoint(hit.point);
        
        scroller.scroll.start = new Vector2(localHitPoint.x, scroller.scroll.start.y);
        return InputLockMode.Unlocked;
    }

    bool paused = true;
    IEnumerator currentCoroutine;
    void Update()
    {
        C_MoveStaff();

    }

    private void C_MoveStaff()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentCoroutine != null)
            {

                StopCoroutine(currentCoroutine);

            }
            currentCoroutine = null;
            paused = !paused;

        }
    }

    void LateUpdate(){
        if(!paused){
            
            float time = (60f / staff_reader.BPM) * staff_reader.bars; 

            if(currentCoroutine == null){
                currentCoroutine = MoveScroll(scroller.scroll.start, staff_reader.staffLength, time);
                StartCoroutine(currentCoroutine);
            }
        }
    }

    IEnumerator MoveScroll(Vector2 position, Vector4 target, float time)
    {
        float modified_time = time - (position.x - beginning_position.x )/ (0.0625f * 2);
        Debug.Log("mtime: " + modified_time + " scroll start: " + scroller.scroll.start + " position " + position + " rect: " + staff_reader.frame.xMax );
        float t = 0f;
        float start_time = Time.time;
        float staff_end = staff_reader.frame.xMax;
        while(t <= modified_time)
        {
            
            t += Time.deltaTime;
            if(t >= modified_time){
                
                //resets back to beginning of staff
                position = beginning_position; 
                scroller.scroll.start = beginning_position;
                
                //reset time
                t = 0f;
                modified_time = time;
                Debug.Log(modified_time);
                float end_time = Time.time;
                Debug.Log("time= " + (end_time - start_time));
                start_time = Time.time;
            }

            LineLength scroll = scroller.scroll;
            float timeElapsed = t / modified_time;
            
            scroller.scroll.start = Vector3.Lerp(position, new Vector3(staff_end, scroll.start.y, 0), timeElapsed);
            
            yield return null;
        }

        yield return null;
    }

    public void HandleKeyAction(string Key, Vector3 mousePosition)
    {
        
    }
}

