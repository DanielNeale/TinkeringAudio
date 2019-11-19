using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicGeneration : MonoBehaviour
{
    private AudioSource SOURCE;
    private AudioClip song;
    public TextAsset NOTES_CSV;
    private Dictionary<string, float> NOTES = new Dictionary<string, float>();
    public int timeSignature;
    public int tempo;
    List<float> tune = new List<float>();
    List<float> noteLengths = new List<float>();


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
        tune.Clear();
        noteLengths.Clear();

        Melody();

        song = CreateToneAudioClip(tune);

        SOURCE.PlayOneShot(song);
    }


    private void Melody()
    {
        float timeLeft = timeSignature;
        List<string> keyList = new List<string>(NOTES.Keys);
        int note = Random.Range(7, 15);
        int i = 0;

        while (timeLeft > 0)
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

            string key = keyList[note];
            tune.Add(NOTES[key]);

            if (newLength >= 30)
            {
                if (newLength < 60)
                {
                    noteLength = 0.25f;
                }

                else if (newLength < 85)
                {
                    noteLength = 0.5f;
                }

                else if (newLength < 100)
                {
                    noteLength = 1;
                }
            }

            else
            {
                tune[i] = 0;

                if (newLength < 15)
                {
                    noteLength = 0.25f;
                }

                else if (newLength < 25)
                {
                    noteLength = 0.5f;
                }

                else if (newLength < 30)
                {
                    noteLength = 1;
                }
            }
           
            noteLengths.Add(noteLength);

            timeLeft -= noteLength;
            i++;
        }
    }

    private AudioClip CreateToneAudioClip(List<float> frequency)
    {
        int sampleDurationSecs = 60 / tempo;
        int sampleRate = 44100;
        int sampleLength = sampleRate * sampleDurationSecs * timeSignature;
        float maxValue = 1f / 4f;

        AudioClip audioClip = AudioClip.Create("song", sampleLength, 1, sampleRate, false);


        List<float> samples = new List<float>();

        for (int f = 0; f < frequency.Count; f++)
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
