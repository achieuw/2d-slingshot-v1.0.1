using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Adjustable parameters 
    [Header("Sling values")]
    [SerializeField]
    float slingForce;
    [SerializeField]
    float slingRadius;
    [SerializeField]
    float slingTimer;
    [SerializeField]
    GameObject ringTimerObject;
    [Header("Forces")]
    [SerializeField]
    float gravity;
    float m_gravity;
    [SerializeField]
    float drag;
    float m_drag;
    [SerializeField]
    bool directionRelativeToMouse;
    public bool enableSlinging = true;

    Vector2 mouseClickPos;
    Vector2 slingPos;

    // Debugging 
    [Header("Debug")]
    [SerializeField]
    bool sling = true;

    [SerializeField]
    Transform groundCheck;
    LayerMask groundMask;
    LayerMask wallMask;
    LineRenderer dirLine;
    Vector2 velocity;
    public Vector2 slingDir;

    [SerializeField]
    bool testPCG;

    public Vector2 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    private void Start()
    {
        GetComponent<CircleCollider2D>().radius = slingRadius; // Changes the radius of the circle that manages sling hitbox
        groundMask = LayerMask.GetMask("Ground"); // Layermask for collision with ground and walls
        wallMask = LayerMask.GetMask("Wall"); // Layermask for collision with ground and walls
        dirLine = GetComponentInChildren<LineRenderer>(); // Line renderer which handles the sling direction
        m_gravity = gravity;
        m_drag = drag;
    }

    private void Update()
    {
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

    private void FixedUpdate()
    {
        HandlePhysics();
    }

    bool IsGrounded()
    {
        // Throws a raycast downwards, left and right from player and checks for the ground or wall layermask
        if ((Velocity.y <= 0 && 
            Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2 + .1f, groundMask)) ||
            Physics2D.Raycast(transform.position, Vector2.right, transform.localScale.y / 2 + .1f, wallMask) ||
            Physics2D.Raycast(transform.position, Vector2.left, transform.localScale.y / 2 + .1f, wallMask))
        {
            sling = true;
            slingPos = transform.position;
            return true;
        }
        else
            return false;
    }

    void HandlePhysics()
    {
        // We only change the position horizontally since the vertical position is handled by the scrolling environment
        transform.position += new Vector3(Velocity.x, 0, 0) * Time.fixedDeltaTime;

        if (!IsGrounded())
            Velocity -= new Vector2(0, m_gravity) * Time.fixedDeltaTime;
        else
            Velocity = Vector2.zero;

        // Drag is added based on the direction of the sling. A more horizontal sling adds drag more quickly and vice versa
        m_drag = (Mathf.Abs(slingDir.x) * drag - m_drag * 0.01f) * Time.fixedDeltaTime;

        Vector2 temp_drag = new Vector2(m_drag, 0);
        if (Velocity.x < -0.2f)
            Velocity += temp_drag;
        else if (Velocity.x > 0.2f)
            Velocity -= temp_drag;
        else
            Velocity = new Vector2(0, Velocity.y);
    }

    // Enables slinging when entering sling radius (change this to detect colliders from sling game object in update?)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Slinger"))
        {
            sling = true;
            slingPos = collision.gameObject.transform.position;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Slinger"))
        {
            sling = false;
        }
    }

    bool CanSling() => sling ? true : false;

    // ---- REFACTOR --------
    void Sling()
    {
        // Executes if player is able to sling
        if (CanSling())
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                mouseClickPos = MouseToWorldPos();
            }
            // Code block for sling mechanic
            if (Input.GetKey(KeyCode.Mouse0))
            {
                SlingTimer(true);             
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
            SetSlingValues(false, Velocity, gravity, 1f);
        }
    }

    void SlingTimer(bool isSlinging)
    {       
        if (isSlinging && ringTimerObject.transform.localScale.x > 0)
        {
            SetSlingValues(true, Vector2.zero, 0, 0.2f);
            ActivateDirectionGFX(isSlinging);
            ringTimerObject.transform.localScale -= Vector3.one * 2 * Time.deltaTime / slingTimer;
            ringTimerObject.GetComponent<SpriteRenderer>().color += new Color(-50, 50, 0, 0) * Time.deltaTime;
        }
        else
        {
            sling = false;
            isSlinging = false;
            SetSlingValues(false, slingForce * Direction(), gravity, 1f);
            ringTimerObject.transform.localScale = Vector3.one * 0.5f;
            ringTimerObject.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 1);          
        }
        ActivateDirectionGFX(isSlinging);
        ringTimerObject.SetActive(isSlinging);
    }

    Vector2 MouseToWorldPos()
    {
        Vector2 mousePos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    void SetSlingValues(bool isSlinging, Vector2 velocity, float gravity, float timeScale)
    {      
        Velocity = velocity;
        m_gravity = gravity;
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






