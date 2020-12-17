using System;
using System.Collections;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

public class SCILLBattlePassLevels : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Choose one of the Battle Pass Level prefabs. This prefab will be instantiated for each level available in the battle pass and will be added to the battlePassLevels transform")]
    public SCILLBattlePassLevel levelPrefab;
    
    [Tooltip("Choose one of the Battle Pass Level prefabs which is used if the level is locked. This prefab will be instantiated for each level unlocked in the battle pass and will be added to the battlePassLevels transform")]
    public SCILLBattlePassLevel lockedLevelPrefab;
    
    [Tooltip("Choose one of the Battle Pass Level prefabs which is used for the current level. This prefab will be instantiated for the one level which is active in the battle pass and will be added to the battlePassLevels transform")]
    public SCILLBattlePassLevel currentLevelPrefab;
    
    [Header("Navigation")]
    [Tooltip("Connect a button that will be used to trigger the previous page of the battle pass levels. It will be hidden if the first page is displayed")]
    public Button prevButton;
    [Tooltip("Connect a button that will be used to trigger the next page of the battle pass levels. It will be hidden if there are no more pages left")]
    public Button nextButton;
    [Tooltip("A text field which is used to set show the user the current navigation state, i.e. Page 1/10")]
    public Text pageText;
    
    [Header("Settings")]
    [Tooltip("Number of battle pass levels shown per page.")]
    public int itemsPerPage = 5;
    [Tooltip("Indicate if you want to show the level number for each reward. This value will be set for each Battle Pass Level Prefab.")]
    public bool showLevelInfo = true;

    [HideInInspector]
    private int currentPageIndex = 0;

    private List<BattlePassLevel> _levels;
    private SCILLBattlePassLevel _selectedBattlePassLevel;

    // Events
    public delegate void SelectedBattlePassLevelChangedAction(BattlePassLevel selectedBattlePassLevel);
    public static event SelectedBattlePassLevelChangedAction OnSelectedBattlePassLevelChanged;
    
    private void Awake()
    {
        ClearList();
    }

    private void OnDestroy()
    {
        SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer -= OnBattlePassLevelsUpdatedFromServer;
    }

    void ClearList()
    {
        foreach (SCILLBattlePassLevel child in GetComponentsInChildren<SCILLBattlePassLevel>()) {
            Destroy(child.gameObject);
        }      
    }

    private void OnEnable()
    {
        if (SCILLBattlePassManager.Instance)
        {
            _levels = SCILLBattlePassManager.Instance.BattlePassLevels;
        }
        SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer += OnBattlePassLevelsUpdatedFromServer;
        UpdateBattlePassLevelUI();
    }

    private void OnBattlePassLevelsUpdatedFromServer(List<BattlePassLevel> battlePassLevels)
    {
        foreach (SCILLBattlePassLevel child in GetComponentsInChildren<SCILLBattlePassLevel>()) {
            Destroy(child.gameObject);
        }

        _levels = battlePassLevels;
        UpdateBattlePassLevelUI();
    }

    void UpdateBattlePassLevelUI()
    {
        ClearList();
        
        UpdateNavigationButtons();
        
        if (_levels == null)
        {
            return;
        }

        Debug.Log("UPDATTING LEVELS");
        for (int i = 0; i < itemsPerPage; i++)
        {
            var levelIndex = (currentPageIndex * itemsPerPage) + i;
            if (levelIndex >= 0 && levelIndex < _levels.Count)
            {
                GameObject levelGO = null;
                if (_levels[i].activated_at != null)
                {
                    if (_levels[i].level_completed == true)
                    {
                        levelGO = Instantiate(levelPrefab.gameObject, this.transform, false);
                    }
                    else
                    {
                        levelGO = Instantiate(currentLevelPrefab.gameObject, this.transform, false);
                    }
                }
                else
                {
                    levelGO = Instantiate(lockedLevelPrefab.gameObject, this.transform, false);
                }
                var levelItem = levelGO.GetComponent<SCILLBattlePassLevel>();
                if (levelItem)
                {
                    levelItem.battlePassLevel = _levels[levelIndex];
                    levelItem.showLevelInfo = showLevelInfo;
                    levelItem.button.onClick.AddListener(delegate{OnBattlePassLevelClicked(levelItem);});
                }                
            }
        }
    }

    public void SetCurrentPageIndex(int pageIndex)
    {
        currentPageIndex = pageIndex;
        UpdateBattlePassLevelUI();
    }
    
    void OnBattlePassLevelClicked(SCILLBattlePassLevel level)
    {
        Debug.Log("CLICKED");
        if (_selectedBattlePassLevel)
        {
            _selectedBattlePassLevel.Deselect();
        }

        _selectedBattlePassLevel = level;
        _selectedBattlePassLevel.Select();

        
        // Inform listening delegates that the selected battle pass level changed
        if (OnSelectedBattlePassLevelChanged != null)
        {
            Debug.Log("SENDING EVENT");
            OnSelectedBattlePassLevelChanged(level.battlePassLevel);
        }
    }
    
    private void UpdateNavigationButtons()
    {
        if (_levels == null || _levels.Count <= 0)
        {
            return;
        }
        
        if (currentPageIndex <= 0)
        {
            if (prevButton) prevButton.gameObject.SetActive(false);
        }
        else
        {
            if (prevButton) prevButton.gameObject.SetActive(true);
        }

        if (currentPageIndex >= Decimal.Ceiling((decimal) _levels.Count / (decimal) itemsPerPage) - 1)
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
                currentPageIndex >= Decimal.Ceiling((decimal) _levels.Count / (decimal) itemsPerPage) - 1)
            {
                pageText.enabled = false;
            }
            else
            {
                pageText.text = "Page " + (currentPageIndex + 1) + "/" + Decimal.Ceiling((decimal) _levels.Count / (decimal) itemsPerPage);
                pageText.enabled = true;
            }
        }
    }

    public void OnNextPage()
    {
        currentPageIndex += 1;
        
        UpdateBattlePassLevelUI();
    }

    public void OnPrevPage()
    {
        currentPageIndex -= 1;

        UpdateBattlePassLevelUI();
    }
    
}
