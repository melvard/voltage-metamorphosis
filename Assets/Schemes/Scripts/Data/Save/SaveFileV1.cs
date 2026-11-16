using System;
using System.Collections.Generic;
using Schemes.Data;

namespace Schemes.Data.Save
{
    /// <summary>
    /// Explicit, self-describing save format (V1). No polymorphic references, no rids.
    /// Every field is a concrete .NET type so the data is human-editable and hand-editable.
    /// </summary>
    [Serializable]
    public class SaveFileV1
    {
        public int version = 1;
        public List<SchemeSave> schemes = new List<SchemeSave>();
    }

    [Serializable]
    public class SchemeSave
    {
        public string name;
        public string description;
        public string guid;
        public bool isEditable;

        public VisualsSave visuals;
        public EditorSave editor;
        public LogicSave logic;
    }

    [Serializable]
    public class VisualsSave
    {
        public string displayName;
        // RGBA as 0-1 floats (matches Unity Color)
        public float r, g, b, a;
        public float sizeX, sizeY;
        public List<Vector2Save> inputPositions = new List<Vector2Save>();
        public List<Vector2Save> outputPositions = new List<Vector2Save>();
        // Note: deviceBodyMaterial is intentionally NOT saved.
        // It is re-resolved from the color via SchemeMaterialPalette at runtime.
    }

    [Serializable]
    public struct Vector2Save
    {
        public float x, y;
        public Vector2Save(float x, float y) { this.x = x; this.y = y; }
    }

    [Serializable]
    public class EditorSave
    {
        public List<ComponentEditorData> componentEditorDatas = new List<ComponentEditorData>();
        public List<WireConnectionEditorData> wireConnectionEditorDatas = new List<WireConnectionEditorData>();
        public List<IOEditorData> inputEditorDatas = new List<IOEditorData>();
        public List<IOEditorData> outputEditorDatas = new List<IOEditorData>();
        public Coordinate cameraPositionOnGrid;
    }

    [Serializable]
    public class LogicSave
    {
        // Discriminator so future logic types can be added without breaking old saves.
        // Currently supported: "CompositionLogicData"
        public string type = "CompositionLogicData";

        public int numberOfInputs;
        public int numberOfOutputs;

        public List<ComponentSchemeSave> componentSchemes = new List<ComponentSchemeSave>();
        public List<SchemeRelationSave> schemeRelations = new List<SchemeRelationSave>();
    }

    [Serializable]
    public class ComponentSchemeSave
    {
        public int componentIndex;
        public string schemeKey;
    }

    [Serializable]
    public class SchemeRelationSave
    {
        public int relationIndex;
        public RelationNodeSave senderNode;
        public RelationNodeSave receiverNode;
    }

    [Serializable]
    public class RelationNodeSave
    {
        public int componentIndexInComposition;
        public int componentPortIndex;
    }
}
