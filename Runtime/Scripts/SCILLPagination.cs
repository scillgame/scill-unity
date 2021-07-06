using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCILL
{
    public class SCILLPagination : MonoBehaviour
    {
        [Header("Connections")]
        [Tooltip(
            "Connect a transform that will be used as a container for the dots. Use HorizontalLayoutGroup for automatic layout on that container.")]
        public Transform paginationContainer;

        [Header("Navigation")]
        [Tooltip(
            "Connect a button that will be used to trigger the previous page of the battle pass levels. It will be hidden if the first page is displayed")]
        public Button prevButton;

        [Tooltip(
            "Connect a button that will be used to trigger the next page of the battle pass levels. It will be hidden if there are no more pages left")]
        public Button nextButton;

        [Tooltip("A text field which is used to set show the user the current navigation state, i.e. Page 1/10")]
        public Text pageText;

        [Header("Settings")] [Tooltip("Number of battle pass levels shown per page.")]
        public int itemsPerPage = 5;

        [Header("Prefabs")]
        [Tooltip("Assign a UI prefab that will be used to render an active dot in the pagination widget")]
        public GameObject dotButtonActivePagePrefab;

        [Tooltip(
            "Assign a UI prefab with a Button component that will be used to render a selectable page dot in the pagination widget")]
        public GameObject dotButtonPrefab;

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


        private int _currentPageIndex;

        public int currentPageIndex
        {
            get => _currentPageIndex;
            set
            {
                _currentPageIndex = value;
                UpdateButtons();
            }
        }

        public int numPages => (int) Decimal.Ceiling((decimal) _numItems / (decimal) itemsPerPage);

        public delegate void ActivePageChangedHandler(int pageIndex);

        public event ActivePageChangedHandler OnActivePageChanged;

        private void Awake()
        {
            if (prevButton)
            {
                prevButton.onClick.AddListener(OnPrevPage);
            }

            if (nextButton)
            {
                nextButton.onClick.AddListener(OnNextPage);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            UpdateButtons();
        }

        void UpdateButtons()
        {
            if (paginationContainer && dotButtonPrefab && dotButtonActivePagePrefab)
            {
                ClearList();

                for (var i = 0; i < numPages; i++)
                {
                    if (currentPageIndex == i)
                    {
                        GameObject go = Instantiate(dotButtonActivePagePrefab.gameObject, paginationContainer, false);
                    }
                    else
                    {
                        GameObject go = Instantiate(dotButtonPrefab.gameObject, paginationContainer, false);
                        Button button = go.GetComponent<Button>();
                        if (button)
                        {
                            var i1 = i;
                            button.onClick.AddListener(delegate { OnButtonPressed(i1); });
                        }
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

        void OnButtonPressed(int pageIndex)
        {
            Debug.Log("BUTTON " + pageIndex);
            currentPageIndex = pageIndex;
            OnActivePageChanged?.Invoke(currentPageIndex);

            UpdateButtons();
        }

        void ClearList()
        {
            Transform container = paginationContainer;
            if (!paginationContainer)
            {
                container = transform;
            }

            foreach (Button child in container.GetComponentsInChildren<Button>())
            {
                Destroy(child.gameObject);
            }
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

        // Update is called once per frame
        void Update()
        {
        }
    }
}