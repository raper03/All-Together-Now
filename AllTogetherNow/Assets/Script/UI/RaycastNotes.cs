using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IHandleClick
{
    public InputLockMode HandleClick(RaycastHit hit, Vector3 mousePosition, int button);
    public InputLockMode HandlePersistentClick(RaycastHit hit, Vector3 mousePosition, int button);
    public InputLockMode HandleClickOff(RaycastHit hit, Vector3 mousePosition, int button);
    public void HandleKeyAction(string Key, Vector3 mousePosition);
}
public enum InputLockMode
{
    Unlocked,
    Locked 
}

public class RaycastNotes : MonoBehaviour
{
    public Camera StaffCamera;
    public Camera StageCamera;

    private List<IHandleClick> SelectedObjects;
    private IHandleClick currentClicker;
    private Dictionary<InputLockMode, Action> lockActions;
    
    void Start()
    {
        lockActions = new Dictionary<InputLockMode, Action>(){
            {InputLockMode.Locked, LockItUp},
            {InputLockMode.Unlocked, OpenItUp}
        };
        StaffCamera = GetComponent<Camera>();    
        SelectedObjects = new List<IHandleClick>();
        
    }

    void LockItUp()
    {

    }
    void OpenItUp()
    {
        
    }
    void Update()
    {
        Ray StaffRay = StaffCamera.ScreenPointToRay(Input.mousePosition);
        Ray StageRay = StageCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(StaffRay, out hit, 1000f, LayerMask.GetMask("UI"), QueryTriggerInteraction.Ignore))
        {
            
            Debug.DrawLine(StaffRay.origin, hit.point, Color.red);
            if(hit.collider.TryGetComponent<IHandleClick>(out IHandleClick clicker)){
                HandleClicker(StaffRay, hit, clicker);
            }  
        }

        else if(Physics.Raycast(StageRay, out hit, 1000f, LayerMask.GetMask("OrchestraMember"), QueryTriggerInteraction.Ignore))
        {
            if(hit.collider.TryGetComponent<IHandleClick>(out IHandleClick clicker))
            {
                HandleClicker(StageRay, hit, clicker);
            }
        }

    }

        void HandleClicker(Ray ray, RaycastHit hit, IHandleClick clicker)
    {
        if(currentClicker != null) clicker = currentClicker;
        

        if(Input.GetMouseButtonDown(0))
        {
            clicker.HandleClick(hit, hit.point, 0);
            currentClicker = clicker;
        } else if(Input.GetMouseButtonDown(1))
        {
            clicker.HandleClick(hit, hit.point, 1);
            currentClicker = clicker;
        }

        if(Input.anyKeyDown){
            if(Input.inputString == "") return;
            clicker.HandleKeyAction(Input.inputString, hit.point);
        }

        if(currentClicker == null) return;
        
        if(Input.GetMouseButton(0))
        {
            clicker.HandlePersistentClick(hit, hit.point, 0);
        } else if(Input.GetMouseButton(1))
        {
            clicker.HandlePersistentClick(hit, hit.point, 1);
        }

        if(Input.GetMouseButtonUp(0))
        {
            clicker.HandleClickOff(hit, hit.point, 0);
            currentClicker = null;
        } else if(Input.GetMouseButtonUp(1))
        {
            clicker.HandleClickOff(hit, hit.point, 1);
            currentClicker = null;
        }
    }

}
