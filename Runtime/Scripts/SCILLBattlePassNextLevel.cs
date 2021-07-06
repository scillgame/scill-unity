using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCILL
{
    public class SCILLBattlePassNextLevel : SCILLBattlePassCurrentLevel
    {
        protected override int GetCurrentLevel()
        {
            int currentLevel = base.GetCurrentLevel();
            if (currentLevel == -1)
            {
                // If battle pass is locked show the first level
                return 0;
            }

            if (currentLevel >= _battlePassLevels.Count - 1)
            {
                return _battlePassLevels.Count - 1;
            }

            return currentLevel + 1;
        }
    }
}