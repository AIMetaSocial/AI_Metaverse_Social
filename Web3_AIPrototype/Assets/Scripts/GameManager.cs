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
        base.Spawned();

        Transform spawnPosition = spawnPositions[Runner.LocalPlayer.PlayerId-1];
        NetworkObject playerObject = Runner.Spawn(PlayerPrefab,spawnPosition.position,spawnPosition.rotation);
        Runner.SetPlayerObject(Runner.LocalPlayer,playerObject);

    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
    }
}

public enum GameState{

}

