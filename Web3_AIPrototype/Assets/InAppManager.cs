using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAppManager : MonoBehaviour
{

    private void OnEnable() {
        
    }

    public void purchaseItem(int _packNO){

    }
    public void CloseInApp(){
        LeanTweenExtension.ClosePopupWithAlpha(this.GetComponent<CanvasGroup>());
    }
}
