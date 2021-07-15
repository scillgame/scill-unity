using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

namespace SCILL
{
    /// <summary>
    ///     This class can be used for a simple text display of the current battle pass level. Requires a single
    ///     <c>UnityEngine.UI.Text</c> to be present on the same GameObject or in a child. Will automatically update the
    ///     display using the events supplied by the <see cref="SCILLBattlePassManager" />.
    /// </summary>
    public class SCILLBattlePassCurrentLevel : MonoBehaviour
    {
        /// <summary>
        ///     The format in which the current level should be displayed. Use {level} for the level number.
        /// </summary>
        [Tooltip("Set the name of the level, use {level} for the level number")]
        public string format = "Level {level}";

        protected List<BattlePassLevel> _battlePassLevels;

        private Text LevelText;

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
            var currentLevelIndex = -1;
            for (var i = 0; i < _battlePassLevels.Count; i++)
                if (_battlePassLevels[i].activated_at == null && _battlePassLevels[i].level_completed == false)
                    break;
                else
                    currentLevelIndex = i;

            return currentLevelIndex;
        }

        private void UpdateUI()
        {
            if (LevelText && _battlePassLevels != null)
            {
                var currentLevelIndex = GetCurrentLevel();
                if (currentLevelIndex == -1) return;

                var newText = format.Replace("{level}", _battlePassLevels[currentLevelIndex].level_priority.ToString());
                if (newText != LevelText.text)
                {
                    LevelText.text = newText;

                    if (GetComponent<Animation>()) GetComponent<Animation>().Play();
                }
            }
        }
    }
}