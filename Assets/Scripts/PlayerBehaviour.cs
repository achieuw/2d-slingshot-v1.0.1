using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    PlayerController player;
    [SerializeField] Animation groundColAnim;
    [SerializeField] GameObject gfx;
    bool wallCol = false;
    Vector3 initialScale;

    private void Start()
    {
        player = GetComponent<PlayerController>();
        initialScale = gfx.transform.localScale;
    }

    private void Update()
    {
        // PlayerAnimation(gfx, Vector3.zero, Vector3.zero, 1f);
        gfx.transform.localScale = new Vector3(initialScale.x - Mathf.Abs(player.Velocity.y) * 0.005f, initialScale.y + Mathf.Abs(player.Velocity.y) * 0.01f, 0.5f);
        gfx.transform.rotation = new Quaternion(0, 0, -player.Velocity.x * 0.02f, 1);
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

        if (collision.CompareTag("Wall"))
        {
            wallCol = true;
        }
    }

    public void PlayerAnimation(GameObject objectToAnim, Vector3 midPos, Vector3 midScale, float timeInSeconds)
    {  
        if (wallCol == true)
        {
            if (objectToAnim.transform.localScale.x >= 0.5f)
                objectToAnim.transform.localScale -= new Vector3(Time.deltaTime * 3f, -Time.deltaTime * 3f, 0);
            else
                wallCol = false;
        }
        else if(wallCol = false && objectToAnim.transform.localScale.x < 0.5f)
        {
            objectToAnim.transform.localScale += new Vector3(Time.deltaTime * 3f , -Time.deltaTime * 3f, 0);
        }
        else
            objectToAnim.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
}
