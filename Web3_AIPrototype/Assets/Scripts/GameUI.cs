using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;
    private void Awake() {
        Instance = this;
    }

    [SerializeField] TMP_Text[] coinText;
    [SerializeField] TMP_Text[] tokenText;


    
    [Header("PROMT FOR TRIGGER AI")]
    [SerializeField] GameObject PromptPanelForAIChat;
    [SerializeField] TMP_Text aiPersonName;    
    [SerializeField] TMP_Text costText;
    [SerializeField] AI_GeneratorPerson lastTriggeredAIPlayer;

    [Header("AI CHAT UI")]
    [SerializeField] GameObject chatUI;
    [SerializeField] AIChatManager aIChatManager;
    internal void PlayerInteractionWithAIPlayer(AI_GeneratorPerson aI_GeneratorPerson,bool triggered){

        aiPersonName.text = "Do You Want To Chat With " + aI_GeneratorPerson.aiPlayerName+"?";
        costText.text = aI_GeneratorPerson.chatCost.ToString();
        lastTriggeredAIPlayer = aI_GeneratorPerson;
        PromptPanelForAIChat.SetActive(triggered);
    }
    public void ChatWithAI(){
        
        if(lastTriggeredAIPlayer == null) return;
        
        //TODO
        //GET DATA AND COMPARE IF IT HAS ENOUGH COINS
        //if(lastTriggeredAIPlayer.chatCost>data.coins) return;
        //Deduct Doins

        aIChatManager.Init(lastTriggeredAIPlayer);
        lastTriggeredAIPlayer.ToggleCamera(true);
        chatUI.SetActive(true);        
        
        CloseAIPrompt();
        
    }
     public void CloseAIChat(){
        
        
        lastTriggeredAIPlayer.ToggleCamera(false);
        chatUI.SetActive(false);                
        
        
    }
    public void CloseAIPrompt(){
        PromptPanelForAIChat.SetActive(false);   
    }

    #region Menu Panel
    [SerializeField] CanvasGroup menuPanel;
    public void OpenMenuPanel(){
        LeanTweenExtension.OpenPopupWithAlpha(menuPanel);
    }    
    public void CloseMenuPanel(){
        LeanTweenExtension.ClosePopupWithAlpha(menuPanel);
    }    

    public void MainMenu(){

    }
    public void OpenSettings(){
        SettingsPanel.Instance?.OpenSettings();
    }
    public void OpenEditProfile(){
        EditProfilePanel.Instance?.OpenEditProfilePanel();
    }
    public void OpenShop(){
        
    }



   



    #endregion

    #region Shop Panel
    [Header("Shop")]
    [SerializeField] CanvasGroup inAppPanel;
    public void OpenInApp(){
        LeanTweenExtension.OpenPopupWithAlpha(inAppPanel);
    }

        
    #endregion

    #region Full Preview
    [Space(20f)]
    [Header("Full Preview")]
    [SerializeField] GameObject fullPreviewPanel;  
    [SerializeField] RawImage fullPriviewRAW;  
    internal void OpenFullPreview(Texture texture)
    {
        fullPriviewRAW.texture = texture;
        fullPreviewPanel.SetActive(true);
    }
    public async void MintNFT(){

    }
    public void CloseFullPreview(){
        fullPreviewPanel.SetActive(false);
    }
        
    #endregion
    
}
