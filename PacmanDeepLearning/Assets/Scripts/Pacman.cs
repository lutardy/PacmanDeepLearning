using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Movement))]

public class Pacman : MonoBehaviour{
    
    public Movement movement { get; private set; }
    public bool isRotating { get; private set; }

    public Toggle toggle;
    private void Awake(){
        this.movement = GetComponent<Movement>();
        this.toggle = GameObject.Find("Toggle").GetComponent<Toggle>();
    }

    public void ResetState()
    {
        this.gameObject.SetActive(true);
        this.movement.ResetState();
    }

    private void Update(){
        if(toggle.isOn){
            Time.timeScale = 30;
        }
        else{
            Time.timeScale = 1;
        }
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
