# ChicknJumper – erste Unity-Demo

Unity **6000.6.9f1** verwenden.

- `Assets/Scenes/PlayableDemo.unity` öffnen und **Play** drücken.
- Alternativ `HomeScreen.unity` öffnen und im Spiel **Play** anklicken oder **Enter** drücken.
- Das Hendl startet sofort und springt automatisch zwischen den Begrenzungen hin und her.
- **Linksklick / Touch:** ein Trampolin platzieren, das den Flug umlenkt. Der nächste Klick ersetzt das Trampolin. Direkt im Hendl oder in einer Wand kann keines gesetzt werden.
- **R / Neustart:** Runde und Sprungzähler zurücksetzen.
- **Leertaste / Pause:** pausieren und fortsetzen.
- **Esc / Menü:** zum Startbildschirm zurückkehren.

Die Demo verwendet die vorhandene Scheune und Hendl-Grafiken. Die Introanimation wird übersprungen. Die Arena ist absichtlich rundherum mit Abprallflächen geschlossen, damit man sofort Bewegung sieht und Trampoline ausprobieren kann.

Unter **ChicknJumper → Build Linux demo** entsteht `Builds/Linux/ChicknJumper.x86_64`. Den gesamten Ordner weitergeben, nicht nur die ausführbare Datei.

`DemoBuild.Create` kann die Demo-Szene neu erzeugen; dabei wird nur die Demo-Szene ersetzt und die Buttons im Startmenü werden verbunden. `LevelDesign.unity` wird nicht verändert. Mit `DemoSmokeCheck.Run` lässt sich die Demo in einer separaten Unity-Instanz mit `-batchmode -nographics -executeMethod DemoSmokeCheck.Run` in Play Mode prüfen (ohne `-quit`).
