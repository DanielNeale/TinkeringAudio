using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicGeneration : MonoBehaviour
{
    private AudioSource SOURCE;
    private AudioClip song;
    public TextAsset NOTES_CSV;
    private Dictionary<string, float> NOTES = new Dictionary<string, float>();

    private void Start()
    {
        SOURCE = GetComponent<AudioSource>();

        // Reference for frequency of musical notes
        // https://pages.mtu.edu/~suits/notefreqs.html

        string[] data = NOTES_CSV.text.Split(new char[] {'\n'});

        for (int i = 0; i < data.Length; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });

            if (row[0] != "")
            {
                string note_name = row[0];
                float note_freq = float.Parse(row[1]);

                NOTES.Add(note_name, note_freq);
            }           
        }
    }

    public void PlayMusic()
    {
        int note = Random.Range(0, NOTES.Count);
        Debug.Log(note);
        List<string> keyList = new List<string>(NOTES.Keys);        
        string key = keyList[note];
        Debug.Log(key);
        Debug.Log(NOTES[key]);

        song = CreateToneAudioClip(NOTES[key]);

        SOURCE.PlayOneShot(song);
    }

    private AudioClip CreateToneAudioClip(float frequency)
    {
        int sampleDurationSecs = 5;
        int sampleRate = 44100;
        int sampleLength = sampleRate * sampleDurationSecs;
        float maxValue = 1f / 4f;

        AudioClip audioClip = AudioClip.Create("song", sampleLength, 1, sampleRate, false);

        float[] samples = new float[sampleLength];

        for (int i = 0; i < sampleLength; i++)
        {
            float s = Mathf.Sin(2.0f * Mathf.PI * frequency * ((float)i / (float)sampleRate));
            float v = s * maxValue;
            samples[i] = v;
        }

        audioClip.SetData(samples, 0);
        return audioClip;
    }
}
