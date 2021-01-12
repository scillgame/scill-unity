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
    [Tooltip("A pagination component that will be used to navigate the pages")]
    public SCILLPagination pagination;
    
    [Header("Settings")]
    [Tooltip("Number of battle pass levels shown per page. Should be equal to the number set in the paginator, but sometimes you want to use more.")]
    public int itemsPerPage = 5;
    [Tooltip("Indicate if you want to show the level number for each reward. This value will be set for each Battle Pass Level Prefab.")]
    public bool showLevelInfo = true;

    [HideInInspector]
    private int currentPageIndex = 0;

    private List<BattlePassLevel> _levels;

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
        
        if (pagination)
        {
            pagination.numItems = _levels.Count;
        }
    }

    void UpdateBattlePassLevelUI()
    {
        ClearList();
        
        if (_levels == null)
        {
            return;
        }
        
        // Calculate the level index to start adding to the list based on the pagination settings
        // We always want to render the number of items set in this component
        int levelStartIndex = (currentPageIndex * pagination.itemsPerPage);
        while (_levels.Count - levelStartIndex < pagination.itemsPerPage)
        {
            levelStartIndex--;
        }
        Debug.Log("LEVEL INDEX: " + levelStartIndex);

        Debug.Log("UPDATTING LEVELS");
        for (int i = 0; i < itemsPerPage; i++)
        {
            var levelIndex = levelStartIndex + i;

            if (levelIndex >= 0 && levelIndex < _levels.Count)
            {
                GameObject levelGO = null;
                if (_levels[levelIndex].activated_at != null)
                {
                    if (_levels[levelIndex].level_completed == true)
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
                    
                    // If a button is attached to the level item then attach a listener to show Reward preview
                    if (levelItem.button)
                    {
                        levelItem.button.onClick.AddListener(delegate{OnBattlePassLevelClicked(levelIndex);});   
                    }

                    if (_levels[levelIndex].level_id == SCILLBattlePassManager.Instance.SelectedBattlePassLevel?.level_id) {
                        levelItem.Select();
                    }
                }                
            }
        }
    }

    public void SetCurrentPageIndex(int pageIndex)
    {
        currentPageIndex = pageIndex;
        UpdateBattlePassLevelUI();
    }

    void OnBattlePassLevelClicked(int levelIndex)
    {
        SCILLBattlePassManager.Instance.SelectedBattlePassLevelIndex = levelIndex;
    }

}
