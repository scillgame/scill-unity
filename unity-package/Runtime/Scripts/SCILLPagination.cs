using UnityEngine;
using UnityEngine.UI;

namespace SCILL
{
    /// <summary>
    ///     <para>
    ///         This class handles pagination for the <see cref="SCILLBattlePassLevels" /> class. Not all levels fit into the
    ///         screen, so they are paginated. Use this class to manage that.
    ///     </para>
    ///     <para>
    ///         This class offers two types of pagination. Via forward and backward buttons. And via pagination dots, where
    ///         each dot stands for one page. This allows the user to quickly navigate to pages further away.
    ///     </para>
    /// </summary>
    public class SCILLPagination : MonoBehaviour
    {
        public delegate void ActivePageChangedHandler(int pageIndex);

        /// <summary>
        ///     Connect a transform that will be used as a container for the dots. Use HorizontalLayoutGroup for automatic layout
        ///     on that container.
        /// </summary>
        [Header("Connections")]
        [Tooltip(
            "Connect a transform that will be used as a container for the dots. Use HorizontalLayoutGroup for automatic layout on that container.")]
        public Transform paginationContainer;

        /// <summary>
        ///     Connect a button which will be used to navigate to the previous page. It will be hidden if there is no previous
        ///     page.
        /// </summary>
        [Header("Navigation")]
        [Tooltip(
            "Connect a button that will be used to trigger the previous page of the battle pass levels. It will be hidden if the first page is displayed")]
        public Button prevButton;

        /// <summary>
        ///     Connect a button which will be used to navigate to the next page. It will be hidden if user has navigated to the
        ///     last available page.
        /// </summary>
        [Tooltip(
            "Connect a button that will be used to trigger the next page of the battle pass levels. It will be hidden if there are no more pages left")]
        public Button nextButton;

        /// <summary>
        ///     A <c>UnityEngine.UI.Text</c> field which is used to set show the user the current navigation state, i.e. Page 1/10
        /// </summary>
        [Tooltip("A text field which is used to set show the user the current navigation state, i.e. Page 1/10")]
        public Text pageText;

        /// <summary>
        ///     Often, the number of levels available in a battle pass cannot be rendered at once on the screen. Use this setting
        ///     to set the number of levels shown at once. Connect the <see cref="prevButton" /> and <see cref="nextButton" /> to
        ///     implement pagination
        ///     functionality.
        /// </summary>
        [Header("Settings")] [Tooltip("Number of battle pass levels shown per page.")]
        public int itemsPerPage = 5;

        /// <summary>
        ///     A prefab that will be used to render an active dot in the pagination dots list.
        /// </summary>
        [Header("Prefabs")]
        [Tooltip("Assign a UI prefab that will be used to render an active dot in the pagination widget")]
        public GameObject dotButtonActivePagePrefab;

        /// <summary>
        ///     A prefab that will be used to render an inactive dot in the pagination dots list. This prefab must have a
        ///     <c>Button</c> component attached, but you do <b>not</b> need to attach a click event, as this class adds a listener
        ///     to the
        ///     correct internal click handler function.
        /// </summary>
        [Tooltip(
            "Assign a UI prefab with a Button component that will be used to render a selectable page dot in the pagination widget")]
        public GameObject dotButtonPrefab;


        private int _currentPageIndex;

        [HideInInspector] private int _numItems;

        public int numItems
        {
            get => _numItems;
            set
            {
                _numItems = value;
                UpdateButtons();

                // Debug.Log("NUM ITEMS " + value);
            }
        }

        public int currentPageIndex
        {
            get => _currentPageIndex;
            set
            {
                _currentPageIndex = value;
                UpdateButtons();
            }
        }

        public int numPages => (int) decimal.Ceiling(_numItems / (decimal) itemsPerPage);

        private void Awake()
        {
            if (prevButton) prevButton.onClick.AddListener(OnPrevPage);

            if (nextButton) nextButton.onClick.AddListener(OnNextPage);
        }

        // Start is called before the first frame update
        private void Start()
        {
            UpdateButtons();
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public event ActivePageChangedHandler OnActivePageChanged;

        private void UpdateButtons()
        {
            if (paginationContainer && dotButtonPrefab && dotButtonActivePagePrefab)
            {
                ClearList();

                for (var i = 0; i < numPages; i++)
                    if (currentPageIndex == i)
                    {
                        var go = Instantiate(dotButtonActivePagePrefab.gameObject, paginationContainer, false);
                    }
                    else
                    {
                        var go = Instantiate(dotButtonPrefab.gameObject, paginationContainer, false);
                        var button = go.GetComponent<Button>();
                        if (button)
                        {
                            var i1 = i;
                            button.onClick.AddListener(delegate { OnButtonPressed(i1); });
                        }
                    }
            }

            if (currentPageIndex <= 0)
            {
                if (prevButton) prevButton.gameObject.SetActive(false);
            }
            else
            {
                if (prevButton) prevButton.gameObject.SetActive(true);
            }

            if (currentPageIndex >= numPages - 1)
            {
                if (nextButton) nextButton.gameObject.SetActive(false);
            }
            else
            {
                if (nextButton) nextButton.gameObject.SetActive(true);
            }

            if (pageText)
            {
                if (currentPageIndex <= 0 &&
                    currentPageIndex >= numPages - 1)
                {
                    pageText.enabled = false;
                }
                else
                {
                    pageText.text = "Page " + (currentPageIndex + 1) + "/" + numPages;
                    pageText.enabled = true;
                }
            }
        }

        private void OnButtonPressed(int pageIndex)
        {
            Debug.Log("BUTTON " + pageIndex);
            currentPageIndex = pageIndex;
            OnActivePageChanged?.Invoke(currentPageIndex);

            UpdateButtons();
        }

        private void ClearList()
        {
            var container = paginationContainer;
            if (!paginationContainer) container = transform;

            foreach (var child in container.GetComponentsInChildren<Button>()) Destroy(child.gameObject);
        }

        public void OnNextPage()
        {
            currentPageIndex += 1;
            OnActivePageChanged?.Invoke(currentPageIndex);

            UpdateButtons();
        }

        public void OnPrevPage()
        {
            currentPageIndex -= 1;
            OnActivePageChanged?.Invoke(currentPageIndex);

            UpdateButtons();
        }
    }
}