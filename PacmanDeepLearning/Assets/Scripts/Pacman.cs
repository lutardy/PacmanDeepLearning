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
