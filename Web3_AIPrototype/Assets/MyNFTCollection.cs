using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class MyNFTCollection : MonoBehaviour
{
    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject failedPanel;
    [SerializeField] GameObject contentPanel;

    [SerializeField] NftItem myNFTPrefab;
    [SerializeField] Transform nftParents;

    private async UniTaskVoid OnEnable(){

        loadingPanel.SetActive(true);
        failedPanel.SetActive(false);
        contentPanel.SetActive(false);

        List<string> jsonURLS =await BlockChainConnections.Instance.GetMyNFTS();
        List<int> uidList =await BlockChainConnections.Instance.GetMyNFT_UID();
        
        if(jsonURLS!=null && uidList!=null && uidList.Count == jsonURLS.Count){
            for (int i = 0; i < jsonURLS.Count; i++)
            {       
                int temp =i;
                Debug.Log(jsonURLS[temp]);

                using (UnityWebRequest request = UnityWebRequest.Get(ConstantManager.jsonGetAPI+jsonURLS[temp]))
                {
                    await request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        string json = request.downloadHandler.text;
                        Debug.Log("Received JSON: " + json);

                        NFTResult nFTResult = JsonConvert.DeserializeObject<NFTResult>(json);

                        NftItem nftItem = Instantiate(myNFTPrefab,nftParents);
                        nftItem.SetData(nFTResult,uidList[temp]);

                    }
                    else
                    {
                        Debug.LogError("Error: " + request.error);
                    }
                }
            }


            loadingPanel.SetActive(false);
            failedPanel.SetActive(false);
            contentPanel.SetActive(true);
        }
        else{
            
             loadingPanel.SetActive(false);
            contentPanel.SetActive(false);
            failedPanel.SetActive(true);
        }


        



    }
    public void CloseMyCollecion(){
        this.gameObject.SetActive(false);
    }   

    void OnDisable()
    {
        
    }
}

[System.Serializable] 
public class NFTResult{
    public string name;
    public string description;
    public string image;
}
