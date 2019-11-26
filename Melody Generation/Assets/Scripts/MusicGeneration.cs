using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicGeneration : MonoBehaviour
{
    private AudioSource source;
    private AudioClip song;
    public TextAsset notesCsv;
    private Dictionary<string, float> notes = new Dictionary<string, float>();
    public int noteCount;
    List<float> tune = new List<float>();
    List<float> noteLengths = new List<float>();

    
    /// <summary>
    /// Sets the audio source and puts all of the notes from the csv file into
    /// a dictionary to be used by the program
    /// </summary>
    private void Start()
    {     
        source = GetComponent<AudioSource>();

        // Reference for frequency of musical notes
        // https://pages.mtu.edu/~suits/notefreqs.html

        string[] data = notesCsv.text.Split(new char[] {'\n'});

        for (int i = 0; i < data.Length; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });

            if (row[0] != "")
            {
                string note_name = row[0];
                float note_freq = float.Parse(row[1]);

                notes.Add(note_name, note_freq);
            }           
        }
    }


    /// <summary>
    /// Called when the button is pressed. Runs through all the setup 
    /// and then plays the tune
    /// </summary>
    public void PlayMusic()
    {
        tune.Clear();
        noteLengths.Clear();

        Melody();

        song = CreateAudioClip(tune);

        source.PlayOneShot(song);
    }


    /// <summary>
    /// Randomly generates a tune the length of noteCount. It's designed to
    /// more likely choose sequencial notes however will have a lower chance
    /// to generate notes further away from the previous to generate more
    /// interesting melodies. Will also generate a random length for each
    /// note and will also include breaks
    /// </summary>
    private void Melody()
    {
        float timeLeft = noteCount;
        List<string> keyList = new List<string>(notes.Keys);
        int note = Random.Range(7, 15);
        int i = 0;

        while (timeLeft > 0)
        {   
            // Genartes percantage chance for next note/length
            int noteStep = Random.Range(0, 100);
            int newLength = Random.Range(0, 100);
            float noteLength = 0;

            // Chooses next note length based on percentage
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
            tune.Add(notes[key]);

            // Generates note length based on percentage
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

            // Has chance to generate a break instead
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

            timeLeft -= noteLength;

            // Rounds up length of last note if it goes past length of
            // the audio clip
            if (timeLeft < 0)
            {
                noteLength += timeLeft;
                noteLengths.Add(noteLength);
            }

            else
            {
                noteLengths.Add(noteLength);
            }

            i++;
        }
    }


    /// <summary>
    /// Takes the melody generated above and creates a sound file to play it
    /// </summary>
    private AudioClip CreateAudioClip(List<float> frequency)
    {
        float sampleDurationSecs = 1f;
        int sampleRate = 44100;
        int sampleLength = Mathf.RoundToInt(sampleRate * sampleDurationSecs * noteCount);
        Debug.Log(sampleLength);
        float maxValue = 1f / 4f;

        AudioClip audioClip = AudioClip.Create("song", sampleLength, 1, sampleRate, false);

        List<float> samples = new List<float>();

        // Simulates audio waves
        for (int f = 0; f < frequency.Count; f++)
        {
            for (int i = 0; i < sampleRate * noteLengths[f]; i++)
            {
                float s = Mathf.Sin(2.0f * Mathf.PI * frequency[f] * ((float)i / (float)sampleRate));
                float v = s * maxValue;
                samples.Add(v);
            }
        }

        // Samples are put into a list and then converted into a array as the
        // audio clip only takes an array however it is much easier to put
        // the frequencies into a list
        float[] newSamples = new float[samples.Count];
        
        for (int s = 0; s < newSamples.Length; s++)
        {
            newSamples[s] = samples[s];
        }

        audioClip.SetData(newSamples, 0);
        return audioClip;
    }
}
