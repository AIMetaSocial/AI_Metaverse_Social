using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class ChatGPTIntegration : MonoBehaviour
{
    public static ChatGPTIntegration Instance;
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
    private static string apiUrlForImage = "https://api.openai.com/v1/images/generations";
    private static string apiUrlForChat = "https://api.openai.com/v1/chat/completions";
    private static string apiKey = "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";

    public static List<string> lastResponses = new List<string>();
    public static List<string> lastUserPrompts = new List<string>();
    public static void Init()
    {
        if (lastResponses != null)
        {
            lastResponses.Clear();
        }
        if (lastUserPrompts != null)
        {
            lastUserPrompts.Clear();
        }
    }

    public static string lastImageURLGot;
    public static string lastDescription;
    public static async UniTask<ImageRespnseData> GenerateAIImage(string prompt)
    {


        try
        {


            DalleRequest requestData = new DalleRequest { prompt = prompt };
            string jsonData = JsonConvert.SerializeObject(requestData);


            Debug.Log("Prompt : " + jsonData);

            UnityWebRequest request = new UnityWebRequest(apiUrlForImage, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {

                DalleResponse dalleResponse = JsonConvert.DeserializeObject<DalleResponse>(request.downloadHandler.text);
                if (dalleResponse?.data != null && dalleResponse.data.Count > 0)
                {
                    lastImageURLGot = dalleResponse.data[0].url;
                    ImageRespnseData imageResponseData = await LoadImageFromURL(dalleResponse.data[0].url, prompt);
                    return imageResponseData;
                }

                Debug.Log(request.downloadHandler.text);

            }
            else
            {
                Debug.LogError("Error: " + request.error + "\nResponse: " + request.downloadHandler.text);
            }
            return null;
        }
        catch
        {
            return null;
        }


    }
    public static async UniTask<ImageRespnseData> LoadImageFromURL(string imageUrl, string promptUserAdded)
    {

        string finalUrl = ConstantManager.imageProxy_api + UnityWebRequest.EscapeURL(imageUrl);

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(finalUrl))
        {
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + request.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                ImageRespnseData imageRespnseData = new ImageRespnseData();

                imageRespnseData.texture = texture;
                imageRespnseData.description = promptUserAdded;
                imageRespnseData.url = imageUrl;

                return imageRespnseData;
            }
        }

        /* using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
         {
             await www.SendWebRequest();

             if (www.result == UnityWebRequest.Result.Success)
             {
                 Texture2D texture = DownloadHandlerTexture.GetContent(www);

                 ImageRespnseData imageRespnseData = new ImageRespnseData();

                 imageRespnseData.texture = texture;
                 imageRespnseData.description = promptUserAdded;
                 imageRespnseData.url = imageUrl;

                 return imageRespnseData;
             }
         }*/
        return null;
    }

    /* public static async UniTask<ImageRespnseData> LoadImageFromURL(string imageUrl, string promptUserAdded)
     {
         using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
         {
             await www.SendWebRequest();

             if (www.result == UnityWebRequest.Result.Success)
             {
                 Texture2D texture = DownloadHandlerTexture.GetContent(www);

                 ImageRespnseData imageRespnseData = new ImageRespnseData();

                 imageRespnseData.texture = texture;
                 imageRespnseData.description = promptUserAdded;
                 imageRespnseData.url = imageUrl;

                 return imageRespnseData;
             }
         }
         return null;
     }*/



    #region CHATBOT
    public static async UniTask<string> GetReplyMessage(string prompt, string initPrompt)
    {

        List<MessageItem> _message = new List<MessageItem>();

        _message.Add(new MessageItem("system", initPrompt));

        //ADD History  For New Questions

        // if(lastUserPrompts != null && lastResponses!= null && lastResponses.Count> 0 && lastResponses.Count == lastUserPrompts.Count){

        //         Debug.Log("ADDING HISORY : " + lastResponses.Count);
        //         for (int i = 0; i < lastResponses.Count; i++)
        //         {
        //               _message.Add(new MessageItem("user", lastUserPrompts[i]));
        //         _message.Add(new MessageItem("assistant", lastResponses[i]));
        //         }

        // }



        _message.Add(new MessageItem("user", prompt));





        ChatGPTMessage msg = new ChatGPTMessage("gpt-3.5-turbo", _message, 1, 1, 1500);




        string jsonData = JsonConvert.SerializeObject(msg);

        Debug.Log("Prompt : " + jsonData);

        UnityWebRequest request = new UnityWebRequest(apiUrlForChat, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {

            string responseText = request.downloadHandler.text;
            OpenAIResponse response = JsonConvert.DeserializeObject<OpenAIResponse>(responseText);
            if (response != null && response.choices != null && response.choices.Length > 0 && response.choices[0].message != null)
            {
                Debug.Log("Response: " + response.choices[0].message.content);

                if (lastUserPrompts != null)
                {
                    lastUserPrompts.Add(prompt);
                }
                else
                {
                    lastUserPrompts = new List<string>();
                    lastUserPrompts.Add(prompt);
                }

                if (lastResponses != null)
                {
                    lastResponses.Add(response.choices[0].message.content);
                }
                else
                {
                    lastUserPrompts = new List<string>();
                    lastUserPrompts.Add(prompt);
                }


                return response.choices[0].message.content;
            }



        }
        else
        {
            Debug.LogError("Error: " + request.error + "\nResponse: " + request.downloadHandler.text);
        }
        return null;

    }

    #endregion




}

[System.Serializable]
public class ImageRespnseData
{
    public Texture2D texture;
    public string url;
    public string description;
}

[System.Serializable]
public class DalleResponse
{
    public List<DalleImageData> data;
}
[System.Serializable]
public class DalleImageData
{
    public string url;
}
[System.Serializable]
public class DalleRequest
{
    public string model = "dall-e-3";  // Explicitly use DALL·E 3
    public string prompt;
    public int n = 1;
    public string size = "1024x1024";
}



[System.Serializable]
public class ChatGPTMessage
{
    public string model;
    public List<MessageItem> messages;
    public float temperature;
    public float top_p;
    public int max_tokens;
    public ChatGPTMessage(string _modal, List<MessageItem> _message, float _temp, float _topp, int _maxToken)
    {
        this.model = _modal;
        this.messages = _message;
        this.temperature = _temp;
        this.top_p = _topp;
        this.max_tokens = _maxToken;
    }
}
[System.Serializable]
public class MessageItem
{
    public string role;
    public string content;
    public MessageItem(string _role, string _content)
    {
        this.role = _role;
        this.content = _content;
    }
}



[System.Serializable]
public class OpenAIResponse
{
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
}
[System.Serializable]
public class Message
{
    public string role;
    public string content;
}