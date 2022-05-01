using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct Note
{
    public int noteIndex;
    public float noteVolume;
    public float noteDuration;
    public float noteStart;

    public Note(int noteIndex, float pointOnPlaylist, float ticks, float volume = 1f){
        this.noteIndex = noteIndex;
        noteVolume = volume;
        noteDuration = ticks;
        noteStart = pointOnPlaylist;
    }

}
public struct Bus
{
    public float[] PCMBuffer;
    public AudioClip renderedClip;
    public float samples;

    public Bus(float[] buffer, AudioClip renderedClip, float samples)
    {
        PCMBuffer = buffer;
        this.renderedClip = renderedClip;
        this.samples = samples;
    }
}

/// <summary>
/// Player note "repo" that contains information of player's audio data and Note placements.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Notation : MonoBehaviour
{
    private FilterController filter_controller;

    public OrchestraMember MemberData;
    public AudioSource source;
    private float BPM;
    private float LengthOfSong;
    private int _sampleRate;
    private float[] PCMBuffer; 
    
    
    private Bus[] channels;
    private List<Note>[] channel_notes;
    public List<Note>[] RequestNotes() => channel_notes;

    void Awake()
    {
        BPM = 120;
        LengthOfSong = (60 / BPM) * 16;
        
        _sampleRate = 48000;
        PCMBuffer = new float[Mathf.CeilToInt(_sampleRate * LengthOfSong) * 2];
        Debug.LogFormat("Length: {0}, PCM Buffer Length {1}, Sample Rate {2}", LengthOfSong, PCMBuffer.Length, _sampleRate);
        
        channels = new Bus[MemberData.notes.Length];
        channel_notes = new List<Note>[MemberData.notes.Length];
        for(int i = 0; i < MemberData.notes.Length; i++){
            channels[i] = new Bus(new float[Mathf.CeilToInt(_sampleRate * LengthOfSong) * 2], MemberData.notes[i], _sampleRate);
            channel_notes[i] = new List<Note>();
        }
        
    }
    void Start()
    {
        
        source = GetComponent<AudioSource>();
        
        filter_controller = (FilterController) Resources.FindObjectsOfTypeAll(typeof(FilterController))[0];
        if(filter_controller.playing) source.Play();
        
        source.spatialBlend = 1f;
        
    }

    public void GenerateSampleData(float[] time_note = null)
    {
        Debug.Log("Sample data generated.");
        #region sample_data_creation
        for(int i = 0; i < MemberData.notes.Length; i++)
        {
            if(i > MemberData.notes.Length / 2) continue;
            //create new notes
            channel_notes[i].Add(new Note(i, UnityEngine.Random.Range(0f,0.9f), .5f));
            channel_notes[i].Add(new Note(i, UnityEngine.Random.Range(1f,1.9f), .5f));
            channel_notes[i].Add(new Note(i, UnityEngine.Random.Range(5,5.9f), .5f));
            

        }
        for(int i = 0; i < channels.Length; i++)
        {
            for(int j = 0; j < channel_notes[i].Count; j++)
                channels[i].PCMBuffer = 
                    Consolidate(channels[i].PCMBuffer, channel_notes[i][j].noteStart, channel_notes[i][j].noteVolume, channels[i].renderedClip);

        }
        #endregion
    }

    float[] Consolidate(float[] buffer, float time, float volume, AudioClip sample)
    {
        
        int startSample = Mathf.CeilToInt( time * _sampleRate ) * 2; // 2 channels because we're in stereo
		
		// Read the PCM data out of our sample.
		float[] sampleData = new float[sample.samples];
		sample.GetData( sampleData, 0 );

        for( int sampleIndex = 0; sampleIndex < sampleData.Length; sampleIndex++)
        {
			int targetIndex = startSample + sampleIndex;
			if( targetIndex >= buffer.Length ) {
				targetIndex -= buffer.Length;
			}

			buffer[targetIndex] += sampleData[sampleIndex] * volume;
        }
        return buffer;
    }

    public void Write(int location, float time, float volume, float length_of_note = 0.5f)
    {
        if(location > channels.Length) return;

        channel_notes[location].Add(new Note(location, time, length_of_note));
        Debug.LogFormat("{0}, {1}, {2}", new object[3]{location, time, length_of_note});
        channels[location].PCMBuffer = 
            Consolidate(channels[location].PCMBuffer, time, volume, channels[location].renderedClip);
    }

    public void PlayPart(float volume, float start_time)
    {
        Debug.Log("Playing Part!");
        
        
        source.volume = volume;
        source.Play();
        start_time = Time.time;
    }
    public void PausePart()
    {
        source.Pause();
    }


    float start_time= 0f;
    bool over;

    private void OnAudioFilterRead(float[] data, int channels)
    {
        // TODO: Move out to player script or its own script to respect SRP
        int currentPos = (int)AudioSettings.dspTime * _sampleRate;
        int nextSample = data.Length;
        for(int i = 0; i < data.Length; i++){

            //data[i] += PCMBuffer[i + s];
            for(int x = 0; x < this.channels.Length; x++){
                data[i] += this.channels[x].PCMBuffer[i + filter_controller.sample_offset];
            }
        }
        
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.E))
            GenerateSampleData();
        if(over){
            float finish = (Time.time - start_time);
            Debug.Log(finish);
            over = false;
            start_time = Time.time;
        }
    }

}
