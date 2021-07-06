using System;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

namespace SCILL
{
    /// <summary>
    ///     <para>
    ///         Add this component to a Unity GameObject. It will create a UI for the <c>SelectedBattlePass</c> of the
    ///         <see cref="SCILLBattlePassManager" />. <c>SelectedBattlePass</c> connects to these delegates of
    ///         <see cref="SCILLBattlePassManager" /> to get notifications
    ///         whenever the battle pass changes and updates UI accordingly:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             <see cref="SCILLBattlePassManager.OnBattlePassUpdatedFromServer" />
    ///         </item>
    ///         <item>
    ///             <see cref="SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer" />
    ///         </item>
    ///     </list>
    ///     <para>
    ///         The <c>SCILLBattlePass</c>  will only handle battle pass related UI, like unlocking/purchase buttons..
    ///         <see cref="SCILLBattlePassLevels" /> is responsible to render the levels of the battle pass and do the proper
    ///         levels
    ///         pagination.
    ///     </para>
    ///     <para>
    ///         The best way to get started is to drop the
    ///         <a href="https://developers.scillgame.com/sdks/unity/prefabs.html#battle-pass">BattlePass prefab</a> into a
    ///         Canvas. This prefab already has prepared
    ///         the
    ///         connections and hierarchy for a battle pass.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     <c>SCILLBattlePass</c> exposes a delegate which allow other parts of the application to get “notified”
    ///     whenever something happens with the battle pass UI. In this case, if the battle pass is unlocked.
    /// </remarks>
    public class SCILLBattlePass : MonoBehaviour
    {
        public delegate void BattlePassUnlockedAction(BattlePass battlePass);
        
        /// <summary>
        /// If the battle pass is unlocked, the event will be called. This will allow you to update your UI once the battle pass has been unlocked.
        /// </summary>
        /// <remarks>
        /// The <see cref="SCILLBattlePassManager"/> uses this delegate to update the UI once the battle pass is unlocked. You may want to add additional functionality after a battle pass has been unlocked.
        /// </remarks>
        /// <example>
        /// Using OnBattlePassUnlocked
        /// <code>
        /// public class ExampleScript : MonoBehaviour
        /// {
        ///     private void Awake()
        ///     {
        ///         SCILLBattlePass.OnBattlePassUnlocked += OnOnBattlePassUnlocked;
        ///     }
        /// 
        ///     private void OnDestroy()
        ///     {
        ///         SCILLBattlePass.OnBattlePassUnlocked -= OnOnBattlePassUnlocked;
        ///     }
        /// 
        ///     private void OnOnBattlePassUnlocked(BattlePass battlePass)
        ///     {
        ///         // Do something - for example show an additional particle effect animation, a nice popup or unlock an additional
        ///         // item.
        ///     }
        /// }
        /// </code>
        /// </example>
        public static event BattlePassUnlockedAction OnBattlePassUnlocked;

        /// <summary>
        ///     Battle passes need to be unlocked, either by script or user interaction. Often Battle passes need to be purchased
        ///     by users. Create the UI (purchase button, description, etc.) for unlocking the battle pass within one container
        ///     GameObject and connect it on this property. This script will hide the GameObject if the battle pass has already
        ///     been unlocked and will show it, if the battle pass needs to be unlocked for the user.
        /// </summary>
        [Header("Required connections")]
        [Tooltip(
            "Connect a game object in your hierarchy that shows a UI with a button to unlock the Battle Pass. It will be hidden if the battle pass is already unlocked")]
        public GameObject unlockGroup;

        /// <summary>
        ///     A <c>UnityEngine.UI.Text</c> field that will be set with the current active level. Just a number like 2 or 99.
        /// </summary>
        [Tooltip("A text field that will be set with the current active level. Just a number like 2 or 99")]
        public Text currentLevel;

        /// <summary>
        ///     A <c>UnityEngine.UI.Text</c> field which is used to render the battle pass name. The <c>battle_pass_name</c> of
        ///     <see cref="BattlePass" />
        ///     will be used.
        /// </summary>
        [Tooltip("A text UI element that is used to render the name of the battle pass")]
        public Text battlePassNameText;


        /// <summary>
        ///     You can set an image name in the Admin Panel for the battle pass. The script will try to load a sprite at runtime
        ///     with the <c>image_xs</c>  name of <see cref="BattlePass" />. Please make sure that the sprite is in a
        ///     <c>Resources</c> folder within the Unity
        ///     Assets folder!
        /// </summary>
        [Header("Optional connections")]
        [Tooltip(
            "An image that will be set with the image set for the battle pass. It will be loaded as a Sprite with the name you set in Admin Panel. Make sure this Sprite is in a Resources folder - otherwise it will not be loaded at runtime")]
        public Image imageXS;

        /// <summary>
        ///     You can set an image name in the Admin Panel for the battle pass. The script will try to load a sprite at runtime
        ///     with the <c>image_s</c>  name of <see cref="BattlePass" />. Please make sure that the sprite is in a
        ///     <c>Resources</c> folder within the Unity
        ///     Assets folder!
        /// </summary>
        [Tooltip(
            "An image that will be set with the image set for the battle pass. It will be loaded as a Sprite with the name you set in Admin Panel. Make sure this Sprite is in a Resources folder - otherwise it will not be loaded at runtime")]
        public Image imageS;

        /// <summary>
        ///     You can set an image name in the Admin Panel for the battle pass. The script will try to load a sprite at runtime
        ///     with the <c>image_m</c>  name of <see cref="BattlePass" />. Please make sure that the sprite is in a
        ///     <c>Resources</c> folder within the Unity
        ///     Assets folder!
        /// </summary>
        [Tooltip(
            "An image that will be set with the image set for the battle pass. It will be loaded as a Sprite with the name you set in Admin Panel. Make sure this Sprite is in a Resources folder - otherwise it will not be loaded at runtime")]
        public Image imageM;

        /// <summary>
        ///     You can set an image name in the Admin Panel for the battle pass. The script will try to load a sprite at runtime
        ///     with the <c>image_l</c>  name of <see cref="BattlePass" />. Please make sure that the sprite is in a
        ///     <c>Resources</c> folder within the Unity
        ///     Assets folder!
        /// </summary>
        [Tooltip(
            "An image that will be set with the image set for the battle pass. It will be loaded as a Sprite with the name you set in Admin Panel. Make sure this Sprite is in a Resources folder - otherwise it will not be loaded at runtime")]
        public Image imageL;

        /// <summary>
        ///     You can set an image name in the Admin Panel for the battle pass. The script will try to load a sprite at runtime
        ///     with the <c>image_xl</c>  name of <see cref="BattlePass" />. Please make sure that the sprite is in a
        ///     <c>Resources</c> folder within the Unity
        ///     Assets folder!
        /// </summary>
        [Tooltip(
            "An image that will be set with the image set for the battle pass. It will be loaded as a Sprite with the name you set in Admin Panel. Make sure this Sprite is in a Resources folder - otherwise it will not be loaded at runtime")]
        public Image imageXL;

        /// <summary>
        ///     A <c>UnityEngine.UI.Text</c> field which is used to render the battle pass start date. The <c>start_date</c> of
        ///     <see cref="BattlePass" />
        ///     will be used.
        /// </summary>
        [Tooltip("Start date for the battle pass")]
        public Text startDate;

        /// <summary>
        ///     A <c>UnityEngine.UI.Text</c> field which is used to render the battle pass end date. The <c>end_date</c> of
        ///     <see cref="BattlePass" />
        ///     will be used.
        /// </summary>
        [Tooltip("End date for this battle pass")]
        public Text endDate;

        private Dictionary<int, GameObject> _levelObjects = new Dictionary<int, GameObject>();

        private List<BattlePassLevel> _levels;
        private SCILLBattlePassLevel _selectedBattlePassLevel;
        [HideInInspector] public BattlePass battlePass;

        // Start is called before the first frame update
        private void Start()
        {
            UpdateUI();
        }

        private void OnEnable()
        {
            if (SCILLBattlePassManager.Instance)
            {
                battlePass = SCILLBattlePassManager.Instance.SelectedBattlePass;
                _levels = SCILLBattlePassManager.Instance.BattlePassLevels;
            }

            SCILLBattlePassManager.OnBattlePassUpdatedFromServer += OnBattlePassUpdatedFromServer;
            SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer += OnBattlePassLevelsUpdatedFromServer;

            UpdateUI();
        }

        private void OnDestroy()
        {
            SCILLBattlePassManager.OnBattlePassUpdatedFromServer -= OnBattlePassUpdatedFromServer;
            SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer -= OnBattlePassLevelsUpdatedFromServer;
        }

        

        private void OnBattlePassUpdatedFromServer(BattlePass battlePass)
        {
            this.battlePass = battlePass;
            UpdateUI();
        }

        private void OnBattlePassLevelsUpdatedFromServer(List<BattlePassLevel> battlePassLevels)
        {
            _levels = battlePassLevels;

            if (currentLevel)
            {
                var currentLevelIndex = 0;
                for (var i = 0; i < _levels.Count; i++)
                    if (_levels[i].level_completed == true)
                        currentLevelIndex = i;
                    else
                        break;

                currentLevel.text = (currentLevelIndex + 1).ToString();
            }
        }

        private void UpdateUI()
        {
            if (battlePass == null) return;

            if (battlePassNameText) battlePassNameText.text = battlePass.battle_pass_name;

            if (battlePass.image_xs != null && imageXS)
            {
                var sprite = Resources.Load<Sprite>(battlePass.image_xs);
                imageXS.sprite = sprite;
            }

            if (battlePass.image_s != null && imageS)
            {
                var sprite = Resources.Load<Sprite>(battlePass.image_s);
                imageS.sprite = sprite;
            }

            if (battlePass.image_m != null && imageM)
            {
                var sprite = Resources.Load<Sprite>(battlePass.image_m);
                imageM.sprite = sprite;
            }

            if (battlePass.image_l != null && imageL)
            {
                var sprite = Resources.Load<Sprite>(battlePass.image_l);
                imageL.sprite = sprite;
            }

            if (battlePass.image_xl != null && imageXL)
            {
                var sprite = Resources.Load<Sprite>(battlePass.image_xl);
                imageXL.sprite = sprite;
            }

            if (unlockGroup)
            {
                if (battlePass.unlocked_at != null)
                    // This battle pass is unlocked
                    unlockGroup.SetActive(false);
                else
                    unlockGroup.SetActive(true);
            }

            if (startDate)
            {
                var date = DateTime.Parse(battlePass.start_date);
                startDate.text = date.ToShortDateString();
            }

            if (endDate)
            {
                var date = DateTime.Parse(battlePass.end_date);
                endDate.text = date.ToShortDateString();
            }
        }

        /// <summary>
        /// Connect a buttons click event to this function. It will unlock the battle pass. However, if you want to add In-App purchase you will need to implement yourself and then use the <see cref="SCILLClient.UnlockBattlePassAsync(System.Action{SCILL.Model.BattlePassUnlockInfo},System.Action{System.Exception},string,SCILL.Model.BattlePassUnlockPayload)"/> function of the SCILL SDK.
        /// </summary>
        /// <example>
        /// Unlocking a battle pass
        /// <code>
        /// public virtual void OnBattlePassUnlockButtonPressed()
        /// {
        ///     var purchaseInfo = new BattlePassUnlockPayload(0, "EUR");
        ///     SCILLManager.Instance.SCILLClient.UnlockBattlePassAsync(
        ///         unlockInfo =>
        ///         {
        ///             if (unlockInfo != null)
        ///             {
        ///                 battlePass.unlocked_at = unlockInfo.purchased_at;
        ///                 OnBattlePassUnlocked?.Invoke(battlePass);
        ///                 UpdateUI();
        ///             }
        ///         }, null,
        ///         battlePass.battle_pass_id, purchaseInfo);
        /// }
        /// </code>
        /// </example>
        public virtual void OnBattlePassUnlockButtonPressed()
        {
            var purchaseInfo = new BattlePassUnlockPayload(0, "EUR");
            SCILLManager.Instance.SCILLClient.UnlockBattlePassAsync(
                unlockInfo =>
                {
                    if (unlockInfo != null)
                    {
                        battlePass.unlocked_at = unlockInfo.purchased_at;
                        OnBattlePassUnlocked?.Invoke(battlePass);
                        UpdateUI();
                    }
                }, null,
                battlePass.battle_pass_id, purchaseInfo);
        }
    }
}