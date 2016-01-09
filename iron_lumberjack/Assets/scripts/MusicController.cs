using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {
	public AudioSource musicAudioSource;

	Object[] myMusic;

	void Awake () {
		myMusic = Resources.LoadAll("Audio/Music", typeof(AudioClip));
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
}
