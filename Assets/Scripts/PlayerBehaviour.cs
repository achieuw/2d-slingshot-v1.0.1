using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    PlayerController player;
    public Animation groundColAnim;
    [SerializeField] GameObject gfx;
    Vector3 initialScale;

    private void Start()
    {
        player = GetComponent<PlayerController>();
        initialScale = gfx.transform.localScale;
    }

    private void Update()
    {
        gfx.transform.localScale = new Vector3(initialScale.x - Mathf.Abs(player.Velocity.y) * 0.005f, initialScale.y + Mathf.Abs(player.Velocity.y) * 0.01f, 0.5f);

        if(Mathf.Abs(player.Velocity.x) > 2)
            gfx.transform.rotation = new Quaternion(0, 0, -player.Velocity.x * 0.02f, 1);
        else
            gfx.transform.rotation = new Quaternion(0, 0, 0, 1);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Waterfall"))
        {
            player.Velocity = new Vector2(0, player.Velocity.y - 60 * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            groundColAnim.Play();
        }
    }
}
