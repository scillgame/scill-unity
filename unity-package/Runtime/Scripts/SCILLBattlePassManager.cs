using System.Collections;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;

namespace SCILL
{
    /// <summary>
    ///     <para>
    ///         This class is designed as a “Singleton” and should be attached to the same GameObject that you have
    ///         <see cref="SCILLManager" />
    ///         attached. It will completely manage battle passes. It selects the first available active battle pass and
    ///         selects
    ///         that battle pass.
    ///     </para>
    ///     <para>
    ///         Add this component to the same <c>GameObject</c>  that has the <c>SCILLManager</c>  attached. If you want to
    ///         customize Battle Pass
    ///         selection, derive a new class from this class and override <see cref="SelectBattlePass" /> function which
    ///         receives the array of
    ///         available battle passes from the backend to return the selected battle pass.
    ///     </para>
    ///     <para>
    ///         This class handles all data required for displaying and acting on claimed battle pass level rewards. Use the
    ///         various delegates to connect your own classes to the manager. In this documentation we provide some example
    ///         code.
    ///         Our code and all prefabs connect to the manager with these delegates. There is no need to connect prefabs or
    ///         other
    ///         things together in Unity - just drop in the prefabs, and they will work automatically as they just listen on
    ///         the
    ///         delegates and update their UI whenever data changes and respective delegates are called.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <c>SCILLBattlePassManager</c>  implements various delegates that get called whenever data changes that the
    ///         manager
    ///         handles for you. The basic idea is, that you can just write some scripts, listen to the static delegates
    ///         provided by the manager to implement your business logic or UI. You don’t need to connect all the various
    ///         pieces together in Unity, instead you just need to make sure to have <c>SCILLBattlePassManager</c>  object in
    ///         the
    ///         scene. The rest will be handled by the delegates.
    ///     </para>
    /// </remarks>
    public class SCILLBattlePassManager : MonoBehaviour
    {
        public delegate void BattlePassChallengeUpdateAction(BattlePassChallengeChangedPayload challengeChangedPayload);

        public delegate void BattlePassLevelRewardClaimedAction(BattlePassLevel level);

        public delegate void BattlePassLevelsUpdatedFromServerAction(List<BattlePassLevel> battlePassLevels);

        public delegate void BattlePassUpdatedFromServerAction(BattlePass battlePass);

        public delegate void SelectedBattlePassLevelChangedAction(BattlePassLevel selectedBattlePassLevel);

        /// <summary>
        ///     Add a listener to this delegate which is called whenever the Battle Pass Manager selection changes. If users can
        ///     switch between multiple battle passes, the delegate will be called serving the new selected battle pass.
        ///     <see cref="SCILLBattlePass" /> adds a listener to automatically update UI once the manager selection changes.
        /// </summary>
        /// <example>
        ///     Using OnBattlePassLevelsUpdatedFromServer
        ///     <code>
        ///  public class ExampleScript : MonoBehaviour{
        ///     private BattlePass battlePass;
        /// 
        ///     private void OnEnable()
        ///     {
        ///         // Make sure we set an already available battle pass (i.e. if this component got activated later)
        ///         if (SCILLBattlePassManager.Instance)
        ///         {
        ///             battlePass = SCILLBattlePassManager.Instance.SelectedBattlePass;
        ///         }
        ///     
        ///         // Make sure we update we update our local instance of the (selected) battle pass changes on server side
        ///         SCILLBattlePassManager.OnBattlePassUpdatedFromServer += OnBattlePassUpdatedFromServer;
        ///     }
        /// 
        ///     private void OnDestroy()
        ///     {
        ///         // Remove listener once this component is destroyed
        ///         SCILLBattlePassManager.OnBattlePassUpdatedFromServer -= OnBattlePassUpdatedFromServer;
        ///     }
        /// 
        ///     private void OnBattlePassUpdatedFromServer(BattlePass battlePass)
        ///     {
        ///         // Update local instance and update UI
        ///         this.battlePass = battlePass;
        ///         UpdateUI();
        ///     }
        /// 
        ///     private void UpdateUI()
        ///     {
        ///         // Update user interface
        ///     }
        /// }
        ///  </code>
        /// </example>
        public static event BattlePassUpdatedFromServerAction OnBattlePassUpdatedFromServer;


        /// <summary>
        ///     Add a listener to this delegate if you want to be informed whenever the battle pass levels change. Whenever a level
        ///     is unlocked or claimed, this function will be called. <see cref="SCILLBattlePassLevels" /> listens on those changes
        ///     to update UI
        ///     of the levels.
        /// </summary>
        /// <example>
        ///     Using OnBattlePassLevelsUpdatedFromServer
        ///     <code>
        /// public class ExampleScript : MonoBehaviour{
        ///     private List&lt;BattlePassLevel&gt; _levels;
        ///     private void OnEnable()
        ///     {
        ///         // Set levels if levels have already been loaded
        ///         if (SCILLBattlePassManager.Instance)
        ///         {
        ///             _levels = SCILLBattlePassManager.Instance.BattlePassLevels;
        ///         }
        ///         // Make sure we update the levels once they change on server side
        ///         SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer += OnBattlePassLevelsUpdatedFromServer;
        ///         UpdateBattlePassLevelUI();
        ///     }
        ///     private void OnDestroy()
        ///     {
        ///         SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer -= OnBattlePassLevelsUpdatedFromServer;
        ///     }
        ///     private void OnBattlePassLevelsUpdatedFromServer(List&lt;BattlePassLevel&gt; battlePassLevels)
        ///     {
        ///         this._levels = battlePassLevels;
        ///         UpdateBattlePassLevelUI();
        ///     }
        ///     private UpdateBattlePassLevelUI()
        ///     {
        ///         // Do whatever you have to do to update the UI
        ///     }
        /// }
        ///   </code>
        /// </example>
        public static event BattlePassLevelsUpdatedFromServerAction OnBattlePassLevelsUpdatedFromServer;

        /// <summary>
        ///     Whenever a challenge in the battle pass changes, i.e. the type changes indicating that a challenge got activated
        ///     due to the level being unlocked or if the progress changes, this delegate will be called.
        /// </summary>
        /// <example>
        ///     Using OnBattlePassChallengeUpdate
        ///     <code>
        /// public class ExampleScript : MonoBehaviour{
        ///     public BattlePassLevelChallenge challenge;
        ///     
        ///     public Slider challengeProgressSlider;
        ///     
        ///     // Start is called before the first frame update
        ///     void Start()
        ///     {
        ///         UpdateUI();
        ///     }
        ///     
        ///     private void OnEnable()
        ///     {
        ///         SCILLBattlePassManager.OnBattlePassChallengeUpdate += OnBattlePassChallengeUpdate;
        ///     }
        ///     
        ///     private void OnDestroy()
        ///     {
        ///         SCILLBattlePassManager.OnBattlePassChallengeUpdate -= OnBattlePassChallengeUpdate;
        ///     }
        /// 
        ///     private void OnBattlePassChallengeUpdate(BattlePassChallengeChangedPayload challengeChangedPayload)
        ///     {
        ///         // Update local challenge object if it has the same id
        ///         if (challengeChangedPayload.new_battle_pass_challenge.challenge_id == challenge.challenge_id)
        ///         {
        ///             challenge.type = challengeChangedPayload.new_battle_pass_challenge.type;
        ///             challenge.user_challenge_current_score =
        ///                 challengeChangedPayload.new_battle_pass_challenge.user_challenge_current_score;
        ///             UpdateUI();   
        ///         }
        ///     }
        /// 
        ///     public void UpdateUI()
        ///     {
        ///         // Update local challenge UI
        ///         if (challengeProgressSlider)
        ///         {
        ///             if (challenge.challenge_goal > 0 &amp;&amp; challenge.user_challenge_current_score > 0)
        ///             {
        ///                 if (challenge.challenge_goal_condition == 0)
        ///                 {
        ///                     // If the challenge_goal_condition is 0 then counter must be greater than the goal, so progress is the
        ///                     // relation between the counter and the goal
        ///                     
        ///                     challengeProgressSlider.value = (float) ((float) challenge.user_challenge_current_score / (float) challenge.challenge_goal);
        ///                 }
        ///                 else if (challenge.challenge_goal_condition == 1)
        ///                 {
        ///                     // If the challenge_goal_condition is 1 then counter must be smaller than the goal, so progress is the
        ///                     // inverted relation between the counter and the goal
        ///                     
        ///                     challengeProgressSlider.value = (float) 1.0f / ((float) challenge.user_challenge_current_score /
        ///                                                              (float) challenge.challenge_goal);
        ///                 }
        ///             }
        ///             else
        ///             {
        ///                 challengeProgressSlider.value = 0;
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        public static event BattlePassChallengeUpdateAction OnBattlePassChallengeUpdate;

        /// <summary>
        ///     This delegate is called whenever a levels reward is claimed. If you want to unlock rewards within your client you
        ///     just need to listen to this delegate (see example below) to unlock that reward. Use
        ///     <a href="https://developers.scillgame.com/api/battlepasses.html#webhooks">Webhooks</a> if you want to unlock
        ///     rewards in your backend.
        /// </summary>
        /// <example>
        ///     Using OnBattlePassLevelRewardClaimed
        ///     <code>
        /// public class ExampleScript : MonoBehaviour
        /// {
        ///     private void OnEnable()
        ///     {
        ///         SCILLBattlePassManager.OnBattlePassLevelRewardClaimed += OnBattlePassLevelRewardClaimed;
        ///     }
        /// 
        ///     private void OnDisable()
        ///     {
        ///         SCILLBattlePassManager.OnBattlePassLevelRewardClaimed -= OnBattlePassLevelRewardClaimed;
        ///     }
        /// 
        ///     private void OnBattlePassLevelRewardClaimed(BattlePassLevel level)
        ///     {
        ///         // Use level.level_reward_type and level.reward_amount to find our what to unlock for the user
        ///     }
        /// }
        /// </code>
        /// </example>
        public static event BattlePassLevelRewardClaimedAction OnBattlePassLevelRewardClaimed;

        /// <summary>
        ///     This delegate is called when the currently selected Battle Pass Level is changed.
        /// </summary>
        public static event SelectedBattlePassLevelChangedAction OnSelectedBattlePassLevelChanged;


        private List<BattlePass> _battlePasses;
        private int _selectedBattlePassLevelIndex;


        /// <summary>
        ///     A reference to the levels loaded for the selected battle pass.
        /// </summary>
        public List<BattlePassLevel> BattlePassLevels;

        /// <summary>
        ///     A reference to the selected battle pass (see <see cref="SelectBattlePass" />) that you can access with this
        ///     property.
        /// </summary>
        public BattlePass SelectedBattlePass;

        /// <summary>
        ///     As this class is designed as a singleton you can use this getter to get a reference to the instance. It allows you
        ///     to access the <c>SCILLBattlePassManager</c>  from everywhere in your code.
        /// </summary>
        public static SCILLBattlePassManager Instance { get; private set; }

        public BattlePassLevel SelectedBattlePassLevel => BattlePassLevels?[_selectedBattlePassLevelIndex];

        public int SelectedBattlePassLevelIndex
        {
            get => _selectedBattlePassLevelIndex;
            set
            {
                _selectedBattlePassLevelIndex = value;
                OnSelectedBattlePassLevelChanged?.Invoke(SelectedBattlePassLevel);
            }
        }


        private int NumUpdateRequests { get; set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        private IEnumerator Start()
        {
            while (null == SCILLManager.Instance.SCILLClient)
                yield return null;

            var battlePassesAsync = SCILLManager.Instance.SCILLClient.GetBattlePassesAsync();
            battlePassesAsync.Then(battlePassesList =>
            {
                _battlePasses = battlePassesList;
                // Select a battle pass
                var selectedBattlePass = SelectBattlePass(_battlePasses);

                if (selectedBattlePass != null)
                {
                    SelectedBattlePass = selectedBattlePass;

                    // Inform delegates that a new battle pass has been selected
                    OnBattlePassUpdatedFromServer?.Invoke(selectedBattlePass);


                    // Load battle pass levels from SCILL backend
                    UpdateBattlePassLevelsFromServer();
                }

                StartRealtimeNotifications();

                SCILLBattlePass.OnBattlePassUnlocked += OnOnBattlePassUnlocked;
            });
        }

        private void OnDestroy()
        {
            StopRealtimeNotifications();
        }


        /// <summary>
        ///     This function receives a List of <see cref="BattlePass" /> objects and returns one <see cref="BattlePass" /> object
        ///     that should be used within the game. You can override this function to adjust behavior of this class.
        /// </summary>
        /// <remarks>
        ///     The default behavior returns the first unlocked battle pass, or if no unlocked battle passes were found, the first
        ///     battle pass.
        /// </remarks>
        /// <param name="battlePasses">
        ///     A list of available batte passes loaded from the SCILL Backend with the
        ///     <see
        ///         cref="SCILLClient.GetBattlePassesAsync(System.Action{System.Collections.Generic.List{SCILL.Model.BattlePass}},System.Action{System.Exception})" />
        ///     function.
        /// </param>
        /// <returns>The selected battle pass.</returns>
        protected virtual BattlePass SelectBattlePass(List<BattlePass> battlePasses)
        {
            BattlePass selectedBattlePass = null;
            for (var i = 0; i < battlePasses.Count; i++)
            {
                var battlePass = battlePasses[i];
                if (battlePass.unlocked_at != null)
                {
                    selectedBattlePass = battlePass;
                    break;
                }
            }

            if (selectedBattlePass == null)
                if (battlePasses.Count > 0)
                    selectedBattlePass = battlePasses[0];

            return selectedBattlePass;
        }

        private void OnOnBattlePassUnlocked(BattlePass battlePass)
        {
            // If the battle pass was unlocked, then reload the levels and challenges
            if (battlePass.battle_pass_id == SelectedBattlePass.battle_pass_id) UpdateBattlePassLevelsFromServer();
        }

        /// <summary>
        ///     Use this function to reload the battle pass levels from the SCILL server and to trigger the
        ///     <see cref="OnBattlePassLevelsUpdatedFromServer" /> event.
        /// </summary>
        public void UpdateBattlePassLevelsFromServer()
        {
            SendUpdateRequest();
        }

        private void SendUpdateRequest()
        {

            var levelsPromise =
                SCILLManager.Instance.SCILLClient.GetBattlePassLevelsAsync(SelectedBattlePass.battle_pass_id);
            // Debug.Log("Requested BP Update from Server");

            levelsPromise.Then(levels =>
            {
                BattlePassLevels = levels;

                // var numBattlePassLevels = null == BattlePassLevels ? 0 : BattlePassLevels.Count;
                // Debug.Log($"Received BP Update from server with {numBattlePassLevels} entries");

                // If we have not selected a battle pass level, let's pick the current one
                if (_selectedBattlePassLevelIndex == 0) SelectedBattlePassLevelIndex = GetCurrentBattlePassLevelIndex();
                
                

                OnBattlePassLevelsUpdatedFromServer?.Invoke(levels);

            }).Catch(exception =>
            {
                Debug.LogError(exception.Message);
            });
        }

        /// <summary>
        ///     Determines the index of the first uncompleted level from the selected battle pass, which is
        ///     the index of the level the user is currently on.
        /// </summary>
        /// <returns>The index of the first uncompleted level from the selected battle pass.</returns>
        public virtual int GetCurrentBattlePassLevelIndex()
        {
            var selectedLevelIndex = 0;
            foreach (var level in BattlePassLevels)
                if (level.level_completed == true)
                    selectedLevelIndex++;
                else
                    break;

            return Mathf.Min(selectedLevelIndex, BattlePassLevels.Count - 1);
        }

        /// <summary>
        ///     Determines the first uncompleted level from the selected battle pass, which is
        ///     the level the user is currently on.
        /// </summary>
        /// <returns>The users current battle pass level.</returns>
        public virtual BattlePassLevel GetCurrentBattlePassLevel()
        {
            var currentLevelIndex = GetCurrentBattlePassLevelIndex();
            if (null != BattlePassLevels && currentLevelIndex > -1 && currentLevelIndex < BattlePassLevels.Count)
                return BattlePassLevels[currentLevelIndex];

            return null;
        }

        private void OnBattlePassChangedNotification(BattlePassChallengeChangedPayload payload)
        {
            // The battle pass challenge changed
            if (payload.webhook_type == "battlepass-challenge-changed")
            {
                // Check if the challenge is still in-progress. If not, we need to reload the levels to update
                // current state - as change is not isolated to one challenge
                if (payload.new_battle_pass_challenge.type == "in-progress")
                    // Inform all delegates of the challenge update
                    OnBattlePassChallengeUpdate?.Invoke(payload);
                else
                    // Reload the levels from the server and update UI
                    StartCoroutine(SendMultipleLevelUpdateRequests());
            }
            else
            {
                // Reload the levels from the server and update UI. Send multiple requests to ensure that newest Level was activated.
                StartCoroutine(SendMultipleLevelUpdateRequests());
            }
        }
        private IEnumerator SendMultipleLevelUpdateRequests(int numRequests = 2, float duration = 0.2f)
        {
            SendUpdateRequest();
            int numSentRequests = 1;
            while (numSentRequests < numRequests)
            {
                yield return new WaitForSeconds(duration);
                SendUpdateRequest();
                numSentRequests++;
            }
        }

        /// <summary>
        ///     Requests the <c>SCILLBattlePassManager</c> to start listening to the currently selected battle passes update
        ///     notifications.
        /// </summary>
        public void StartRealtimeNotifications()
        {
            if (null != SelectedBattlePass)
                // Get notifications from SCILL backend whenever battle pass changes
                SCILLManager.Instance.StartBattlePassUpdateNotifications(SelectedBattlePass.battle_pass_id,
                    OnBattlePassChangedNotification);
        }

        /// <summary>
        ///     Requests the <c>SCILLBattlePassManager</c> to stop listening to the currently selected battle passes update
        ///     notifications.
        /// </summary>
        public void StopRealtimeNotifications()
        {
            if (SelectedBattlePass != null)
                SCILLManager.Instance.StopBattlePassUpdateNotifications(SelectedBattlePass.battle_pass_id,
                    OnBattlePassChangedNotification);
        }

        /// <summary>
        ///     Call this function to claim the reward attached to the battle pass level. It will call the
        ///     <see
        ///         cref="SCILLClient.ClaimBattlePassLevelRewardAsync(System.Action{SCILL.Model.ActionResponse},System.Action{System.Exception},string)" />
        ///     function and will trigger the <see cref="OnBattlePassLevelRewardClaimed" /> event.
        /// </summary>
        /// <param name="level">The Level for which the reward should be claimed.</param>
        public void ClaimBattlePassLevelReward(BattlePassLevel level)
        {
            var responsePromise = SCILLManager.Instance.SCILLClient.ClaimBattlePassLevelRewardAsync(level.level_id);

            responsePromise.Then(response =>
            {
                if (response != null && response.message == "OK") OnBattlePassLevelRewardClaimed?.Invoke(level);
            });
        }
    }
}