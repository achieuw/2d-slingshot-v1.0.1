using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    PlayerController player;
    public Animator animator;
    [SerializeField] GameObject gfx;
    Vector3 initialScale;
    public bool playerActive;

    private void Start()
    {
        playerActive = false;
        animator.Play("Base Layer.PlayerSpawn", -1, 0);
        player = GetComponent<PlayerController>();
        initialScale = gfx.transform.localScale;
    }

    private void Update()
    {
        if (!AnimatorIsPlaying())
            playerActive = true;

        gfx.transform.localScale = new Vector3(initialScale.x - Mathf.Abs(player.Velocity.y) * 0.005f, initialScale.y + Mathf.Abs(player.Velocity.y) * 0.01f, 0.5f);

        if(Mathf.Abs(player.Velocity.x) > 2)
            gfx.transform.rotation = new Quaternion(0, 0, -player.Velocity.x * 0.02f, 1);
        else
            gfx.transform.rotation = new Quaternion(0, 0, 0, 1);
    }

    bool AnimatorIsPlaying() { return animator.GetCurrentAnimatorStateInfo(0).length > animator.GetCurrentAnimatorStateInfo(0).normalizedTime; }

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
            if(playerActive)
                animator.Play("Base Layer.PlayerGroundCollision", 0, 0);
        }
    }
}
