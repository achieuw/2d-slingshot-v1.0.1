using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    [Header("Forces")]
    public float gravity;
    float m_gravity;
    public float drag;
    float m_drag;
    Vector2 m_velocity = Vector2.zero;
    [SerializeField][Tooltip("0 = No bounce, 1 = Force inverted")][Range(0, 2)]
    public float wallBounceForce;

    public Vector2 Velocity
    {
        get { return m_velocity; }
        set { m_velocity = value; }
    }

    public float Gravity
    {
        get { return m_gravity; }
        set { m_gravity = value; }
    }

    private void Start()
    {
        m_gravity = gravity;
        m_drag = drag;
    }
    public void HandlePhysics(bool grounded)
    {
        // We only change the position horizontally since the vertical position is handled by the scrolling environment
        transform.position += new Vector3(Velocity.x, 0, 0) * Time.fixedDeltaTime;

        SetGravity(grounded);
        Drag();     
    }
    void SetGravity(bool grounded)
    {
        if (!grounded)
        {
            Velocity -= new Vector2(0, m_gravity) * Time.fixedDeltaTime;
        }        
        else
        {
            Velocity = new Vector2(Velocity.x, 0);
        }
    }
    private void Drag()
    {
        // Drag is added based on the direction of the sling. A more horizontal sling adds drag more quickly and vice versa
        m_drag = (Mathf.Abs(Velocity.x) * drag - m_drag * 0.01f) * Time.fixedDeltaTime;
        Vector2 temp_drag = new Vector2(m_drag, 0);

        if (Velocity.x < -0.2f)
            Velocity += temp_drag;
        else if (Velocity.x > 0.2f)
            Velocity -= temp_drag;
        else
            Velocity = new Vector2(0, Velocity.y);
    }
    public void InvertForce(Vector2 bounceForce)
    {
        Velocity = new Vector2(-bounceForce.x, -bounceForce.y);
    }    
}
