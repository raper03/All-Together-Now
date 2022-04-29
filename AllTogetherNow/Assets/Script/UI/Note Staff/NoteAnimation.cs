using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class NoteEvent : UnityEvent<Vector3, UINote>
{}
public class NoteAnimation : MonoBehaviour, IHandleClick
{
    public UnityEvent<Vector3, UINote> NoteAdded;
    public UnityEvent<Vector3, UINote> NoteEditing;
    public UnityEvent<Vector3, UINote> PreNoteEdited;

    public UnityEvent<Vector3, UINote> NoteDeletion;

    public InputLockMode HandleClick(RaycastHit hit, Vector3 mousePosition, int button)
    {
        Vector3 localHitPoint = hit.collider.transform.InverseTransformPoint(hit.point);
        
        UnityEvent<Vector3, UINote> eventToInvoke = button < 1 ? PreNoteEdited : NoteDeletion;

        eventToInvoke.Invoke(localHitPoint, hit.collider.GetComponent<UINote>());
        return InputLockMode.Unlocked;
    }

    public InputLockMode HandleClickOff(RaycastHit hit, Vector3 mousePosition, int button)
    {
        Vector3 localHitPoint = hit.collider.transform.InverseTransformPoint(hit.point);
        UnityEvent<Vector3, UINote> eventToInvoke = button < 1 ? NoteAdded: NoteDeletion;
        eventToInvoke?.Invoke(localHitPoint, hit.collider.GetComponent<UINote>());
        
        return InputLockMode.Unlocked;
    }

    public void HandleKeyAction(string Key, Vector3 mousePosition)
    {
        
    }

    public InputLockMode HandlePersistentClick(RaycastHit hit, Vector3 mousePosition, int button)
    {
        Vector3 localHitPoint = hit.collider.transform.InverseTransformPoint(hit.point);
        UnityEvent<Vector3, UINote> eventToInvoke = button < 1 ? NoteEditing : NoteDeletion;

        eventToInvoke.Invoke(localHitPoint, hit.collider.GetComponent<UINote>());
        return InputLockMode.Unlocked;
    }

   

}

