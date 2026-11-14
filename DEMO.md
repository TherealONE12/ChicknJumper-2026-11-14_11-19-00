# ChicknJumper – Endless Run

Unity **6000.6.9f1** verwenden.

- `Assets/Scenes/PlayableDemo.unity` öffnen und **Play** drücken, oder im Startmenü **Play / Enter** wählen.
- Die animierte Turbine bläst das Hendl beim Start und nach jedem Neustart ins Spiel.
- **Linksklick / Touch:** ein Trampolin im aktuellen Bildausschnitt setzen. Der nächste Klick ersetzt es. Direkt im Hendl kann keines gesetzt werden.
- Die Kamera folgt dem Hendl nach rechts. Die Landschaft wird lückenlos wiederverwendet; es gibt kein horizontales Levelende.
- Jeder Trampolinsprung zählt einen Punkt. Der Score bleibt während des Laufs oben rechts sichtbar.
- Alle **5 Punkte** steigt die Fluggeschwindigkeit um **20 %** gegenüber der vorherigen Stufe (5 → 6 → 7,2 → 8,64 …).
- Am oberen Bildschirmrand wird das Hendl wieder nach unten gelenkt. Wer unten vorbeifliegt, beendet den Lauf.
- **R:** Neustart mit Score 0 und Anfangsgeschwindigkeit.
- **Esc / Menü:** zum Startbildschirm zurückkehren.

Unter **ChicknJumper → Build Linux demo** entsteht `Builds/Linux/ChicknJumper.x86_64`. Den gesamten Ordner weitergeben, nicht nur die ausführbare Datei.

`DemoBuild.ConfigureEndless` richtet die Endless-Komponenten in der vorhandenen Demo-Szene ein. `DemoBuild.Create` erzeugt die Demo-Szene einschließlich Endless-Modus neu. `LevelDesign.unity` bleibt unverändert.
