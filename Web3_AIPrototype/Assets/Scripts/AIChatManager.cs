using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AIChatManager : MonoBehaviour
{   
    public static AIChatManager Instance;
    private void Awake() {
        Instance = this;
    }
    [SerializeField] GameObject playerMSGPrefab;
    [SerializeField] GameObject AiResponseTextPrefab;
    [SerializeField] GameObject AiResponseImagePrefab;
    [SerializeField] Transform parentForMessages;

    public static event Action<bool> OnChatToggleEvent;
    [SerializeField] AI_GeneratorPerson currentAIGenerator;


    
    internal async void Init(AI_GeneratorPerson lastTriggeredAIPlayer)
    {
        if(currentAIGenerator == null || currentAIGenerator != lastTriggeredAIPlayer){
            ChatGPTIntegration.Init();

          
            foreach (Transform _child in parentForMessages)
            {   
                if(_child.GetChild(0).GetChild(0).TryGetComponent<RawImage>(out RawImage raw)){
                    DestroyImmediate(raw.texture);
                }

                Destroy(_child.gameObject);
            }
           
        }

        currentAIGenerator  = lastTriggeredAIPlayer;
        OnChatToggleEvent?.Invoke(true);
    }

    public void CloseChat(){
       
        OnChatToggleEvent?.Invoke(false);
        GameUI.Instance?.CloseAIChat();
    }   
    
    private void OnDisable() {
        
    }

    [SerializeField] Button sendBTN;
    [SerializeField] TMP_InputField inputText;

    private void Update() {
        if((Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) && sendBTN.interactable && sendBTN.gameObject.activeInHierarchy){
            SendMessageToAI();
        }
    }

    public void OnClickSendBTN(){
        SendMessageToAI();    
    }
    [SerializeField] Texture2D lastGeneratedTexture;
    public async UniTaskVoid SendMessageToAI()
    {
        if(string.IsNullOrEmpty(inputText.text.Trim())){
            return;
        }   
        sendBTN.interactable = false;


        string prompt = inputText.text.Trim();


        switch (currentAIGenerator.AiType)
        {
            case AI_TYPE.CHATBOT:{

                GenerateMyBox(prompt);   
                GenerateAIBoxText();                   
                break;
            }
            case AI_TYPE.IMAGE:{

                GenerateMyBox(prompt);   
                prompt = $"{prompt}, cartoon-style";                  
                GenerateAIBoxImage();   
                break;
            }
        }

        inputText.text = "";      
        


        
        switch (currentAIGenerator.AiType)
        {
            case AI_TYPE.CHATBOT:{

                    string response = await ChatGPTIntegration.GetReplyMessage(prompt,currentAIGenerator.promptForInit);
                    
                    if (response != null)
                    {
                        Debug.Log("RESPONSE GOT");
                        SetAiMessage(response);
                    }
                    else
                    {
                        Debug.Log("RESPONSE CANT BE GENERATED");
                        DestroyImmediate(lastGeneratedAIMessage);
                        GenerateAIBoxText();
                        SetAiMessage("Could Not Get Response");
                    }

                break;
            }
            case AI_TYPE.IMAGE:{

                    ImageRespnseData imageRespnseData = await ChatGPTIntegration.GenerateAIImage(prompt);
                     
                    if (imageRespnseData != null && imageRespnseData.texture != null )
                    {
                        Debug.Log("IMAGE GENERATED SUCCESSFULLY");
                        SetAiMessage(imageRespnseData);                       
                        
                    }
                    else
                    {
                        Debug.Log("IMAGE CANT BE GENERATED");
                        DestroyImmediate(lastGeneratedAIMessage);
                        GenerateAIBoxText();
                        SetAiMessage("Could Not Create Image At Now!");
                    }

                    break;
            }
         
        }

        sendBTN.interactable = true;

    }




    private void GenerateMyBox(string message)
    {
        GameObject myMsg = Instantiate(playerMSGPrefab, parentForMessages);

        RectTransform textRect = myMsg.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        RectTransform textRectImage = myMsg.transform.GetChild(0).GetComponent<RectTransform>();
        TMP_Text textComponent = myMsg.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();

        textComponent.text = message;

        StartCoroutine(changeTextComponentHeight(textComponent, textRect,textRectImage));



    }


  

    GameObject lastGeneratedAIMessage;
    private void GenerateAIBoxText()
    {
        lastGeneratedAIMessage = Instantiate(AiResponseTextPrefab, parentForMessages);

        LeanTween.scale(lastGeneratedAIMessage.transform.GetChild(0).gameObject,Vector3.one,0.3f).setDelay(0.3f);
    }
    private void GenerateAIBoxImage()
    {
        lastGeneratedAIMessage = Instantiate(AiResponseImagePrefab, parentForMessages);

        LeanTween.scale(lastGeneratedAIMessage.transform.GetChild(0).gameObject,Vector3.one,0.3f).setDelay(0.3f);
    }
    private void SetAiMessage(string message)
    {
        if (lastGeneratedAIMessage != null)
        {

            lastGeneratedAIMessage.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = message;


            RectTransform textRect = lastGeneratedAIMessage.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
            RectTransform textRectImage = lastGeneratedAIMessage.transform.GetChild(0).GetComponent<RectTransform>();
            TMP_Text textComponent = lastGeneratedAIMessage.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();

            textComponent.text = message;
            
            StartCoroutine(changeTextComponentHeight(textComponent, textRect,textRectImage,0.3f));

            Destroy(lastGeneratedAIMessage.transform.GetChild(0).GetChild(1).gameObject);

        }

    }
    private void SetAiMessage(ImageRespnseData imageRespnseData)
    {
        if (lastGeneratedAIMessage != null)
        {   
            RawImage rawImage = lastGeneratedAIMessage.transform.GetChild(0).GetChild(0).GetComponent<RawImage>();

            rawImage.texture = imageRespnseData.texture;            
            rawImage.gameObject.SetActive(true);            

            Destroy(lastGeneratedAIMessage.transform.GetChild(0).GetChild(1).gameObject);
            rawImage.GetComponent<Button>().onClick.AddListener(()=> OpenImageInFullScreen(imageRespnseData));  
            lastGeneratedAIMessage.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        }
    }

    private void OpenImageInFullScreen(ImageRespnseData rawImage)
    {
        GameUI.Instance?.OpenFullPreview(rawImage);
    }

    [SerializeField] float maxWidth;
    [SerializeField] float minWidth;
    [SerializeField] float marginForImageX;
    [SerializeField] float marginForImageY;    
    [SerializeField] VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] ScrollRect contentScrollRect;
    
    IEnumerator changeTextComponentHeight(TMP_Text textComponent, RectTransform rectComponent,RectTransform imageRect,float delay=0f)
    {
        yield return new WaitForEndOfFrame();

        float desiredWidth = textComponent.preferredWidth;

        if(desiredWidth<minWidth){
            desiredWidth = minWidth;
        }
        if (desiredWidth > maxWidth)
        {
            desiredWidth = maxWidth;
        }

        rectComponent.sizeDelta = new Vector2(desiredWidth, rectComponent.sizeDelta.y);

        // Force TextMeshPro to recalculate its layout based on new width
        textComponent.ForceMeshUpdate();

         Canvas.ForceUpdateCanvases(); 
            verticalLayoutGroup.enabled = false; 
            verticalLayoutGroup.enabled = true;
               contentScrollRect.verticalNormalizedPosition = 0f;

        // Get the preferred height for the current text and width
        float preferredHeight = textComponent.GetPreferredValues(desiredWidth, float.MaxValue).y;

      
        // Adjust the height of RectTransform based on preferred height
        rectComponent.sizeDelta = new Vector2(rectComponent.sizeDelta.x, preferredHeight);
        imageRect.sizeDelta = new Vector2(rectComponent.sizeDelta.x + marginForImageX, preferredHeight + + marginForImageY);
        imageRect.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(imageRect.parent.GetComponent<RectTransform>().sizeDelta.x,imageRect.sizeDelta.y);
        LeanTween.scale(imageRect.gameObject,Vector3.one,0.3f).setDelay(delay).setOnComplete(()=>{

            Canvas.ForceUpdateCanvases(); 
            verticalLayoutGroup.enabled = false; 
            verticalLayoutGroup.enabled = true;


                contentScrollRect.verticalNormalizedPosition = 0f;
                                  
            
        });


        
    }
}
