using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNFTCollection : MonoBehaviour
{
    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject noNFTPanel;
    [SerializeField] GameObject failedPanel;
    [SerializeField] GameObject contentPanel;

    [SerializeField] NftItem myNFTPrefab;
    [SerializeField] Transform nftParents;

    public List<string> jsonURLS;
    public List<int> uidList;
    private async UniTaskVoid OnEnable()
    {

        loadingPanel.SetActive(true);
        failedPanel.SetActive(false);
        contentPanel.SetActive(false);
        noNFTPanel.SetActive(false);

        jsonURLS.Clear();
        uidList.Clear();
        jsonURLS = await BlockChainConnections.Instance.GetMyNFTS();
        uidList = await BlockChainConnections.Instance.GetMyNFT_UID();

        if (jsonURLS != null && uidList != null && uidList.Count == jsonURLS.Count)
        {

            if (uidList.Count == 0)
            {

                loadingPanel.SetActive(false);
                noNFTPanel.SetActive(true);
                return;
            }


            for (int i = nftParents.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(nftParents.GetChild(i).gameObject);
            }


            for (int i = 0; i < jsonURLS.Count; i++)
            {
                int temp = i;
                Debug.Log(jsonURLS[temp]);

                using (UnityWebRequest request = UnityWebRequest.Get(jsonURLS[temp]))
                //using (UnityWebRequest request = UnityWebRequest.Get(ConstantManager.jsonGetAPI + jsonURLS[temp]))
                {
                    await request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        string json = request.downloadHandler.text;
                        Debug.Log("Received JSON: " + json);

                        NFTResult nFTResult = JsonConvert.DeserializeObject<NFTResult>(json);

                        NftItem nftItem = Instantiate(myNFTPrefab, nftParents);
                        nftItem.SetData(nFTResult, uidList[temp]);

                    }
                    else
                    {
                        Debug.LogError("Error: " + request.error);
                    }
                }
            }


            loadingPanel.SetActive(false);
            contentPanel.SetActive(true);
        }
        else
        {

            loadingPanel.SetActive(false);
            failedPanel.SetActive(true);
        }






    }
    public void CloseMyCollecion()
    {
        this.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        jsonURLS.Clear();
        uidList.Clear();

        for (int i = nftParents.childCount - 1; i >= 0; i--) // Loop from last child to first
        {
            Destroy(nftParents.GetChild(i).gameObject);
        }

    }
}

[System.Serializable]
public class NFTResult
{
    public string name;
    public string description;
    public string image;
}
