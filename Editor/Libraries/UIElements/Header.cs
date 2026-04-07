using UnityEngine;
using UnityEngine.UIElements;

namespace CodeDestroyer.Editor.UIElements
{
    /// <summary>
    /// Basic header.
    /// </summary>
    [UxmlElement]
    public partial class Header : Label
    {

        /// <summary>
        /// Basic header fontsize = 18f.
        /// </summary>
        public Header()
        {
            style.unityTextAlign = TextAnchor.MiddleLeft;
            style.unityFontStyleAndWeight = FontStyle.Bold;
            style.whiteSpace = WhiteSpace.Normal;
            style.fontSize = 18;
        }

        /// <summary>
        /// Basic header with customized text and fontsize.
        /// </summary>
        /// <param name="headerText">Header text.</param>
        /// <param name="fontSize">Font size.</param>
        public Header(string headerText, float fontSize = 18)
        {
            text = headerText;
            style.unityTextAlign = TextAnchor.MiddleLeft;
            style.unityFontStyleAndWeight = FontStyle.Bold;
            style.whiteSpace = WhiteSpace.Normal;
            style.fontSize = fontSize;
        }
    }

}