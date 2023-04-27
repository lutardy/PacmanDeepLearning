using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Movement))]

public class Pacman : MonoBehaviour{
    
    public Movement movement { get; private set; }
    public bool isRotating { get; private set; }

    public Transform LeftU;
    public Transform RightU;
    public Transform UpR;
    public Transform DownR;
    public Transform LeftD;
    public Transform RightD;
    public Transform UpL;
    public Transform DownL;

    public bool collidingNode = false;
    public Node currentNode;

    private void Awake(){
        this.movement = GetComponent<Movement>();
    }

    public void Update(){
        collidingNode = false;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        if (node != null && this.enabled)
        {
            collidingNode = true;
            currentNode = node;
        }
    }

    /*private void OnTriggerExit2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        if (node != null && this.enabled)
        {
            collidingNode = false;
        }
    }*/
}
