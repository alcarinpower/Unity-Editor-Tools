using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;


namespace CodeDestroyer.Editor.UIElements
{
    /// <summary>
    /// Exact same UIElement with ToolbarSearchField but search algorithm implemented.
    /// </summary>
    [UxmlElement]
    public partial class ToolbarSearchPanel : ToolbarSearchField
    {

        /// <summary>
        /// Default constructor for UI Toolkit instantiation. No Search algorithm implemented.
        /// </summary>
        public ToolbarSearchPanel()
        {

        }

        /// <summary>
        /// Initializes the search panel with a search algorithm.
        /// </summary>
        /// <param name="searchList">The list of strings to search from.</param>
        /// <param name="resultList">The list that will hold search results.</param>
        /// <param name="OnEmpty">Callback when no search results are found.</param>
        /// <param name="OnFilled">Callback when search results are available.</param>
        /// <param name="OnUndoRedo">Callback for handling undo/redo actions.</param>
        public ToolbarSearchPanel(List<string> searchList, List<string> resultList, Action OnEmpty = null, Action OnFilled = null, Action OnUndoRedo = null)
        {
            resultList.Clear();
            this.RegisterValueChangedCallback(evt =>
            {
                OnUndoRedo?.Invoke();

                if (!string.IsNullOrEmpty(this.value))
                {
                    string searchQuery = this.value;
                    searchQuery.Trim();

                    resultList.Clear();

                    for (int i = 0; i < searchList.Count; i++)
                    {
                        string searchString = searchList[i];
                        if (!string.IsNullOrEmpty(searchString))
                        {
                            if (searchString.ToLower().Contains(searchQuery.ToLower()))
                            {
                                if (!resultList.Contains(searchString))
                                {
                                    resultList.Add(searchString);
                                }
                            }
                        }

                    }

                    OnFilled?.Invoke();
                }
                else
                {
                    OnEmpty?.Invoke();
                }
            });

        }
    }
}
