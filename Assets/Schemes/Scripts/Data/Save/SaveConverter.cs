using System;
using System.Collections.Generic;
using System.Reflection;
using Schemes.Data.LogicData;
using Schemes.Data.LogicData.Composition;
using UnityEngine;

namespace Schemes.Data.Save
{
    /// <summary>
    /// Bidirectional mapping between the runtime Scheme model and the explicit SaveFileV1 format.
    /// Serialization should never touch serializer-internal rids; this converter builds the
    /// object graph explicitly from the save data.
    /// </summary>
    public static class SaveConverter
    {
        #region Reflection helpers

        private static object GetField(object obj, string fieldName)
        {
            var type = obj.GetType();
            while (type != null)
            {
                var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null) return field.GetValue(obj);
                type = type.BaseType;
            }
            throw new SaveValidationException($"Field '{fieldName}' not found on {obj.GetType().Name}");
        }

        private static void SetField(object obj, string fieldName, object value)
        {
            var type = obj.GetType();
            while (type != null)
            {
                var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(obj, ConvertValue(value, field.FieldType));
                    return;
                }
                type = type.BaseType;
            }
            throw new SaveValidationException($"Field '{fieldName}' not found on {obj.GetType().Name}");
        }

        private static object ConvertValue(object value, Type targetType)
        {
            if (value is null) return null;
            if (targetType.IsInstanceOfType(value)) return value;

            var backing = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (backing == typeof(byte)) return Convert.ToByte(value);
            if (backing == typeof(int)) return Convert.ToInt32(value);
            if (backing == typeof(float)) return Convert.ToSingle(value);
            if (backing == typeof(bool)) return Convert.ToBoolean(value);
            return value;
        }

        private static void SetProperty(object obj, string propertyName, object value)
        {
            var type = obj.GetType();
            while (type != null)
            {
                var prop = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(obj, ConvertValue(value, prop.PropertyType));
                    return;
                }
                type = type.BaseType;
            }
            throw new SaveValidationException($"Property '{propertyName}' not found (or has no setter) on {obj.GetType().Name}");
        }

        private static object GetProperty(object obj, string propertyName)
        {
            var type = obj.GetType();
            while (type != null)
            {
                var prop = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null) return prop.GetValue(obj);
                type = type.BaseType;
            }
            throw new SaveValidationException($"Property '{propertyName}' not found on {obj.GetType().Name}");
        }

        #endregion

        #region SchemeKey helpers (private ctor in MyGuid/SchemeKey)

        private static SchemeKey CreateSchemeKey(string guid)
        {
            var ctor = typeof(SchemeKey).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                null, new[] { typeof(string) }, null);
            if (ctor == null) throw new SaveValidationException("SchemeKey private constructor not found");
            return (SchemeKey)ctor.Invoke(new object[] { guid });
        }

        private static object TryGetProperty(object obj, string propertyName)
        {
            var type = obj.GetType();
            while (type != null)
            {
                var prop = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null) return prop.GetValue(obj);
                type = type.BaseType;
            }
            return null;
        }

        #endregion

        #region Public API

        public static SaveFileV1 ToSaveFile(IEnumerable<Scheme> schemes)
        {
            var file = new SaveFileV1();
            foreach (var scheme in schemes)
                file.schemes.Add(ToSchemeSave(scheme));
            return file;
        }

        private static SchemeSave ToSchemeSave(Scheme scheme)
        {
            var data = scheme.SchemeData;
            var key = data.SchemeKey;
            var visuals = data.SchemeVisualsData;
            var editor = data.SchemeEditorData;
            var logic = data.SchemeLogicData;

            var save = new SchemeSave
            {
                name = data.Name,
                description = data.Description,
                guid = key.ToString(),
                isEditable = data.IsEditable,
                visuals = new VisualsSave
                {
                    displayName = visuals.DisplayName,
                    r = visuals.DeviceBodyColor.r,
                    g = visuals.DeviceBodyColor.g,
                    b = visuals.DeviceBodyColor.b,
                    a = visuals.DeviceBodyColor.a,
                    sizeX = visuals.Size.x,
                    sizeY = visuals.Size.y,
                },
                editor = ToEditorSave(editor),
                logic = ToLogicSave(logic),
            };

            var inputPositions = (Vector2[])GetField(visuals, "inputPositions");
            var outputPositions = (Vector2[])GetField(visuals, "outputPositions");
            if (inputPositions != null)
                foreach (var p in inputPositions)
                    save.visuals.inputPositions.Add(new Vector2Save(p.x, p.y));
            if (outputPositions != null)
                foreach (var p in outputPositions)
                    save.visuals.outputPositions.Add(new Vector2Save(p.x, p.y));

            return save;
        }

        private static EditorSave ToEditorSave(SchemeEditorData editor)
        {
            var save = new EditorSave
            {
                componentEditorDatas = editor.componentEditorDatas?.ConvertAll(c => c) ?? new List<ComponentEditorData>(),
                wireConnectionEditorDatas = DeepCopyWireEditorDatas(editor.wireConnectionEditorDatas),
                inputEditorDatas = editor.inputEditorDatas?.ConvertAll(c => c) ?? new List<IOEditorData>(),
                outputEditorDatas = editor.outputEditorDatas?.ConvertAll(c => c) ?? new List<IOEditorData>(),
                cameraPositionOnGrid = editor.cameraPositionOnGrid,
            };
            return save;
        }

        private static List<WireConnectionEditorData> DeepCopyWireEditorDatas(List<WireConnectionEditorData> data)
        {
            if (data == null) return new List<WireConnectionEditorData>();
            var copy = new List<WireConnectionEditorData>(data.Count);
            foreach (var wire in data)
            {
                copy.Add(new WireConnectionEditorData(
                    new List<Coordinate>(wire.wireNodesCoordinates ?? new List<Coordinate>()),
                    wire.relationIndex));
            }
            return copy;
        }

        private static LogicSave ToLogicSave(SchemeLogicData logic)
        {
            if (logic is CompositionLogicData comp)
            {
                var save = new LogicSave
                {
                    type = nameof(CompositionLogicData),
                    numberOfInputs = GetProperty(comp, "NumberOfInputs") is byte i ? i : 0,
                    numberOfOutputs = GetProperty(comp, "NumberOfOutputs") is byte o ? o : 0,
                };
                var components = (List<ComponentScheme>)GetField(comp, "componentSchemes");
                if (components != null)
                    foreach (var c in components)
                        save.componentSchemes.Add(new ComponentSchemeSave
                        {
                            componentIndex = c.ComponentIndex,
                            schemeKey = c.SchemeKey.ToString(),
                        });

                var relations = (List<SchemeRelation>)GetField(comp, "schemeRelations");
                if (relations != null)
                    foreach (var r in relations)
                        save.schemeRelations.Add(new SchemeRelationSave
                        {
                            relationIndex = r.relationIndex,
                            senderNode = NodeToSave(r.senderNode),
                            receiverNode = NodeToSave(r.receiverNode),
                        });
                return save;
            }

            // UserInputLogicData / UserOutputLogicData / RelayLogicData / PersistentVoltageLogicData
            // are trivial (only I/O byte fields). Save a minimal record for forward compatibility.
            var inByte = TryGetProperty(logic, "NumberOfInputs") as byte? ?? 0;
            var outByte = TryGetProperty(logic, "NumberOfOutputs") as byte? ?? 0;
            return new LogicSave
            {
                type = logic?.GetType().Name ?? "Unknown",
                numberOfInputs = inByte,
                numberOfOutputs = outByte,
            };
        }

        private static RelationNodeSave NodeToSave(ComponentRelationNode node)
        {
            if (node == null) return null;
            return new RelationNodeSave
            {
                componentIndexInComposition = (int)GetField(node, "componentIndexInComposition"),
                componentPortIndex = GetField(node, "componentPortIndex") is byte b ? b : (byte)0,
            };
        }

        public static List<Scheme> FromSaveFile(SaveFileV1 file)
        {
            SaveValidator.Validate(file);

            var result = new List<Scheme>();
            foreach (var save in file.schemes)
                result.Add(SchemeFromSave(save));
            return result;
        }

        private static Scheme SchemeFromSave(SchemeSave save)
        {
            var schemeKey = CreateSchemeKey(save.guid);
            var schemeData = new SchemeData(schemeKey);
            SetField(schemeData, "name", save.name ?? "");
            SetField(schemeData, "description", save.description ?? "");
            SetField(schemeData, "isEditable", save.isEditable);

            var visuals = (SchemeVisualsData)GetField(schemeData, "schemeVisualsData");
            ApplyVisuals(visuals, save.visuals);

            var editor = new SchemeEditorData
            {
                componentEditorDatas = save.editor?.componentEditorDatas ?? new List<ComponentEditorData>(),
                wireConnectionEditorDatas = save.editor?.wireConnectionEditorDatas ?? new List<WireConnectionEditorData>(),
                inputEditorDatas = save.editor?.inputEditorDatas ?? new List<IOEditorData>(),
                outputEditorDatas = save.editor?.outputEditorDatas ?? new List<IOEditorData>(),
                cameraPositionOnGrid = save.editor?.cameraPositionOnGrid ?? new Coordinate(0, 0),
            };
            SetField(schemeData, "schemeEditorData", editor);

            var logic = LogicFromSave(save.logic);
            SetField(schemeData, "schemeLogicData", logic);

            return new Scheme(schemeData);
        }

        private static void ApplyVisuals(SchemeVisualsData visuals, VisualsSave save)
        {
            if (save == null) return;

            SetField(visuals, "displayName", save.displayName ?? "");
            SetField(visuals, "deviceBodyColor", new Color(save.r, save.g, save.b, save.a));
            SetField(visuals, "size", new Vector2(save.sizeX, save.sizeY));

            var inputPositions = new Vector2[save.inputPositions.Count];
            for (int i = 0; i < inputPositions.Length; i++)
                inputPositions[i] = new Vector2(save.inputPositions[i].x, save.inputPositions[i].y);
            SetField(visuals, "inputPositions", inputPositions);

            var outputPositions = new Vector2[save.outputPositions.Count];
            for (int i = 0; i < outputPositions.Length; i++)
                outputPositions[i] = new Vector2(save.outputPositions[i].x, save.outputPositions[i].y);
            SetField(visuals, "outputPositions", outputPositions);
        }

        private static SchemeLogicData LogicFromSave(LogicSave save)
        {
            if (save == null)
                return new CompositionLogicData();

            if (save.type == nameof(CompositionLogicData))
            {
                var comp = new CompositionLogicData();
                SetProperty(comp, "NumberOfInputs", (byte)Mathf.Clamp(save.numberOfInputs, 0, 255));
                SetProperty(comp, "NumberOfOutputs", (byte)Mathf.Clamp(save.numberOfOutputs, 0, 255));

                var components = new List<ComponentScheme>();
                if (save.componentSchemes != null)
                {
                    foreach (var c in save.componentSchemes)
                    {
                        var key = CreateSchemeKey(c.schemeKey);
                        components.Add(new ComponentScheme(c.componentIndex, key));
                    }
                }
                SetField(comp, "componentSchemes", components);

                var relations = new List<SchemeRelation>();
                if (save.schemeRelations != null)
                {
                    foreach (var r in save.schemeRelations)
                    {
                        relations.Add(new SchemeRelation
                        {
                            relationIndex = r.relationIndex,
                            senderNode = NodeFromSave(r.senderNode),
                            receiverNode = NodeFromSave(r.receiverNode),
                        });
                    }
                }
                SetField(comp, "schemeRelations", relations);
                return comp;
            }

            throw new SaveValidationException($"Unsupported logic type '{save.type}'");
        }

        private static ComponentRelationNode NodeFromSave(RelationNodeSave save)
        {
            if (save == null) return null;
            return new ComponentRelationNode(save.componentIndexInComposition, (byte)save.componentPortIndex);
        }

        #endregion
    }
}
