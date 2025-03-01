using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAppManager : MonoBehaviour
{

    private void OnEnable() {
        
    }

    [SerializeField] int[] rewardCoins;
    public async void purchaseItem(int _packNO){
        MessageBox.Instance.ShowMessage("Purchasing Coins!",false);

        bool response = await BlockChainConnections.Instance.purchaseCoins(_packNO);

        if(response){
            MessageBox.Instance.ShowMessage("Coins " +rewardCoins[_packNO].ToString() + " Purchased Successfully!");

            LocalData data = DatabaseManager.Instance.GetLocalData();
            data.coins += rewardCoins[_packNO];
            DatabaseManager.Instance.UpdateData(data);
        }else{
            MessageBox.Instance.ShowMessage("Purchase Couldn't Complete!");
        }
    }
    public void CloseInApp(){
        LeanTweenExtension.ClosePopupWithAlpha(this.GetComponent<CanvasGroup>());
    }
}
