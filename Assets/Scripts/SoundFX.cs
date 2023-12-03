using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundFX : MonoBehaviour {

    public List<AudioClip> clips = new List<AudioClip>();
    public List<Sound> sounds = new List<Sound>();

    public static SoundFX instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        SetAudioClips();
    }

    private void SetAudioClips() {
        for (int i = 0; i < clips.Count; i++) {
            sounds[i].clip = clips[i];
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = clips[i];
            sounds[i].audioSource = audioSource;
        }
    }

    public void PlaySound(string clipName) {
        sounds.Where(s => s.name == clipName).SingleOrDefault().audioSource.Play();
    }

    public void Play3DSound() {
        //Spawn a sound in world position
    }

}

[System.Serializable]
public class Sound {
    public string name;
    public float volume = 1f;
    public float pitch = 1f;
    public AudioClip clip;
    [HideInInspector] public AudioSource audioSource;
}