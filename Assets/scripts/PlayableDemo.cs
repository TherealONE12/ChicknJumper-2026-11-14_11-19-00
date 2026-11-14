using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayableDemo : MonoBehaviour
{
    [SerializeField] private Ball chicken;
    [SerializeField] private TrampolinePlacement placement;
    [SerializeField] private SpriteRenderer chickenRenderer;
    [SerializeField] private Sprite[] flyingFrames;
    [SerializeField] private Text scoreLabel;
    [SerializeField] private Text stateLabel;
    [SerializeField] private Vector2 spawnPosition = new Vector2(-4f, 1.5f);
    private bool paused;

    private void OnEnable()
    {
        Time.timeScale = 1f;
        chicken.Bounced += UpdateScore;
        chicken.Died += ShowGameOver;
        UpdateScore();
    }

    private void OnDisable()
    {
        chicken.Bounced -= UpdateScore;
        chicken.Died -= ShowGameOver;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.rKey.wasPressedThisFrame) Restart();
            if (keyboard.spaceKey.wasPressedThisFrame) TogglePause();
            if (keyboard.escapeKey.wasPressedThisFrame) MainMenu();
        }
        if (chicken.IsAlive && !paused && flyingFrames.Length > 0)
            chickenRenderer.sprite = flyingFrames[(int)(Time.time * 8f) % flyingFrames.Length];
        if (chicken.IsAlive && (Mathf.Abs(chicken.transform.position.x) > 10f || Mathf.Abs(chicken.transform.position.y) > 7f))
            chicken.Die();
    }

    private void UpdateScore() => scoreLabel.text = $"SPRÜNGE  {chicken.BounceCount:000}";
    private void ShowGameOver() => stateLabel.text = "Hendl gelandet!\nR oder Neustart für die nächste Runde";

    public void Restart()
    {
        Time.timeScale = 1f;
        paused = false;
        placement.ClearTrampoline();
        chicken.Launch(spawnPosition);
        stateLabel.text = "";
        UpdateScore();
    }

    public void TogglePause()
    {
        if (!chicken.IsAlive) return;
        paused = !paused;
        Time.timeScale = paused ? 0f : 1f;
        stateLabel.text = paused ? "PAUSE\nLeertaste oder Pause zum Weiterspielen" : "";
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HomeScreen");
    }
}
