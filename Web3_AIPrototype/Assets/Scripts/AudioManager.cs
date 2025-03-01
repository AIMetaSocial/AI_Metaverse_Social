using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioSource sfx;
    [SerializeField] AudioSource bg_Music;
    [SerializeField] AudioMixer audioMixer;
    
    public void Awake()
    {
        if (Instance == null){
            Instance = this;
             SetVolumes(PlayerPrefs.GetFloat("music",1),PlayerPrefs.GetFloat("sfx",1));
             DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }       
    }

    [SerializeField] AudioClip buttonSound;
    [SerializeField] AudioClip coinSound;    
    [SerializeField] AudioClip hitSound; 
    public void PlayButtonSound()
    {
        sfx.PlayOneShot(buttonSound);
    }   
    public void PlayCoinSound()
    {
        sfx.PlayOneShot(coinSound);
    }   
   
    internal void PlayHitSound()
    {
        sfx.PlayOneShot(hitSound);
    }
   
  

    public void SetVolumes(float _music,float _sfx)
    {
        float musicValue = _music;
        float soundFx = _sfx;        


        PlayerPrefs.SetFloat("music",_music);
        PlayerPrefs.SetFloat("sfx",_sfx);


        audioMixer.SetFloat("Music", Mathf.Log10(musicValue) * 20);
        audioMixer.SetFloat("Sound fx", Mathf.Log10(soundFx) * 20);
    }

   
}
