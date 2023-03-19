using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class Movement : MonoBehaviour {

    public static bool isMoving { get; private set; } = true;
    public Rigidbody2D rigidbody { get; private set; }
    public float speed = 8.0f;
    public float speedMultiplier = 1.0f;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;
    public Vector2 currentDirection { get; private set; }
    public Vector2 nextDirection { get; private set; }
    public Vector3 initialPosition { get; private set; }

    private void Awake(){

        this.rigidbody = GetComponent<Rigidbody2D>();
        this.initialPosition = this.transform.position;
    }

    private void Start() {
        ResetState();
    }

    public void ResetState() {

        this.speedMultiplier = 1.0f;
        this.currentDirection = this.initialDirection;
        this.nextDirection = Vector2.zero;
        this.transform.position = this.initialPosition;
        this.rigidbody.isKinematic = false;
        this.enabled = true;
    }

    private void Update() {
        if (this.nextDirection != Vector2.zero) {
            SetDirection(this.nextDirection);
        }
    }

    public void FixedUpdate(){
        if (!isMoving) {
            return;
        }

        Vector2 currentPosition = this.rigidbody.position;
        Vector2 currentTranslation = this.currentDirection * this.speed * this.speedMultiplier * Time.fixedDeltaTime;

        
        this.rigidbody.MovePosition(currentPosition + currentTranslation);
    }

    public void SetDirection(Vector2 direction, bool forced = false){
        if (!Wall(direction) || forced){
            this.currentDirection = direction;
            this.nextDirection = Vector2.zero;
        } else {
            this.nextDirection = direction;
        }

    }

    public bool Wall(Vector2 direction) {
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.75f, 0.0f, direction, 1.0f, this.obstacleLayer);
        return hit.collider != null;
    }

    public static void startMoving()
    {
        isMoving = true;
    }

    public static void stopMoving()
    {
        isMoving = false;
    }



}
