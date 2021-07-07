using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

namespace SCILL
{
    public enum SCILLBattlePassVisibility
    {
        Visible,
        Hidden,
        DoNothing
    }

    /// <summary>
    ///     <para>
    ///         This utility script can be used to automatically show or hide an <c>UnityEngine.UI.Image</c> component based on
    ///         the
    ///         status of the currently selected <see cref="SCILLBattlePass" /> of the <see cref="SCILLBattlePassManager" />.
    ///         Requires an <c>UnityEngine.UI.Image</c> to be present on the same
    ///         GameObject.
    ///     </para>
    ///     <para>
    ///         For example if you'd like to show an icon only if the currently selected battle pass is unlocked, you'd set
    ///         <see cref="ifUnlocked" /> to <c>Visible</c> and <see cref="ifLocked" /> to <c>Hidden</c>.
    ///     </para>
    ///     <para>
    ///         The script will update automatically based on the status of the currently selected <see cref="SCILLBattlePassLevel"/>, using the <see cref="SCILLBattlePassManager.OnBattlePassUpdatedFromServer"/> event.
    ///     </para>
    /// </summary>
    public class SCILLBattlePassToggleVisibility : MonoBehaviour
    {
        /// <summary>
        ///     Choose the visibility behavior if the currently selected Battle Pass is locked.
        /// </summary>
        public SCILLBattlePassVisibility ifLocked;
        /// <summary>
        ///     Choose the visibility behavior if the currently selected Battle Pass is unlocked.
        /// </summary>
        public SCILLBattlePassVisibility ifUnlocked;

        private BattlePass _battlePass;

        private Image _image;

        // Start is called before the first frame update
        private void Start()
        {
            _image = GetComponent<Image>();
        }

        private void OnEnable()
        {
            SCILLBattlePassManager.OnBattlePassUpdatedFromServer += OnBattlePassUpdatedFromServer;
        }

        private void OnDisable()
        {
            SCILLBattlePassManager.OnBattlePassUpdatedFromServer -= OnBattlePassUpdatedFromServer;
        }

        private void OnBattlePassUpdatedFromServer(BattlePass battlePass)
        {
            _battlePass = battlePass;
            UpdateUI();
        }

        // Update is called once per frame
        private void UpdateUI()
        {
            if (!_image || _battlePass == null) return;

            if (ifLocked != SCILLBattlePassVisibility.DoNothing)
                if (_battlePass.unlocked_at == null)
                    _image.enabled = ifLocked == SCILLBattlePassVisibility.Visible;

            if (ifUnlocked != SCILLBattlePassVisibility.DoNothing)
                if (_battlePass.unlocked_at != null)
                    _image.enabled = ifUnlocked == SCILLBattlePassVisibility.Visible;
        }
    }
}