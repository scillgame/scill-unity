using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;

namespace SCILL
{
    /// <summary>
    ///     <para>
    ///         This component will instantiate level UI prefabs and will add them as children. Use some sort of auto layout
    ///         like
    ///         the vertical, horizontal or grid layout components to build the user interface.
    ///     </para>
    ///     <para>
    ///         This component will also handle pagination and exposes functions to be connected to buttons click event and
    ///         properties that you can use to connect buttons that will be hidden and shown depending on the current
    ///         pagination
    ///         state.
    ///     </para>
    ///     <para>
    ///         There are three different types of levels in a battle pass:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Locked</item>
    ///         <item>Unlocked but not complete (current level)</item>
    ///         <item>Completed</item>
    ///     </list>
    ///     <para>
    ///         You need to connect prefabs from your Asset browser for each of these states. It’s easier to build three
    ///         different
    ///         prefabs rather than having one very complex prefab for these states. Best way is to build a level prefab and
    ///         then
    ///         creating prefab variants for the other states.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The level prefab must have a <c>UnityEngine.UI.Button</c> element somewhere. After instantiating the level
    ///         prefab it will search for a <c>Button</c> component in the level prefab and will attach a listener to that
    ///         button. Whenever the level is clicked, the event
    ///         <see cref="SCILLBattlePassManager.OnSelectedBattlePassLevelChanged" /> will be triggered.
    ///     </para>
    ///     <para>
    ///         The <c>SCILLRewardPreview</c> will listen on that event and will render a preview of the reward (if available)
    ///         of the selected level.
    ///     </para>
    /// </remarks>
    public class SCILLBattlePassLevels : MonoBehaviour
    {
        /// <summary>
        ///     Connect a <see cref="SCILLBattlePassLevel" /> prefab that will be used for all locked levels. This prefab will be
        ///     instantiated for
        ///     the locked levels in the battle pass and will be added as a child. Make sure that a <c>Button</c> element is
        ///     available
        ///     somewhere in the level prefab hierarchy so that the automatic level selection mechanism can do its job.
        /// </summary>
        [Header("Prefabs")]
        [Tooltip(
            "Choose one of the Battle Pass Level prefabs. This prefab will be instantiated for each level available in the battle pass and will be added to the battlePassLevels transform")]
        public SCILLBattlePassLevel levelPrefab;

        /// <summary>
        ///     Connect a
        ///     <see cref="SCILLBattlePassLevel" /> prefab that will be used for all locked levels. This prefab will be
        ///     instantiated for the locked levels in the battle pass
        ///     and will be added as a child. Make sure that a <c>Button</c> element is available somewhere in the level prefab
        ///     hierarchy
        ///     so that the automatic level selection mechanism can do its job.
        /// </summary>
        [Tooltip(
            "Choose one of the Battle Pass Level prefabs which is used if the level is locked. This prefab will be instantiated for each level unlocked in the battle pass and will be added to the battlePassLevels transform")]
        public SCILLBattlePassLevel lockedLevelPrefab;

        /// <summary>
        ///     Connect a
        ///     <see cref="SCILLBattlePassLevel" /> prefab
        ///     that will be used for the current levels. This prefab will be instantiated for the current active in the battle
        ///     pass
        ///     and will be added as a child. Make sure that a <c>Button</c> element is available somewhere in the level prefab
        ///     hierarchy
        ///     so that the automatic level selection mechanism can do its job.
        /// </summary>
        [Tooltip(
            "Choose one of the Battle Pass Level prefabs which is used for the current level. This prefab will be instantiated for the one level which is active in the battle pass and will be added to the battlePassLevels transform")]
        public SCILLBattlePassLevel currentLevelPrefab;

        /// <summary>
        ///     Connect a <see cref="SCILLPagination"/> object which is part of your hierarchy to allow the user to paginate through your levels.
        ///     The <c>itemsPerPage</c> in this class determines how many items are <b>rendered</b> for the current page, and the
        ///     <c>itemsPerPage</c> in
        ///     the pagination object determine how many levels are <b>skipped</b> for the next page. This allows you to render a
        ///     couple
        ///     of more levels than necessary but helps users to understand that there is more if they fade away for example.
        /// </summary>
        [Header("Navigation")] [Tooltip("A pagination component that will be used to navigate the pages")]
        public SCILLPagination pagination;

        /// <summary>
        ///     Often, the number of levels available in a battle pass cannot be rendered at once on the screen. Use this setting
        ///     to set the number of levels shown at once. Connect a <see cref="SCILLPagination" /> object using the
        ///     <see cref="pagination" /> member to set the current page.
        /// </summary>
        [Header("Settings")]
        [Tooltip(
            "Number of battle pass levels shown per page. Should be equal to the number set in the paginator, but sometimes you want to use more.")]
        public int itemsPerPage = 5;

        /// <summary>
        ///     Per default, the level number is rendered in each level object. You can set this to <c>false</c> if you don’t want
        ///     to render the level number.
        /// </summary>
        [Tooltip(
            "Indicate if you want to show the level number for each reward. This value will be set for each Battle Pass Level Prefab.")]
        public bool showLevelInfo = true;

        private List<BattlePassLevel> _levels;

        [HideInInspector] private int currentPageIndex;

        private void Awake()
        {
            pagination.OnActivePageChanged += index =>
            {
                Debug.Log("PAGE SET " + index);
                currentPageIndex = index;
                UpdateBattlePassLevelUI();
            };

            ClearList();
        }

        private void Start()
        {
        }

        private void OnEnable()
        {
            if (SCILLBattlePassManager.Instance) _levels = SCILLBattlePassManager.Instance.BattlePassLevels;

            SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer += OnBattlePassLevelsUpdatedFromServer;
            UpdateBattlePassLevelUI();
        }

        private void OnDestroy()
        {
            SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer -= OnBattlePassLevelsUpdatedFromServer;
        }

        private void ClearList()
        {
            foreach (var child in GetComponentsInChildren<SCILLBattlePassLevel>()) Destroy(child.gameObject);
        }

        private void OnBattlePassLevelsUpdatedFromServer(List<BattlePassLevel> battlePassLevels)
        {
            foreach (var child in GetComponentsInChildren<SCILLBattlePassLevel>()) Destroy(child.gameObject);

            _levels = battlePassLevels;
            UpdateBattlePassLevelUI();

            if (pagination) pagination.numItems = _levels.Count;
        }

        /// <summary>
        ///     Update the UI if anything changed. This is called automatically whenever required, but you can also trigger a
        ///     refresh from the outside be calling this function. It will clear the list and rebuild it with all levels available
        ///     in the <see cref="SCILLBattlePassManager" />.
        /// </summary>
        public void UpdateBattlePassLevelUI()
        {
            ClearList();

            if (_levels == null) return;

            // Calculate the level index to start adding to the list based on the pagination settings
            // We always want to render the number of items set in this component
            var levelStartIndex = currentPageIndex * pagination.itemsPerPage;
            while (_levels.Count - levelStartIndex < pagination.itemsPerPage) levelStartIndex--;
            // Debug.Log("LEVEL INDEX: " + levelStartIndex);

            // Debug.Log("UPDATTING LEVELS");
            for (var i = 0; i < itemsPerPage; i++)
            {
                var levelIndex = levelStartIndex + i;

                if (levelIndex >= 0 && levelIndex < _levels.Count)
                {
                    GameObject levelGO = null;

                    if (_levels[levelIndex].activated_at != null)
                    {
                        if (_levels[levelIndex].level_completed == true)
                            levelGO = Instantiate(levelPrefab.gameObject, transform, false);
                        else
                            levelGO = Instantiate(currentLevelPrefab.gameObject, transform, false);
                    }
                    else
                    {
                        // Debug.Log($"Non Activated Level: {_levels[levelIndex]}");

                        levelGO = Instantiate(lockedLevelPrefab.gameObject, transform, false);
                    }

                    var levelItem = levelGO.GetComponent<SCILLBattlePassLevel>();
                    if (levelItem)
                    {
                        levelItem.battlePassLevel = _levels[levelIndex];
                        levelItem.showLevelInfo = showLevelInfo;

                        // If a button is attached to the level item then attach a listener to show Reward preview
                        if (levelItem.button)
                            levelItem.button.onClick.AddListener(delegate { OnBattlePassLevelClicked(levelIndex); });

                        if (_levels[levelIndex].level_id ==
                            SCILLBattlePassManager.Instance.SelectedBattlePassLevel?.level_id)
                            levelItem.Select();
                    }
                }
            }
        }

        /// <summary>
        ///     Set the current page index and update the User Interface
        /// </summary>
        /// <param name="pageIndex">The new page index.</param>
        public void SetCurrentPageIndex(int pageIndex)
        {
            currentPageIndex = pageIndex;
            UpdateBattlePassLevelUI();
        }

        /// <summary>
        ///     This class will connect the first <c>Button</c> element found in the level prefabs to this function after they have
        ///     been
        ///     instantiated. This function will trigger the <see cref="SCILLBattlePassManager.OnSelectedBattlePassLevelChanged" />
        ///     delegate and will call the <c>Select</c> and
        ///     <c>Deselect</c> methods of the previous and current levels.
        /// </summary>
        /// <param name="levelIndex">
        ///     The index of the <see cref="SCILLBattlePassLevel" /> component of the level that has been
        ///     clicked. This can be used to retrieve the <see cref="SCILLBattlePassLevel" /> instance from
        ///     <c>SCILLBattlePassManager.Instance.BattlePassLevels</c>.
        /// </param>
        private void OnBattlePassLevelClicked(int levelIndex)
        {
            SCILLBattlePassManager.Instance.SelectedBattlePassLevelIndex = levelIndex;
        }
    }
}