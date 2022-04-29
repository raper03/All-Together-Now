using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Keeps track of current sample offset time and playing status, 
/// for instantiated instruments that need to catch up
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class FilterController : MonoBehaviour 
{
    
    public int sample_offset {get; private set;}
    public int buffer_length {get; private set;}
    public int bpm {get; private set;}
    public int bars {get; private set;}
    private float[] dummy_PCMBuffer;    
    private AudioSource source;
    public bool playing;


    public void Start() 
    {

        sample_offset = 0;
        source = GetComponent<AudioSource>();
        playing = false;

        // begin OnAudioFilterRead on play
        MusicPlatform staff = transform.parent.GetComponentInChildren<MusicPlatform>();
        staff.Play += PlayInternal;
        staff.Pause += () =>
        {
            source.Pause();
            playing = false;
        };

        // initializing dummy buffer
        StaffReader reader = transform.parent.GetComponentInChildren<StaffReader>();
        float length = (float)((60f / reader.BPM) * reader.bars);
        if(staff != null && reader != null ){            
            Debug.LogFormat("BPM: {0}, BARS: {1}, LENGTH: {2}", reader.BPM, reader.bars, length);
        }

        float _sampleRate = 48000;
        float _songLength = (float)((60 / reader.BPM) * reader.bars);
        buffer_length = Mathf.CeilToInt( _sampleRate * length) * 2;
        Debug.Log("buffer_length: " + buffer_length);
        dummy_PCMBuffer = new float[buffer_length];
        
    }
    public void PlayInternal(float volume, float position)
    {
        Debug.Log("beginning playback");
        source.volume = volume;
        source.Play();
        playing = true;
    }
    void OnAudioFilterRead(float[] data, int channels)
    {
        if(!playing) return;

        for(int i = 0; i <= data.Length; i++)
        {

            if(i + sample_offset >= buffer_length){ 
                sample_offset = 0;
                Debug.Log("over");
            }
        }
        
        sample_offset += data.Length;
    }
}