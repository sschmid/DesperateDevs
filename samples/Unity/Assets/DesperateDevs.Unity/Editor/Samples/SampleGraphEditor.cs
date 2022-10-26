using UnityEditor;

namespace DesperateDevs.Unity.Editor.Samples
{
    [CustomEditor(typeof(SampleGraph))]
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
}
