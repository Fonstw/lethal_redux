using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour {

    public float startSpeed = .3f;
    public float maxSpeed = .5f;
    public float increaseOnHit = 1.1f;
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public Vector3 direction;
    [HideInInspector]
    public int lastHitter = -1;
    [HideInInspector]
    public Vector3 position;

    //more behavoir types of the ball can be added here
    public enum BallState
    {
        SERVE,
        NORMAL,
        HITSTUN,
        STANDBY
    }
    [HideInInspector]
    public BallState ballState = BallState.SERVE;

    public static BallScript ball;

    void Start () {
        ball = this;
        speed = startSpeed;

        //randomly spawn left or right
        if (Random.Range(0, 2) < 1)
            position = new Vector3(8, 0, 0);
        else
            position = new Vector3(-8, 0, 0);
    }

    //updating the state of the ball
    void Update() {

        //NORMAL is the state where the ball moves
        if (ballState == BallState.NORMAL)
        {
            position += direction * speed;
        }

        //if the ball is not dead, we have to make sure it stays in the stage
        if (ballState != BallState.STANDBY)
        {
            if (position.x < -12)
            {
                HitWall(true);
                position.x = -12;
            }
            if (position.x > 12)
            {
                HitWall(true);
                position.x = 12;
            }
            if (position.y < -5)
            {
                HitWall(false);
                position.y = -5;
            }
            if (position.y > 5)
            {
                HitWall(false);
                position.y = 5;
            }
        }

        transform.position = position;
    }

    //the ball gets hit
    public void GetHit(PlayerScript byPlayer, float newSpeed, float hitPause)
    {
        //you can set the starting speed in the editor
        //we can play around with this of course
        //try speed *= 1.1f instead

        speed = newSpeed;

        //if (speed < maxSpeed)
        //    speed *= increaseOnHit;

        lastHitter = byPlayer.playerNum;

        //default direction set to which player it is
        if (byPlayer.playerNum == 0) direction = Vector2.right;
        else direction = Vector2.left;
        StartCoroutine(GetHitCoroutine(hitPause));
    }

    public void HitWall(bool vertical)
    {
        if (vertical) direction.x *= -1;
        else direction.y *= -1;
        lastHitter = -1;
    }

    public IEnumerator GetHitCoroutine(float hitPause)
    {
        ballState = BallState.HITSTUN;

        yield return new WaitForSeconds(hitPause);

        ballState = BallState.NORMAL;

        yield return new WaitForSeconds(hitPause * 6f);

        lastHitter = -1;
    }

    public IEnumerator HitPlayerCoroutine(PlayerScript player)
    {
        ballState = BallState.STANDBY;
        position = 1000 * Vector3.down;
        speed = startSpeed;

        yield return new WaitForSeconds(1.4f);

        lastHitter = -1;
        ballState = BallState.SERVE;
        position = player.position + Vector3.right*(player.playerNum == 0 ? 1.5f : -1.5f);
    }
}
