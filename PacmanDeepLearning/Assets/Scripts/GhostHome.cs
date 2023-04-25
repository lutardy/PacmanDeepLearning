using System.Collections;
using UnityEngine;

public class GhostHome : GhostBehavior
{
    public Transform inside;
    public Transform outside;

    private void OnEnable()
    {
        ExitTransition();
    }

    private void OnDisable()
    {
        if (this.gameObject.activeInHierarchy)
        {
            ExitTransition();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.enabled && collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            this.ghost.movement.SetDirection(-this.ghost.movement.currentDirection);
        }
    }

    private void ExitTransition()
    {
        this.ghost.transform.position = this.outside.position;
        this.ghost.movement.SetDirection(new Vector2(Random.value < 0.5f ? -1.0f : 1.0f, 0.0f), true) ;
        this.ghost.movement.rigidbody.isKinematic = false;
        this.ghost.movement.enabled = true;
    }
}
