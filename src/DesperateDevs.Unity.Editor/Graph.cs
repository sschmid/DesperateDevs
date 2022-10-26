using UnityEditor;
using UnityEngine;

namespace DesperateDevs.Unity.Editor
{
    public class Graph
    {
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

        readonly GUIStyle _labelTextStyle;
        readonly GUIStyle _centeredStyle;
        readonly Vector3[] _cachedLinePointVertices;
        readonly Vector3[] _linePoints;

        public Graph(int dataLength)
        {
            _labelTextStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperRight
            };
            _centeredStyle = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal =
                {
                    textColor = Color.white
                }
            };
            _linePoints = new Vector3[dataLength];
            _cachedLinePointVertices = new[]
            {
                new Vector3(-1, 1, 0) * AnchorRadius,
                new Vector3(1, 1, 0) * AnchorRadius,
                new Vector3(1, -1, 0) * AnchorRadius,
                new Vector3(-1, -1, 0) * AnchorRadius,
            };
        }

        public void Draw(float[] data, float height)
        {
            var rect = GUILayoutUtility.GetRect(EditorGUILayout.GetControlRect().width, height);
            var top = rect.y + BorderY;
            var floor = rect.y + rect.height - BorderY;
            var availableHeight = floor - top;

            var min = 0f;
            var max = 0f;
            var avg = 0f;

            if (data.Length > 0)
            {
                min = float.MaxValue;
                max = float.MinValue;
                foreach (var value in data)
                {
                    if (value < min) min = value;
                    if (value > max) max = value;
                    avg += value;
                }

                avg /= data.Length;
            }

            if (min % AxisRounding != 0)
                min = min < 0
                    ? min - AxisRounding - min % AxisRounding
                    : min - min % AxisRounding;

            if (max % AxisRounding != 0)
                max = max > 0
                    ? max + AxisRounding - max % AxisRounding
                    : max - max % AxisRounding;

            var range = max - min;
            DrawGridLines(top, rect.width, availableHeight, min, range);
            DrawAvg(floor, rect.width, availableHeight, min, range, avg);
            DrawLine(data, floor, rect.width, availableHeight, min, range);
        }

        void DrawGridLines(float top, float width, float availableHeight, float min, float range)
        {
            var handleColor = Handles.color;
            Handles.color = GridLinesColor;

            var n = GridLines + 1;
            var lineSpacing = availableHeight / n;
            for (var i = 0; i <= n; i++)
            {
                var lineY = top + lineSpacing * i;
                Handles.DrawLine(
                    new Vector2(BorderX, lineY),
                    new Vector2(width - RightLinePadding, lineY)
                );
                GUI.Label(
                    new Rect(0, lineY - 8, BorderX - 2, 50),
                    string.Format(AxisFormat, min + range * (1f - i / (float)n)),
                    _labelTextStyle
                );
            }

            Handles.color = handleColor;
        }

        void DrawAvg(float floor, float width, float availableHeight, float min, float range, float avg)
        {
            var handleColor = Handles.color;
            Handles.color = AvgLineColor;

            var lineY = floor + availableHeight * (min / range) - availableHeight * (avg / range);
            Handles.DrawLine(
                new Vector2(BorderX, lineY),
                new Vector2(width - RightLinePadding, lineY)
            );

            Handles.color = handleColor;
        }

        void DrawLine(float[] data, float floor, float width, float availableHeight, float min, float range)
        {
            var handleColor = Handles.color;
            Handles.matrix = Matrix4x4.identity;
            HandleUtility.handleMaterial.SetPass(0);

            var lineWidth = (width - BorderX - RightLinePadding) / data.Length;
            var labelRect = new Rect();
            var mousePositionDiscovered = false;
            var mouseHoverDataValue = 0f;
            for (var i = 0; i < data.Length; i++)
            {
                var value = data[i];
                var lineTop = floor + availableHeight * (min / range) - availableHeight * (value / range);
                var newLine = new Vector2(BorderX + lineWidth * i, lineTop);
                _linePoints[i] = new Vector3(newLine.x, newLine.y, 0);
                var linePointScale = PointScale;
                if (!mousePositionDiscovered)
                {
                    var anchorPosRadius3 = AnchorRadius * 3;
                    var anchorPosRadius6 = AnchorRadius * 6;
                    var anchorPos = newLine - Vector2.up * 0.5f;
                    labelRect = new Rect(anchorPos.x - anchorPosRadius3, anchorPos.y - anchorPosRadius3, anchorPosRadius6, anchorPosRadius6);
                    if (labelRect.Contains(Event.current.mousePosition))
                    {
                        mousePositionDiscovered = true;
                        mouseHoverDataValue = value;
                        linePointScale = SelectedPointScale;
                    }
                }

                Handles.color = PointColor;
                Handles.matrix = Matrix4x4.TRS(_linePoints[i], Quaternion.identity, Vector3.one * linePointScale);
                Handles.DrawAAConvexPolygon(_cachedLinePointVertices);
            }

            Handles.color = LineColor;
            Handles.matrix = Matrix4x4.identity;
            Handles.DrawAAPolyLine(LineWidth, data.Length, _linePoints);

            if (mousePositionDiscovered)
            {
                labelRect.y -= 16;
                labelRect.width += 50;
                labelRect.x -= 25;
                GUI.Label(labelRect, string.Format(LabelFormat, mouseHoverDataValue), _centeredStyle);
            }

            Handles.color = handleColor;
        }
    }
}
