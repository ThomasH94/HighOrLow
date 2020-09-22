using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The purpose of this class is to have a singleton that any class can call to play a sound or music
/// Singletons may not be best practice, but in this case, it's OK
/// </summary>
namespace HighOrLow
{
	public class SoundManager : MonoBehaviour 
	{
		public static SoundManager Instance = null;
		
		[Header("Audio Sources")]
		public AudioSource sfxSource;
		public AudioSource musicSource;

		[Header("Sound Modifiers")]
		public float lowPitchRange = 0.95f;
		public float highPitchRange = 1.05f;

		void Awake()
		{
			if(Instance == null)
			{
				Instance = this;
			}
			else if(Instance != this)
			{
				Destroy(gameObject);
			}
			
		}

		void Start()
		{
			if(musicSource.clip != null)
			PlayMusic(musicSource.clip);
		}

		public void PlaySingleClip(AudioClip clip)
		{
			sfxSource.clip = clip;
			sfxSource.PlayOneShot(clip);
		}

		public void PlayRandomSound(params AudioClip[] clips)
		{
			int randomSoundIndex = Random.Range(0, clips.Length);
			float randomPitch = Random.Range(lowPitchRange, highPitchRange);

			sfxSource.pitch = randomPitch;
			sfxSource.clip = clips[randomSoundIndex];

			sfxSource.Play();

		}

		public void PlayMusic(AudioClip musicClip)
		{
			musicSource.clip = musicClip;
			musicSource.Play();
		}

		public void PauseMusic()
		{
			musicSource.Pause();
		}
	}		
}
