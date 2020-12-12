using System;
using System.Collections;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;

public class SCILLBattlePassLevels : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Choose one of the Battle Pass Level prefabs. This prefab will be instantiated for each level available in the battle pass and will be added to the battlePassLevels transform")]
    public GameObject levelPrefab;
    
    [Tooltip("Choose one of the Battle Pass Level prefabs which is used if the level is locked. This prefab will be instantiated for each level unlocked in the battle pass and will be added to the battlePassLevels transform")]
    public GameObject lockedLevelPrefab;
    
    [Tooltip("Choose one of the Battle Pass Level prefabs which is used for the current level. This prefab will be instantiated for the one level which is active in the battle pass and will be added to the battlePassLevels transform")]
    public GameObject currentLevelPrefab;

    [HideInInspector]
    private int currentPageIndex = 0;
    
    private List<BattlePassLevel> _levels;
    private SCILLBattlePassLevel _selectedBattlePassLevel;
    private SCILLBattlePass _battlePassUI;

    // Events
    public delegate void SelectedBattlePassLevelChangedAction(BattlePassLevel selectedBattlePassLevel);
    public static event SelectedBattlePassLevelChangedAction OnSelectedBattlePassLevelChanged;
    
    private void Awake()
    {
        ClearList();
        _battlePassUI = GetComponentInParent<SCILLBattlePass>();
        if (_battlePassUI)
        {
            _battlePassUI.OnCurrentPageChanged += OnCurrentPageChanged;
        }
    }

    private void OnDestroy()
    {
        if (_battlePassUI)
        {
            _battlePassUI.OnCurrentPageChanged -= OnCurrentPageChanged;
        }
        
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

        this._levels = battlePassLevels;
        UpdateBattlePassLevelUI();
    }

    private void OnCurrentPageChanged(int currentPageIndex)
    {
        SetCurrentPageIndex(currentPageIndex);
    }

    void UpdateBattlePassLevelUI()
    {
        ClearList();
        
        if (_levels == null || _battlePassUI == null)
        {
            return;
        }

        Debug.Log("UPDATTING LEVELS");
        for (int i = 0; i < _battlePassUI.itemsPerPage; i++)
        {
            var levelIndex = (currentPageIndex * _battlePassUI.itemsPerPage) + i;
            if (levelIndex >= 0 && levelIndex < _levels.Count)
            {
                GameObject levelGO = null;
                if (_levels[i].activated_at != null)
                {
                    if (_levels[i].level_completed == true)
                    {
                        levelGO = Instantiate(levelPrefab, this.transform, false);
                    }
                    else
                    {
                        levelGO = Instantiate(currentLevelPrefab, this.transform, false);
                    }
                }
                else
                {
                    levelGO = Instantiate(lockedLevelPrefab, this.transform, false);
                }
                var levelItem = levelGO.GetComponent<SCILLBattlePassLevel>();
                if (levelItem)
                {
                    levelItem.battlePassLevel = _levels[levelIndex];
                    levelItem.showLevelInfo = _battlePassUI.showLevelInfo;
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
    
}
