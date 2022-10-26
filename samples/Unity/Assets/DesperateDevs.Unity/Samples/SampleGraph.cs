using UnityEngine;

namespace DesperateDevs.Unity.Editor.Samples
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
}
