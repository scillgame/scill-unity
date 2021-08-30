using System.Collections;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;

namespace SCILL
{
    /// <summary>
    ///     <para>
    ///         This class is designed as a “Singleton” and should be attached to the same GameObject that you have
    ///         <see cref="SCILLManager" /> attached. It will completely manage notifications.
    ///     </para>
    ///     <para>
    ///         This class will:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             Automatically display success and error notifications on screen if API requests, sent using the
    ///             <see cref="SCILLManager" />, succeed or fail, e.g. when sending an event.
    ///             This enables faster debugging especially on platforms like WebGL.
    ///         </item>
    ///         <item>
    ///             Display general notifications or notifications regarding challenge updates on a dedicated spot on the
    ///             UI.
    ///         </item>
    ///     </list>
    ///     <para>
    ///         Notifications are used by SCILL to automatically display success and error notifications on screen, e.g.
    ///         display error messages on screen when a request fails. To disable those notifications in your final product,
    ///         simply set <see cref="successNotificationPrefab" /> and <see cref="errorNotificationPrefab" /> to None.
    ///     </para>
    ///     <para>
    ///         See <see cref="AddNotification" /> for more on success and error notifications.
    ///     </para>
    ///     <para>
    ///         The center and challenge notifications (see <see cref="AddCenterNotification" /> and
    ///         <see cref="AddChallengeUpdate" /> methods) are instantiated into the <see cref="centerNotification" />
    ///         transform. Center and challenge notifications will be enqueued and displayed one after another, so using an
    ///         auto layout component is not necessary.
    ///     </para>
    ///     <para>
    ///         Center and challenge notifications can be used for example to inform the user that a challenge has been updated
    ///         or for information regarding
    ///         your custom gameplay events.
    ///     </para>
    /// </summary>
    public class SCILLNotificationManager : MonoBehaviour
    {
        public static SCILLNotificationManager Instance; // **<- reference link to SCILL

        /// <summary>
        ///     The notification pre
        /// </summary>
        public SCILLNotification successNotificationPrefab;

        public SCILLNotification errorNotificationPrefab;
        public SCILLNotification centerNotificationPrefab;
        public SCILLChallengeItem challengeNotificationPrefab;

        /// <summary>
        ///     Connect a <c>Transform</c> component. The prefabs for each success or error notification will be instantiated into
        ///     this transform.
        ///     Make sure you add an auto layout component (like <c>VerticalLayoutGroup</c> when using Unity UI) to that
        ///     <c>rankingsContainer</c> object
        ///     so that
        ///     items get displayed correctly.
        /// </summary>
        public Transform container;

        /// <summary>
        ///     Connect a <c>Transform</c> component. The prefabs for each center or challenge notification will be instantiated
        ///     into
        ///     this transform. The notifications will enqueued and played one after another, therefore it is not necessary to use
        ///     auto layout scripts on this.
        /// </summary>
        public Transform centerNotification;

        private readonly Queue<SCILLCenterNotification> centerNotifications = new Queue<SCILLCenterNotification>();

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

        private void Update()
        {
            if (centerNotification.childCount <= 0)
                if (centerNotifications.Count > 0)
                    ShowNextCenterNotification();
        }

        /// <summary>
        ///     <para>
        ///         Adds a notification to the screen, using the <see cref="successNotificationPrefab" /> for notifications of type
        ///         <c>Success</c> and <see cref="errorNotificationPrefab" /> for notifications of type <c>Error</c>.
        ///     </para>
        ///     <para>
        ///         The prefabs for each success or error notification will be instantiated into the <see cref="container" />
        ///         object and destroyed automatically after the set lifetime. Error and success notifications will be displayed
        ///         instantly on
        ///         calling <see cref="AddNotification" />. Using an auto layout component like
        ///         <c>VerticalLayoutGroup</c> ensures that notifications are not displayed on top of each other.
        ///     </para>
        /// </summary>
        /// <param name="type">The type of the notification, either <c>Success</c> or <c>Error</c></param>
        /// <param name="text">The message to display on the notification</param>
        /// <param name="image">
        ///     The image to load from the <c>Resources</c> folder for displaying on the notification.
        /// </param>
        /// <param name="duration">The notification lifetime. Defaults to <c>5.0f</c> seconds.</param>
        public virtual void AddNotification(SCILLNotificationType type, string text, string image = null,
            float duration = 5.0f)
        {
            GameObject prefab = null;

            if (SCILLNotificationType.Success == type && successNotificationPrefab)
                prefab = successNotificationPrefab.gameObject;
            if (type == SCILLNotificationType.Error && errorNotificationPrefab)
                prefab = errorNotificationPrefab.gameObject;

            if (prefab)
            {
                var notificationGo = Instantiate(prefab, container, false);
                var notification = notificationGo.GetComponent<SCILLNotification>();
                if (notification)
                {
                    notification.message.text = text;
                    StartCoroutine(LoadNotificationIcon(notification, image));
                }

                StartCoroutine(SelfDestruct(notificationGo, duration));
            }
        }

        private IEnumerator LoadNotificationIcon(SCILLNotification notification, string imageResourceName)
        {
            if (string.IsNullOrEmpty(imageResourceName)) yield break;

            var imageRequest = Resources.LoadAsync<Sprite>(imageResourceName);
            yield return imageRequest;
            if (imageRequest.asset is Sprite sprite)
                if (notification && notification.image)
                    notification.image.sprite = sprite;
        }

        private IEnumerator SelfDestruct(GameObject gameObject, float timeout = 2f)
        {
            yield return new WaitForSeconds(timeout);
            Destroy(gameObject);
        }

        /// <summary>
        ///     <para>
        ///         Adds a simple text notification to the <see cref="centerNotification" /> transform. Center notifications will
        ///         be enqueued and displayed one after
        ///         another,
        ///         so using an
        ///         auto layout component on the <see cref="centerNotification" /> transform is not necessary.
        ///     </para>
        ///     <para>
        ///         Instantiates the <see cref="centerNotificationPrefab" /> prefab.
        ///     </para>
        /// </summary>
        /// <param name="text">The message to display on the notification.</param>
        /// <param name="image">
        ///     The image to load from the <c>Resources</c> folder for displaying on the notification. This is not used in the base method,
        /// so if you'd like to display an image on the center notification, please override this method.
        /// </param>
        /// <param name="duration">The notifications lifetime.</param>
        public virtual void AddCenterNotification(string text, string image = null, float duration = 2.0f)
        {
            if (centerNotificationPrefab)
            {
                var notification = new SCILLCenterTextNotification(centerNotificationPrefab.gameObject, text, duration);
                centerNotifications.Enqueue(notification);
            }
        }

        /// <summary>
        ///     <para>
        ///         Adds a challenge notification to the <see cref="centerNotification" /> transform. Challenge notifications will
        ///         be enqueued and displayed one after
        ///         another,
        ///         so using an
        ///         auto layout component on the <see cref="centerNotification" /> object is not necessary.
        ///     </para>
        ///     <para>
        ///         Having a <see cref="SCILLChallengeItem" /> component in the <see cref="challengeNotificationPrefab" />
        ///         hierarchy will let the notification automatically
        ///         display the challenge data on the <see cref="SCILLChallengeItem" /> user interface.
        ///     </para>
        ///     <para>
        ///         Instantiates the <see cref="challengeNotificationPrefab" /> prefab.
        ///     </para>
        /// </summary>
        /// <param name="challenge">The updated challenge data.</param>
        /// <param name="text">
        ///     An additional text that should be displayed using the notifications <see cref="SCILLNotification.message" />
        ///     <c>UnityEngine.UI.Text</c> component
        /// </param>
        /// <param name="duration">The notifications lifetime.</param>
        public void AddChallengeUpdate(Challenge challenge, string text, float duration = 3.0f)
        {
            if (challengeNotificationPrefab)
            {
                var notification =
                    new SCILLCenterChallengeNotification(challengeNotificationPrefab.gameObject, challenge, text,
                        duration);
                centerNotifications.Enqueue(notification);
            }
        }

        private void ShowNextCenterNotification()
        {
            if (centerNotifications.Count <= 0) return;

            var notification = centerNotifications.Dequeue();
            var go = notification.Show(centerNotification);
            StartCoroutine(SelfDestruct(go, notification.PreferredTimeout));
        }
    }


    public enum SCILLNotificationType
    {
        Success,
        Error
    }

    internal abstract class SCILLCenterNotification
    {
        public GameObject Prefab;
        public float PreferredTimeout = 2f;

        public SCILLCenterNotification(GameObject prefab, float timeout = 2.0f)
        {
            Prefab = prefab;
            PreferredTimeout = timeout;
        }

        public abstract GameObject Show(Transform container);
    }

    internal class SCILLCenterTextNotification : SCILLCenterNotification
    {
        public string Text;

        public SCILLCenterTextNotification(GameObject prefab, string text, float timeout = 2.0f) : base(prefab, timeout)
        {
            Text = text;
        }

        public override GameObject Show(Transform container)
        {
            var notificationGo = Object.Instantiate(Prefab, container, false);
            var notification = notificationGo.GetComponent<SCILLNotification>();
            if (notification && notification.message) notification.message.text = Text;

            return notificationGo;
        }
    }

    internal class SCILLCenterChallengeNotification : SCILLCenterTextNotification
    {
        public Challenge Challenge;

        public SCILLCenterChallengeNotification(GameObject prefab, Challenge challenge, string text,
            float timeout = 3.0f) : base(prefab,
            text, timeout)
        {
            Challenge = challenge;
        }

        public override GameObject Show(Transform container)
        {
            var challengeGo = Object.Instantiate(Prefab, container, false);
            var challengeItem = challengeGo.GetComponent<SCILLChallengeItem>();
            if (challengeItem) challengeItem.UpdateChallenge(Challenge);

            var notification = challengeGo.GetComponent<SCILLNotification>();
            if (notification && notification.message) notification.message.text = Text;

            return challengeGo;
        }
    }
}