using System;
using System.Collections;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

public class SCILLBattlePassCurrentLevel : MonoBehaviour
{
    [Tooltip("Set the name of the level, use {level} for the level number")]
    public string format = "Level {level}";

    private Text LevelText;
    protected List<BattlePassLevel> _battlePassLevels;

    private void Awake()
    {
        LevelText = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        OnBattlePassLevelsUpdatedFromServer(SCILLBattlePassManager.Instance.BattlePassLevels);
    }

    private void OnEnable()
    {
        SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer += OnBattlePassLevelsUpdatedFromServer;
        UpdateUI();
    }

    private void OnDestroy()
    {
        SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer -= OnBattlePassLevelsUpdatedFromServer;
    }
    
    private void OnBattlePassLevelsUpdatedFromServer(List<BattlePassLevel> battlePassLevels)
    {
        _battlePassLevels = battlePassLevels;
        UpdateUI();
    }

    protected virtual int GetCurrentLevel()
    {
        int currentLevelIndex = -1;
        for (int i = 0; i < _battlePassLevels.Count; i++)
        {
            if (_battlePassLevels[i].activated_at == null && _battlePassLevels[i].level_completed == false)
            {
                break;
            }
            else
            {
                currentLevelIndex = i;
            }
        }

        return currentLevelIndex;
    }
    
    void UpdateUI()
    {
        if (LevelText && _battlePassLevels != null)
        {
            int currentLevelIndex = GetCurrentLevel();
            if (currentLevelIndex == -1)
            {
                return;
            }
            
            var newText = format.Replace("{level}", _battlePassLevels[currentLevelIndex].level_priority.ToString());
            if (newText != LevelText.text)
            {
                LevelText.text = newText;

                if (GetComponent<Animation>())
                {
                    GetComponent<Animation>().Play();
                }
            }
        }
    }
}
