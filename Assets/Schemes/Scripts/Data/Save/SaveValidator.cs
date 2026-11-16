using System;
using System.Collections.Generic;
using System.Linq;
using Schemes.Data.LogicData.Composition;

namespace Schemes.Data.Save
{
    /// <summary>
    /// Validates a SaveFileV1 object graph. Throws SaveValidationException on any problem.
    /// </summary>
    public static class SaveValidator
    {
        public static void Validate(SaveFileV1 file)
        {
            if (file == null) throw new SaveValidationException("SaveFileV1 cannot be null");
            if (file.schemes == null) throw new SaveValidationException("SaveFileV1.schemes cannot be null");

            var guids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < file.schemes.Count; i++)
            {
                var scheme = file.schemes[i];
                ValidateScheme(scheme, i, guids);
            }
        }

        private static void ValidateScheme(SchemeSave scheme, int index, HashSet<string> guids)
        {
            var label = scheme?.name ?? $"scheme[{index}]";
            if (scheme == null) throw new SaveValidationException($"Scheme entry {index} is null");

            if (string.IsNullOrEmpty(scheme.guid))
                throw new SaveValidationException($"{label}: guid is null or empty");
            if (!guids.Add(scheme.guid))
                throw new SaveValidationException($"{label}: duplicate guid '{scheme.guid}'");
            if (scheme.visuals == null)
                throw new SaveValidationException($"{label}: visuals is null");
            if (scheme.editor == null)
                throw new SaveValidationException($"{label}: editor is null");
            if (scheme.logic == null)
                throw new SaveValidationException($"{label}: logic is null");

            // Logic validation (CompositionLogicData)
            if (scheme.logic.type == nameof(CompositionLogicData))
            {
                ValidateCompositionLogic(scheme, label);
            }

            // Component index consistency check between editor and logic
            var logicIndices = new HashSet<int>(scheme.logic.componentSchemes.Select(c => c.componentIndex));
            foreach (var comp in scheme.editor.componentEditorDatas)
            {
                if (comp.componentIndex >= 0 && !logicIndices.Contains(comp.componentIndex))
                    throw new SaveValidationException($"{label}: editor references component index {comp.componentIndex} that is not in logic.componentSchemes");
            }

            // IO editor data must reference valid composition indices
            var ioIndices = new HashSet<int>(scheme.editor.componentEditorDatas.Select(c => c.componentIndex));
            foreach (var io in scheme.editor.inputEditorDatas)
                if (!ioIndices.Contains(io.componentIndexInComposition))
                    throw new SaveValidationException($"{label}: inputEditorData references unknown component index {io.componentIndexInComposition}");
            foreach (var io in scheme.editor.outputEditorDatas)
                if (!ioIndices.Contains(io.componentIndexInComposition))
                    throw new SaveValidationException($"{label}: outputEditorData references unknown component index {io.componentIndexInComposition}");
        }

        private static void ValidateCompositionLogic(SchemeSave scheme, string label)
        {
            var componentIndices = new HashSet<int>();
            foreach (var c in scheme.logic.componentSchemes)
            {
                if (componentIndices.Contains(c.componentIndex))
                    throw new SaveValidationException($"{label}: duplicate componentIndex {c.componentIndex} in logic");
                componentIndices.Add(c.componentIndex);

                if (string.IsNullOrEmpty(c.schemeKey))
                    throw new SaveValidationException($"{label}: component {c.componentIndex} has empty schemeKey");
            }

            var relationIndices = new HashSet<int>();
            foreach (var r in scheme.logic.schemeRelations)
            {
                if (!relationIndices.Add(r.relationIndex))
                    throw new SaveValidationException($"{label}: duplicate relationIndex {r.relationIndex}");

                ValidateRelationNode(r.senderNode, $"{label}: senderNode of relation {r.relationIndex}");
                ValidateRelationNode(r.receiverNode, $"{label}: receiverNode of relation {r.relationIndex}");

                if (r.senderNode != null && !componentIndices.Contains(r.senderNode.componentIndexInComposition))
                    throw new SaveValidationException($"{label}: senderNode references unknown component {r.senderNode.componentIndexInComposition}");
                if (r.receiverNode != null && !componentIndices.Contains(r.receiverNode.componentIndexInComposition))
                    throw new SaveValidationException($"{label}: receiverNode references unknown component {r.receiverNode.componentIndexInComposition}");
            }

            // Wires must reference an existing relation index
            foreach (var wire in scheme.editor.wireConnectionEditorDatas)
            {
                if (!relationIndices.Contains(wire.relationIndex))
                    throw new SaveValidationException($"{label}: wire references unknown relationIndex {wire.relationIndex}");
            }
        }

        private static void ValidateRelationNode(RelationNodeSave node, string context)
        {
            if (node == null) throw new SaveValidationException($"{context} is null");
            if (node.componentIndexInComposition < 0)
                throw new SaveValidationException($"{context} has invalid componentIndexInComposition {node.componentIndexInComposition}");
            if (node.componentPortIndex < 0)
                throw new SaveValidationException($"{context} has invalid componentPortIndex {node.componentPortIndex}");
        }
    }

    /// <summary>
    /// Thrown when a save file fails validation or cannot be loaded.
    /// </summary>
    public class SaveValidationException : Exception
    {
        public SaveValidationException(string message) : base(message) { }
        public SaveValidationException(string message, Exception inner) : base(message, inner) { }
    }
}
