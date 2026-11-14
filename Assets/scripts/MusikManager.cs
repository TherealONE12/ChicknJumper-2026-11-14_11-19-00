using UnityEngine;
using UnityEngine.SceneManagement;

// Spielt die Hintergrundmusik über alle Szenen hinweg.
// Im Startscreen laut, im Spiel leiser. Alle X Sekunden kommt der Chicken-Sound.
public class MusikManager : MonoBehaviour
{
    [Header("Sounds")]
    public AudioClip musik;
    public AudioClip chickenSound;

    [Header("Startscreen")]
    public string startScreenName = "HomeScreen";

    [Header("Lautstärke")]
    [Range(0f, 1f)] public float lautStartscreen = 0.8f;
    [Range(0f, 1f)] public float lautImSpiel = 0.3f;
    [Range(0f, 1f)] public float chickenLautstaerke = 1f;

    [Header("Zeiten (Sekunden)")]
    public float chickenAbstand = 30f;
    public float ueberblendZeit = 1f;

    static MusikManager instanz;
    AudioSource musikQuelle;
    AudioSource effektQuelle;
    float zielLautstaerke;
    float timer;

    void Awake()
    {
        // Nur einen MusikManager behalten, sonst läuft die Musik doppelt
        if (instanz != null)
        {
            Destroy(gameObject);
            return;
        }
        instanz = this;
        DontDestroyOnLoad(gameObject);

        musikQuelle = gameObject.AddComponent<AudioSource>();
        musikQuelle.clip = musik;
        musikQuelle.loop = true;
        musikQuelle.playOnAwake = false;

        effektQuelle = gameObject.AddComponent<AudioSource>();
        effektQuelle.playOnAwake = false;

        SceneManager.sceneLoaded += SzeneGeladen;
        LautstaerkeSetzen(SceneManager.GetActiveScene().name);
        musikQuelle.volume = zielLautstaerke;
        musikQuelle.Play();
    }

    void OnDestroy()
    {
        if (instanz == this)
        {
            SceneManager.sceneLoaded -= SzeneGeladen;
        }
    }

    void SzeneGeladen(Scene szene, LoadSceneMode modus)
    {
        LautstaerkeSetzen(szene.name);
    }

    void LautstaerkeSetzen(string szenenName)
    {
        if (szenenName == startScreenName)
        {
            zielLautstaerke = lautStartscreen;
        }
        else
        {
            zielLautstaerke = lautImSpiel;
        }
    }

    void Update()
    {
        // Lautstärke langsam anpassen, damit es nicht ruckartig leiser wird
        float schritt = Time.unscaledDeltaTime / Mathf.Max(0.01f, ueberblendZeit);
        musikQuelle.volume = Mathf.MoveTowards(musikQuelle.volume, zielLautstaerke, schritt);

        // Alle X Sekunden den Chicken-Sound abspielen
        timer += Time.unscaledDeltaTime;
        if (timer >= chickenAbstand)
        {
            timer = 0f;
            if (chickenSound != null)
            {
                effektQuelle.PlayOneShot(chickenSound, chickenLautstaerke);
            }
        }
    }
}
