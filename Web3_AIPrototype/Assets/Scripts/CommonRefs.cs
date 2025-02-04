using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CommonRefs : MonoBehaviour
{
    public static CommonRefs Instance;
    private void Awake() {
        Instance = this;
    }


    [SerializeField] CinemachineFreeLook playerCM;
    [field:SerializeField] public PlayerController myPlayerController {get; private set;}
    
    public void SetPlayer(PlayerController _playerController){

        myPlayerController = _playerController;
        playerCM.Follow = _playerController.transform;
        playerCM.LookAt = _playerController.transform;
        
    }
}
