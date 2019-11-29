// Melody Generation
// Written by Daniel Neale

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
    List<int> tune = new List<int>();
    List<float> noteLengths = new List<float>();
    public int[] progression;

    
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

        // Runs through each line of the CSV file and splits up the text into
        // the note name (dictionary key) and frequency (dictionary value)
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

        song = CreateAudioClip();

        source.PlayOneShot(song);
    }

    public void Save()
    {
        SavWav.Save("Song", song);
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
        int note = Random.Range(7, 15);
        int i = 0;

        while (timeLeft > 0)
        {
            // Genartes percantage chance for next note/length, if note is at the
            // end of the dictionary it moves the tune in the other direction
            int noteStep;
            int newLength;

            if (note <= 5)
            {
                noteStep = Random.Range(61, 100);
            }

            else if (note >= 15)
            {
                noteStep = Random.Range(21, 60);
            }

            else
            {
                noteStep = Random.Range(0, 100);
            }
           
            newLength = Random.Range(0, 100);
            float noteLength = 0;

            // Chooses next note length based on percentage
            if (noteStep >= 20)
            {
                if (noteStep < 40)
                {
                    note -= 1;
                }

                else if (noteStep < 55)
                {
                    note -= 2;
                }

                else if (noteStep < 60)
                {
                    note -= 3;
                }

                else if (noteStep < 80)
                {
                    note += 1;
                }

                else if (noteStep < 95)
                {
                    note += 2;
                }

                else if (noteStep < 100)
                {
                    note += 3;
                }
            }

            tune.Add(note);

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
                tune[i] = -1;

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
    private AudioClip CreateAudioClip()
    {
        float sampleDurationSecs = 1f;
        int sampleRate = 44100;
        int sampleLength = Mathf.RoundToInt(sampleRate * sampleDurationSecs * noteCount * progression.Length);
        float volume = 0.25f;
        List<string> keyList = new List<string>(notes.Keys);

        AudioClip audioClip = AudioClip.Create("song", sampleLength, 1, sampleRate, false);

        List<float> tuneSamples = new List<float>();
        List<float> bassSamples = new List<float>();

        // Baseline
        int[] bassline = new int[progression.Length];

        for (int b = 0; b < bassline.Length; b++)
        {
            bassline[b] = tune[0] + progression[b];
        }

        // Creates waves from the frequencies
        for (int p = 0; p < progression.Length; p++)
        {
            for (int f = 0; f < tune.Count; f++)
            {
                if (tune[f] != -1)
                {
                    tune[f] += progression[p];
                }
            }

            for (int f = 0; f < tune.Count; f++)
            {
                // Gets frequency from notes dictionary
                float frequency;

                if (tune[f] == -1)
                {
                    frequency = 0;
                }

                else
                {
                    string key = keyList[tune[f]];
                    frequency = notes[key];
                }

                for (int i = 0; i < sampleRate * noteLengths[f]; i++)
                {                  
                    float s = Mathf.Sin(2.0f * Mathf.PI * frequency * ((float)i / (float)sampleRate));
                    float v = s * volume;
                    tuneSamples.Add(v);
                }
            }            
        }

        // Turns the bassline into waves
        for (int b = 0; b < bassline.Length; b++)
        {
            // Gets frequency from notes dictionary
            float frequency;
            string key = keyList[bassline[b]];
            frequency = notes[key];
            
            for (int i = 0; i < sampleRate * noteCount; i++)
            {
                float s = Mathf.Sin(2.0f * Mathf.PI * frequency * ((float)i / (float)sampleRate));
                float v = s * volume;
                bassSamples.Add(v);
            }
        }

        // Samples are put into a list and then converted into a array as the
        // audio clip only takes an array however it is much easier to put
        // the frequencies into a list
        float[] newSamples = new float[tuneSamples.Count];
        
        for (int s = 0; s < newSamples.Length; s++)
        {
            newSamples[s] = (tuneSamples[s] + bassSamples[s]) / 2;
        }

        audioClip.SetData(newSamples, 0);
        return audioClip;
    }
}
