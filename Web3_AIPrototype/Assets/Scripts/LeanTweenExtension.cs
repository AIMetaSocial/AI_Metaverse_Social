using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTweenExtension : MonoBehaviour
{
    public static LeanTweenExtension Instance;
     private void Awake()
    {
        if(Instance != null){
            Destroy(this.gameObject);
            return;
        }
        Instance = this;       
        DontDestroyOnLoad(this.gameObject);
    }

    [SerializeField] AnimationCurve _xScale;
    [SerializeField] AnimationCurve _yScale;

    private static AnimationCurve xCurve;
    private static AnimationCurve yCurve;
    private void Start() {
        xCurve = _xScale;
        yCurve = _yScale;
    }

    public static void OpenPopupWithAlpha(CanvasGroup _canvasGroup){
        _canvasGroup.gameObject.SetActive(true);
        LeanTween.cancel(_canvasGroup.gameObject);
        LeanTween.alphaCanvas(_canvasGroup,1,0.3f);
        LeanTween.cancel(_canvasGroup.transform.GetChild(0).gameObject);
        LeanTween.scaleX(_canvasGroup.transform.GetChild(0).gameObject,1,0.3f).setEase(xCurve);
        LeanTween.scaleY(_canvasGroup.transform.GetChild(0).gameObject,1,0.3f).setEase(yCurve);
    }
    public static void ClosePopupWithAlpha(CanvasGroup _canvasGroup){
        LeanTween.cancel(_canvasGroup.gameObject);
        LeanTween.alphaCanvas(_canvasGroup,1,0.3f);
        LeanTween.cancel(_canvasGroup.transform.GetChild(0).gameObject);
        LeanTween.scaleX(_canvasGroup.transform.GetChild(0).gameObject,0,0.3f).setEase(xCurve);
        LeanTween.scaleY(_canvasGroup.transform.GetChild(0).gameObject,0,0.3f).setEase(yCurve).setOnComplete(()=>{

            if(_canvasGroup.gameObject!=null){
                _canvasGroup.gameObject.SetActive(false);
            }
        });
    }

    public static void OpenPopup(GameObject go){
        LeanTween.cancel(go);       
        LeanTween.scaleX(go,1,0.3f).setEase(xCurve);
        LeanTween.scaleY(go,1,0.3f).setEase(yCurve);
    }
    public static void ClosePopup(GameObject go){
      
        LeanTween.cancel(go);
        LeanTween.scaleX(go,0,0.3f).setEase(xCurve);
        LeanTween.scaleY(go,0,0.3f).setEase(yCurve).setOnComplete(()=>{
            if(go!=null){
                go.SetActive(false);
            }
        });
    }

    internal static void ScaleDownButton(GameObject go)
    {
         LeanTween.cancel(go);
          LeanTween.scaleX(go,0.8f,0.3f).setEase(xCurve);
          LeanTween.scaleY(go,0.8f,0.3f).setEase(yCurve);
    }

    internal static void ScaleUpButton(GameObject go)
    {
         LeanTween.cancel(go);
          LeanTween.scaleX(go,1,0.3f).setEase(xCurve);
          LeanTween.scaleY(go,1,0.3f).setEase(yCurve);
    }
}
