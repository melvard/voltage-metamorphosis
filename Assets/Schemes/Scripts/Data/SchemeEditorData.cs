using System;
using System.Collections.Generic;
using UnityEngine;

namespace Schemes.Data
{
    [Serializable]
    public struct Coordinate
    {
        public int x;
        public int y;
    }
    
    [Serializable]
    public struct ComponentEditorData
    {
        public int componentIndex;
        public Coordinate coordinateOnGrid;

        public ComponentEditorData(int componentIndex, Coordinate coordinateOnGrid)
        {
            this.componentIndex = componentIndex;
            this.coordinateOnGrid = coordinateOnGrid;
        }
    }

    [Serializable]
    public struct WireConnectionEditorData
    {
        public List<Coordinate> wireNodesCoordinates;

        public WireConnectionEditorData(List<Coordinate> wireNodesCoordinates)
        {
            this.wireNodesCoordinates = wireNodesCoordinates;
        }
    }
    
    [Serializable]
    public class SchemeEditorData
    {
        public List<ComponentEditorData> componentEditorDatas;
        public List<WireConnectionEditorData> wireConnectionEditorDatas;
    }
}