using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Abstract class to set strategies for objects to subscribe to.
/// </summary>
public abstract class Platform<T, V> : MonoBehaviour
{
    public abstract T Join(V client);
}

public abstract class MusicPlatform : Platform<MusicPlatform, Player>
{
    public abstract void Write(Player player, int row, float start, float volume);

    public UnityAction<int, float> StaffPositionChange;
    public UnityAction<float, float> Play;
    public UnityAction Pause;
    public UnityAction Replay;

}

