using System;
using UnityEngine;
using UnityEditor;
[CreateAssetMenu(fileName = "Member", menuName = "Orchestra/Member", order = 1)]
[Serializable]
[ExecuteInEditMode]
public class OrchestraMember : ScriptableObject
{
    [Header("Note Audio Clips")]
    private const int SIZE = 9;
    public AudioClip[] notes = new AudioClip[SIZE];
    void OnValidate()
    {
        if(notes.Length > SIZE)
            Array.Resize(ref notes, SIZE);
    }
    [Header("Player Settings")]
    public string Instrument;

}
