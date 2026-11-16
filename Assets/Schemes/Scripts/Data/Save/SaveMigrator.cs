using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Schemes.Data.Save
{
    /// <summary>
    /// Migrates the legacy Odin/rid-based save format to SaveFileV1.
    /// Legacy format: schemes[] -> schemeData -> schemeLogicData { rid: N },
    /// references.RefIds[] -> { rid, type, data } (Odin "SerializedReference" graph).
    /// </summary>
    public static class SaveMigrator
    {
        public static SaveFileV1 Migrate(string legacyJson)
        {
            var root = JObject.Parse(legacyJson);
            var refIds = root["references"]?["RefIds"] as JArray ?? new JArray();
            var refLookup = BuildRefLookup(refIds);

            var file = new SaveFileV1();
            var schemes = root["schemes"] as JArray ?? new JArray();

            foreach (var schemeToken in schemes)
            {
                var sd = schemeToken["schemeData"] as JObject;
                if (sd == null) continue;

                var save = new SchemeSave
                {
                    name = sd.Value<string>("name") ?? "",
                    description = sd.Value<string>("description") ?? "",
                    guid = (string)sd["schemeKey"]?["myGuid"]?["guidStr"],
                    isEditable = sd.Value<bool?>("isEditable") ?? true,
                    visuals = ParseVisuals(sd),
                    editor = ParseEditor(sd),
                    logic = ParseLogic(sd, refLookup),
                };
                file.schemes.Add(save);
            }

            return file;
        }

        private static Dictionary<long, JObject> BuildRefLookup(JArray refIds)
        {
            var lookup = new Dictionary<long, JObject>();
            foreach (var entry in refIds.OfType<JObject>())
            {
                var rid = entry.Value<long>("rid");
                lookup[rid] = entry;
            }
            return lookup;
        }

        private static VisualsSave ParseVisuals(JObject sd)
        {
            var vis = sd["schemeVisualsData"] as JObject;
            var vs = new VisualsSave
            {
                displayName = vis?.Value<string>("displayName") ?? "",
                r = vis?["deviceBodyColor"]?.Value<float>("r") ?? 1f,
                g = vis?["deviceBodyColor"]?.Value<float>("g") ?? 1f,
                b = vis?["deviceBodyColor"]?.Value<float>("b") ?? 1f,
                a = vis?["deviceBodyColor"]?.Value<float>("a") ?? 1f,
                sizeX = vis?["size"]?.Value<float>("x") ?? 2f,
                sizeY = vis?["size"]?.Value<float>("y") ?? 2f,
            };
            if (vis?["inputPositions"] is JArray inputs)
                foreach (var p in inputs.OfType<JObject>())
                    vs.inputPositions.Add(new Vector2Save(p.Value<float>("x"), p.Value<float>("y")));
            if (vis?["outputPositions"] is JArray outputs)
                foreach (var p in outputs.OfType<JObject>())
                    vs.outputPositions.Add(new Vector2Save(p.Value<float>("x"), p.Value<float>("y")));
            return vs;
        }

        private static EditorSave ParseEditor(JObject sd)
        {
            var ed = sd["schemeEditorData"] as JObject;
            var es = new EditorSave
            {
                cameraPositionOnGrid = new Coordinate(
                    ed?["cameraPositionOnGrid"]?.Value<int>("x") ?? 0,
                    ed?["cameraPositionOnGrid"]?.Value<int>("y") ?? 0),
            };

            if (ed?["componentEditorDatas"] is JArray components)
                foreach (var c in components.OfType<JObject>())
                    es.componentEditorDatas.Add(new ComponentEditorData(
                        c.Value<int>("componentIndex"),
                        new Coordinate(c["coordinateOnGrid"]?.Value<int>("x") ?? 0,
                                      c["coordinateOnGrid"]?.Value<int>("y") ?? 0)));

            if (ed?["wireConnectionEditorDatas"] is JArray wires)
                foreach (var w in wires.OfType<JObject>())
                {
                    var coords = new List<Coordinate>();
                    if (w["wireNodesCoordinates"] is JArray nodes)
                        foreach (var n in nodes.OfType<JObject>())
                            coords.Add(new Coordinate(n.Value<int>("x"), n.Value<int>("y")));
                    es.wireConnectionEditorDatas.Add(new WireConnectionEditorData(coords, w.Value<int>("relationIndex")));
                }

            if (ed?["inputEditorDatas"] is JArray inputs)
                foreach (var i in inputs.OfType<JObject>())
                    es.inputEditorDatas.Add(new IOEditorData(i.Value<int>("componentIndexInComposition"), i.Value<int>("numberOfIOForScheme")));

            if (ed?["outputEditorDatas"] is JArray outputs)
                foreach (var o in outputs.OfType<JObject>())
                    es.outputEditorDatas.Add(new IOEditorData(o.Value<int>("componentIndexInComposition"), o.Value<int>("numberOfIOForScheme")));

            return es;
        }

        private static LogicSave ParseLogic(JObject sd, Dictionary<long, JObject> refLookup)
        {
            var rid = sd["schemeLogicData"]?.Value<long>("rid");
            if (rid == null)
                return new LogicSave { type = "Unknown" };

            var logicEntry = ResolveRef(refLookup, rid.Value);
            if (logicEntry == null)
                return new LogicSave { type = "Unknown" };

            var data = logicEntry["data"] as JObject;
            if (data == null)
                return new LogicSave { type = "Unknown" };

            var cls = (string)logicEntry["type"]?["class"];
            if (cls != "CompositionLogicData")
                return new LogicSave { type = cls ?? "Unknown" };

            var numberOfInputs = data.Value<int>("<NumberOfInputs>k__BackingField");
            var numberOfOutputs = data.Value<int>("<NumberOfOutputs>k__BackingField");

            var ls = new LogicSave
            {
                type = "CompositionLogicData",
                numberOfInputs = numberOfInputs,
                numberOfOutputs = numberOfOutputs,
            };

            if (data["componentSchemes"] is JArray components)
                foreach (var c in components.OfType<JObject>())
                    ls.componentSchemes.Add(new ComponentSchemeSave
                    {
                        componentIndex = c.Value<int>("componentIndex"),
                        schemeKey = (string)c["schemeKey"]?["myGuid"]?["guidStr"],
                    });

            if (data["schemeRelations"] is JArray relations)
                foreach (var r in relations.OfType<JObject>())
                    ls.schemeRelations.Add(new SchemeRelationSave
                    {
                        relationIndex = r.Value<int>("relationIndex"),
                        senderNode = ParseRelationNode(r["senderNode"]?.Value<long>("rid"), refLookup),
                        receiverNode = ParseRelationNode(r["receiverNode"]?.Value<long>("rid"), refLookup),
                    });

            return ls;
        }

        private static RelationNodeSave ParseRelationNode(long? rid, Dictionary<long, JObject> refLookup)
        {
            if (rid == null) return null;
            var entry = ResolveRef(refLookup, rid.Value);
            if (entry == null) return null;

            var data = entry["data"] as JObject;
            if (data == null) return null;

            return new RelationNodeSave
            {
                componentIndexInComposition = data.Value<int>("componentIndexInComposition"),
                componentPortIndex = data.Value<byte>("componentPortIndex"),
            };
        }

        private static JObject ResolveRef(Dictionary<long, JObject> refLookup, long rid)
        {
            return refLookup.TryGetValue(rid, out var entry) ? entry : null;
        }
    }
}
