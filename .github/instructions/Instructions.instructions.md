---
description: Projektweite Coding- und Design-Guidelines für das Dungeon-Crawler-Projekt "Stoneshard-inspired" (Arbeitstitel). Wird von allen Agents (Gameplay, Shader, Level, UI, Audio, Performance, Netcode, QA) berücksichtigt.
applyTo: '**/*.cs, **/*.shader, **/*.shadergraph, **/*.asset, **/*.prefab, **/*.unity'
---

# Projektkontext

Wir entwickeln ein **2D Pixel-Art Dungeon Crawler RPG in Unity**, das sich stark an **Stoneshard** orientiert, aber insgesamt **etwas zugänglicher/leichter** sein soll (weniger punishing als Stoneshards Hardcore-Injury-System, aber mit ähnlicher Tiefe bei Skills und Kampf).

**Genre-Kern:** Rundenbasiertes/tick-basiertes Taktik-RPG mit isometrischer oder Top-Down Pixel-Art-Darstellung, Skilltrees pro Kampfstil/Klasse, Loot- und Progression-System, prozedural generierte oder handcrafted Dungeon-Etagen.

## Referenz-Pfeiler aus Stoneshard, die wir übernehmen
- **Rundenbasierter/AP-basierter Kampf** (Action Points oder klassisches Turn-Order-System) statt Echtzeit-Action.
- **Skilltrees pro Waffentyp/Kampfstil** (z.B. Einhand, Zweihand, Magie, Fernkampf), bei denen Punkte durch Nutzung oder Level-Up freigeschaltet werden.
- **Ausrüstungs- und Inventar-Tiefe** (Gewicht, Rüstungsslots, Tränke/Verbrauchsgüter im Kampf).
- **Zustände/Status-Effekte** (Blutung, Vergiftung, Erschöpfung) als taktische Kampfschicht.
- **Dungeon-Erkundung mit Risiko/Belohnung** (Fallen, versteckter Loot, stärkere Elite-Gegner).

## Abweichungen (bewusst "leichter" als Stoneshard)
- **Kein permanentes Verletzungssystem** mit Langzeitfolgen (z.B. keine wochenlangen Blindheits-/Infektions-Debuffs) – Verletzungen heilen schneller oder wirken nur temporär.
- **Vereinfachtes Hunger-/Bedürfnis-System** oder ganz weglassen, falls es zu viel Micromanagement erzeugt.
- **Fairere Zufallsgenerierung**: keine Situationen, die durch reinen RNG unvermeidbar zum Tod führen.
- **Klarere UI-Rückmeldung** zu Trefferchancen, Schadensberechnung und Status-Effekten (Stoneshard wird oft für seine steile Lernkurve/unklare Zahlen kritisiert – das vermeiden wir).

---

# Arbeitsweise aller Agents (verbindlich für ALLE Rollen)

## 1. Schritt-für-Schritt-Anleitung ist Pflicht
Jeder Agent muss am Ende JEDER Antwort eine klare **"Was du jetzt tun musst"-Liste** liefern, z.B.: