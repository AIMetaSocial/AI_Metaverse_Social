using Defective.JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class DatabaseManager : MonoBehaviour
{
    #region Singleton
    public static DatabaseManager Instance;
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

    public static event Action<LocalData> OnDataChanged;




    [SerializeField] private LocalData data = new LocalData();


    public LocalData GetLocalData()
    {

        return data;
    }


    private void Start()
    {

        //GetData(true);
    }
    IEnumerator updateProfile(int dataType, bool createnew = false)
    {
        Debug.Log("UPDATING PROFILE");

        JSONObject a = new JSONObject();
        JSONObject b = new JSONObject();
        JSONObject c = new JSONObject();
        //JSONObject d = new JSONObject();

        string url = ConstantManager.getProfile_api + PlayerPrefs.GetString("Account", "test").ToLower();

        if (createnew)
        {
            data = new LocalData();
            

        }
        switch (dataType)
        {
            case 0:

                if (!createnew) url += "?updateMask.fieldPaths=userdata";
                else
                {
                  
                   // LoadingCanvas.Instance.ToggleLoadingCanvas(false);
                }

                c.AddField("stringValue", Newtonsoft.Json.JsonConvert.SerializeObject(data));
                b.AddField("userdata", c);
                break;
                /* case 3:
                     if (!createnew) url += "?updateMask.fieldPaths=gamedata";
                     c.AddField("stringValue", PlayerPrefs.GetString("data"));
                     b.AddField("gamedata", c);
                     break;*/
        }

        WWWForm form = new WWWForm();

        // Serialize body as a Json string
        //string requestBodyString = "";

        a.AddField("fields", b);



        // Convert Json body string into a byte array
        byte[] requestBodyData = System.Text.Encoding.UTF8.GetBytes(a.Print());

        using (UnityWebRequest www = UnityWebRequest.Put(url, requestBodyData))
        {
            www.method = "PATCH";

            // Set request headers i.e. conent type, authorization etc
            //www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Content-length", (requestBodyData.Length.ToString()));
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
               // LoadingCanvas.Instance?.ToggleLoadingCanvas(false);
                MessageBox.Instance?.ShowMessage("Something Went Wrong!");
            }
            else
            {
                if (createnew)
                {
                   //LoadingCanvas.Instance?.ToggleLoadingCanvas(false);
                    //SceneManager.LoadScene("MAIN");
                }
                else
                {

                }
            }
        }
    }

    IEnumerator CheckProfile()
    {
        string url = ConstantManager.getProfile_api + PlayerPrefs.GetString("Account", "test2").ToLower();

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            //www.method = "PATCH";

            // Set request headers i.e. conent type, authorization etc
            //www.SetRequestHeader("Content-Type", "application/json");
            // www.SetRequestHeader("Content-length", (requestBodyData.Length.ToString()));
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Profile not found " + www.downloadHandler.text);
                //Debug.Log(www.error);


                StartCoroutine(updateProfile(0, true));
            }
            else
            {
                JSONObject obj = new JSONObject(www.downloadHandler.text);

                //Debug.Log("CheckProfile " + www.downloadHandler.text);
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<LocalData>(obj.GetField("fields").GetField("userdata").GetField("stringValue").stringValue);



            }

            OnDataChanged?.Invoke(data);



            if (data.transactionsInformation != null && data.transactionsInformation.Count > 0)
            {
                for (int i = 0; i < data.transactionsInformation.Count; i++)
                {
                    if (data.transactionsInformation[i].transactionStatus.Equals("pending"))
                    {
                        Debug.Log("Pending Test 1");
                        //CoreChainManager.Instance.CheckTransactionStatus(data.transactionsInformation[i].transactionId);
                    }
                }
            }

           
            //Debug.Log(obj.GetField("fields").GetField("musedata").GetField("stringValue").stringValue);

        }
    }










    public void GetData()
    {
        StartCoroutine(CheckProfile());
        //ConvertEpochToDatatime(1659504437);
    }

    public void UpdateData(LocalData localData)
    {
        data = localData;
        OnDataChanged?.Invoke(data);
        StartCoroutine(updateProfile(0));


    }
    public void UpdateData()
    {
        StartCoroutine(updateProfile(0));
        OnDataChanged?.Invoke(data);

    }
   
    /*  public DateTime ConvertEpochToDatatime(long epochSeconds) {
          DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(epochSeconds);
          DateTime dateTime = dateTimeOffset.DateTime;

          return dateTime;
      }
    */
  

    public void AddTransaction(string TransId, string status, int _shopId)
    {
        TranscationInfo info = new TranscationInfo(TransId, status);

        
       // info.coinAmount = CoreChainManager.Instance.coinsAmount[_shopId];
        
        data.transactionsInformation.Add(info);
        UpdateData(data);
    }

    public void ChangeTransactionStatus(string transID, string txConfirmed)
    {
        Debug.Log("Changing Database " + transID + " " + txConfirmed);
        TranscationInfo trans_info = data.transactionsInformation.Find(x => x.transactionId == transID);
        if (trans_info != null)
        {
            int index = data.transactionsInformation.IndexOf(trans_info);
            trans_info.transactionStatus = txConfirmed;
            data.transactionsInformation[index] = trans_info;
            if (txConfirmed.Equals("success"))
            {
                data.coins += trans_info.coinAmount;
                data.transactionsInformation.RemoveAt(index);
                AudioManager.Instance?.PlayCoinSound();
                MessageBox.Instance?.ShowMessage(trans_info.coinAmount.ToString() + " Coins Purchased Successfully!", true);
            }
            // UIManager.insta.ShowCoinPurchaseStatus(trans_info);

            UpdateData(data);
            /*
                        if (UIManager.insta)
                        {
                            ///  UIManager.insta.UpdatePlayerUIData(true, data);
                            ///  
                            UIManager.insta.UpdateBalance();
                        }*/
        }


    }


}
[System.Serializable]
public class LocalData
{

    public string playername;  
    public int selectedGender;  
    public int coins = 100;
    
    
    public List<TranscationInfo> transactionsInformation = new List<TranscationInfo>();

    
    





    public LocalData()
    {
        playername = "";
       
       selectedGender =0;
        coins = 100;
        
        transactionsInformation = new List<TranscationInfo>();        

    }
}


[System.Serializable]
public class SkinSelectedData
{
    public List<int> skinSelected;
}

[System.Serializable]
public class TranscationInfo
{
    public string transactionId;
    public string transactionStatus;
    public int coinAmount;
    public TranscationInfo(string Id, string status)
    {
        transactionId = Id;
        transactionStatus = status;
    }
}

