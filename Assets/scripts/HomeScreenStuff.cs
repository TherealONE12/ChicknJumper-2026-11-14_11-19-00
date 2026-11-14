using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HomeScreenStuff : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;

    private void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.enterKey.wasPressedThisFrame) Play();
        if (Keyboard.current.escapeKey.wasPressedThisFrame && creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    public void Play()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("PlayableDemo");
    }

    public void Credits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(!creditsPanel.activeSelf);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
