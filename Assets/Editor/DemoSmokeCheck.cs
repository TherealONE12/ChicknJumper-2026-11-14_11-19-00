using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Runs the real scene in Play Mode, including 2D physics and serialized UI callbacks.
public static class DemoSmokeCheck
{
    private static int stage;
    private static float deadline;
    private static int errors;
    private static double startedAt;
    private const string Running = "ChicknJumper.DemoSmokeCheck";

    public static void Run()
    {
        SessionState.SetBool(Running, true);
        EditorSceneManager.OpenScene(DemoBuild.ScenePath);
        EditorApplication.isPlaying = true;
    }

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (!SessionState.GetBool(Running, false)) return;
        startedAt = EditorApplication.timeSinceStartup;
        Application.logMessageReceived += OnLog;
        EditorApplication.update += Tick;
    }

    private static void OnLog(string message, string stack, LogType type)
    {
        // Unity 6.6's editor search index can throw during batch startup.
        if (stack.Contains("UnityEditor.Search.")) return;
        if (type == LogType.Exception || type == LogType.Error || type == LogType.Assert) errors++;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private static void Tick()
    {
        try
        {
            Require(EditorApplication.timeSinceStartup - startedAt < 90, "Play Mode smoke check timed out");
            if (!EditorApplication.isPlaying || !Application.isPlaying) return;
            var chicken = UnityEngine.Object.FindAnyObjectByType<Ball>();
            var demo = UnityEngine.Object.FindAnyObjectByType<PlayableDemo>();
            var placement = UnityEngine.Object.FindAnyObjectByType<TrampolinePlacement>();
            if (stage == 0)
            {
                if (chicken == null || !chicken.IsAlive) return;
                Time.timeScale = 10f;
                deadline = Time.time + 20f;
                stage = 1;
            }
            else if (stage == 1 && Time.time >= deadline)
            {
                Require(chicken.IsAlive && chicken.BounceCount >= 8, "Chicken did not keep bouncing in the arena");
                Require(Mathf.Abs(chicken.transform.position.x) < 8.3f && Mathf.Abs(chicken.transform.position.y) < 4f, "Chicken escaped arena");
                Require(Mathf.Abs(chicken.GetComponent<Rigidbody2D>().linearVelocity.magnitude - 5f) < 0.1f, "Bounce speed drifted");
                Require(chicken.GetComponent<SpriteRenderer>().sprite != null, "Missing chicken sprite");
                chicken.Launch(new Vector2(0f, 2f));
                Require(placement.TryPlaceWorld(new Vector2(2f, -0.5f)), "Could not place trampoline");
                Require(placement.PlacedTrampoline.GetComponent<BounceSurface>() != null, "Trampoline is not bouncy");
                Require(!placement.TryPlaceWorld(chicken.transform.position), "Trampoline can spawn inside chicken");
                deadline = Time.time + 1f;
                stage = 2;
            }
            else if (stage == 2 && Time.time >= deadline)
            {
                Require(chicken.BounceCount > 0 && chicken.GetComponent<Rigidbody2D>().linearVelocity.y > 0f, "Placed trampoline did not bounce chicken upwards");
                Require(placement.TryPlaceWorld(new Vector2(-4f, 1f)), "Replacing trampoline failed");
                demo.TogglePause();
                Require(Time.timeScale == 0f, "Pause failed");
                demo.Restart();
                Require(Time.timeScale == 1f && chicken.BounceCount == 0 && placement.PlacedTrampoline == null, "Restart did not reset state");
                Require(Vector2.Distance(chicken.GetComponent<Rigidbody2D>().position, new Vector2(-4f, 1.5f)) < 0.01f, "Restart spawn incorrect: " + chicken.GetComponent<Rigidbody2D>().position);
                chicken.Die();
                Require(!chicken.IsAlive && !chicken.GetComponent<Rigidbody2D>().simulated, "Death did not stop chicken");
                demo.Restart();
                Require(chicken.IsAlive && chicken.GetComponent<Rigidbody2D>().simulated, "Restart after death failed");
                Require(errors == 0, "Runtime logged errors");
                demo.MainMenu();
                stage = 3;
            }
            else if (stage == 3 && SceneManager.GetActiveScene().name == "HomeScreen")
            {
                var home = UnityEngine.Object.FindAnyObjectByType<HomeScreenStuff>();
                Require(home != null, "Menu controller missing");
                var button = GameObject.Find("PlayButton").GetComponent<Button>();
                Require(button.onClick.GetPersistentEventCount() == 1, "Play button is not wired");
                button.onClick.Invoke();
                stage = 4;
            }
            else if (stage == 4 && SceneManager.GetActiveScene().name == "PlayableDemo" && chicken != null && chicken.IsAlive)
            {
                Require(errors == 0, "Scene transitions logged errors");
                SessionState.SetBool(Running, false);
                Debug.Log("DEMO_SMOKE_OK: automatic bouncing, constant speed, trampoline collision and replacement, overlap rejection, pause, reset, death recovery, menu and Play button");
                EditorApplication.Exit(0);
            }
        }
        catch (Exception exception)
        {
            SessionState.SetBool(Running, false);
            Debug.LogError("DEMO_SMOKE_FAILED: " + exception);
            EditorApplication.Exit(1);
        }
    }
}
