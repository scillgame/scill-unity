using UnityEngine;
using UnityEngine.UI;

namespace SCILL
{
    /// <summary>
    ///     This script supplies the <see cref="SCILLNotificationManager" /> with access to the user interface elements, e.g.
    ///     the notification icon and message. Add this component to a prefab and fill the <see cref="image" /> and
    ///     <see cref="message" /> references. The prefab will then be used by the <see cref="SCILLNotificationManager" /> to
    ///     display notifications on screen.
    /// </summary>
    public class SCILLNotification : MonoBehaviour
    {
        /// <summary>
        /// The notification icon.
        /// </summary>
        public Image image;

        /// <summary>
        /// Connect a <c>UnityEngine.UI.Text</c> component, which will be set to the notification message 
        /// </summary>
        public Text message;

        private float _startTime;

        private void Start()
        {
            _startTime = Time.time;
        }
    }
}