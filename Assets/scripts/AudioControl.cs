using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour
{
    public AudioSource clip;
    public AudioMixerGroup pitchBend; 
    /*only changing "pitch" of an AudioSource changes both speed and frequency of sound
    operating on Pitch Shifter with an audio mixer corrects for frequency such that only tempo changes*/
    public Piece piece;

    public void Start()
    {
        clip.pitch = 0.75f; //start at slow tempo
        pitchBend.audioMixer.SetFloat("Pitch", 1.333f);
        clip.Play(); //on loop
    }

    public void setTempo(float newPitch) {
        clip.pitch = newPitch; 
        pitchBend.audioMixer.SetFloat("Pitch", 1f / newPitch);
    }
}
