using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goalBehaviour : MonoBehaviour
{
    public PlayerScript player;

    private void OnCollisionEnter(Collision collision)
    {
        BallScript ball = collision.rigidbody.GetComponent<BallScript>();

        //only if the ball is flying around, you could put other exemptions here
        if (ball != null && ball.ballState == BallScript.BallState.NORMAL)
        {
            player.StopAllCoroutines(); //stop whatever you're doing
            player.StartCoroutine(player.DieCoroutine()); //start dying
            ball.StartCoroutine(ball.HitPlayerCoroutine(player)); //remove the ball but let it know who it killed
        }
    }
}
