using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    #region Singleton
    public static SettingsPanel Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion
    [SerializeField] CanvasGroup settingsPanel;
    [SerializeField] Slider sfx_slider;
    [SerializeField] Slider music_slider;
    public void OpenSettings(){
        LeanTweenExtension.OpenPopupWithAlpha(settingsPanel);
    }
    public void CloseSettings(){
        LeanTweenExtension.ClosePopupWithAlpha(settingsPanel);
    }
    
    public void OnSliderValueChanged_SFX(){

        float music = music_slider.value;
        float sfx = sfx_slider.value;

        AudioManager.Instance?.SetVolumes(music,sfx);
    }
    

}
