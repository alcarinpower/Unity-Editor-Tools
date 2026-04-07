using UnityEngine;
using UnityEngine.UIElements;

namespace CodeDestroyer.Editor.UIElements
{
    /// <summary>
    /// A custom visual element that represents a line. It can be horizontal or vertical, with customizable length and color.
    /// </summary>
    [UxmlElement]
    public partial class Line : VisualElement
    {

        /// <summary>
        /// Creates a horizontal line with the default height of 1 unit.
        /// </summary>
        public Line()
        {
            style.height = 1;
            //style.width = Length.Percent(100f);
            style.backgroundColor = GlobalVariables.DefaultLineColor;
        }

        /// <summary>
        /// Creates a line with a customizable length, orientation (horizontal or vertical), and color.
        /// The length is clamped between 1 and 200 units.
        /// </summary>
        /// <param name="lineLength">The length of the line (1f to 200f).</param>
        /// <param name="isVertical">Determines if the line is vertical (default is horizontal).</param>
        /// <param name="lineColor">The color of the line. Defaults to Unity’s default line color.</param>
        public Line(float lineLength = 1f, bool isVertical = false, Color lineColor = default)
        {
            style.backgroundColor = (lineColor == default) ? GlobalVariables.DefaultLineColor : lineColor;
            lineLength = Mathf.Clamp(lineLength, 1f, 200f);

            if (isVertical)
            {
                style.width = 1;
                style.height = lineLength;
            }
            else
            {
                style.width = lineLength;
                style.height = 1;
            }
        }
    }
}
