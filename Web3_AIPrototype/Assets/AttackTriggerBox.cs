using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class AttackTriggerBox : MonoBehaviour
{
    [SerializeField] int damage;    
    private void OnTriggerEnter(Collider other) {

        if(other.transform.parent.TryGetComponent<PlayerController>(out PlayerController pc)){
            if(BattleManager.Instance.isSameAsChallengePlayer(pc.GetComponent<NetworkObject>().StateAuthority)){

                
                pc.GotDamage(damage,other.ClosestPoint(transform.position));
                this.GetComponent<Collider>().enabled = false;

            }
        }
    }
}
