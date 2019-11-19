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
    public int tempo;
    float[] tune;
    float[] noteLengths;


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
        Melody();

        song = CreateToneAudioClip(tune);

        SOURCE.PlayOneShot(song);
    }


    private void Melody()
    {
        tune = new float[note_count];
        noteLengths = new float[note_count];
        List<string> keyList = new List<string>(NOTES.Keys);
        int note = Random.Range(0, NOTES.Count);

        for (int i = 0; i < note_count; i++)
        {           
            int noteStep = Random.Range(0, 100);
            int newLength = Random.Range(0, 100);
            float noteLength = 0;

            if (noteStep >= 20)
            {
                if (noteStep < 40)
                {
                    note -= 1;
                }

                else if (noteStep < 60)
                {
                    note += 1;
                }

                else if (noteStep < 75)
                {
                    note -= 2;
                }

                else if (noteStep < 90)
                {
                    note += 2;
                }

                else if (noteStep < 95)
                {
                    note -= 3;
                }

                else if (noteStep < 100)
                {
                    note += 3;
                }
            }

            if (newLength >= 30)
            {
                if (newLength < 60)
                {
                    noteLength = 0.5f;
                }

                else if (newLength < 85)
                {
                    noteLength = 1;
                }

                else if (newLength < 100)
                {
                    noteLength = 2;
                }
            }

            else
            {
                //silence note
                if (newLength < 15)
                {
                    noteLength = 0.5f;
                }

                else if (newLength < 25)
                {
                    noteLength = 1;
                }

                else if (newLength < 30)
                {
                    noteLength = 2;
                }
            }

            string key = keyList[note];
            tune[i] = NOTES[key];
            noteLengths[i] = noteLength;
        }
    }


    private AudioClip CreateToneAudioClip(float[] frequency)
    {
        int sampleDurationSecs = 60 / tempo;
        int sampleRate = 44100;
        int sampleLength = sampleRate * sampleDurationSecs * note_count;
        float maxValue = 1f / 4f;

        AudioClip audioClip = AudioClip.Create("song", sampleLength, 1, sampleRate, false);


        List<float> samples = new List<float>();

        for (int f = 0; f < frequency.Length; f++)
        {
            for (int i = 0; i < sampleRate * noteLengths[f]; i++)
            {
                float s = Mathf.Sin(2.0f * Mathf.PI * frequency[f] * ((float)i / (float)sampleRate));
                float v = s * maxValue;
                samples.Add(v);
            }
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
