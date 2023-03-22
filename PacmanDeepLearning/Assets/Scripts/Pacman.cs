using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]

public class Pacman : MonoBehaviour{
    
    public Movement movement { get; private set; }
    public bool isRotating { get; private set; }
    private void Awake(){
        this.movement = GetComponent<Movement>();
    }

    private void Update(){
        Vector2[] directions = {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right
        };

        this.movement.SetDirection(directions[Random.Range(0, directions.Length)]);

        if (isRotating)
        {
            float angle = Mathf.Atan2(this.movement.currentDirection.y, this.movement.currentDirection.x);
            this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
        }
    }

    public void ResetState()
    {
        this.gameObject.SetActive(true);
        this.movement.ResetState();
    }

    public void resetRotation()
    {
        this.transform.rotation = Quaternion.identity;
    }

    public void stopRotating()
    {
        this.isRotating = false;
    }

    public void startRotating()
    {
        this.isRotating = true;
    }
}
