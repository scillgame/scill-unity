using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

namespace SCILL
{
    public enum SCILLBattlePassLevelVisibility
    {
        Visible,
        Hidden,
        DoNothing
    }

    /// <summary>
    ///     <para>
    ///         This utility script can be used to automatically show or hide an <c>UnityEngine.UI.Image</c> component based on the
    ///         status of a parent <see cref="SCILLBattlePassLevel" />. Requires a <see cref="SCILLBattlePassLevel" />
    ///         component to be present in the parent hierarchy and a <c>UnityEngine.UI.Image</c> to be present on the same
    ///         GameObject.
    ///     </para>
    ///     <para>
    ///         For example if you'd like to show a battle pass level icon only if it is unlocked and uncompleted, you'd set
    ///         <see cref="ifUnlocked" /> and <see cref="ifUncompleted" /> to <c>Visible</c>, <see cref="ifLocked" /> and
    ///         <see cref="ifCompleted" /> to <c>Hidden</c>.
    ///     </para>
    ///     <para>
    ///         The script will update automatically based on the status of the parents <see cref="SCILLBattlePassLevel" />.
    ///     </para>
    /// </summary>
    public class SCILLBattlePassLevelToggleVisibility : MonoBehaviour
    {
        /// <summary>
        ///     Choose the visibility behavior if the Battle Pass Level is locked.
        /// </summary>
        public SCILLBattlePassLevelVisibility ifLocked;

        /// <summary>
        ///     Choose the visibility behavior if the Battle Pass Level is unlocked.
        /// </summary>
        public SCILLBattlePassLevelVisibility ifUnlocked;

        /// <summary>
        ///     Choose the visibility behavior if the Battle Pass Level is completed.
        /// </summary>
        public SCILLBattlePassLevelVisibility ifCompleted;

        /// <summary>
        ///     Choose the visibility behavior if the Battle Pass Level is uncompleted.
        /// </summary>
        public SCILLBattlePassLevelVisibility ifUncompleted;

        private Image _image;

        [HideInInspector] public BattlePassLevel battlePassLevel;

        // Start is called before the first frame update
        private void Start()
        {
            var battlePassLevelUI = GetComponentInParent<SCILLBattlePassLevel>();
            if (battlePassLevelUI) battlePassLevel = battlePassLevelUI.battlePassLevel;

            _image = GetComponent<Image>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_image || battlePassLevel == null) return;

            if (ifLocked != SCILLBattlePassLevelVisibility.DoNothing)
                if (battlePassLevel.activated_at == null)
                    Show(ifLocked == SCILLBattlePassLevelVisibility.Visible);

            if (ifUnlocked != SCILLBattlePassLevelVisibility.DoNothing)
                if (battlePassLevel.activated_at != null)
                    Show(ifUnlocked == SCILLBattlePassLevelVisibility.Visible);

            if (ifCompleted != SCILLBattlePassLevelVisibility.DoNothing)
                if (battlePassLevel.level_completed == true)
                    Show(ifCompleted == SCILLBattlePassLevelVisibility.Visible);

            if (ifUncompleted != SCILLBattlePassLevelVisibility.DoNothing)
                if (battlePassLevel.level_completed == false)
                    Show(ifUncompleted == SCILLBattlePassLevelVisibility.Visible);
        }

        private void Show(bool show)
        {
            _image.enabled = show;
        }
    }
}