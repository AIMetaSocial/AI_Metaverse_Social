using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] TMP_Text welcomeText;
    // Start is called before the first frame update
    void Start()
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        if(data.playername ==""){
            EditProfilePanel.Instance?.OpenEditProfilePanel();
        }
        welcomeText.text  ="Welcome, " + LoginManager.address;
    }

   
}
