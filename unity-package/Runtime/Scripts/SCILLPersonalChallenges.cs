using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;

namespace SCILL
{
    /// <summary>
    ///     <para>
    ///         This component handles communication with the SCILL backend to load and update personal challenges in real
    ///         time. It also implements user interfaces to display personal challenges.
    ///     </para>
    ///     <para>
    ///         This class does three things:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             Load personal challenges with the
    ///             <see
    ///                 cref="SCILLClient.GetAllPersonalChallengesAsync(System.Action{System.Collections.Generic.List{SCILL.Model.ChallengeCategory}},System.Action{System.Exception},System.Collections.Generic.List{string},System.Collections.Generic.List{string})" />
    ///             method.
    ///         </item>
    ///         <item>
    ///             Instantiate the prefab set in <see cref="categoryPrefab" /> property for each
    ///             <see cref="ChallengeCategory" /> object contained in the
    ///             <a href="https://developers.scillgame.com/api/challenges.html#request-challenges">response</a> and add as
    ///             child to the transform.
    ///         </item>
    ///         <item>
    ///             Uses the <see cref="SCILLPersonalChallengesManager" /> to listen to server-side changes and update the
    ///             challenges state in the UI.
    ///         </item>
    ///     </list>
    ///     <para>
    ///         The <see cref="categoryPrefab" /> must have a <see cref="SCILLCategoryItem" /> attached that will create
    ///         instances of the <see cref="challengePrefab" /> and
    ///         add them as childs to category game object instance or if provided to the <see cref="SCILLCategoryItem" />
    ///         transform provided
    ///         in the challenge category prefab.
    ///     </para>
    ///     <para>
    ///         In the end, you’ll have this hierarchy:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             Container
    ///             <list type="bullet">
    ///                 <item>
    ///                     Challenge Category 1
    ///                     <list type="bullet">
    ///                         <item>
    ///                             Header
    ///                         </item>
    ///                         <item>
    ///                             Challenge 1
    ///                         </item>
    ///                         <item>
    ///                             Challenge 2
    ///                         </item>
    ///                         <item>
    ///                             ...
    ///                         </item>
    ///                     </list>
    ///                 </item>
    ///             </list>
    ///             <list type="bullet">
    ///                 <item>
    ///                     Challenge Category 2
    ///                     <list type="bullet">
    ///                         <item>
    ///                             Header
    ///                         </item>
    ///                         <item>
    ///                             Challenge 1
    ///                         </item>
    ///                         <item>
    ///                             Challenge 2
    ///                         </item>
    ///                         <item>
    ///                             ...
    ///                         </item>
    ///                     </list>
    ///                 </item>
    ///             </list>
    ///             <list type="bullet">
    ///                 <item>
    ///                     ...
    ///                 </item>
    ///             </list>
    ///         </item>
    ///     </list>
    /// </summary>
    public class SCILLPersonalChallenges : MonoBehaviour
    {
        /// <summary>
        ///     Set this to a prefab that has a <see cref="SCILLCategoryItem" /> component attached. It will be instantiated for
        ///     each challenge
        ///     category. If you don’t want to have UI for categories you can just use an empty Game Object as a prefab and attach
        ///     the <see cref="SCILLCategoryItem" /> component to it.
        /// </summary>
        [Tooltip("A prefab to be used as a category item. This will be instantiated for each category in the response")]
        public SCILLCategoryItem categoryPrefab;

        /// <summary>
        ///     A prefab with the <see cref="SCILLChallengeItem" /> attached. It will be instantiated for each challenge in the
        ///     category and will be added as a child of the category prefab.
        /// </summary>
        [Tooltip(
            "A prefab that will be used for each challenge. It will be instantiated and added as child to the category game object")]
        public SCILLChallengeItem challengePrefab;

        private readonly Dictionary<string, SCILLCategoryItem> _categoryObjects =
            new Dictionary<string, SCILLCategoryItem>();

        private List<ChallengeCategory> _categories;

        private void Awake()
        {
            // Remove any dummies
            foreach (var child in GetComponentsInChildren<SCILLCategoryItem>()) Destroy(child.gameObject);
            
            SCILLPersonalChallengesManager.OnPersonalChallengesUpdatedFromServer += OnPersonalChallengesUpdated;
            SCILLPersonalChallengesManager.OnPersonalChallengeUpdatedFromServer += OnPersonalChallengeUpdated;
        }

        // Start is called before the first frame update
        private void Start()
        {
            if (SCILLPersonalChallengesManager.Instance.Categories != null)
            {
                UpdateCategories(SCILLPersonalChallengesManager.Instance.Categories);
            }
        }

        private void OnDestroy()
        {
            SCILLPersonalChallengesManager.OnPersonalChallengesUpdatedFromServer -= OnPersonalChallengesUpdated;
            SCILLPersonalChallengesManager.OnPersonalChallengeUpdatedFromServer -= OnPersonalChallengeUpdated;
        }

        private void OnPersonalChallengeUpdated(Challenge challenge,
            SCILLPersonalChallengeModificationType modificationtype)
        {
            Debug.Log("OnPersonalChallengeUpdated");
            UpdateChallenge(challenge);
            
        }

        private void OnPersonalChallengesUpdated(List<ChallengeCategory> categories)
        {
            Debug.Log("OnPersonalChallengesUpdated");
            UpdateCategories(categories);
            _categories = categories;
        }


        /// <summary>
        ///     This function loads the challenges with the
        ///     <see
        ///         cref="SCILLClient.GetAllPersonalChallengesAsync(System.Action{System.Collections.Generic.List{SCILL.Model.ChallengeCategory}},System.Action{System.Exception},System.Collections.Generic.List{string},System.Collections.Generic.List{string})" />
        ///     method and rebuilds the UI. It updates all child elements for categories and challenges.
        /// </summary>
        public void UpdatePersonalChallengesList()
        {
            var categoriesPromise = SCILLManager.Instance.SCILLClient.GetAllPersonalChallengesAsync();
            categoriesPromise.Then(categories =>
            {
                OnPersonalChallengesUpdated(categories);
            }).Catch(exception => Debug.LogError("UpdatePersonalChallengesList: " + exception.Message));
        }

        private void UpdateCategories(List<ChallengeCategory> updatedCategories)
        {
            foreach (var categoryData in updatedCategories)
            {
                string categoryID = categoryData.category_id;
                SCILLCategoryItem categoryItem = null;
                if (_categoryObjects.ContainsKey(categoryID))
                {
                    categoryItem = _categoryObjects[categoryID];
                }
                else
                {
                    categoryItem = Instantiate(categoryPrefab.gameObject, transform, false)
                        .GetComponent<SCILLCategoryItem>();
                    if (categoryItem) _categoryObjects.Add(categoryID, categoryItem);
                }
                categoryItem.UpdateCategory(categoryData);
            }
        }
 
        private void UpdateChallenge(Challenge toUpdate)
        {
            foreach (SCILLCategoryItem categoryItem in _categoryObjects.Values)
            {
                if (categoryItem.ContainsChallenge(toUpdate.challenge_id))
                {
                    categoryItem.UpdateChallenge(toUpdate);
                    break;
                }
            }
        }


        /// <summary>
        ///     Unlocks the personal challenge provided and will update UI elements if required. The default prefabs connect
        ///     buttons to these functions to trigger unlocking challenges by the user. You can unlock challenges for the user
        ///     automatically.
        /// </summary>
        /// <param name="challenge">The challenge you want to unlock. </param>
        public void UnlockPersonalChallenge(Challenge challenge)
        {
            var responsePromise =
                SCILLManager.Instance.SCILLClient.UnlockPersonalChallengeAsync(challenge.challenge_id);
            responsePromise.Then(response =>
            {
                if (response.status >= 200 && response.status < 300)
                    if (response.challenge != null)
                        UpdateChallenge(response.challenge);
            });
        }

        /// <summary>
        ///     Activates the personal challenge provided and will update UI elements if required. The default prefabs connect
        ///     buttons to these functions to trigger activating challenges by the user. You can also do that for the user
        ///     automatically.
        /// </summary>
        /// <param name="challenge">The challenge you want to activate.</param>
        public void ActivatePersonalChallenge(Challenge challenge)
        {
            var responsePromise =
                SCILLManager.Instance.SCILLClient.ActivatePersonalChallengeAsync(challenge.challenge_id);
            responsePromise.Then(response =>
            {
                if (response.status >= 200 && response.status < 300)
                    if (response.challenge != null)
                        UpdateChallenge(response.challenge);
            });
        }

        /// <summary>
        ///     <para>
        ///         Claims the reward for the personal challenge provided and will update UI elements if required. The default
        ///         prefabs connect buttons to these functions to trigger claiming the reward by the user. You can also do this for
        ///         the user automatically.
        ///     </para>
        ///     <para>
        ///         Calling this function will mark the challenge as claimed and if the challenge is <c>repeatable</c> it will be
        ///         available again for the next round. SCILL Backend will send a realtime notification to inform the client. You
        ///         can also set a Webhook in the Admin Panel to get a notification in the backend to unlock the reward there (for
        ///         example if you need to send an email).
        ///     </para>
        ///     <para>
        ///         This function will call the OnPersonalChallengeRewardClaimed function that you can override to unlock the
        ///         reward.
        ///     </para>
        /// </summary>
        /// <param name="challenge">The challenge you want to claim</param>
        public void ClaimPersonalChallengeReward(Challenge challenge)
        {
            var responsePromise =
                SCILLManager.Instance.SCILLClient.ClaimPersonalChallengeRewardAsync(challenge.challenge_id);
            responsePromise.Then(response =>
            {
                if (response.status >= 200 && response.status < 300)
                    if (response.challenge != null)
                    {
                        UpdateChallenge(response.challenge);
                        OnPersonalChallengeRewardClaimed(response.challenge);
                    }
            });
        }

        /// <summary>
        ///     <para>
        ///         Cancels the personal challenge provided and will update UI elements if required. The default prefabs connect
        ///         buttons to these functions to trigger canceling the challenge by the user.
        ///     </para>
        ///     <para>
        ///         Calling this function will mark the challenge as <c>cancelled</c> (type cancelled) and if the challenge is
        ///         <c>repeatable</c>
        ///         it will be available again for the next round. Otherwise the challenge will not be reported again. SCILL
        ///         Backend will send a realtime notification to inform the client. You can also set a Webhook in the Admin Panel
        ///         to get a notification in the backend to unlock the reward there (for example if you need to send an email).
        ///     </para>
        /// </summary>
        /// <param name="challenge">The challenge you want to cancel</param>
        public void CancelPersonalChallenge(Challenge challenge)
        {
            var responsePromise =
                SCILLManager.Instance.SCILLClient.CancelPersonalChallengeAsync(challenge.challenge_id);
            responsePromise.Then(response =>
            {
                if (response.status >= 200 && response.status < 300)
                    if (response.challenge != null)
                        // In this case we need to reload the list from the server as we don't know if this challenge will
                        // be available as it's set to repeatable or not. 
                        UpdatePersonalChallengesList();
            });
        }

        /// <summary>
        ///     Called whenever the personal challenges has been marked as claimed in SCILL. Override this function to implement
        ///     your own business logic to unlock the reward set in the challenge provided.
        /// </summary>
        /// <param name="challenge">The challenge for which the reward was claimed.</param>
        protected virtual void OnPersonalChallengeRewardClaimed(Challenge challenge)
        {
        }
    }
}