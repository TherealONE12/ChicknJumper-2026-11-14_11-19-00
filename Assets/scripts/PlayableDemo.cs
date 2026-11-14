using System.Collections;
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
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private GameObject turbine;
    private Coroutine launchSequence;

    private void Start() => Restart();

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
            if (keyboard.escapeKey.wasPressedThisFrame) MainMenu();
        }
        if (chicken.IsAlive && flyingFrames.Length > 0)
            chickenRenderer.sprite = flyingFrames[(int)(Time.time * 8f) % flyingFrames.Length];
        if (turbine.activeSelf && chicken.transform.position.x - turbine.transform.position.x > Camera.main.orthographicSize * Camera.main.aspect * 2f)
            turbine.SetActive(false);
        if (chicken.IsAlive && chicken.transform.position.y < -7f)
            chicken.Die();
    }

    private void UpdateScore()
    {
        if (scoreLabel != null) scoreLabel.text = $"Score: {chicken.BounceCount:000}";
    }
    private void ShowGameOver() => stateLabel.text = $"The Chicken has Landet! Retry with R for a new attempt! Score: {chicken.BounceCount:000}";

    public void Restart()
    {
        Time.timeScale = 1f;
        placement.ClearTrampoline();
        if (launchSequence != null) StopCoroutine(launchSequence);
        launchSequence = StartCoroutine(LaunchFromTurbine());
    }

    private IEnumerator LaunchFromTurbine()
    {
        chicken.Launch(spawnPosition);
        chicken.GetComponent<Rigidbody2D>().simulated = false;
        placement.enabled = false;
        turbine.SetActive(true);
        turbine.transform.position = new Vector3(spawnPosition.x - 1.7f, spawnPosition.y, 0f);
        stateLabel.text = "";
        UpdateScore();
        float elapsed = 0f;
        while (elapsed < 0.9f)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / 0.9f);
            chicken.transform.position = (Vector3)spawnPosition + Vector3.left * (2.2f * (1f - t * t));
            yield return null;
        }
        chicken.Launch(spawnPosition);
        placement.enabled = true;
        launchSequence = null;
        if (cameraFollow != null) cameraFollow.SnapToTarget();
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HomeScreen");
    }
}
