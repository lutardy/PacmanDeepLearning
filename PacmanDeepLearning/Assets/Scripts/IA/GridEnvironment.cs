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

    void Start()
    {
        maxSteps = 100000;
        waitTime = 0.1f;
        BeginNewGame();
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

    }

    /// <summary>
    /// Established the Grid.
    /// </summary>
    public override void SetUp()
    {
        envParameters = new EnvironmentParameters()
        {
            observation_size = 0,
            state_size = 512,
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
        RunMdp();
    }

    /// <summary>
    /// Gets the agent's current position and transforms it into a discrete integer state.
    /// </summary>
    /// <returns>The state.</returns>
    public override List<float> collectState()
    {
        List<float> state = new List<float>();

        float point = 0;
        /*
         * if (Mathf.Abs(ghosts.First().transform.position.x - visualAgent.transform.position.x) > 5) //Loin ou non
        {
            point += 256f;
            if (ghosts.First().transform.position.x - visualAgent.transform.position.x > 0) // Droite ou Gauche
            {
                point += 256f;
            }

        }
        else if (Mathf.Abs(ghosts.First().transform.position.x - visualAgent.transform.position.x) > 1) // Neutre ou Non
        {
            point += 1f;
            if (ghosts.First().transform.position.x - visualAgent.transform.position.x > 0) // Droite ou Gauche
            {
                point += 1f;
            }
        }

        if (Mathf.Abs(ghosts.First().transform.position.y - visualAgent.transform.position.y) > 5)
        {
            point += 4f;
            if (ghosts.First().transform.position.y - visualAgent.transform.position.y > 0)
            {
                point += 4f;
            }
        }
        else if (Mathf.Abs(ghosts.First().transform.position.y - visualAgent.transform.position.y) > 1)
        {
            point += 1024f;
            if (ghosts.First().transform.position.y - visualAgent.transform.position.y > 0)
            {
                point += 1024f;
            }
        }

        */

        int obstacleLayer = visualAgent.GetComponent<Movement>().obstacleLayer;
        int ghostLayer = ghosts[0].gameObject.layer;

        if (Physics2D.OverlapArea(visualAgent.LeftU.position,visualAgent.LeftD.position, ghostLayer))
            point += 1f;
        if (Physics2D.OverlapArea(visualAgent.RightU.position,visualAgent.RightD.position, ghostLayer))
            point += 2f;
        if (Physics2D.OverlapArea(visualAgent.DownR.position,visualAgent.DownL.position, ghostLayer))
            point += 4f;
        if (Physics2D.OverlapArea(visualAgent.UpR.position,visualAgent.UpL.position, ghostLayer))
            point += 8f;

        if (Physics2D.OverlapArea(visualAgent.LeftU.position, visualAgent.LeftD.position, obstacleLayer))
            point += 16f;
        if (Physics2D.OverlapArea(visualAgent.RightU.position, visualAgent.RightD.position, obstacleLayer))
            point += 32f;
        if (Physics2D.OverlapArea(visualAgent.DownR.position, visualAgent.DownL.position, obstacleLayer))
            point += 64f;
        if (Physics2D.OverlapArea(visualAgent.UpR.position, visualAgent.UpL.position, obstacleLayer))
            point += 128f;

        if (gameManager.ghostEatable)
            point += 256f;

        //point = visualAgent.gameObject.transform.position.x - 12 + (visualAgent.gameObject.transform.position.y -10) * 30;
       // point += 450 * (ghosts[0].transform.position.x - 12 + (ghosts[0].transform.position.y - 10) * 30);

        state.Add(point);

        reward = gameManager.reward;
        if(gameManager.pacmanEaten || !gameManager.HasRemainingPellets() )
            done = true;

        return state;
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
    }

    /// <summary>
    /// Allows the agent to take actions, and set rewards accordingly.
    /// </summary>
    /// <param name="action">Action.</param>
    public override void MiddleStep(int action)
    {
        reward = 0.05f;
        // 0 - Forward, 1 - Backward, 2 - Left, 3 - Right
        if (action == 3)
        {
            visualAgent.movement.SetDirection(Vector2.right);
        }

        if (action == 2)
        {
            visualAgent.movement.SetDirection(Vector2.left);
        }

        if (action == 0)
        {
            visualAgent.movement.SetDirection(Vector2.up);
        }

        if (action == 1)
        {
            visualAgent.movement.SetDirection(Vector2.down);
        }
    }
}
