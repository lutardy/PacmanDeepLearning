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

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        SetScore(0);
        NewRound();
    }

    private void NewRound()
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
    }

    private void GameOver()
    {
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false);

        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void GhostEaten(Ghost ghost)
    {
        SetScore(this.score + ghost.points * ghostMultiplier);
        ghostMultiplier++;
    }

    public void PacmanEaten()
    {
        Movement.stopMoving();
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
