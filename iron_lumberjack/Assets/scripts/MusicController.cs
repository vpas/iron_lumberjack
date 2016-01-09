using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {
	public AudioSource musicAudioSource;
	public AudioSource deathAudioSource;

	Object[] myMusic;
	Object[] deathSounds;

	void Awake () {
		myMusic = Resources.LoadAll("Audio/Music", typeof(AudioClip));
		deathSounds = Resources.LoadAll("Audio/Death", typeof(AudioClip));
		Debug.Log (System.Environment.CurrentDirectory);
		playRandomMusic();
	}

	void Start () {
		musicAudioSource.Play(); 
	}

	// Update is called once per frame
	void Update () {
		if (!musicAudioSource.isPlaying) {
			playRandomMusic();
		}
	}

	void playRandomMusic() {
		musicAudioSource.clip = myMusic[Random.Range(0, myMusic.Length)] as AudioClip;
		musicAudioSource.Play();
	}

	public void playDeathSound() {
		deathAudioSource.clip = deathSounds[Random.Range(0, deathSounds.Length)] as AudioClip;
		deathAudioSource.Play();
	}
}
