using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;

    public int score { get; private set; }
    public int ghostMultiplier = 1;
    private Vector3 positionStart;

    public bool pacmanEaten = false;

    public Toggle toggle;
    public float SpeedUpValue = 1f;

    public Text Speedtext;

    public Text numberOfGameText;
    public int numberOfGame;


    public void Start()
    {
        this.toggle = GameObject.Find("Toggle").GetComponent<Toggle>();
        positionStart = this.pacman.gameObject.transform.position;
        numberOfGame = 0;
        NewGame();
    }

    void Update()
    {
        if (Input.GetKeyDown("up") && SpeedUpValue<64)
           SpeedUpValue *= 2;

        if (Input.GetKeyDown("down"))
            SpeedUpValue *= 0.5f;

        if(!toggle.isOn)
            Time.timeScale = 1;
        else
            Time.timeScale = SpeedUpValue;

        Speedtext.text = "X " + SpeedUpValue.ToString();
    }

    private void NewGame()
    {
        numberOfGame++;
        numberOfGameText.text = "Game N° : " + numberOfGame.ToString();
        SetScore(0);
        NewRound();
    }

    public void NewRound()
    {
        foreach (Transform pellets in this.pellets)
        {
            pellets.gameObject.SetActive(true);
        }
        ResetState();
    }

    private void ResetState()
    {
        ResetGhostMultiplier();

        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].ResetState();
        }

        this.pacman.gameObject.SetActive(true);
        this.pacman.gameObject.transform.position = positionStart;
        this.pacmanEaten = false;
    }

    private void GameOver()
    {
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false);

        NewGame();
    }

    public void GhostEaten(Ghost ghost)
    {
        SetScore(this.score + ghost.points * ghostMultiplier);
        ghostMultiplier++;
    }

    public void PacmanEaten()
    {
        pacmanEaten = true;
        GameOver();
    }

    public void SetScore(int score)
    {
        this.score = score;
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);
        SetScore(score + pellet.points);

        if(!HasRemainingPellets())
        {
            // Tell IA it won
            Movement.stopMoving();
            GameOver();
        }
    }

    public void PowerPelletEatent(PowerPellet pellet)
    {
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].frightened.Enable(pellet.duration);
        }

        CancelInvoke();
        Invoke(nameof(ResetGhostMultiplier), 3.0f);
        PelletEaten(pellet);
    }

    public bool HasRemainingPellets()
    {
        foreach (Transform pellets in this.pellets)
        {
            if (pellets.gameObject.activeSelf)
                return true;   
        }
        return false;
    }

    public void ResetGhostMultiplier()
    {
       ghostMultiplier = 1;
    }
}
