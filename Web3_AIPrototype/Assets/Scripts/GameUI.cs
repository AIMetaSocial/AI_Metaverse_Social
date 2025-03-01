using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;
    private void Awake() {
        Instance = this;
    }

    [SerializeField] TMP_Text[] coinText;
    [SerializeField] TMP_Text[] tokenText;
    void OnEnable()
    {
        DatabaseManager.OnDataChanged+=HandleDataChange;    
    }
    void Start()
    {
        UpdateCoinText();
    }

    private void HandleDataChange(LocalData data)
    {
        UpdateCoinText();
    }

    public void UpdateCoinText(){

       
        int coins = DatabaseManager.Instance.GetLocalData().coins;
        foreach (var item in coinText)
        {
            item.text = coins.ToString();
        }

    }


    
    [Header("PROMT FOR TRIGGER AI")]
    [SerializeField] GameObject PromptPanelForAIChat;
    [SerializeField] TMP_Text aiPersonName;    
    [SerializeField] TMP_Text costText;
    [SerializeField] AI_GeneratorPerson lastTriggeredAIPlayer;

    [Header("AI CHAT UI")]
    [SerializeField] GameObject chatUI;
    [SerializeField] AIChatManager aIChatManager;
    internal void PlayerInteractionWithAIPlayer(AI_GeneratorPerson aI_GeneratorPerson,bool triggered){

        if(aI_GeneratorPerson!=null)
        {
            switch (aI_GeneratorPerson.AiType)
            {
                case AI_TYPE.CHATBOT:{
                    aiPersonName.text = "Do You Want To Chat With " + aI_GeneratorPerson.aiPlayerName+"?";
                    break;
                }
                case AI_TYPE.IMAGE:
                {
                    aiPersonName.text = "Do You Want Generate Images With AI Powered " + aI_GeneratorPerson.aiPlayerName+"?";
                    break;
                }
            }
            
            costText.text = aI_GeneratorPerson.chatCost.ToString();
        }

        lastTriggeredAIPlayer = aI_GeneratorPerson;
        PromptPanelForAIChat.SetActive(triggered);
    }
    public void ChatWithAI(){
        
        if(lastTriggeredAIPlayer == null) return;
        
        LocalData data = DatabaseManager.Instance.GetLocalData();
               
        
        if(lastTriggeredAIPlayer.chatCost>data.coins){ 
            MessageBox.Instance?.ShowMessage("Not Enought Coins!");
            return;
        }
        //Deduct Doins
        data.coins -= lastTriggeredAIPlayer.chatCost;        
        DatabaseManager.Instance.UpdateData(data);


        aIChatManager.Init(lastTriggeredAIPlayer);
        lastTriggeredAIPlayer.ToggleCamera(true);
        chatUI.SetActive(true);        
        
        CloseAIPrompt();
        
    }
    public void CloseAIChat()
    {   
        lastTriggeredAIPlayer.ToggleCamera(false);
        chatUI.SetActive(false);
    }
    public void CloseAIPrompt()
    {
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
        NetworkRunner networkRunner = FindObjectOfType<NetworkRunner>();
        if(networkRunner!=null){
            networkRunner.Shutdown();
        }
        SceneManager.LoadScene("MainMenu");
    }
    public void OpenSettings(){
        SettingsPanel.Instance?.OpenSettings();
    }
    public void OpenEditProfile(){
        EditProfilePanel.Instance?.OpenEditProfilePanel();
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
    [SerializeField] Button mintBTN;

    ImageRespnseData lastImageResponseData; 
    internal void OpenFullPreview(ImageRespnseData imageRespnseData)
    {       mintBTN.interactable = true;
        lastImageResponseData = imageRespnseData;
        fullPriviewRAW.texture = imageRespnseData.texture;
        fullPreviewPanel.SetActive(true);
    }
    public async void MintNFT(){
        mintBTN.interactable = false;

        string imageUrl = lastImageResponseData.url;
        string imageDesription = lastImageResponseData.description;
        MessageBox.Instance.ShowMessage("Minting NFT! Please Wait",false);
        
        string jsonResponse = await UploadImageToServer(imageUrl,imageDesription);
        if(jsonResponse.Contains("error")){
            MessageBox.Instance.ShowMessage("Something Went Wrong!");
            mintBTN.interactable = true;
        }
        else{
            Debug.Log("IMage Uploaded To Server Now Mint It With Json");
            Debug.Log(jsonResponse);
            //MessageBox.Instance.OkBTN();
            bool succussMint = await BlockChainConnections.Instance.Mint(jsonResponse);
            if(succussMint){
                mintBTN.interactable = false;
                MessageBox.Instance.ShowMessage("NFT Minted");
            }
            else{
                mintBTN.interactable = true;
                MessageBox.Instance.ShowMessage("Something Went Wrong");
            }
        
        }

    }

    public async UniTask<string> UploadImageToServer(string _imageURL,string _desc,CancellationToken cancellationToken = default){
         using (UnityWebRequest request = new UnityWebRequest(ConstantManager.imageUpload_api, "POST"))
        {
            // Create form data
            WWWForm form = new WWWForm();
            form.AddField("image_url", _imageURL); 
            form.AddField("description", _desc);
            Debug.Log(_imageURL +"-- "+ _desc);

            request.uploadHandler = new UploadHandlerRaw(form.data);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            // Send the request asynchronously with UniTask
            await request.SendWebRequest().WithCancellation(cancellationToken);

            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text; // API response as a string
            }
            else
            {
                Debug.LogError("Upload failed: " + request.error);
                return "Error: " + request.error;
            }
        }
    }
    public void CloseFullPreview(){
        fullPreviewPanel.SetActive(false);
    }

   

    #endregion


    #region Challenge Incoming UI
    [SerializeField] GameObject challengeIncomingUI;
    [SerializeField] TMP_Text incmingChallengeText;
    private PlayerRef lastChallengeGotPlayer;

    internal void ShowIncomingChallengeUI(PlayerRef localPlayer, string playerName)
    {   
        lastChallengeGotPlayer = localPlayer;
        incmingChallengeText.text = "Got a Challenge Request from " + playerName + "<br>Ready To Fight?";   
        challengeIncomingUI.SetActive(true);     
    }

    public void ChallengeDecline(){        
        challengeIncomingUI.SetActive(false);
    }

    
    public void ChallengeAccept(){
            
        challengeIncomingUI.SetActive(false);
        BattleManager.Instance.ChallengeAccepted(lastChallengeGotPlayer);
    }

    
    #endregion

    #region Game Complete
    [SerializeField] GameCompleteUI gameCompleteUI;
    internal void OpenGameCompleteUI(GameEndReason gameEndReason)
    {
        int coinsGot =0;

        switch (gameEndReason)
        {
            case GameEndReason.OPPONENT_LEFT:{
                
                coinsGot =UnityEngine.Random.Range(50,100);
                gameCompleteUI.SetUI(true,coinsGot);
                break;
            }
            case GameEndReason.WON:
            {
                coinsGot =UnityEngine.Random.Range(200,400);
                gameCompleteUI.SetUI(true,coinsGot);
                break;
            }
            case GameEndReason.LOSE:{
                coinsGot =UnityEngine.Random.Range(20,50);
                gameCompleteUI.SetUI(false,coinsGot);
                break;
            }
        }

        LeanTweenExtension.OpenPopupWithAlpha(gameCompleteUI.GetComponent<CanvasGroup>());

        
        LocalData data = DatabaseManager.Instance.GetLocalData();
        data.coins+= coinsGot;
        DatabaseManager.Instance.UpdateData(data);
        
    }

   

    #endregion

    #region Transfer NFT UI
    [SerializeField] GameObject transferPanel;
    [SerializeField] RawImage previewFortransfer;
    [SerializeField] TMP_InputField toTransferIDInput;
    [SerializeField] Button transferBTN;
    [SerializeField] int  lastUID;
    internal void OpenToTransferNFT(int uidNumber, Texture texture)
    {
        lastUID = uidNumber;
        toTransferIDInput.text = "";
        previewFortransfer.texture = texture;
        transferPanel.SetActive(true);
        

    }
    public async void Transfer(){
        string toTransfer = toTransferIDInput.text.Trim();

        if(string.IsNullOrEmpty(toTransfer)){
            MessageBox.Instance.ShowMessage("Please Enter Reciever's Address");
            return;
        }

        if(LoginManager.address == toTransfer){
            MessageBox.Instance.ShowMessage("Why you need to transfer to yourself?");
            return;
        }

        transferBTN.interactable = false;
        MessageBox.Instance.ShowMessage("Transferring NFT",false);

        bool result = await BlockChainConnections.Instance.TransferNFT(toTransfer,lastUID);

        if(result){
            MessageBox.Instance.ShowMessage("Successfully Transferred!");
            transferPanel.SetActive(false);
        }
        else{
            MessageBox.Instance.ShowMessage("Could Not Transfer!");

        }

    }


    #endregion

}
