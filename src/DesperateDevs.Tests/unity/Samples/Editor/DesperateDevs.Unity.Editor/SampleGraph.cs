using DesperateDevs.Unity.Editor;
using UnityEngine;

namespace Samples.DesperateDevs.Unity.Editor
{
    public class SampleGraph : MonoBehaviour
    {
        public float[] Data = new float[100];
        public float Height = 180f;
        public float Min = -10;
        public float Max = 10f;

        public float BorderX = 48;
        public float BorderY = 20;
        public int RightLinePadding = -15;
        public string LabelFormat = "{0:0.0}";
        public string AxisFormat = "{0:0.0}";
        public int GridLines = 1;
        public float AxisRounding = 1f;
        public float AnchorRadius = 1f;
        public float PointScale = 1f;
        public float SelectedPointScale = 3f;
        public float LineWidth = 2f;
        public Color GridLinesColor = Color.gray;
        public Color LineColor = Color.red;
        public Color AvgLineColor = Color.yellow;
        public Color PointColor = Color.red;
    }

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(SampleGraph))]
    public class SampleGraphEditor : UnityEditor.Editor
    {
        Graph _graph;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var sample = (SampleGraph)target;
            for (var i = 0; i < sample.Data.Length; i++)
                sample.Data[i] = sample.Min + (sample.Max - sample.Min) * ((float)i / sample.Data.Length);

            if (_graph == null) _graph = new Graph(100);
            _graph.BorderX = sample.BorderX;
            _graph.BorderY = sample.BorderY;
            _graph.RightLinePadding = sample.RightLinePadding;
            _graph.LabelFormat = sample.LabelFormat;
            _graph.AxisFormat = sample.AxisFormat;
            _graph.GridLines = sample.GridLines;
            _graph.AxisRounding = sample.AxisRounding;
            _graph.AnchorRadius = sample.AnchorRadius;
            _graph.PointScale = sample.PointScale;
            _graph.SelectedPointScale = sample.SelectedPointScale;
            _graph.LineWidth = sample.LineWidth;
            _graph.GridLinesColor = sample.GridLinesColor;
            _graph.LineColor = sample.LineColor;
            _graph.AvgLineColor = sample.AvgLineColor;
            _graph.PointColor = sample.PointColor;
            _graph.Draw(sample.Data, sample.Height);

            Repaint();
        }
    }
#endif
}
