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
    [SerializeField] AudioClip quizCompleted;
    [SerializeField] AudioClip rightOption;
    [SerializeField] AudioClip wrongOption;
    [SerializeField] AudioClip hintClue;
    [SerializeField] AudioClip startGame;
    
    public void PlayButtonSound()
    {
        sfx.PlayOneShot(buttonSound);
    }   
    public void PlayCoinSound()
    {
        sfx.PlayOneShot(coinSound);
    }   
    public void QuizCompelted(){
        sfx.PlayOneShot(quizCompleted);
    }
     public void TimeOver(){
        sfx.PlayOneShot(quizCompleted);
    }
    public void WrongOption(){
        sfx.PlayOneShot(wrongOption);
    }
    public void RightOption(){
        sfx.PlayOneShot(rightOption);
    }
    public void HintClue(){
        sfx.PlayOneShot(hintClue);
    }
    internal void StartGame()
    {
       sfx.PlayOneShot(startGame);
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
