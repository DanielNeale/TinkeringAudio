using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicGeneration : MonoBehaviour
{
    private AudioSource SOURCE;
    private AudioClip song;
    public TextAsset NOTES_TXT;
    private Dictionary<string, float> NOTES;
    private int line_num;

    private void Start()
    {
        SOURCE = GetComponent<AudioSource>();

        // Reference for frequency of musical notes
        // https://pages.mtu.edu/~suits/notefreqs.html

        for (int i = 0; i < NOTES_TXT.text.Length; i++)
        {

        }
    }

    public void PlayMusic()
    {
        int tone;

        song = CreateToneAudioClip(150);

        SOURCE.PlayOneShot(song);
    }

    private AudioClip CreateToneAudioClip(int frequency)
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
