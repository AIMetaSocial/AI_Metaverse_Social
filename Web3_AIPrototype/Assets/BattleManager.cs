using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BattleManager : NetworkBehaviour,IPlayerLeft
{
    public static BattleManager Instance;

    public void PlayerLeft(PlayerRef player)
    {
        if(inChallengePlayer!=null && inChallengePlayer == player){
            EndBattle(GameEndReason.OPPONENT_LEFT,false);
        }
    }
    
    private void Awake() {
        Instance = this;
    }

    PlayerRef inChallengePlayer;

    public void EndBattle(GameEndReason gameEndReason,bool sendRPC){
        switch (gameEndReason)
        {

            case GameEndReason.OPPONENT_LEFT:{
                Debug.Log("OPPONENT LEFT THE GAME");
                Debug.Log("I WON");    
                   
                break;
            }
            case GameEndReason.WON:{
                Debug.Log("I WON");               
               
                break;
            }
            case GameEndReason.LOSE:{
                Debug.Log("I LOSE");
                if(sendRPC){
                    Rpc_EndBattle(GameEndReason.WON,inChallengePlayer);
                }
                break;
            }
            
        }
        GameUI.Instance.OpenGameCompleteUI(gameEndReason);  
        Runner.GetPlayerObject(Runner.LocalPlayer)?.GetComponent<PlayerController>()?.EndBattle();
        Runner.GetPlayerObject(inChallengePlayer)?.GetComponent<PlayerController>()?.EndBattle();
    }
    [Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_EndBattle(GameEndReason gameEndReason,PlayerRef _player){
        if(Runner.LocalPlayer == _player){
            EndBattle(gameEndReason,false);
        }
    }

    internal void ChallengeAccepted(PlayerRef lastChallengeGotPlayer)
    {
        PlayerRef player_whoAccepted =  Runner.LocalPlayer;
        PlayerRef player_whosent =  lastChallengeGotPlayer;
       
        Rpc_StartBattleFor2Players(player_whoAccepted,player_whosent);   
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_StartBattleFor2Players(PlayerRef player_whoAccepted, PlayerRef player_whosent)
    {   
            PlayerController playerWhoAccepted = Runner.GetPlayerObject(player_whoAccepted).GetComponent<PlayerController>();
            PlayerController playerWhoSend = Runner.GetPlayerObject(player_whosent).GetComponent<PlayerController>();

                if(player_whoAccepted == Runner.LocalPlayer){
                    inChallengePlayer = player_whosent;
                }else{
                    inChallengePlayer = player_whoAccepted;
                }

        
            playerWhoAccepted.StartBattle();
            playerWhoSend.StartBattle();
        
        
    }

    internal bool isSameAsChallengePlayer(PlayerRef _attackerPlayer)
    {
        return _attackerPlayer == inChallengePlayer;
    }

    
}
public enum GameEndReason{
    OPPONENT_LEFT,WON,LOSE
}
