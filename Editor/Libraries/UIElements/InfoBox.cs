using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace CodeDestroyer.Editor.UIElements
{
    /// <summary>
    /// A custom Box.
    /// </summary>
    [UxmlElement]
    public partial class InfoBox : Box
    {
        public Image infoIconImage { get; set; }
        public Label boxText { get; set; }

        private Color defaultBackgroundColor = new Color(0f, 0f, 0f, 0.3f);

        /// <summary>
        /// Creates an InfoBox.
        /// </summary>
        public InfoBox()
        {
            Color notSoBlack = new Color(0f, 0f, 0f, 0.3f);
            style.backgroundColor = notSoBlack;

            boxText = new Label("Info Box");
            boxText.style.whiteSpace = WhiteSpace.Normal;
            Add(boxText);
        }

        /// <summary>
        /// Create an infobox with borderOfBox.
        /// </summary>
        /// <param name="boxText">Text of InfoBox.</param>
        /// <param name="infoBoxIconType">Icon type of Infobox.</param>
        /// <param name="borderRadiusOfBox">Border radius.</param>
        public InfoBox(string boxText, InfoBoxIconType infoBoxIconType, float borderRadiusOfBox)
        {
            style.backgroundColor = defaultBackgroundColor;
            style.borderTopLeftRadius = borderRadiusOfBox;
            style.borderTopRightRadius = borderRadiusOfBox;
            style.borderBottomLeftRadius = borderRadiusOfBox;
            style.borderBottomRightRadius = borderRadiusOfBox;
            style.paddingTop = 8f;
            style.paddingBottom = 8f;


            switch (infoBoxIconType)
            {
                case InfoBoxIconType.None:
                    style.paddingLeft = 8f;

                    break;

                case InfoBoxIconType.Info:
                    ChangeIconProperties();
                    style.paddingLeft = 45f;
                    infoIconImage.image = EditorGUIUtility.IconContent(GlobalVariables.UnityInfoIconName).image;

                    break;
                case InfoBoxIconType.Error:
                    ChangeIconProperties();
                    style.paddingLeft = 45f;
                    infoIconImage.image = EditorGUIUtility.IconContent(GlobalVariables.UnityErrorIconName).image;

                    break;
                case InfoBoxIconType.Warning:
                    ChangeIconProperties();
                    style.paddingLeft = 45f;
                    infoIconImage.image = EditorGUIUtility.IconContent(GlobalVariables.UnityWarnIconName).image;

                    break;
                case InfoBoxIconType.Ok:
                    ChangeIconProperties();
                    style.paddingLeft = 45f;
                    infoIconImage.image = EditorGUIUtility.IconContent(GlobalVariables.UnityInstalledIconName).image;

                    break;
                default:
                    break;
            }

            this.boxText = new Label(boxText);
            this.boxText.style.whiteSpace = WhiteSpace.Normal;
            Add(infoIconImage);
            Add(this.boxText);
        }


        /// <summary>
        /// Create a UIElement box with control of all corner radius with one variable and background color.
        /// </summary>
        /// <param name="boxText">Text of InfoBox.</param>
        /// <param name="infoBoxIconType">Icon type of Infobox.</param>
        /// <param name="borderRadiusOfBox">All borders radius of box.</param>
        /// <param name="boxBackgroundColor">Background color of box.</param>
        public InfoBox(string boxText, InfoBoxIconType infoBoxIconType, float borderRadiusOfBox, Color boxBackgroundColor)
        {
            //style.flexDirection = FlexDirection.Column;
            style.borderTopLeftRadius = borderRadiusOfBox;
            style.borderTopRightRadius = borderRadiusOfBox;
            style.borderBottomLeftRadius = borderRadiusOfBox;
            style.borderBottomRightRadius = borderRadiusOfBox;
            style.paddingTop = 8f;
            style.paddingBottom = 8f;
            if (boxBackgroundColor == default)
            {
                style.backgroundColor = defaultBackgroundColor;
            }
            else
            {
                style.backgroundColor = boxBackgroundColor;
            }


            switch (infoBoxIconType)
            {
                case InfoBoxIconType.None:
                    style.paddingLeft = 8f;

                    break;

                case InfoBoxIconType.Info:
                    ChangeIconProperties();
                    style.paddingLeft = 45f;
                    infoIconImage.image = EditorGUIUtility.IconContent(GlobalVariables.UnityInfoIconName).image;

                    break;
                case InfoBoxIconType.Error:
                    ChangeIconProperties();
                    style.paddingLeft = 45f;
                    infoIconImage.image = EditorGUIUtility.IconContent(GlobalVariables.UnityErrorIconName).image;

                    break;
                case InfoBoxIconType.Warning:
                    ChangeIconProperties();
                    style.paddingLeft = 45f;
                    infoIconImage.image = EditorGUIUtility.IconContent(GlobalVariables.UnityWarnIconName).image;

                    break;
                case InfoBoxIconType.Ok:
                    ChangeIconProperties();
                    style.paddingLeft = 45f;
                    infoIconImage.image = EditorGUIUtility.IconContent(GlobalVariables.UnityInstalledIconName).image;

                    break;
                default:
                    break;
            }

            this.boxText = new Label(boxText);
            this.boxText.style.whiteSpace = WhiteSpace.Normal;
            Add(infoIconImage);
            Add(this.boxText);
        }

        /// <summary>
        /// Create box with control of four corners and background color.
        /// </summary>
        /// <param name="boxText">Text of InfoBox.</param>
        /// <param name="infoBoxIconType">Icon type of Infobox.</param>
        /// <param name="borderTopLeftRadius">Top left radius of box.</param>
        /// <param name="borderTopRightRadius">Top right radius of box.</param>
        /// <param name="borderBottomLeftRadius">Bottom left radius of box.</param>
        /// <param name="borderBottomRightRadius">Bottom right radius of box.</param>
        /// <param name="boxBackgroundColor">Background color of box.</param>
        public InfoBox(string boxText, InfoBoxIconType infoBoxIconType, float borderTopLeftRadius, float borderTopRightRadius, float borderBottomLeftRadius, float borderBottomRightRadius,
            Color boxBackgroundColor)
        {
            //style.flexDirection = FlexDirection.Column;
            style.backgroundColor = defaultBackgroundColor;
            style.borderTopLeftRadius = borderTopLeftRadius;
            style.borderTopRightRadius = borderTopRightRadius;
            style.borderBottomLeftRadius = borderBottomLeftRadius;
            style.borderBottomRightRadius = borderBottomRightRadius;
            style.paddingTop = 8f;
            style.paddingBottom = 8f;
            if (boxBackgroundColor == default)
            {
                style.backgroundColor = defaultBackgroundColor;
            }
            else
            {
                style.backgroundColor = boxBackgroundColor;
            }

            switch (infoBoxIconType)
            {
                case InfoBoxIconType.None:
                    style.paddingLeft = 8f;

                    break;

                case InfoBoxIconType.Info:
                    ChangeIconProperties();
                    style.paddingLeft = 45f;
                    infoIconImage.image = EditorGUIUtility.IconContent(GlobalVariables.UnityInfoIconName).image;

                    break;
                case InfoBoxIconType.Error:
                    ChangeIconProperties();
                    style.paddingLeft = 45f;
                    infoIconImage.image = EditorGUIUtility.IconContent(GlobalVariables.UnityErrorIconName).image;

                    break;
                case InfoBoxIconType.Warning:
                    ChangeIconProperties();
                    style.paddingLeft = 45f;
                    infoIconImage.image = EditorGUIUtility.IconContent(GlobalVariables.UnityWarnIconName).image;

                    break;
                case InfoBoxIconType.Ok:
                    ChangeIconProperties();
                    style.paddingLeft = 45f;
                    infoIconImage.image = EditorGUIUtility.IconContent(GlobalVariables.UnityInstalledIconName).image;

                    break;
                default:
                    break;
            }


            this.boxText = new Label(boxText);
            this.boxText.style.whiteSpace = WhiteSpace.Normal;
            Add(infoIconImage);
            Add(this.boxText);
        }

        private void ChangeIconProperties()
        {
            infoIconImage = new Image();
            infoIconImage.style.left = 5f;
            infoIconImage.style.top = 0;
            infoIconImage.style.bottom = 0;
            infoIconImage.style.width = 40;
            infoIconImage.style.position = Position.Absolute;
        }
    }
    public enum InfoBoxIconType
    {
        None,
        Info,
        Error,
        Warning,
        Ok
    }
}