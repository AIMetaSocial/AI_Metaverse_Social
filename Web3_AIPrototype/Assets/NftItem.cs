using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NftItem : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    [SerializeField] string description;
    [SerializeField] string _name;
    [SerializeField] GameObject loadingObject;
    [SerializeField] int uidNumber;

    [SerializeField] Button onClickBTN;
    internal async void SetData(NFTResult nFTResult, int _uid)
    {
        uidNumber = _uid;
       description  =nFTResult.description;
       _name  =nFTResult.name;
       rawImage.texture = await LoadImageFromURL(nFTResult.image);   
       onClickBTN.interactable =true;   
       loadingObject.SetActive(false);
       rawImage.gameObject.SetActive(true);
    }

    public void OnClickTransfer(){
        GameUI.Instance.OpenToTransferNFT(uidNumber,rawImage.texture);
    }
     public  async UniTask<Texture> LoadImageFromURL(string imageUrl)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);

                return texture;
            }
        }
        return null;
    }

    void OnDestroy()
    {
        if(rawImage.texture !=null){
            DestroyImmediate(rawImage.texture);
        }
    }
}
