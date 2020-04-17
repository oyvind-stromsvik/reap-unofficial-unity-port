using UnityEngine;
using System.Collections.Generic;

public class SoundController : MonoBehaviour {

    public static SoundController instance;

    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;
    public AudioClip[] audioClips;

    Dictionary<string, AudioSource> soundPlayers;

    void Awake() {
        instance = this;
    }
    
    void Start() {
        // Initialize our sound players.
        // Create an audio source for each of our audio clips so we can control
        // them independently.
        soundPlayers = new Dictionary<string, AudioSource>();
        foreach (AudioClip clip in audioClips) {
            GameObject newObject = new GameObject();
            newObject.AddComponent<AudioSource>();
            newObject.GetComponent<AudioSource>().playOnAwake = false;
            newObject.GetComponent<AudioSource>().clip = clip;
            newObject.transform.SetParent(transform);
            newObject.name = clip.name + "_soundPlayer";
            soundPlayers.Add(clip.name, newObject.GetComponent<AudioSource>());
        }
    }

    public void PlaySound(string name, float volume = 1) {
        // Don't play the sound if this sound is currently being played.
        if (!soundPlayers[name].isPlaying) {
            soundPlayers[name].pitch = Random.Range(minPitch, maxPitch);
            soundPlayers[name].volume = volume;
            soundPlayers[name].Play();
        }
    }
}
