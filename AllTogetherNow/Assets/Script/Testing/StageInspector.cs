using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script performs a stress test for All Together, Now:
///     <para>Checks for existing members in scene,</para>
///     <para>Generates sample data for each member,</para>
///     <para>Subscribes them to the staff platform to allow play,</para>
/// </summary>
public class StageInspector : MonoBehaviour
{
    private Player[] existing_players;
    private float startTime;
    private float endTime;
    
    void Awake()
    {

    }
    void Start()
    {
        startTime = Time.realtimeSinceStartup;
        existing_players = GameObject.FindObjectsOfType<Player>();   

        
        foreach(Player player in existing_players)
        {
            player.notation.GenerateSampleData();
        }
    

        GameObject.FindObjectsOfType<StaffPlatform>()[0].FinishInit += () =>
        {
            foreach(Player player in existing_players)
            {
                player.HandleClick(new RaycastHit(), Vector3.zero, 0);
            }
        };
        endTime = Time.realtimeSinceStartup;

        Debug.LogFormat("STRESS TEST: Took {0} seconds to load {1} players \n\t  The size of {2} orchestras.", 
                        endTime - startTime, existing_players.Length, (float)(existing_players.Length / 50));
    }



}
