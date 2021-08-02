using UnityEngine;

public class PlayerController : PlayerPhysics
{
    // Adjustable parameters 
    [Header("Sling values")]
    [SerializeField] float slingForce;
    [SerializeField] float slingRadius;
    [SerializeField] float slingTimer;
    [SerializeField] GameObject ringTimerObject;   
    [SerializeField] [Tooltip("0 = Time stop, 1 = Normal time")] [Range(0, 1)] float timeSlow = 0.5f;
    [SerializeField] bool directionRelativeToMouse;
    public bool enableSlinging = true;
    public AudioSource audio;
    public AudioClip slingingSound;
    public AudioClip slingSound;

    Vector2 mouseClickPos;
    Vector2 slingPos;
    LineRenderer dirLine;
    public Vector2 slingDir;

    LayerMask groundMask;
    LayerMask wallMask;

    // Debugging 
    [Header("Debug")] 
    [SerializeField] bool sling = true;
    [SerializeField] bool testPCG;
    
    private void Start()
    {
        GetComponent<CircleCollider2D>().radius = slingRadius; // Changes the radius of the circle that manages sling hitbox
        dirLine = GetComponentInChildren<LineRenderer>(); // Line renderer which handles the sling direction
        groundMask = LayerMask.GetMask("Ground"); // Layermask for collision with ground and walls
        wallMask = LayerMask.GetMask("Wall"); // Layermask for collision with ground and walls
    }

    private void FixedUpdate()
    {
        HandlePhysics(IsGrounded());
    }

    private void Update()
    {
        if (IsGrounded())
        {
            sling = true;
            slingPos = transform.position;
        }
            
        if (enableSlinging && !testPCG)
            Sling();

        if (testPCG)
        {
            gravity = 0;           
            Velocity = Vector3.zero;
            if(Input.GetKey(KeyCode.W))
                transform.position += Vector3.up * Time.deltaTime * 10;
        }      
    }

    // Enables slinging when entering sling radius
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Slinger"))
        {
            sling = true;
            slingPos = collision.gameObject.transform.position;

            if (collision.gameObject.GetComponent<Slinger>().scoreable)
            {
                ScoreManager.Instance.Score++;
                collision.gameObject.GetComponent<Slinger>().scoreable = false;
            }
                
        }    
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Slinger"))
        {          
            sling = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            InvertForce(new Vector2(Velocity.x * wallBounceForce, -Velocity.y));
        }
    }
    public bool IsGrounded()
    {
        // Throws a raycast downwards, left and right from player and checks for the ground or wall layermask
        if (Velocity.y <= 0 && Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2 + .03f, groundMask))
        {
            return true;
        }
        else
            return false;
    }
    public bool WallCheck()
    {
        return Physics2D.Raycast(transform.position, Vector2.right, transform.localScale.x / 2 + .1f, wallMask) ||
                Physics2D.Raycast(transform.position, Vector2.left, transform.localScale.x / 2 + .1f, wallMask);
    }

    bool CanSling() => sling ? true : false;

    // ---- REFACTOR --------
    void Sling()
    {
        // Executes if player is able to sling
        if (CanSling())
        {
            if(!IsGrounded())
            {
                mouseClickPos = MouseToWorldPos();
                transform.position = Vector3.Lerp(transform.position, slingPos, 0.01f);
                SlingTimer(true);
            }       
            else
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    mouseClickPos = MouseToWorldPos();                    
                }
                // Code block for sling mechanic
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    transform.position = Vector3.Lerp(transform.position, slingPos, 0.01f);

                    // -- Change to better sling animation xd -- 
                    GetComponent<PlayerBehaviour>().groundColAnim.Play();
                    //----------------------

                    SlingTimer(true);
                }
            }
            
            // Code block for sling release
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                SlingTimer(false);
                transform.position = slingPos;
                slingDir = Direction();
            }
        }
        // Executes if player is not able to sling
        else
        {
            //Defaults if player manages to enter and exit sling state without releasing 
            SetSlingValues(Velocity, gravity, 1f);
        }
    }

    void SlingGFX(bool isSlinging)
    {
        if (isSlinging)
        {
            ringTimerObject.transform.localScale -= Vector3.one * 2 * Time.deltaTime / slingTimer;
            //ringTimerObject.GetComponent<SpriteRenderer>().color += new Color(-50, 50, 0, 0) * Time.deltaTime;
        }
        else
        {
            ringTimerObject.transform.localScale = Vector3.one * 0.5f;
            //ringTimerObject.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 1);
        }
    }

    void SlingTimer(bool isSlinging)
    {       
        if (isSlinging && ringTimerObject.transform.localScale.x > 0)
        {
            SetSlingValues(Vector2.zero, 0, timeSlow);
            ActivateDirectionGFX(isSlinging);
            SlingGFX(isSlinging);
        }
        else
        {
            sling = false;
            isSlinging = false;
            SetSlingValues(slingForce * Direction(), gravity, 1f);
            SlingGFX(isSlinging);
            audio.Stop();
            audio.PlayOneShot(slingSound);
        }

        ActivateDirectionGFX(isSlinging);
        ringTimerObject.SetActive(isSlinging);
    }

    Vector2 MouseToWorldPos()
    {
        Vector2 mousePos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    void SetSlingValues(Vector2 velocity, float gravity, float timeScale)
    {      
        Velocity = velocity;
        Gravity = gravity;
        Time.timeScale = timeScale;
    }

    // Line renderer temporarily shows the direction before adding sweet ass gfx
    void ActivateDirectionGFX(bool active)
    {
        if (active)
        {
            dirLine.enabled = true;
            dirLine.SetPosition(0, slingPos);
            dirLine.SetPosition(1, new Vector3(slingPos.x + Direction().x * 5, slingPos.y + Direction().y * 5, 0));
        }
        else
            dirLine.enabled = false;
    }

    // Return the direction of the sling by normalizing the inverse of the direction from the player position to the mouse position
    public Vector2 Direction()
    {
        Vector3 direction;
        Vector3 mouseWorldPos = MouseToWorldPos();

        // Get direction relative to initial mouse pos
        if (directionRelativeToMouse)
            direction = Vector3.Normalize(new Vector3(mouseClickPos.x, mouseClickPos.y, 0) - new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0));
        // Get direction relative to player posititon
        else
            direction = Vector3.Normalize(new Vector3(transform.position.x, transform.position.y, 0) - new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0));

        return new Vector2(direction.x, Mathf.Clamp(direction.y, 0, 1));
    }
}






