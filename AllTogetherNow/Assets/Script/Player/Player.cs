using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IHandleClick
{

    public Notation notation;
    [SerializeField]
    private MusicPlatform staff;
    private Dictionary<string, Action> context_actions;
    void Start()
    {
        staff = (MusicPlatform) Resources.FindObjectsOfTypeAll(typeof(MusicPlatform))[0];        
        FilterController filter_controller = (FilterController) Resources.FindObjectsOfTypeAll(typeof(FilterController))[0];
        
        staff.Play += notation.PlayPart;
        staff.Pause += notation.PausePart;

        PlayerActions actions = new PlayerActions(this);
        context_actions = new Dictionary<string, Action>();
        context_actions.Add("m" , actions.Mute);

    }
    public InputLockMode HandleClick(RaycastHit hit, Vector3 mousePosition, int button)
    {
        staff.Join(this);
        return InputLockMode.Unlocked;
    }

    public InputLockMode HandlePersistentClick(RaycastHit hit, Vector3 mousePosition, int button)
    {
        return InputLockMode.Unlocked;
    }

    public InputLockMode HandleClickOff(RaycastHit hit, Vector3 mousePosition, int button)
    {
        return InputLockMode.Unlocked;
    }
    public void HandleKeyAction(string key, Vector3 mousePosition)
    {
        
        if(context_actions.ContainsKey(key))
            context_actions[key].Invoke();
        
    }

    public class PlayerActions
    {
        private Player player;
        
        private Dictionary<string, Action> context_actions;
        public PlayerActions(Player player) => this.player = player;

        public void Mute()
        {

            AudioSource player_source = player.notation.source;
            player_source.volume += player_source.volume > 0 ? -1 : 1;
            
        }
    }
}
