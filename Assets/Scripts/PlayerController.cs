using UnityEngine;

public class PlayerController : GenericSingleton<PlayerController>
{
    [Header("Sling values")]
    [SerializeField]
    float m_slingForce;
    [SerializeField]
    float slingRadius;
    [Header("Forces")]
    [SerializeField]
    float gravity;
    [SerializeField] 
    float drag;
    [Header("Debug")]
    [SerializeField]
    bool m_movement = true;
    [SerializeField]
    bool grounded;
    [SerializeField]
    bool sling;

    LayerMask ground;
    Vector2 velocity = Vector2.zero;

    [SerializeField]
    Transform groundCheck;
    LineRenderer dirLine;

    public Vector2 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    private void Start()
    {
        GetComponent<CircleCollider2D>().radius = slingRadius;
        ground = LayerMask.GetMask("Wall");
        dirLine = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        Sling();

        if (Velocity.y > 0 && Physics2D.Raycast(transform.position, transform.position + groundCheck.position, transform.localScale.y / 2 + 0.01f, ground))
        {
            grounded = true;
            velocity = Vector2.zero;
            Debug.Log("collision with wall or ground");
        }
    }

    private void FixedUpdate()
    {  
        if(m_movement)
            transform.position -= new Vector3(Velocity.x, 0, 0) * Time.deltaTime;
        SetVelocity();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Slinger")
        {
            Time.timeScale = 1;
            sling = false;
        }  
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Slinger")
        {           
            sling = true;
        }
    }

    bool CanMove() => m_movement;

    void SetVelocity()
    {
        if(!grounded)
            velocity += Vector2.up * Time.deltaTime * gravity;
        if(!(velocity.x < 0.1f && velocity.x > -0.1f))
            velocity -= new Vector2(Direction().x, 0) * Time.deltaTime * drag;

        if (!CanMove())
        {
            velocity = Vector2.zero;
        }     
    }

    void Sling()
    {
        if (sling)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                dirLine.enabled = false;
                Time.timeScale = 1f;
                m_movement = true;
                grounded = false;
                velocity += Direction() * m_slingForce;
            }
            if (Input.GetKey(KeyCode.Mouse0))
            {
                dirLine.enabled = true;
                dirLine.SetPosition(0, transform.position);
                dirLine.SetPosition(1, new Vector3(transform.position.x + -Direction().x * 5, transform.position.y + -Direction().y * 5, transform.position.z));
                Time.timeScale = 0.1f;
                m_movement = false;
            }
        }
        else
        {
            m_movement = true;
            dirLine.enabled = false;
        }
            
    }
    public Vector2 Direction()
    {
        Vector3 direction;
        Vector2 mousePos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        direction = Vector3.Normalize(new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0) - new Vector3(transform.position.x, transform.position.y, 0));

        return new Vector2(direction.x, direction.y);
    }
}
