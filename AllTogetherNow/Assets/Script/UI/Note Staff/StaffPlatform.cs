using System;
using UnityEngine;
using System.Collections;

/// </summary>
/// Responsible for communicating players with the staff.
/// <summary>
public class StaffPlatform : MusicPlatform
{
    private StaffEditor writer;
    private StaffReader reader;
    public Action FinishInit;
    public void Awake()
    {
        
        writer = transform.parent.GetComponentInChildren<StaffEditor>();
        reader = GetComponent<StaffReader>();
        Debug.Log(writer);
        writer.FinishInit += () =>
        {
            writer.rowEdit += Write;
        };   
        
    }

    public void Start() => FinishInit?.Invoke();
    public override MusicPlatform Join(Player client)
    {
        Debug.Log(writer);
        writer.gameObject.SetActive(true);
        writer.Join(client);  
        return this;
    }

    public override void Write(Player player, int row, float start, float volume)
    {
        player.notation.Write(row, start, volume);
    }

    bool change = true;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && change)
        {
            float position = reader.currentPosition;
            base.Play?.Invoke(1f, position);
            change = false;
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            base.Pause?.Invoke();
            change = true;
        }
    }
}