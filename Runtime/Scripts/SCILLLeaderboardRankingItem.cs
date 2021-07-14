using SCILL.Model;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SCILL
{
    /// <summary>
    ///     This class implements user interface for a leaderboard ranking (i.e. rank position in a leaderboard table). It gets
    ///     a <see cref="LeaderboardRanking" /> via its <c>ranking</c> property and updates text and images provided in
    ///     properties.
    /// </summary>
    public class SCILLLeaderboardRankingItem : MonoBehaviour
    {
        /// <summary>
        ///     Connect a <c>UnityEngine.UI.Text</c> component which will be set with the username of the user. The username must
        ///     be retrieved using the <see cref="SCILLManager.SetUserInfoAsync" /> beforehand. If no user info is available the
        ///     string <c>Guest</c> will be used.
        /// </summary>
        public Text username;

        /// <summary>
        ///     The rank of the user will be set into this <c>UnityEngine.UI.Text</c> component as text with the
        ///     <see cref="rankSuffix" /> suffix, e.g. <c>1.</c> or <c>10.</c>.
        /// </summary>
        public Text rank;

        /// <summary>
        ///     The suffix to use for displaying the users rank. Default is ".", displaying a rank as <c>1.</c> or <c>10.</c>.
        /// </summary>
        public string rankSuffix = ".";

        /// <summary>
        ///     The score of the user will be set into this <c>UnityEngine.UI.Text</c> component as text. You can use the
        ///     <c>numberOfDecimals</c> setting in the <see cref="SCILLLeaderboard" /> item to format the score into a decimal
        ///     value.
        /// </summary>
        public Text score;

        /// <summary>
        ///     The <c>avatarImage</c> of the additional user info will be used to load a sprite from the <c>Resources</c> folder.
        /// </summary>
        /// <remarks>
        ///     Please note: The sprite is loaded at runtime and must be within a <c>Resources</c> folder in your Asset database so
        ///     that Unity exposes the asset for dynamic loading.
        /// </remarks>
        [FormerlySerializedAs("image")] public Image avatarImage;

        /// <summary>
        ///     The path from a Resources folder to a folder in which the avatar images are stored. E.g. if the
        ///     <c>avatarResourcesPath</c> is <c>"Avatars/"</c>,
        ///     this class will attempt to load the avatar image from a <c>"Resources/Avatars"</c> folder located in your Unity
        ///     Assets.
        /// </summary>
        public string avatarResourcesPath = "Avatars/";

        private int _numberOfDecimals;

        private LeaderboardRanking _ranking;

        /// <summary>
        ///     The leaderboard ranking data for this component. Setting this value will automatically update the UI.
        /// </summary>
        public LeaderboardRanking ranking
        {
            get => _ranking;
            set
            {
                _ranking = value;
                UpdateUI();
            }
        }

        /// <summary>
        ///     The number of decimals after which the decimal delimiter ("." in this case) is inserted into the score. Setting
        ///     this value will automatically update the UI.
        /// </summary>
        /// <remarks>
        ///     See <see cref="SCILLLeaderboard.numberOfDecimals" /> for an in-depth explanation.
        /// </remarks>
        public int numberOfDecimals
        {
            get => _numberOfDecimals;
            set
            {
                _numberOfDecimals = value;
                UpdateUI();
            }
        }


        /// <summary>
        ///     Called on changes to <see cref="ranking" /> or <see cref="numberOfDecimals" /> for updating the UI elements.
        ///     Override this function,
        ///     if you'd like to adjust the way the UI is displayed.
        /// </summary>
        protected virtual void UpdateUI()
        {
            if (ranking == null) return;

            if (username)
            {
                if (ranking.additional_info != null && !string.IsNullOrEmpty(ranking.additional_info.username))
                    username.text = ranking.additional_info.username;
                else
                    username.text = "Guest";
            }

            if (avatarImage)
                if (ranking.additional_info != null && !string.IsNullOrEmpty(ranking.additional_info.avatarImage))
                {
                    var sprite = Resources.Load<Sprite>(avatarResourcesPath + ranking.additional_info.avatarImage);
                    if (sprite) avatarImage.sprite = sprite;
                }

            if (rank) rank.text = ranking.rank + rankSuffix;

            if (score)
            {
                score.text = ranking.score.ToString();
                if (numberOfDecimals > 0 && score.text.Length > numberOfDecimals)
                    score.text = score.text.Insert(score.text.Length - numberOfDecimals, ".");
            }
        }
    }
}