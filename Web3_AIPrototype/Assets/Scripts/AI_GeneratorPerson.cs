using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AI_GeneratorPerson : MonoBehaviour
{
    [field:SerializeField]public string aiPlayerName{get; private set;}
    [field:SerializeField]public int  chatCost{get; private set;}
    [field:SerializeField]public CinemachineVirtualCamera  chatCamera{get; private set;}

    public async UniTaskVoid SendMessageToAI(){
        await UniTask.Delay(2000);

    }

    internal void ToggleCamera(bool _enabled)
    {
        chatCamera.Priority = (_enabled)?40:0;
    }
}
