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
    float m_timer;
    [Header("Forces")]
    [SerializeField]
    float gravity;
    float m_gravity;
    [SerializeField]
    float drag;
    float m_drag;
    [SerializeField]
    bool directionRelativeToMouse;

    Vector2 mouseClickPos;

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
        Sling();
    }

    private void FixedUpdate()
    {
        HandlePhysics();
    }

    bool IsGrounded()
    {
        // Throws a raycast downwards from player and checks for the ground layermask
        if (Velocity.y <= 0 && Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2 + .1f, groundMask))
        {
            sling = true;
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
            if (Input.GetKeyDown(KeyCode.Mouse0) && sling)
            {
                mouseClickPos = MouseToWorldPos();
            }
            // Code block for sling mechanic
            if (Input.GetKey(KeyCode.Mouse0))
            {
                SetSlingValues(true, Vector2.zero, 0, 0.2f);
            }
            // Code block for sling release
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                m_drag = 0;
                sling = false;
                SetSlingValues(false, slingForce * Direction(), gravity, 1f);
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

    void SlingTimer()
    {
        if (m_timer > 0)
            return;

        sling = false;
        SetSlingValues(false, slingForce * Direction(), gravity, 1f);
        m_timer = slingTimer;
    }

    Vector2 MouseToWorldPos()
    {
        Vector2 mousePos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    void SetSlingValues(bool isSlinging, Vector2 velocity, float gravity, float timeScale)
    {
        ActivateDirectionGFX(isSlinging);
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
            dirLine.SetPosition(0, transform.position);
            dirLine.SetPosition(1, new Vector3(transform.position.x + Direction().x * 5, transform.position.y + Direction().y * 5, transform.position.z));
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






