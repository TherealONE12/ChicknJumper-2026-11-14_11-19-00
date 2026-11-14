using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor.Build.Reporting;

public static class DemoBuild
{
    public const string ScenePath = "Assets/Scenes/PlayableDemo.unity";
    private const string MenuPath = "Assets/Scenes/HomeScreen.unity";
    private static Font Font => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    private static Sprite square;

    [MenuItem("ChicknJumper/Rebuild playable demo")]
    public static void Create()
    {
        square = AssetDatabase.LoadAllAssetsAtPath("Assets/scripts/Square.png").OfType<Sprite>().First();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        var camera = cameraObject.GetComponent<Camera>();
        camera.transform.position = new Vector3(0f, 0f, -10f);
        camera.orthographic = true;
        camera.orthographicSize = 5.4f;
        camera.backgroundColor = new Color(0.12f, 0.08f, 0.06f);

        var background = AssetDatabase.LoadAllAssetsAtPath("Assets/static/Background_Scheune_3840x2160.png").OfType<Sprite>().First();
        var barn = SpriteObject("Scheune", background, Vector2.zero, Color.white, -10);
        barn.transform.localScale = new Vector3(19.2f / background.bounds.size.x, 10.8f / background.bounds.size.y, 1f);
        var shade = Box("Hintergrund abdunkeln", Vector2.zero, new Vector2(19.2f, 10.8f), new Color(0.09f, 0.05f, 0.03f, 0.32f), -9);

        CreateWall("Linke Wand", new Vector2(-8.25f, 0f), new Vector2(0.3f, 8.1f));
        CreateWall("Rechte Wand", new Vector2(8.25f, 0f), new Vector2(0.3f, 8.1f));
        CreateWall("Decke", new Vector2(0f, 3.95f), new Vector2(16.8f, 0.3f));
        CreateWall("Sprungboden", new Vector2(0f, -3.95f), new Vector2(16.8f, 0.3f));

        // Existing artwork has a large transparent canvas. Import cropped sprite
        // rectangles as separate assets, leaving the artists' source files intact.
        Directory.CreateDirectory("Assets/DemoArt");
        Sprite[] frames = {
            ImportChicken("07_Flattern_1", new Rect(2400, 800, 1600, 1600)),
            ImportChicken("08_Flattern_2", new Rect(2300, 800, 1700, 1600)),
            ImportChicken("09_Flattern_3", new Rect(2400, 800, 1600, 1800))
        };
        var chickenObject = SpriteObject("Hendl", frames[0], new Vector2(-4f, 1.5f), Color.white, 5);
        chickenObject.transform.localScale = Vector3.one * 0.85f;
        chickenObject.AddComponent<Rigidbody2D>();
        var chickenCollider = chickenObject.AddComponent<CircleCollider2D>();
        chickenCollider.radius = 0.42f;
        var ball = chickenObject.AddComponent<Ball>();

        // Replace the branch's square placeholder with a readable trampoline.
        var trampoline = Box("Trampoline", Vector2.zero, new Vector2(2.2f, 0.22f), new Color(0.35f, 0.82f, 0.83f), 3);
        trampoline.AddComponent<BoxCollider2D>();
        trampoline.AddComponent<BounceSurface>();
        GameObject trampolinePrefab = PrefabUtility.SaveAsPrefabAsset(trampoline, "Assets/scripts/Trampoline.prefab");
        UnityEngine.Object.DestroyImmediate(trampoline);

        var canvasObject = new GameObject("Demo UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.matchWidthOrHeight = 0.5f;
        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        Text title = Label(canvas.transform, "Titel", "CHICKN JUMPER  /  DEMO", new Vector2(-420f, 323f), new Vector2(390f, 42f), 26);
        title.alignment = TextAnchor.MiddleLeft;
        Text score = Label(canvas.transform, "Sprünge", "SPRÜNGE  000", new Vector2(430f, 323f), new Vector2(350f, 42f), 26);
        score.alignment = TextAnchor.MiddleRight;
        Label(canvas.transform, "Steuerung", "Klick / Touch: Trampolin setzen    ·    R: Neustart    ·    Leertaste: Pause    ·    Esc: Menü", new Vector2(0f, -323f), new Vector2(1230f, 34f), 19);
        Text state = Label(canvas.transform, "Status", "", Vector2.zero, new Vector2(1000f, 150f), 34);
        state.gameObject.AddComponent<Outline>().effectColor = Color.black;

        var controller = new GameObject("Demo Controller");
        controller.SetActive(false);
        var placement = controller.AddComponent<TrampolinePlacement>();
        SetObject(placement, "prefab", trampolinePrefab);
        SetObject(placement, "cam", camera);
        SetObject(placement, "chicken", ball);
        var demo = controller.AddComponent<PlayableDemo>();
        SetObject(demo, "chicken", ball);
        SetObject(demo, "placement", placement);
        SetObject(demo, "chickenRenderer", chickenObject.GetComponent<SpriteRenderer>());
        SetObject(demo, "scoreLabel", score);
        SetObject(demo, "stateLabel", state);
        var serializedDemo = new SerializedObject(demo);
        var frameArray = serializedDemo.FindProperty("flyingFrames");
        frameArray.arraySize = frames.Length;
        for (int i = 0; i < frames.Length; i++) frameArray.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
        serializedDemo.ApplyModifiedPropertiesWithoutUndo();
        Button(canvas.transform, "Neustart", new Vector2(-190f, -278f), demo.Restart);
        Button(canvas.transform, "Pause", new Vector2(0f, -278f), demo.TogglePause);
        Button(canvas.transform, "Menü", new Vector2(190f, -278f), demo.MainMenu);
        controller.SetActive(true);
        EditorSceneManager.SaveScene(scene, ScenePath);

        WireMenu();
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(MenuPath, true), new EditorBuildSettingsScene(ScenePath, true) };
        PlayerSettings.productName = "ChicknJumper Demo";
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        AssetDatabase.SaveAssets();
        EditorSceneManager.OpenScene(ScenePath);
        Debug.Log("DEMO_CREATED: " + ScenePath);
    }

    private static Sprite ImportChicken(string sourceName, Rect rect)
    {
        string path = "Assets/DemoArt/" + sourceName + ".png";
        File.Copy("Assets/moving/Startsequenz/" + sourceName + ".png", path, true);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.maxTextureSize = 2048;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.spritePixelsPerUnit = 1600f;
#pragma warning disable CS0618
        importer.spritesheet = new[] { new SpriteMetaData { name = sourceName, rect = rect, alignment = (int)SpriteAlignment.Center, pivot = new Vector2(0.5f, 0.5f) } };
#pragma warning restore CS0618
        importer.SaveAndReimport();
        return AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().First();
    }

    private static void WireMenu()
    {
        Scene menu = EditorSceneManager.OpenScene(MenuPath);
        var canvas = UnityEngine.Object.FindAnyObjectByType<Canvas>();
        var home = canvas.GetComponent<HomeScreenStuff>() ?? canvas.gameObject.AddComponent<HomeScreenStuff>();
        Bind("PlayButton", home.Play);
        Bind("CreditsButton", home.Credits);
        Bind("ExitButton", home.Exit);
        var previousPanel = canvas.transform.Find("Credits Panel");
        if (previousPanel != null) UnityEngine.Object.DestroyImmediate(previousPanel.gameObject);
        var panel = new GameObject("Credits Panel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        var rect = panel.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(480f, 180f);
        panel.GetComponent<Image>().color = new Color(0.08f, 0.06f, 0.05f, 0.98f);
        Label(panel.transform, "Credits", "CHICKN JUMPER\\nJakob · SeeSee · Paul\\n\\nCredits oder Esc zum Schließen", Vector2.zero, new Vector2(450f, 160f), 24);
        panel.SetActive(false);
        SetObject(home, "creditsPanel", panel);
        EditorSceneManager.SaveScene(menu);
    }

    private static void Bind(string name, UnityAction action)
    {
        var button = GameObject.Find(name).GetComponent<UnityEngine.UI.Button>();
        button.onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
        UnityEventTools.AddPersistentListener(button.onClick, action);
        EditorUtility.SetDirty(button);
    }

    private static GameObject SpriteObject(string name, Sprite sprite, Vector2 position, Color color, int order)
    {
        var obj = new GameObject(name, typeof(SpriteRenderer));
        obj.transform.position = position;
        var renderer = obj.GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingOrder = order;
        renderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
        return obj;
    }

    private static GameObject Box(string name, Vector2 position, Vector2 size, Color color, int order)
    {
        var obj = SpriteObject(name, square, position, color, order);
        obj.transform.localScale = new Vector3(size.x / square.bounds.size.x, size.y / square.bounds.size.y, 1f);
        return obj;
    }

    private static void CreateWall(string name, Vector2 position, Vector2 size)
    {
        var wall = Box(name, position, size, new Color(0.63f, 0.39f, 0.19f), 1);
        wall.AddComponent<BoxCollider2D>();
        wall.AddComponent<BounceSurface>();
    }

    private static Text Label(Transform parent, string name, string text, Vector2 position, Vector2 size, int fontSize)
    {
        var obj = new GameObject(name, typeof(RectTransform), typeof(Text));
        obj.transform.SetParent(parent, false);
        var rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        var label = obj.GetComponent<Text>();
        label.font = Font;
        label.text = text.Replace("\\n", "\n");
        label.fontSize = fontSize;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = new Color(1f, 0.95f, 0.8f);
        label.raycastTarget = false;
        return label;
    }

    private static void Button(Transform parent, string title, Vector2 position, UnityAction action)
    {
        var obj = new GameObject(title, typeof(RectTransform), typeof(Image), typeof(UnityEngine.UI.Button));
        obj.transform.SetParent(parent, false);
        var rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(170f, 36f);
        obj.GetComponent<Image>().color = new Color(0.17f, 0.12f, 0.08f, 0.95f);
        UnityEventTools.AddPersistentListener(obj.GetComponent<UnityEngine.UI.Button>().onClick, action);
        Label(obj.transform, title + " Text", title, Vector2.zero, rect.sizeDelta, 20);
    }

    private static void SetObject(UnityEngine.Object target, string property, UnityEngine.Object value)
    {
        var serialized = new SerializedObject(target);
        serialized.FindProperty(property).objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    [MenuItem("ChicknJumper/Build Linux demo")]
    public static void BuildLinux()
    {
        var result = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = new[] { MenuPath, ScenePath }, locationPathName = "Builds/Linux/ChicknJumper.x86_64",
            target = BuildTarget.StandaloneLinux64, options = BuildOptions.Development
        });
        if (result.summary.result != BuildResult.Succeeded) throw new Exception("Demo build failed: " + result.summary.result);
        Debug.Log("DEMO_BUILD_OK: " + result.summary.outputPath);
    }
}
