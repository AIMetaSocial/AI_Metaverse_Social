using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EditProfilePanel : MonoBehaviour
{
    #region Singleton
    public static EditProfilePanel Instance;
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
    [SerializeField] CanvasGroup editProfilePanel;
    [SerializeField] int selectedGender =0;
    [SerializeField] GameObject[] genderSelectedIcons;
    [SerializeField] TMP_InputField name_input;

    public static event Action OnProfileEdited;
    
    public void OpenEditProfilePanel(){


        
        selectedGender = DatabaseManager.Instance.GetLocalData().selectedGender;
        name_input.text = DatabaseManager.Instance.GetLocalData().playername;

        SetSpriteForSelectedGender();
        LeanTweenExtension.OpenPopupWithAlpha(editProfilePanel);
    }

    private void SetSpriteForSelectedGender()
    {
        for (int i = 0; i < genderSelectedIcons.Length; i++)
        {
            genderSelectedIcons[i].SetActive(i == selectedGender);
        }
    }

    public void CloseEditProfilePanel(){
        LeanTweenExtension.ClosePopupWithAlpha(editProfilePanel);
    }
    public void SelectGender(int _selectedGender){
        selectedGender = _selectedGender;
        SetSpriteForSelectedGender();
    }

    public void SaveSettings(){

        string _name = name_input.text;
        if(string.IsNullOrEmpty(_name.Trim())){
            MessageBox.Instance?.ShowMessage("Enter Valid Name!");
            return;
        }
        

        LocalData data = DatabaseManager.Instance.GetLocalData();
        data.playername = _name;
        data.selectedGender = selectedGender;
        DatabaseManager.Instance.UpdateData(data);



        CloseEditProfilePanel();

        OnProfileEdited?.Invoke(); 
    }
}
