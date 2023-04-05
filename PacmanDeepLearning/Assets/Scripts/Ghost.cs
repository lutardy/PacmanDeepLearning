using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Movement))]

public class Ghost : MonoBehaviour
{
    
    public Movement movement { get; private set; }
    public GhostHome home { get; private set; }
    public GhostScatter scatter { get; private set; }
    public GhostChase chase { get; private set; }
    public GhostFrightened frightened { get; private set; }

    public Toggle toggle;

    public GhostBehavior initialBehavior;

    public Transform target;

    public int points = 200;

    public void Awake() {
        this.movement = GetComponent<Movement>();
        this.home = GetComponent<GhostHome>();
        this.scatter = GetComponent<GhostScatter>();
        this.chase = GetComponent<GhostChase>();
        this.frightened = GetComponent<GhostFrightened>();
        this.toggle = GameObject.Find("Toggle").GetComponent<Toggle>();
    }

    public void Start() {
        ResetState();
    }

    private void Update(){
        if(toggle.isOn){
            Time.timeScale = 30;
        }
        else{
            Time.timeScale = 1;
        }
    }

    public void ResetState() {
        this.gameObject.SetActive(true);
        this.movement.ResetState();

        this.frightened.Disable();
        this.chase.Disable();
        this.scatter.Enable();
        
        if(home != this.initialBehavior)
        {
            home.Disable();
        }

        if(initialBehavior != null)
        {
            initialBehavior.Enable();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            if (frightened.enabled)
            {
                FindObjectOfType<GameManager>().GhostEaten(this);
            }
            else
            {
                FindObjectOfType<GameManager>().PacmanEaten();
            }
        }
    }

    public void SetPosition(Vector3 position)
    {
        position.z = transform.position.z;
        transform.position = position;
    }
}