using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageBox : MonoBehaviour
{
    #region  Singleton
    public static MessageBox Instance;
    private void Awake() {
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(this.gameObject);
        }
    }
    #endregion


    [SerializeField] CanvasGroup messagePanel;
    [SerializeField] TMP_Text messageText;
    [SerializeField] GameObject okBTN;
    public void ShowMessage(string _msg,bool showBTN = true){
        messageText.text = _msg;        
        LeanTweenExtension.OpenPopupWithAlpha(messagePanel);
        okBTN.SetActive(showBTN);
    }

    public void OkBTN(){
        LeanTweenExtension.ClosePopupWithAlpha(messagePanel);
    }

    

}
