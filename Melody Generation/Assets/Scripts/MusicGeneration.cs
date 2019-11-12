using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicGeneration : MonoBehaviour
{
    private AudioSource SOURCE;
    private AudioClip song;
    public TextAsset NOTES_CSV;
    private Dictionary<string, float> NOTES = new Dictionary<string, float>();
    public int note_count;
    float[] tune;


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
        tune = new float[note_count];

        for (int i = 0; i < note_count; i++)
        {
            int note = Random.Range(0, NOTES.Count);
            List<string> keyList = new List<string>(NOTES.Keys);
            string key = keyList[note];
            tune[i] = NOTES[key];           
        }        

        song = CreateToneAudioClip(tune);

        SOURCE.PlayOneShot(song);
    }


    private AudioClip CreateToneAudioClip(float[] frequency)
    {
        int sampleDurationSecs = 1;
        int sampleRate = 44100;
        int sampleLength = sampleRate * sampleDurationSecs * note_count;
        float maxValue = 1f / 4f;

        AudioClip audioClip = AudioClip.Create("song", sampleLength, 1, sampleRate, false);


        List<float> samples = new List<float>();

        for (int f = 0; f < frequency.Length; f++)
        {
            for (int i = 0; i < sampleRate; i++)
            {
                float s = Mathf.Sin(2.0f * Mathf.PI * frequency[f] * ((float)i / (float)sampleRate));
                float v = s * maxValue;
                samples.Add(v);
            }
            Debug.Log(frequency[f]);
        }

        float[] new_samples = new float[samples.Count];
        
        for (int s = 0; s < new_samples.Length; s++)
        {
            new_samples[s] = samples[s];
        }

        audioClip.SetData(new_samples, 0);
        return audioClip;
    }
}
