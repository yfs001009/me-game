using GameLogic.SheepBattle.Battle;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI, location: "BattleMainUI")]
    internal sealed class BattleMainUI : UIWindow
    {
        private GameObject _goTopInfo;
        private GameObject _itemRoleInfo;
        private GameObject _itemMonsterInfo;
        private GameObject _itemTouch;
        private Text _roleHpText;
        private Text _monsterHpText;
        private Button _touchButton;

        protected override void ScriptGenerator()
        {
            _goTopInfo = FindChild("m_goTopInfo")?.gameObject;
            _itemRoleInfo = FindChild("m_goTopInfo/m_itemRoleInfo")?.gameObject;
            _itemMonsterInfo = FindChild("m_goTopInfo/m_itemMonsterInfo")?.gameObject;
            _itemTouch = FindChild("m_rectContainer/m_itemTouch")?.gameObject;
            _roleHpText = FindChildComponent<Text>("m_goTopInfo/m_itemRoleInfo/m_sliderHp/m_tmpHpValue");
            _monsterHpText = FindChildComponent<Text>("m_goTopInfo/m_itemMonsterInfo/m_sliderHp/m_tmpHpValue");
            _touchButton = FindChildComponent<Button>("m_rectContainer/m_itemTouch/TouchButton");
        }

        protected override void OnCreate()
        {
            _touchButton?.onClick.AddListener(OnClickTouch);

            if (_goTopInfo != null)
            {
                _goTopInfo.SetActive(true);
            }

            if (_itemRoleInfo != null)
            {
                _itemRoleInfo.SetActive(true);
            }

            if (_itemMonsterInfo != null)
            {
                _itemMonsterInfo.SetActive(true);
            }

            if (_itemTouch != null)
            {
                _itemTouch.SetActive(true);
            }

            RefreshBattleHud();
        }

        private void RefreshBattleHud()
        {
            if (_roleHpText != null)
            {
                _roleHpText.text = "HP 100/100";
            }

            if (_monsterHpText != null)
            {
                _monsterHpText.text = "HP 100/100";
            }

            Log.Info("已进入战斗界面");
        }

        private void OnClickTouch()
        {
            Log.Info("战斗触摸按钮点击");
        }

        protected override void OnDestroy()
        {
            Log.Info("战斗界面关闭");
        }
    }
}
