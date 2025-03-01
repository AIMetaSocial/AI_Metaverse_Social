using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{   
    [SerializeField] GameObject PlayerPrefab;
    [SerializeField] Transform[] spawnPositions;
    public override void Spawned()
    {

        Transform spawnPosition = spawnPositions[Runner.LocalPlayer.PlayerId-1];
        Vector3 position = spawnPosition.position;
        Quaternion rotation = spawnPosition.rotation;
        Debug.Log("PLAYER JOINED : "+ (Runner.LocalPlayer.PlayerId-1).ToString());
        NetworkObject playerObject = Runner.Spawn(PlayerPrefab,position,rotation,Runner.LocalPlayer,(Runner,obj)=>{

            var characterController = obj.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false; // Disable the controller
                obj.transform.position = position;   // Manually set the position
                obj.transform.rotation = rotation;   // Manually set the rotation
                characterController.enabled = true;  // Re-enable the controller
            }           
            Debug.Log("PLAYER SPAWNED : " + position+ " " + rotation.eulerAngles);

        });
        
        Runner.SetPlayerObject(Runner.LocalPlayer,playerObject);

        base.Spawned();

        

    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
    }

}

public enum GameState{

}

