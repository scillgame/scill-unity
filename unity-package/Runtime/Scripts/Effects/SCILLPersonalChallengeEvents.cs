using System;
using System.Collections.Generic;
using SCILL;
using SCILL.Model;
using UnityEngine;
using UnityEngine.Events;

namespace SCILL.Effects
{
    /// <summary>
    /// Simple Utility class for accessing Personal Challenge related realtime events in the editor via <c>UnityEvent</c>s.
    /// This script connects to <see cref="SCILLPersonalChallengesManager"/> events.
    /// </summary>
    public class SCILLPersonalChallengeEvents : MonoBehaviour
    {
        /// <summary>
        /// Called when a personal challenge was unlocked.
        /// </summary>
        [SerializeField] private UnityEvent onUnlocked;
        /// <summary>
        /// Called when a personal challenge was activated.
        /// </summary>
        [SerializeField] private UnityEvent onActivated;
        /// <summary>
        /// Called when the progress towards the challenge's goal changed.
        /// </summary>
        [SerializeField] private UnityEvent onProgressUpdated;
        /// <summary>
        /// Called when a personal challenge was completed.
        /// </summary>
        [SerializeField] private UnityEvent onCompleted;
        /// <summary>
        /// Called when a personal challenge reward was claimed.
        /// </summary>
        [SerializeField] private UnityEvent onClaimed;

        protected Dictionary<SCILLPersonalChallengeModificationType, UnityEvent> TypeToEventMap = new Dictionary<SCILLPersonalChallengeModificationType, UnityEvent>();

        protected virtual void Awake()
        {
            TypeToEventMap[SCILLPersonalChallengeModificationType.Unlocked] = onUnlocked;
            TypeToEventMap[SCILLPersonalChallengeModificationType.Activated] = onActivated;
            TypeToEventMap[SCILLPersonalChallengeModificationType.Completed] = onCompleted;
            TypeToEventMap[SCILLPersonalChallengeModificationType.Claimed] = onClaimed;
            TypeToEventMap[SCILLPersonalChallengeModificationType.Progress] = onProgressUpdated;
        }

        protected virtual void OnEnable()
        {
            SCILLPersonalChallengesManager.OnPersonalChallengeUpdatedFromServer += OnPersonalChallengeUpdated;
        }
        
        protected virtual void OnDisable()
        {
            SCILLPersonalChallengesManager.OnPersonalChallengeUpdatedFromServer -= OnPersonalChallengeUpdated;
        }

        
        /// <summary>
        /// Invoked on changes to a personal challenge. Uses the <see cref="modificationtype"/> to identify
        /// the appropriate <c>UnityEvent</c> to call.
        /// </summary>
        /// <param name="challenge">The updated challenge data.</param>
        /// <param name="modificationtype">The type of modification that was applied to the challenge.</param>
        private void OnPersonalChallengeUpdated(Challenge challenge, SCILLPersonalChallengeModificationType modificationtype)
        {
            if (TypeToEventMap.ContainsKey(modificationtype))
            {
                UnityEvent unityEvent = TypeToEventMap[modificationtype];
                if (null != unityEvent)
                {
                    unityEvent.Invoke();
                }
            }
        }

       
    }
}
