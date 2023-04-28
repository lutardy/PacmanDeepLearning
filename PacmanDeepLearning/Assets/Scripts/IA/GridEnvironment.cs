using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GridEnvironment : Environment
{

    public List<Ghost> ghosts;
    public Pacman visualAgent;
    float episodeReward;
    public GameManager gameManager;

    public LayerMask pelletLayer;

    public Node currentNode;
    
    int nbPellets;

    void Start()
    {
        maxSteps = 100000;
        waitTime = 0.01f;
        BeginNewGame();
        nbPellets = gameManager.nbPellet();
        currentNode = gameManager.pacman.currentNode;
    }

    /// <summary>
    /// Restarts the learning process with a new Grid.
    /// </summary>
    public void BeginNewGame()
    {
        gameManager.Start();
        SetUp();
        agent = new InternalAgent();
        agent.SendParameters(envParameters);
        Reset();
        RunMdp();
    }

    /// <summary>
    /// Established the Grid.
    /// </summary>
    public override void SetUp()
    {
        envParameters = new EnvironmentParameters()
        {
            observation_size = 0,
            state_size = 5,
            action_descriptions = new List<string>() { "Up", "Down", "Left", "Right" },
            action_size = 4,
            env_name = "Pacman",
            action_space_type = "discrete",
            state_space_type = "discrete",
            num_agents = 1
        };
        ghosts = new List<Ghost>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown("r")))
			Reset();
        if(gameManager.pacman.currentNode != currentNode){
            RunMdp();
            currentNode = gameManager.pacman.currentNode;
        }
    }

    /// <summary>
    /// Gets the agent's current position and transforms it into a discrete integer state.
    /// </summary>
    /// <returns>The state.</returns>
    public override List<float> collectState()
    {
        List<float> states = new List<float>();

        states.Add(this.gameManager.pacman.transform.position.x);
        states.Add(this.gameManager.pacman.transform.position.y);

        states.Add(this.gameManager.ghosts.First().transform.position.x);
        states.Add(this.gameManager.ghosts.First().transform.position.y);

        if(this.gameManager.ghostEatable)
            states.Add(1);
        else
            states.Add(0);

        states.Add(gameManager.NbPelletRemaining());

        RaycastHit2D hit = Physics2D.BoxCast(gameManager.pacman.transform.position, Vector2.one * 0.5f, 0.0f, Vector2.up, 1.0f, this.pelletLayer);
        if (hit.collider == null)
        {
            states.Add(1);
        }
        else{
            states.Add(0);
        }

        hit = Physics2D.BoxCast(gameManager.pacman.transform.position, Vector2.one * 0.5f, 0.0f, Vector2.down, 1.0f, this.pelletLayer);
        if (hit.collider == null)
        {
            states.Add(1);
        }
        else{
            states.Add(0);
        }

        hit = Physics2D.BoxCast(gameManager.pacman.transform.position, Vector2.one * 0.5f, 0.0f, Vector2.left, 1.0f, this.pelletLayer);
        if (hit.collider == null)
        {
            states.Add(1);
        }
        else{
            states.Add(0);
        }

        hit = Physics2D.BoxCast(gameManager.pacman.transform.position, Vector2.one * 0.5f, 0.0f, Vector2.right, 1.0f, this.pelletLayer);
        if (hit.collider == null)
        {
            states.Add(1);
        }
        else{
            states.Add(0);
        }
        /*float direction = 0;

        if(this.gameManager.pacman.movement.currentDirection == Vector2.right)
            direction = 4;
        if(this.gameManager.pacman.movement.currentDirection == Vector2.left)
            direction = 3;
        if(this.gameManager.pacman.movement.currentDirection == Vector2.up)
            direction = 2;
        if(this.gameManager.pacman.movement.currentDirection == Vector2.down)
            direction = 1;
        

        states.Add(direction);*/

        if(gameManager.NbPelletRemaining() < nbPellets)
            reward = 0.1f;
        /*else
            reward = -0.1f;*/

        nbPellets = gameManager.NbPelletRemaining();

        if(gameManager.pacmanEaten){
            reward = -1;
        }

        if(!gameManager.HasRemainingPellets()){
            reward = 1;
        }

        if(gameManager.pacmanEaten || !gameManager.HasRemainingPellets() ){
            done = true;
            Reset();
        }

        return states;
    }

    /// <summary>
    /// Resets the episode by placing the objects in their original positions.
    /// </summary>
    public override void Reset()
    {
        base.Reset();

        ghosts = new List<Ghost>();

        gameManager.NewGame();

        visualAgent = gameManager.pacman;

        foreach(Ghost ghost in gameManager.ghosts){
            ghosts.Add(ghost);
        }

        episodeReward = 0;
        EndReset();
        RunMdp();
    }

    /// <summary>
    /// Allows the agent to take actions, and set rewards accordingly.
    /// </summary>
    /// <param name="action">Action.</param>
    public override void MiddleStep(int action)
    {
        Vector2 newDirection = new Vector2(0, 0);

        while(newDirection.x == 0 && newDirection.y == 0){
            int random = Random.Range(0, 4);
            Vector2 direction = Vector2.up;
            if(random == 0)
                direction = Vector2.up;
            if(random == 1)
                direction = Vector2.down;
            if(random == 2)
                direction = Vector2.left;
            if(random == 3)
                direction = Vector2.right;
            if(gameManager.pacman.currentNode.availableDirections.Contains(direction))
                newDirection = direction;
        }
        //Debug.Log(action);
        //Debug.Log(newDirection);

        // 0 - Forward, 1 - Backward, 2 - Left, 3 - Right
        if (action == 3)
        {
            if(!gameManager.pacman.currentNode.availableDirections.Contains(Vector2.right)){
                visualAgent.movement.SetDirection(newDirection);
                //reward = -0.1f;
            }
                
            else 
                visualAgent.movement.SetDirection(Vector2.right);
        }

        if (action == 2)
        {
            if(!gameManager.pacman.currentNode.availableDirections.Contains(Vector2.left)){
                visualAgent.movement.SetDirection(newDirection);
                //reward = -0.1f;
            }
            else 
                visualAgent.movement.SetDirection(Vector2.left);
        }

        if (action == 0)
        {
            if(!gameManager.pacman.currentNode.availableDirections.Contains(Vector2.up)){
                visualAgent.movement.SetDirection(newDirection);
                //reward = -0.1f;
            }
            else 
                visualAgent.movement.SetDirection(Vector2.up);
        }

        if (action == 1)
        {
            if(!gameManager.pacman.currentNode.availableDirections.Contains(Vector2.down)){
                visualAgent.movement.SetDirection(newDirection);
                //reward = -0.1f;
            }
            else
                visualAgent.movement.SetDirection(Vector2.down);
        }
    }
}
