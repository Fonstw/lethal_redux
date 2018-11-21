using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    //which player it is. 0 is left, 1 is right
    public int playerNum;

    //these can be tweaked in the editor
    public float moveSpeed;
    public float swingDuration;
    public float recoveryDuration;

    //you can add stuff here and just drag a sprite on it in the editor
    //same for the ball, if you want a different visual there
    //spriteRenderer.sprite = whateverSprite; is how you change picture
    private SpriteRenderer spriteRenderer;
    public Collider swingCollider;
    public Sprite idleSprite;
    public Sprite swingSprite;
    public Sprite hitSprite;
    public Sprite recoverySprite;
    public Sprite dieSprite;

    //input mapping (just set these in the editor)
    public KeyCode hitKey = KeyCode.C;
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode leftKey = KeyCode.A;

    [HideInInspector]
    public Vector3 position;

    //if you want to add more abilities or states of the player, add them in here
    //and then put them in Update() so the game knows what to do during them
    public enum PlayerState
    {
        NORMAL,
        SWINGING,
        HITTING,
        SWING_RECOVERY,
        GET_HIT,
        DEAD,
    }
    PlayerState playerState = PlayerState.NORMAL;
    Vector3 startPosition;

    void Start()
    {
        position = startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        swingCollider.enabled = false;
        position.y = Random.Range(-3.5f, 3.5f);
    }

    //checking if we hit the ball
    public void OnTriggerStay(Collider otherCollider)
    {
        BallScript ball = otherCollider.GetComponent<BallScript>();
        if (ball.lastHitter != playerNum) //don't hit the ball twice
        {
            //you can do exemptions or calculations with this depending on the ball or the player
            float hitPause = .1f;
            
            ball.GetHit(this, hitPause);//send info to the ball
            StopAllCoroutines(); //stop the swinging 
            StartCoroutine(HitCoroutine(hitPause)); //start the hitting with the right hitpause duration
        }
    }

    //checking if we get hit
    //public void OnCollisionEnter(Collision collision)
    //{
    //    BallScript ball = collision.rigidbody.GetComponent<BallScript>();

    //    //only if the ball is flying around, you could put other exemptions here
    //    if (ball != null && ball.ballState == BallScript.BallState.NORMAL) 
    //    {
    //        StopAllCoroutines(); //stop whatever you're doing
    //        StartCoroutine(DieCoroutine()); //start dying
    //        ball.StartCoroutine(ball.HitPlayerCoroutine(this)); //remove the ball but let it know who it killed
    //    }
    //}

    //we update the state of the player
    //what sprite to show and what hitbox to enable
    //you can add more abilities here as PlayerStates, more or other hitboxes etc
    void Update()
    {
        switch (playerState)
        {
            case PlayerState.NORMAL:
                swingCollider.enabled = false;
                spriteRenderer.sprite = idleSprite;

                //this is where we can check to start actions
                //since the player is doing nothing right now
                if (Input.GetKeyDown(hitKey))
                {
                    StartCoroutine(SwingCoroutine());
                }

                //move around
                if (Input.GetKey(rightKey))
                    position += Vector3.right * moveSpeed;
                if (Input.GetKey(leftKey))
                    position += Vector3.left * moveSpeed;
                if (Input.GetKey(upKey))
                    position += Vector3.up * moveSpeed/* * .6667f*/;
                if (Input.GetKey(downKey))
                    position += Vector3.down * moveSpeed/* * .6667f*/;

                position.x = Mathf.Clamp(position.x, -8f, 8f);
                position.y = Mathf.Clamp(position.y, -3.5f, 3.5f);
                break;

            case PlayerState.SWINGING:
                swingCollider.enabled = true;
                spriteRenderer.sprite = swingSprite;
                break;

            case PlayerState.HITTING:
                swingCollider.enabled = true;
                spriteRenderer.sprite = hitSprite;
                //as we're hitting, we can aim the ball
                //feel free to remove or alter this completely

                //for setting specific directions you can use: 
                //new Vector2(0.5f, 0.5f); is diagonally up for example
                //see it as distance from zero, the direction is the line between zero and your x y numbers
                //Vector3.up or down etc also works

                if (playerNum == 0)//left side player
                {
                    if (Input.GetKey(rightKey))
                        BallScript.ball.direction = Vector3.right;
                    if (Input.GetKey(upKey))
                        BallScript.ball.direction = new Vector2(0.5f, 0.33f);
                    if (Input.GetKey(downKey))
                        BallScript.ball.direction = new Vector2(0.5f, -0.33f);
                }
                else
                {
                    if (Input.GetKey(leftKey))
                        BallScript.ball.direction = Vector3.left;
                    if (Input.GetKey(upKey))
                        BallScript.ball.direction = new Vector2(-0.5f, 0.33f);
                    if (Input.GetKey(downKey))
                        BallScript.ball.direction = new Vector2(-0.5f, -0.33f);
                }
                //tip: you can use BallScript.ball to access the ball from anywhere
                break;
            case PlayerState.SWING_RECOVERY:
                swingCollider.enabled = false;
                spriteRenderer.sprite = recoverySprite;
                break;

            case PlayerState.GET_HIT:
                swingCollider.enabled = false;
                spriteRenderer.sprite = dieSprite;
                break;

            case PlayerState.DEAD:
                swingCollider.enabled = false;
                spriteRenderer.sprite = dieSprite;
                position += Vector3.down * 0.25f;
                break;
        }

        transform.position = position;
    }

    //these are for doing the timings
    //after WaitForSeconds it goes on where it left off
    //if you want to add different abilities you can copy one over and change it

    public IEnumerator SwingCoroutine()
    {
        playerState = PlayerState.SWINGING;
        yield return new WaitForSeconds(swingDuration);

        playerState = PlayerState.SWING_RECOVERY;
        yield return new WaitForSeconds(recoveryDuration);

        playerState = PlayerState.NORMAL;
    }

    public IEnumerator HitCoroutine(float hitPause)
    {
        playerState = PlayerState.HITTING;
        yield return new WaitForSeconds(hitPause);

        playerState = PlayerState.SWING_RECOVERY;
        yield return new WaitForSeconds(recoveryDuration);

        playerState = PlayerState.NORMAL;
    }

    public IEnumerator DieCoroutine()
    {
        playerState = PlayerState.GET_HIT;
        yield return new WaitForSeconds(0.2f);

        playerState = PlayerState.DEAD;
        yield return new WaitForSeconds(1.0f);

        playerState = PlayerState.NORMAL;
        position = startPosition;
    }
}