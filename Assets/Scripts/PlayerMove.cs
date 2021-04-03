using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMove : MonoBehaviour
{
    public float maxSpeed,jumpVelocity = 12f;
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 8f;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anime;
    
    private float curTime;
    public float coolTime = 0.5f;
    public Transform pos;
    public Vector2 boxSize;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anime = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        //Fall
        if (rigid.velocity.y < 0)
        {
            rigid.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if(rigid.velocity.y>0 && !Input.GetButton("Jump"))
        {
            rigid.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        //Jump

        if (Input.GetButtonDown("Jump") && !anime.GetBool("isJumping"))
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.up * jumpVelocity;
            anime.SetBool("isJumping", true);
        }


        //atk
        if (curTime <= 0)
        {
            if (Input.GetKeyDown("x"))
            {
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                    Debug.Log(collider.name);
                }
                anime.SetTrigger("attack");
                curTime = coolTime;
            }

        }
        else
        {
            curTime -= Time.deltaTime;
        }

        
        //horizontal movement
        float h = Input.GetAxisRaw("Horizontal");
        rigid.velocity = new Vector2(maxSpeed*h, rigid.velocity.y);

        //Direction Sprite
        if (Input.GetButtonDown("Horizontal"))
        {
            gameObject.transform.eulerAngles = new Vector2(0, 180* (Input.GetAxisRaw("Horizontal") == -1 ? 1:0));
            //spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }


        //Animation
        if (rigid.velocity.normalized.x == 0)
            anime.SetBool("isWalking", false);
        else
            anime.SetBool("isWalking", true);

        if (rigid.velocity.y < 0 && anime.GetBool("isJumping"))
            anime.SetBool("isFalling", true);
        else
            anime.SetBool("isFalling", false);

    }

    private void FixedUpdate()
    {
        //LandingPlatform
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1,LayerMask.GetMask("Platform"));

        if(rayHit.collider != null && rigid.velocity.y<0)
        {
            anime.SetBool("isJumping", false);
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Debug.Log("D");
            onDamaged(collision.gameObject.transform.position.x);
        }
    }

    void onDamaged(float targetx)
    {
        //Change layer
        gameObject.layer = 11;

        //View
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Reaction Force
        rigid.velocity = new Vector2(targetx>gameObject.transform.position.x ? -10:10, 10);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }
}
