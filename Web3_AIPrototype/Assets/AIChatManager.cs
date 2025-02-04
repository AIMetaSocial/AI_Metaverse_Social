using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChatManager : MonoBehaviour
{

    public static event Action<bool> OnChatToggleEvent;
    [SerializeField] AI_GeneratorPerson currentAIGenerator;
    internal void Init(AI_GeneratorPerson lastTriggeredAIPlayer)
    {
        currentAIGenerator  = lastTriggeredAIPlayer;
        OnChatToggleEvent?.Invoke(true);
    }

    public void CloseChat(){
       
        OnChatToggleEvent?.Invoke(false);
        GameUI.Instance?.CloseAIChat();
    }
}
