using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCILL
{
    /// <summary>
    ///     This utility script can be used for a simple text display of the next battle pass level. Requires a single
    ///     <c>UnityEngine.UI.Text</c> to be present on the same GameObject or in a child. Will automatically update the
    ///     display using the events supplied by the <see cref="SCILLBattlePassManager" />.
    /// </summary>
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