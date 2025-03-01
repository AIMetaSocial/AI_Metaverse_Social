using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameCompleteUI : MonoBehaviour
{   
    [SerializeField] TMP_Text coinsGotText;
    [SerializeField] TMP_Text gameWonText;
    [SerializeField] CanvasGroup gotITBTN;
    [TextArea][SerializeField] string wonText;
    [TextArea][SerializeField] string loseText;
    private void OnEnable() {        
        gotITBTN.alpha = 0;
        gameWonText.GetComponent<CanvasGroup>().alpha =0;
        coinsGotText.GetComponent<CanvasGroup>().alpha =0;

        LeanTween.cancel(gameWonText.gameObject);
        LeanTween.cancel(coinsGotText.gameObject);
        LeanTween.cancel(gotITBTN.gameObject);

        LeanTween.scale(gameWonText.gameObject,Vector3.one,0.3f).setFrom(Vector3.one * 7f);
        LeanTween.alphaCanvas(gameWonText.GetComponent<CanvasGroup>(),1,0.2f).setFrom(0).setDelay(0.1f);

        LeanTween.scale(coinsGotText.gameObject,Vector3.one,0.3f).setFrom(Vector3.one * 7f).setDelay(0.35f);
        LeanTween.alphaCanvas(coinsGotText.GetComponent<CanvasGroup>(),1,0.2f).setFrom(0).setDelay(0.45f);

        LeanTween.scale(gotITBTN.gameObject,Vector3.one,0.3f).setFrom(Vector3.one * 7f).setDelay(0.7f);
        LeanTween.alphaCanvas(gotITBTN,1,0.2f).setFrom(0).setDelay(0.75f);
    }

    public void CLoseGameCompleteUI(){
        LeanTweenExtension.ClosePopupWithAlpha(this.GetComponent<CanvasGroup>());
    }

    public void SetUI(bool won,int coinsgot){
        coinsGotText.text = coinsgot.ToString();
        gameWonText.text = won ? wonText : loseText;
    }
}
