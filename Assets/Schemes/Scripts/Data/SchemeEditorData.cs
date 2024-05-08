using System;
using System.Collections.Generic;

namespace Schemes.Data
{
    [Serializable]
    public struct Coordinate
    {
        public int x;
        public int y;

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
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
        public int relationIndex;

        public WireConnectionEditorData(List<Coordinate> wireNodesCoordinates, int relationIndex)
        {
            this.wireNodesCoordinates = wireNodesCoordinates;
            this.relationIndex = relationIndex;
        }
    }

    [Serializable]
    public struct IOEditorData
    {
        public int componentIndexInComposition;
        public int numberOfIOForScheme;

        public IOEditorData(int componentIndexInComposition, int numberOfIOForScheme)
        {
            this.componentIndexInComposition = componentIndexInComposition;
            this.numberOfIOForScheme = numberOfIOForScheme;
        }
    }
    
    [Serializable]
    public class SchemeEditorData
    {
        public List<ComponentEditorData> componentEditorDatas= new();
        public List<WireConnectionEditorData> wireConnectionEditorDatas = new();
        public List<IOEditorData> inputEditorDatas = new();
        public List<IOEditorData> outputEditorDatas = new();
        public Coordinate cameraPositionOnGrid = new(0,0);

        public static SchemeEditorData CopyFrom(SchemeEditorData schemeEditorData)
        {
            var copyData = new SchemeEditorData
            {
                componentEditorDatas = new (schemeEditorData.componentEditorDatas),
                wireConnectionEditorDatas = new (schemeEditorData.wireConnectionEditorDatas),
                inputEditorDatas = new (schemeEditorData.inputEditorDatas),
                outputEditorDatas = new (schemeEditorData.outputEditorDatas),
                cameraPositionOnGrid = schemeEditorData.cameraPositionOnGrid
            };

            return copyData;
        }
    }
}