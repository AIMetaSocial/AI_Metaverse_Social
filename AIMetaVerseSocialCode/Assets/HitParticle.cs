using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HitParticle : NetworkBehaviour
{
    private Camera mainCam;
    [SerializeField] float delay;
    private void Awake() {
        mainCam = Camera.main;
        AudioManager.Instance?.PlayHitSound();
    }

    public override void Spawned()
    {
        base.Spawned();
        if(HasStateAuthority){
            StartCoroutine(despawnWithDelay());
        }
    }
    IEnumerator despawnWithDelay(){

        yield return new WaitForSeconds(delay);
        Runner.Despawn(this.Object);
    }
    private void LateUpdate() {
        transform.rotation = Quaternion.LookRotation(mainCam.transform.forward);        
    }
}
