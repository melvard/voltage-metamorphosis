# PlayerData.json — Save Format V1

Save file location: `Application.persistentDataPath/PlayerData.json`

## What changed

The old save was an Odin `[SerializeReference]` graph (`rid` references resolved through a
`references.RefIds[]` table). `JsonUtility` cannot read that format, which made the save file
unusable outside the Odin editor pipeline and made hand edits error-prone.

V1 is an explicit, self-describing JSON document written with Newtonsoft.Json. No rids, no
type discriminators, no Unity-specific serialization. Every byte of data in the file is a
plain `int`/`float`/`string`/`bool`, so the file can be hand-edited safely.

The runtime model (`Schemes.Data.*`) is unchanged. `SaveConverter` maps between the runtime
objects and the DTOs via reflection (private `[SerializeField]` fields included), so no model
class needed to be modified.

## Format (V1)

```json
{
  "version": 1,
  "schemes": [
    {
      "name": "AND", "description": "...", "guid": "<scheme-guid>", "isEditable": true,
      "visuals": { "displayName": "AND", "r": 0.36, "g": 0.76, "b": 0.93, "a": 1.0,
                   "sizeX": 2.0, "sizeY": 2.0,
                   "inputPositions": [{ "x": -1.0, "y": -0.33 }],
                   "outputPositions": [{ "x": 1.0, "y": 0.0 }] },
      "editor":  { "componentEditorDatas": [...], "wireConnectionEditorDatas": [...],
                   "inputEditorDatas": [...], "outputEditorDatas": [...],
                   "cameraPositionOnGrid": { "x": 0, "y": 0 } },
      "logic":   { "type": "CompositionLogicData", "numberOfInputs": 2, "numberOfOutputs": 1,
                   "componentSchemes": [{ "componentIndex": 5, "schemeKey": "<guid>" }],
                   "schemeRelations": [ { "relationIndex": 0,
                                          "senderNode": { "componentIndexInComposition": 5, "componentPortIndex": 0 },
                                          "receiverNode": { "componentIndexInComposition": 6, "componentPortIndex": 0 } } ] }
    }
  ]
}
```

Deliberately **not** saved:

- `deviceBodyMaterial` — it is a `UnityEngine.Material` asset reference and cannot survive
  round-trips. The runtime re-resolves it from `deviceBodyColor` via the
  `SchemeMaterialPalette` on first access (`SchemeVisualsData.DeviceBodyMaterial`).

## Safety behavior

- `SaveValidator` runs on every load and every save. It checks GUID uniqueness, editor/logic
  index consistency, unique `relationIndex`, and that I/O and wire endpoints resolve.
  Violations throw `SaveValidationException` with a message naming the offending scheme.
- On every save, the previous file is copied to `PlayerData.json.bak` before overwriting.
- A legacy (Odin rid) file is detected structurally (top-level `"references"` object) and is
  migrated automatically on load (`SaveMigrator`), then validated. Migration is lossless for
  `CompositionLogicData` schemes: GUIDs, logic graph, wires, port positions and colors are all
  preserved.

## Files

- `SaveFileV1.cs` — DTOs (namespace `Schemes.Data.Save`)
- `SaveConverter.cs` — runtime ↔ DTO mapping (reflection-based)
- `SaveValidator.cs` — integrity validation + `SaveValidationException`
- `SaveMigrator.cs` — legacy (Odin rid) → V1 migration
- `../SchemesSaverLoader.cs` — public API (`LoadSchemes` / `SaveScheme` / `OnRemoveSchemeHandler`) is unchanged
