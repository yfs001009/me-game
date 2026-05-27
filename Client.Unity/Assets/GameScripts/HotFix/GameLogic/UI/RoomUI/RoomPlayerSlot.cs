using GameLogic.SheepBattle.Lobby;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    internal sealed class RoomPlayerSlot : UIWidget
    {
        private Text _txtNickname;
        private Text _txtLevel;
        private Text _txtReadyState;
        private Text _txtOwner;

        protected override void ScriptGenerator()
        {
            _txtNickname = FindChildComponent<Text>("m_txtNickname");
            _txtLevel = FindChildComponent<Text>("m_txtLevel");
            _txtReadyState = FindChildComponent<Text>("m_txtReadyState");
            _txtOwner = FindChildComponent<Text>("m_txtOwner");
        }

        protected override void OnCreate()
        {
            var background = gameObject.GetComponent<Image>();
            DynamicUI.ApplySprite(background, DynamicUI.ArtRoomPlayerSlot);
        }

        public void Refresh(RoomPlayerViewModel player, int slotIndex)
        {
            if (player == null)
            {
                SetEmpty(slotIndex);
                return;
            }

            var nickname = string.IsNullOrWhiteSpace(player.Nickname) ? $"玩家{player.PlayerId}" : player.Nickname;
            if (_txtNickname != null)
            {
                _txtNickname.text = nickname;
                _txtNickname.color = Color.black;
            }

            if (_txtLevel != null)
            {
                _txtLevel.text = $"Lv.{player.Level}";
            }

            if (_txtReadyState != null)
            {
                _txtReadyState.gameObject.SetActive(!player.IsOwner);
                if (!player.IsOwner)
                {
                    _txtReadyState.text = player.IsReady ? "已准备" : "未准备";
                    _txtReadyState.color = player.IsReady ? new Color(0.1f, 0.5f, 0.22f, 1f) : new Color(0.55f, 0.43f, 0.12f, 1f);
                }
            }

            if (_txtOwner != null)
            {
                _txtOwner.gameObject.SetActive(player.IsOwner);
                _txtOwner.text = "房主";
            }
        }

        private void SetEmpty(int slotIndex)
        {
            if (_txtNickname != null)
            {
                _txtNickname.text = $"空位 {slotIndex + 1}";
                _txtNickname.color = new Color(0.45f, 0.45f, 0.45f, 1f);
            }

            if (_txtLevel != null)
            {
                _txtLevel.text = string.Empty;
            }

            if (_txtReadyState != null)
            {
                _txtReadyState.text = "等待加入";
                _txtReadyState.color = new Color(0.45f, 0.45f, 0.45f, 1f);
            }

            if (_txtOwner != null)
            {
                _txtOwner.gameObject.SetActive(false);
            }
        }
    }
}
