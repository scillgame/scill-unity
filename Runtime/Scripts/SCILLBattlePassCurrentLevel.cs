using System;
using System.Collections;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

public class SCILLBattlePassCurrentLevel : MonoBehaviour
{
    private Text LevelText;
    private List<BattlePassLevel> _battlePassLevels;

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
    
    void UpdateUI()
    {
        if (LevelText && _battlePassLevels != null)
        {
            int currentLevelIndex = 0;
            for (int i = 0; i < _battlePassLevels.Count; i++)
            {
                if (_battlePassLevels[i].level_completed == false)
                {
                    break;
                }
                else
                {
                    currentLevelIndex = i;
                }
            }
            
            var newText = _battlePassLevels[currentLevelIndex].level_priority.ToString();
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
