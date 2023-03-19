using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
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

    }

    public void PacmanEaten()
    {
        Movement.stopMoving();
        GameOver();
    }
}
