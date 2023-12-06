using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
   public float hp = 100;
   public Platform platform;
   public float moveSpeed = 5f;
   public float jumpForce = 2f;
   private Rigidbody2D rb;
    Animator animator;
    bool jumpInput = false;
    int movedir = 0;
    public int lastmovedir = 1;
    bool canJump = true;
    bool canmove = true;
    bool attkInput = false;
    bool canattk = true;
    public AudioClip[] attacksounds;

   // Start is called before the first frame update
   void Start()
   {
        animator = GetComponent<Animator>();
       rb = GetComponent<Rigidbody2D>();
   }

   // Update is called once per fram

   void Update()
   {
        GetInputs();
       if(movedir != 0 && canmove)
       {
            transform.position += (new Vector3(moveSpeed * movedir * Time.deltaTime, 0, 0));
            animator.SetBool("Walking", true);
       }
        if (jumpInput && canJump)
        {
            Jump();
            canJump = false;
        }
       
        if(canattk && attkInput)
        {
            StartCoroutine(AttackDelay());
        }

        if(lastmovedir == 1)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX=true;
        }
   }

    public void PlayAttkSound()
    {
        GetComponent<AudioSource>().clip = attacksounds[Random.Range(0, attacksounds.Length)];
        GetComponent<AudioSource>().Play();
    }

   void Jump()
   {
       rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
   }

    private void Attack()
    {
        canattk = false;
        canJump = false;
        canmove = false;
        animator.SetBool("Attacking", true);
    }

    void GetInputs()
    {
        movedir = 0;
        if (Input.GetKey(KeyCode.A))
        {
            movedir -= 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            movedir += 1;
        }
        else
        {
            animator.SetBool("Walking", false);
        }

        if(movedir != 0 && canmove)
        {
            lastmovedir = movedir;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            jumpInput = true;
        }
        else
        {
            jumpInput = false;
        }

        if(Input.GetMouseButtonDown(0))
        {
            attkInput = true;
        }
        else 
        { 
            attkInput = false; 
        }
    }

   private void OnCollisionEnter2D(Collision2D collision)
   {
       if (collision.gameObject.CompareTag("Ground"))
       {
           if(collision.gameObject.GetComponent<Platform>() != null)
           {
               platform = collision.gameObject.GetComponent<Platform>();
                animator.SetBool("Midair", false);
                canJump = true;
           }
       }
   }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            animator.SetBool("Midair", true);
            canJump = false;
        }
    }

    IEnumerator AttackDelay()
    {
        Attack();
        yield return new WaitForSeconds(.5f);
        canmove = true;
        canJump = true;
        canattk = true;
    }

    public void Damage(float amount)
    {
        hp -= amount;
        if(hp <= 0)
        {
            gameObject.SetActive(false);
            SceneManager.LoadScene("GameOver");
        }
    }
}
