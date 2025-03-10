using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonExtension : Button
{

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        AudioManager.Instance?.PlayButtonSound();
        LeanTweenExtension.ScaleDownButton(this.gameObject);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        LeanTweenExtension.ScaleUpButton(this.gameObject);
    }

   

}
