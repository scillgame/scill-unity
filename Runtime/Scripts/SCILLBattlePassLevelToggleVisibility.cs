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

    public class SCILLBattlePassLevelToggleVisibility : MonoBehaviour
    {
        public SCILLBattlePassLevelVisibility ifLocked;
        public SCILLBattlePassLevelVisibility ifUnlocked;
        public SCILLBattlePassLevelVisibility ifCompleted;
        public SCILLBattlePassLevelVisibility ifUncompleted;

        [HideInInspector] public BattlePassLevel battlePassLevel;

        private Image _image;

        // Start is called before the first frame update
        void Start()
        {
            var battlePassLevelUI = GetComponentInParent<SCILLBattlePassLevel>();
            if (battlePassLevelUI)
            {
                battlePassLevel = battlePassLevelUI.battlePassLevel;
            }

            _image = GetComponent<Image>();
        }

        void Show(bool show)
        {
            _image.enabled = show;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_image || battlePassLevel == null)
            {
                return;
            }

            if (ifLocked != SCILLBattlePassLevelVisibility.DoNothing)
            {
                if (battlePassLevel.activated_at == null)
                {
                    Show(ifLocked == SCILLBattlePassLevelVisibility.Visible);
                }
            }

            if (ifUnlocked != SCILLBattlePassLevelVisibility.DoNothing)
            {
                if (battlePassLevel.activated_at != null)
                {
                    Show(ifUnlocked == SCILLBattlePassLevelVisibility.Visible);
                }
            }

            if (ifCompleted != SCILLBattlePassLevelVisibility.DoNothing)
            {
                if (battlePassLevel.level_completed == true)
                {
                    Show(ifCompleted == SCILLBattlePassLevelVisibility.Visible);
                }
            }

            if (ifUncompleted != SCILLBattlePassLevelVisibility.DoNothing)
            {
                if (battlePassLevel.level_completed == false)
                {
                    Show(ifUncompleted == SCILLBattlePassLevelVisibility.Visible);
                }
            }
        }
    }
}