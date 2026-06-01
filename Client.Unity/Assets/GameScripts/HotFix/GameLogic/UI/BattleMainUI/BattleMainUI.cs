using System.Collections.Generic;
using System.Linq;
using Fantasy.Async;
using GameConfig.battle;
using GameLogic.SheepBattle.Battle;
using GameLogic.SheepBattle.Common;
using GameLogic.SheepBattle.Config;
using GameLogic.SheepBattle.Network;
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
        private GameObject _legacyRootBackground;
        private Text _txtElfCount;
        private Text _txtBattleTime;
        private Text _txtTrollCount;
        private RectTransform _teammateAvatarRoot;
        private Image _avatarTemplate;
        private Text _txtGold;
        private Text _txtWood;
        private GameObject _buildPanel;
        private RectTransform _buildCardRoot;
        private Button _buildCardTemplate;
        private Text _txtBuildingInfo;
        private RectTransform _buildingOperationButtonRoot;
        private Button _buildingOperationButtonTemplate;
        private Text _txtSelectedHint;
        private Text _txtSyncState;
        private Text _txtPosition;
        private RectTransform _skillPanel;
        private Button _btnPrimarySkill;
        private Button _btnSecondarySkill;
        private RectTransform _playerInfoPanel;
        private Text _txtPlayerStats;
        private RectTransform _equipmentSlotRoot;
        private RectTransform _battleShopPanel;
        private Text _txtBattleShopTitle;
        private Text _txtBattleShopHint;
        private RectTransform _battleShopGoodsRoot;
        private Button _battleShopGoodsTemplate;
        private RectTransform _buildingActionPanel;
        private Text _txtBuildingActionTitle;
        private Text _txtBuildingActionBody;
        private Button _btnBuildingActionConfirm;
        private Button _btnBuildingActionClose;

        private readonly List<GameObject> _spawnedAvatars = new();
        private readonly List<GameObject> _spawnedBuildCards = new();
        private readonly List<GameObject> _spawnedOperationButtons = new();
        private readonly List<GameObject> _spawnedEquipmentSlots = new();
        private readonly List<GameObject> _spawnedBattleShopGoods = new();
        private float _nextHudRefreshTime;
        private long _lastOperationBuildingId;
        private string _lastOperationSignature = string.Empty;
        private bool _buildCardsReady;
        private bool _battleShopVisible;
        private int _nearbyShopId;
        private long _pendingBuildingActionInstanceId;

        protected override void ScriptGenerator()
        {
            _goTopInfo = FindChild("m_goTopInfo")?.gameObject;
            _itemRoleInfo = FindChild("m_goTopInfo/m_itemRoleInfo")?.gameObject;
            _itemMonsterInfo = FindChild("m_goTopInfo/m_itemMonsterInfo")?.gameObject;
            _itemTouch = FindChild("m_rectContainer/m_itemTouch")?.gameObject;
            _legacyRootBackground = FindChild("m_imgBg")?.gameObject
                                    ?? FindChild("m_imgBackground")?.gameObject
                                    ?? FindChild("Background")?.gameObject
                                    ?? FindChild("Bg")?.gameObject;

            _txtElfCount = FindHudComponent<Text>("m_txtElfCount");
            _txtBattleTime = FindHudComponent<Text>("m_txtBattleTime");
            _txtTrollCount = FindHudComponent<Text>("m_txtTrollCount");
            _teammateAvatarRoot = FindHudChild("m_avatarList") as RectTransform;
            _avatarTemplate = FindHudComponent<Image>("m_avatarTemplate");
            _txtGold = FindResourceValueText("m_goldRow");
            _txtWood = FindResourceValueText("m_woodRow");
            _buildPanel = FindHudChild("m_buildPanel")?.gameObject;
            _buildCardRoot = FindHudChild("m_cardList") as RectTransform;
            _buildCardTemplate = FindHudComponent<Button>("m_buildCardTemplate");
            _txtBuildingInfo = FindHudComponent<Text>("m_txtBuildingInfo");
            _buildingOperationButtonRoot = FindHudChild("m_buttonList") as RectTransform;
            _buildingOperationButtonTemplate = FindHudComponent<Button>("m_operationButtonTemplate");
            _txtSelectedHint = FindHudComponent<Text>("m_txtSelectedHint");
            _txtSyncState = FindHudComponent<Text>("m_txtSyncState");
            _txtPosition = FindHudComponent<Text>("m_txtPosition");
            _skillPanel = FindHudChild("m_skillPanel") as RectTransform;
            _btnPrimarySkill = FindHudComponent<Button>("m_btnPrimarySkill");
            _btnSecondarySkill = FindHudComponent<Button>("m_btnSecondarySkill");
        }

        protected override void OnCreate()
        {
            SetLegacyHudVisible(false);
            SetTemplateVisible(_avatarTemplate, false);
            SetTemplateVisible(_buildCardTemplate, false);
            SetTemplateVisible(_buildingOperationButtonTemplate, false);
            BuildBuildingCardsFromPrefab();
            BindSkillButtons();
            EnsurePlayerInfoHud();
            EnsureBuildingActionPanel();
            RefreshBattleHud();
        }

        protected override void OnRefresh()
        {
            RefreshBattleHud();
        }

        protected override void OnUpdate()
        {
            if (Time.unscaledTime < _nextHudRefreshTime)
            {
                return;
            }

            _nextHudRefreshTime = Time.unscaledTime + 0.25f;
            RefreshBattleHud();
        }

        private void SetLegacyHudVisible(bool visible)
        {
            _legacyRootBackground?.SetActive(false);
            _goTopInfo?.SetActive(visible);
            _itemRoleInfo?.SetActive(visible);
            _itemMonsterInfo?.SetActive(visible);
            _itemTouch?.SetActive(visible);
        }

        private static void SetTemplateVisible(Component template, bool visible)
        {
            if (template != null)
            {
                template.gameObject.SetActive(visible);
            }
        }

        private void BuildBuildingCardsFromPrefab()
        {
            if (_buildCardsReady || _buildCardRoot == null || _buildCardTemplate == null)
            {
                return;
            }

            ClearSpawned(_spawnedBuildCards);
            var profile = SheepNetworkService.Instance.Profile;
            var loadoutCardIds = BattleController.Instance.CurrentSnapshot?.Players
                .FirstOrDefault(player => player.PlayerId == profile?.PlayerId)?
                .SelectedBuildingCardIds;
            var cards = ConfigSystem.Instance.Tables.TbBuildingCard.DataList
                .Where(card => loadoutCardIds == null || loadoutCardIds.Count == 0 || loadoutCardIds.Contains(card.CardId))
                .OrderBy(card => card.SortOrder)
                .ThenBy(card => card.CardId)
                .ToList();

            for (var i = 0; i < cards.Count; i++)
            {
                CreateBuildingCard(cards[i]);
            }

            _buildCardsReady = true;
        }

        private void CreateBuildingCard(BuildingCardConfig card)
        {
            var instance = Object.Instantiate(_buildCardTemplate.gameObject, _buildCardRoot, false);
            instance.name = $"m_buildCard_{card.CardId}";
            instance.SetActive(true);
            _spawnedBuildCards.Add(instance);

            SetChildText(instance.transform, "m_txtName", card.CardName);
            SetChildText(instance.transform, "m_txtCost", $"金 {card.CostGold}  木 {card.CostWood}");
            SetChildText(instance.transform, "m_txtDesc", string.Empty);

            var button = instance.GetComponent<Button>();
            button.onClick.AddListener(() => OnClickBuildCard(card));
        }

        private void RefreshBattleHud()
        {
            var snapshot = BattleController.Instance.CurrentSnapshot;
            var profile = SheepNetworkService.Instance.Profile;
            var me = snapshot?.Players.FirstOrDefault(item => item.PlayerId == profile?.PlayerId);

            RefreshCampScore(snapshot);
            RefreshTime();
            RefreshResources(me);
            RefreshTeammateAvatars(snapshot, me);
            RefreshStateTexts(snapshot, me);
            RefreshBuildPanel(me);
            RefreshSkillButtons(me);
            RefreshBuildingOperationButtons(me);
            RefreshPlayerInfoHud(me);
        }

        private void RefreshCampScore(Fantasy.BattleSnapshotInfo snapshot)
        {
            SetText(_txtElfCount, $"精灵  {snapshot?.Players.Count(IsElf) ?? 0}");
            SetText(_txtTrollCount, $"巨魔  {snapshot?.Players.Count(IsTroll) ?? 0}");
        }

        private void RefreshTime()
        {
            var duration = Mathf.Max(GameRuleService.Instance.BattleDurationSeconds, 0);
            var remain = Mathf.Max(duration - BattleController.Instance.BattleElapsedSeconds, 0);
            SetText(_txtBattleTime, FormatTime(remain));
        }

        private void RefreshResources(Fantasy.BattlePlayerStateInfo me)
        {
            SetText(_txtGold, me == null ? "--" : me.Gold.ToString());
            SetText(_txtWood, me == null ? "--" : me.Wood.ToString());
        }

        private void RefreshTeammateAvatars(Fantasy.BattleSnapshotInfo snapshot, Fantasy.BattlePlayerStateInfo me)
        {
            if (_teammateAvatarRoot == null || _avatarTemplate == null)
            {
                return;
            }

            if (snapshot == null)
            {
                DeactivateFrom(_spawnedAvatars, 0);
                return;
            }

            var myCamp = me?.Camp ?? "Elf";
            var teammates = snapshot.Players.Where(player => SameCamp(player.Camp, myCamp)).ToList();
            for (var i = 0; i < teammates.Count; i++)
            {
                RefreshAvatar(i, teammates[i], me != null && teammates[i].PlayerId == me.PlayerId);
            }

            DeactivateFrom(_spawnedAvatars, teammates.Count);
        }

        private void RefreshAvatar(int index, Fantasy.BattlePlayerStateInfo player, bool isMe)
        {
            var instance = GetOrCreatePooled(_spawnedAvatars, index, _avatarTemplate.gameObject, _teammateAvatarRoot);
            instance.name = $"m_avatar_{player.PlayerId}";
            instance.SetActive(true);

            var image = instance.GetComponent<Image>();
            if (image != null)
            {
                image.color = isMe ? new Color(0.34f, 0.96f, 0.62f, 0.96f) : new Color(0.82f, 0.92f, 1f, 0.92f);
            }

            SetChildText(instance.transform, "m_txtInitial", GetAvatarInitial(player));
            SetChildText(instance.transform, "m_txtSelf", isMe ? "我" : string.Empty);

            var button = instance.GetComponent<Button>() ?? instance.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.RemoveAllListeners();
            var playerId = player.PlayerId;
            button.onClick.AddListener(() => BattleController.Instance.FocusPlayer(playerId));
        }

        private void RefreshStateTexts(Fantasy.BattleSnapshotInfo snapshot, Fantasy.BattlePlayerStateInfo me)
        {
            SetText(_txtSyncState, snapshot == null
                ? "同步：等待服务器快照"
                : $"同步：{snapshot.State} Tick {snapshot.Tick} 玩家 {snapshot.Players.Count} 建筑 {snapshot.Buildings.Count}");
            SetText(_txtPosition, me == null ? "位置 --" : $"位置 {me.PosX:0.0}, {me.PosY:0.0}");

            var selectedId = BattleController.Instance.SelectedBuildingId;
            var selected = selectedId <= 0 ? null : ConfigSystem.Instance.Tables.TbBuilding.GetOrDefault(selectedId);
            SetText(_txtSelectedHint, selected == null ? "移动模式：WASD 控制角色" : $"建造：{selected.BuildingName}");
        }

        private void OnClickBuildCard(BuildingCardConfig card)
        {
            GameEvent.Get<IBattleCommand>()?.OnSelectBuilding(card.BuildingId);
            RefreshBattleHud();
        }

        private void BindSkillButtons()
        {
            if (_btnPrimarySkill != null)
            {
                _btnPrimarySkill.onClick.RemoveAllListeners();
                _btnPrimarySkill.onClick.AddListener(OnClickPrimarySkill);
            }

            if (_btnSecondarySkill != null)
            {
                _btnSecondarySkill.onClick.RemoveAllListeners();
                _btnSecondarySkill.onClick.AddListener(OnClickSecondarySkill);
            }
        }

        private void RefreshSkillButtons(Fantasy.BattlePlayerStateInfo me)
        {
            if (_skillPanel != null)
            {
                _skillPanel.gameObject.SetActive(me != null);
            }

            if (_btnPrimarySkill != null)
            {
                SetChildText(_btnPrimarySkill.transform, "m_txtLabel", IsTroll(me) ? "猛击" : "建造");
                _btnPrimarySkill.interactable = me != null;
            }

            if (_btnSecondarySkill != null)
            {
                SetChildText(_btnSecondarySkill.transform, "m_txtLabel", IsTroll(me) ? "咆哮" : "取消");
                _btnSecondarySkill.interactable = me != null;
            }
        }

        private void OnClickPrimarySkill()
        {
            var me = GetLocalPlayer();
            if (IsTroll(me))
            {
                CommonNoticeService.Show("巨魔技能待接入", "技能");
                return;
            }

            GameEvent.Get<IBattleCommand>()?.OnOpenBuildPanel();
            RefreshBattleHud();
        }

        private void OnClickSecondarySkill()
        {
            var me = GetLocalPlayer();
            if (IsTroll(me))
            {
                CommonNoticeService.Show("巨魔技能待接入", "技能");
                return;
            }

            GameEvent.Get<IBattleCommand>()?.OnExitBuildMode();
            RefreshBattleHud();
        }

        private void RefreshBuildingOperationButtons(Fantasy.BattlePlayerStateInfo me)
        {
            if (_buildingOperationButtonRoot == null || _buildingOperationButtonTemplate == null)
            {
                return;
            }

            var selected = BattleController.Instance.GetSelectedBuilding();
            if (selected == null)
            {
                DeactivateFrom(_spawnedOperationButtons, 0);
                _lastOperationBuildingId = 0;
                _lastOperationSignature = string.Empty;
                SetText(_txtBuildingInfo, string.Empty);
                return;
            }

            var config = ConfigSystem.Instance.Tables.TbBuilding.GetOrDefault(selected.BuildingId);
            var isOwner = me != null && selected.OwnerPlayerId == me.PlayerId;
            SetText(_txtBuildingInfo, FormatBuildingInfo(selected, config, false));

            var canUpgrade = CanUpgrade(selected, config, isOwner);
            var shopId = IsTroll(me) ? GetShopId(selected) : 0;
            var signature = $"{selected.InstanceId}:{isOwner}:{canUpgrade}:{shopId}";
            if (_lastOperationBuildingId == selected.InstanceId && _lastOperationSignature == signature)
            {
                return;
            }

            _lastOperationBuildingId = selected.InstanceId;
            _lastOperationSignature = signature;
            var buttonIndex = 0;

            RefreshOperationButton(buttonIndex++, "信息", () => OpenBuildingInfoPanel(selected, config));
            if (canUpgrade)
            {
                RefreshOperationButton(buttonIndex++, "升级", () => OpenUpgradeBuildingPanel(selected, config));
            }

            if (isOwner)
            {
                RefreshOperationButton(buttonIndex++, "回收", () => OpenRecycleBuildingPanel(selected, config));
            }

            if (shopId > 0)
            {
                if (_battleShopVisible && _nearbyShopId != shopId)
                {
                    OpenBattleShop(shopId, me);
                }

                RefreshOperationButton(buttonIndex++, "购买", () =>
                {
                    OpenBattleShop(shopId, me);
                });
            }
            else
            {
                _nearbyShopId = 0;
                SetBattleShopVisible(false);
            }

            DeactivateFrom(_spawnedOperationButtons, buttonIndex);
        }

        private void RefreshBuildPanel(Fantasy.BattlePlayerStateInfo me)
        {
            var showElfTools = IsElf(me) && BattleController.Instance.IsBuildMode;
            if (_buildPanel != null)
            {
                _buildPanel.SetActive(showElfTools);
                if (showElfTools)
                {
                    _buildPanel.transform.SetAsLastSibling();
                }
            }

            if (_buildCardRoot != null)
            {
                _buildCardRoot.gameObject.SetActive(showElfTools);
            }

            if (!showElfTools)
            {
                if (IsTroll(me))
                {
                    SetText(_txtSelectedHint, "移动模式：WASD 控制角色");
                }
            }
        }

        private void EnsurePlayerInfoHud()
        {
            if (_playerInfoPanel != null)
            {
                return;
            }

            if (!TryBindPlayerInfoHudFromPrefab())
            {
                Log.Error("BattleMainUI.prefab 缺少玩家信息 UI 节点，请执行 SheepBattle/Migrate Battle Main UI。");
                return;
            }

            SetBattleShopVisible(false);
        }

        private bool TryBindPlayerInfoHudFromPrefab()
        {
            _playerInfoPanel = FindHudChild("m_playerInfoPanel") as RectTransform
                               ?? FindHudChild("m_trollPanel") as RectTransform;
            if (_playerInfoPanel == null)
            {
                return false;
            }

            _txtPlayerStats = FindDescendant(_playerInfoPanel, "m_txtPlayerStats")?.GetComponent<Text>()
                              ?? FindDescendant(_playerInfoPanel, "m_txtTrollStats")?.GetComponent<Text>();
            _equipmentSlotRoot = FindDescendant(_playerInfoPanel, "m_equipmentSlots") as RectTransform;
            _battleShopPanel = FindHudChild("m_trollShopPanel") as RectTransform;
            _txtBattleShopTitle = FindDescendant(_battleShopPanel, "m_txtBattleShopTitle")?.GetComponent<Text>();
            _txtBattleShopHint = FindDescendant(_battleShopPanel, "m_txtBattleShopHint")?.GetComponent<Text>();
            _battleShopGoodsRoot = FindDescendant(_battleShopPanel, "m_battleShopGoods") as RectTransform;
            _battleShopGoodsTemplate = FindDescendant(_battleShopGoodsRoot, "m_btnBattleGoodsTemplate")?.GetComponent<Button>();

            var closeButton = FindDescendant(_battleShopPanel, "m_btnBattleShopClose")?.GetComponent<Button>();
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(() => SetBattleShopVisible(false));
            }

            if (_battleShopGoodsTemplate != null)
            {
                _battleShopGoodsTemplate.gameObject.SetActive(false);
            }

            _spawnedEquipmentSlots.Clear();
            if (_equipmentSlotRoot != null)
            {
                HideUnexpectedEquipmentSlots();
                for (var i = 0; i < 6; i++)
                {
                    var slot = FindDescendant(_equipmentSlotRoot, $"m_equipmentSlot_{i}");
                    if (slot != null)
                    {
                        _spawnedEquipmentSlots.Add(slot.gameObject);
                    }
                }
            }

            return _txtPlayerStats != null
                   && _equipmentSlotRoot != null
                   && _spawnedEquipmentSlots.Count >= 6
                   && _battleShopPanel != null
                   && _txtBattleShopTitle != null
                   && _txtBattleShopHint != null
                   && _battleShopGoodsRoot != null
                   && _battleShopGoodsTemplate != null;
        }

        private void HideUnexpectedEquipmentSlots()
        {
            for (var i = 0; i < _equipmentSlotRoot.childCount; i++)
            {
                var child = _equipmentSlotRoot.GetChild(i);
                if (!child.name.StartsWith("m_equipmentSlot_", System.StringComparison.Ordinal))
                {
                    continue;
                }

                var keep = false;
                for (var slotIndex = 0; slotIndex < 6; slotIndex++)
                {
                    if (child.name == $"m_equipmentSlot_{slotIndex}")
                    {
                        keep = true;
                        break;
                    }
                }

                if (!keep)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        private void RefreshPlayerInfoHud(Fantasy.BattlePlayerStateInfo me)
        {
            EnsurePlayerInfoHud();
            if (_playerInfoPanel == null)
            {
                return;
            }

            _playerInfoPanel.gameObject.SetActive(me != null);
            if (me == null)
            {
                SetBattleShopVisible(false);
                return;
            }

            SetText(_txtPlayerStats, IsTroll(me)
                ? $"巨魔  攻 {me.Attack}  血 {me.Hp}/{me.MaxHp}  速 {me.MoveSpeed:0.0}  距 {me.AttackRange:0.0}"
                : $"精灵  血 {me.Hp}/{me.MaxHp}  金 {me.Gold}  木 {me.Wood}");
            RefreshEquipmentSlots(me);
            if (_battleShopVisible)
            {
                RefreshBattleShopGoods(me);
            }
        }

        private void RefreshEquipmentSlots(Fantasy.BattlePlayerStateInfo me)
        {
            if (_equipmentSlotRoot == null)
            {
                return;
            }

            for (var i = 0; i < 6; i++)
            {
                var instance = GetPrefabSlot(i);
                if (instance == null)
                {
                    continue;
                }

                instance.name = $"m_equipmentSlot_{i}";
                var slot = me.EquipmentSlots?.FirstOrDefault(item => item.SlotIndex == i);
                var label = slot == null || slot.ItemId <= 0 ? "空" : ShortName(slot.ItemName, slot.ItemId);
                SetChildText(instance.transform, "m_txtLabel", label);
            }
        }

        private void OpenBattleShop(int shopId, Fantasy.BattlePlayerStateInfo me)
        {
            if (shopId <= 0)
            {
                _nearbyShopId = 0;
                SetBattleShopVisible(false);
                return;
            }

            SetBuildingActionPanelVisible(false);
            _nearbyShopId = shopId;
            SetBattleShopVisible(true);
            RefreshBattleShopGoods(me ?? GetLocalPlayer());
        }

        private void SetBattleShopVisible(bool visible)
        {
            _battleShopVisible = visible;
            if (_battleShopPanel != null)
            {
                _battleShopPanel.gameObject.SetActive(visible);
            }
        }

        private void RefreshBattleShopGoods(Fantasy.BattlePlayerStateInfo me)
        {
            if (_nearbyShopId <= 0)
            {
                SetText(_txtBattleShopTitle, "局内商店");
                SetText(_txtBattleShopHint, "选择地图商店后可购买装备");
                DeactivateFrom(_spawnedBattleShopGoods, 0);
                return;
            }

            var shop = ConfigSystem.Instance.Tables.TbBattleShop.GetOrDefault(_nearbyShopId);
            SetText(_txtBattleShopTitle, shop?.ShopName ?? $"商店 {_nearbyShopId}");
            var canBuy = CanUseSelectedShop(me, out var distance, out var range);
            SetText(_txtBattleShopHint, canBuy
                ? $"金币 {me.Gold}  木材 {me.Wood}  空格 {me.EquipmentSlots?.Count(slot => slot.ItemId <= 0) ?? 0}"
                : $"距离不足 {distance:0.0}/{range:0.0}  靠近后可购买");
            var goods = ConfigSystem.Instance.Tables.TbBattleShopGoods.DataList
                .Where(item => shop != null && item.GoodsGroupId == shop.GoodsGroupId)
                .OrderBy(item => item.GoodsId)
                .ToList();
            for (var i = 0; i < goods.Count; i++)
            {
                RefreshBattleShopGoodsButton(i, goods[i], canBuy);
            }

            DeactivateFrom(_spawnedBattleShopGoods, goods.Count);
        }

        private void RefreshBattleShopGoodsButton(int index, BattleShopGoodsConfig goods, bool canBuy)
        {
            var instance = GetOrCreateGoodsButton(index);
            if (instance == null)
            {
                return;
            }

            instance.name = $"m_btnBattleGoods_{goods.GoodsId}";
            SetChildText(instance.transform, "m_txtLabel", $"{goods.ItemName}  {goods.Currency}:{goods.Price}\n{goods.EffectDesc}");
            var button = instance.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.interactable = canBuy;
            if (canBuy)
            {
                button.onClick.AddListener(() => BuyBattleGoodsAsync(_nearbyShopId, goods.GoodsId).Coroutine());
            }
        }

        private void EnsureBuildingActionPanel()
        {
            if (_buildingActionPanel != null)
            {
                return;
            }

            var hud = FindChild("m_battleHud") as RectTransform ?? transform as RectTransform;
            if (hud == null)
            {
                return;
            }

            _buildingActionPanel = CreatePanel(hud, "m_buildingActionPanel", new Color(0.94f, 0.96f, 0.94f, 0.98f));
            SetRect(_buildingActionPanel, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 266f), new Vector2(520f, 260f));
            _buildingActionPanel.SetAsLastSibling();

            _txtBuildingActionTitle = CreateText(_buildingActionPanel, "m_txtBuildingActionTitle", "建筑", 24, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black);
            SetRect(_txtBuildingActionTitle.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(118f, -30f), new Vector2(-150f, 44f));

            _txtBuildingActionBody = CreateText(_buildingActionPanel, "m_txtBuildingActionBody", string.Empty, 18, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.12f, 0.12f, 0.12f, 1f));
            SetRect(_txtBuildingActionBody.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(24f, 42f), new Vector2(-24f, -78f));

            _btnBuildingActionClose = CreateButton(_buildingActionPanel, "m_btnBuildingActionClose", "关闭", new Color(0.35f, 0.37f, 0.36f, 1f));
            SetRect(_btnBuildingActionClose.GetComponent<RectTransform>(), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-54f, -30f), new Vector2(84f, 38f));
            _btnBuildingActionClose.onClick.AddListener(() => SetBuildingActionPanelVisible(false));

            _btnBuildingActionConfirm = CreateButton(_buildingActionPanel, "m_btnBuildingActionConfirm", "确认", new Color(0.20f, 0.48f, 0.32f, 0.98f));
            SetRect(_btnBuildingActionConfirm.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 32f), new Vector2(150f, 44f));

            SetBuildingActionPanelVisible(false);
        }

        private void OpenBuildingInfoPanel(Fantasy.BattleBuildingStateInfo building, BuildingConfig config)
        {
            EnsureBuildingActionPanel();
            _pendingBuildingActionInstanceId = building.InstanceId;
            SetBattleShopVisible(false);
            SetText(_txtBuildingActionTitle, "建筑信息");
            SetText(_txtBuildingActionBody, FormatBuildingInfo(building, config, true));
            SetBuildingActionConfirmVisible(false, string.Empty);
            SetBuildingActionPanelVisible(true);
        }

        private void OpenUpgradeBuildingPanel(Fantasy.BattleBuildingStateInfo building, BuildingConfig config)
        {
            EnsureBuildingActionPanel();
            _pendingBuildingActionInstanceId = building.InstanceId;
            SetBattleShopVisible(false);

            var nextLevel = GetNextBuildingLevel(building);
            var costText = nextLevel == null
                ? "已达到最高等级。"
                : $"消耗 金币 {nextLevel.UpgradeCostGold}  木材 {nextLevel.UpgradeCostWood}\n升级后生命 {nextLevel.Hp}  攻击 {nextLevel.Attack}  范围 {nextLevel.AttackRange}";

            SetText(_txtBuildingActionTitle, "升级建筑");
            SetText(_txtBuildingActionBody, $"{FormatBuildingInfo(building, config, true)}\n\n{costText}");
            SetBuildingActionConfirmVisible(nextLevel != null, "确认升级");
            _btnBuildingActionConfirm.onClick.RemoveAllListeners();
            _btnBuildingActionConfirm.onClick.AddListener(() =>
            {
                if (_pendingBuildingActionInstanceId > 0)
                {
                    GameEvent.Get<IBattleCommand>()?.OnUpgradeBuilding(_pendingBuildingActionInstanceId);
                }

                SetBuildingActionPanelVisible(false);
                RefreshBattleHud();
            });
            SetBuildingActionPanelVisible(true);
        }

        private void OpenRecycleBuildingPanel(Fantasy.BattleBuildingStateInfo building, BuildingConfig config)
        {
            EnsureBuildingActionPanel();
            _pendingBuildingActionInstanceId = building.InstanceId;
            SetBattleShopVisible(false);

            var percent = Mathf.Max(config?.RecyclePercent ?? 0, 0);
            SetText(_txtBuildingActionTitle, "拆除建筑");
            SetText(_txtBuildingActionBody, $"{FormatBuildingInfo(building, config, true)}\n\n拆除后返还约 {percent}% 建造资源。");
            SetBuildingActionConfirmVisible(true, "确认拆除");
            _btnBuildingActionConfirm.onClick.RemoveAllListeners();
            _btnBuildingActionConfirm.onClick.AddListener(() =>
            {
                if (_pendingBuildingActionInstanceId > 0)
                {
                    GameEvent.Get<IBattleCommand>()?.OnRecycleBuilding(_pendingBuildingActionInstanceId);
                }

                SetBuildingActionPanelVisible(false);
                RefreshBattleHud();
            });
            SetBuildingActionPanelVisible(true);
        }

        private void SetBuildingActionPanelVisible(bool visible)
        {
            if (_buildingActionPanel != null)
            {
                _buildingActionPanel.gameObject.SetActive(visible);
            }

            if (!visible)
            {
                _pendingBuildingActionInstanceId = 0;
            }
        }

        private void SetBuildingActionConfirmVisible(bool visible, string label)
        {
            if (_btnBuildingActionConfirm == null)
            {
                return;
            }

            _btnBuildingActionConfirm.gameObject.SetActive(visible);
            SetChildText(_btnBuildingActionConfirm.transform, "m_txtLabel", label);
        }

        private async FTask BuyBattleGoodsAsync(int shopId, int goodsId)
        {
            if (BattleController.Instance.CurrentBattle == null)
            {
                return;
            }

            var response = await SheepNetworkService.Instance.BuyBattleShopGoodsAsync(BattleController.Instance.CurrentBattle.BattleId, shopId, goodsId);
            if (response.Success && response.Snapshot != null)
            {
                BattleController.Instance.ApplyExternalSnapshot(response.Snapshot);
                CommonNoticeService.Show(response.Message, "购买成功");
            }
            else
            {
                CommonNoticeService.Show(response.Message, "无法购买");
            }

            RefreshBattleHud();
        }

        private void RefreshOperationButton(int index, string label, UnityEngine.Events.UnityAction onClick)
        {
            var instance = GetOrCreatePooled(_spawnedOperationButtons, index, _buildingOperationButtonTemplate.gameObject, _buildingOperationButtonRoot);
            instance.name = $"m_btnOperation_{label}";
            instance.SetActive(true);
            SetChildText(instance.transform, "m_txtLabel", label);
            var button = instance.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                onClick?.Invoke();
                BattleController.Instance.ClearSelectedBuilding();
                RefreshBattleHud();
            });
        }

        private static bool CanUpgrade(Fantasy.BattleBuildingStateInfo selected, BuildingConfig config, bool isOwner)
        {
            if (!isOwner || config == null || !config.CanUpgrade || selected.Level >= config.MaxLevel)
            {
                return false;
            }

            return ConfigSystem.Instance.Tables.TbBuildingLevel.DataList.Any(level =>
                level.BuildingId == selected.BuildingId &&
                level.Level == selected.Level &&
                level.NextLevelId > 0);
        }

        private static BuildingLevelConfig GetNextBuildingLevel(Fantasy.BattleBuildingStateInfo selected)
        {
            if (selected == null)
            {
                return null;
            }

            var currentLevel = ConfigSystem.Instance.Tables.TbBuildingLevel.DataList.FirstOrDefault(level =>
                level.BuildingId == selected.BuildingId &&
                level.Level == selected.Level);
            if (currentLevel?.NextLevelId <= 0)
            {
                return null;
            }

            return ConfigSystem.Instance.Tables.TbBuildingLevel.GetOrDefault(currentLevel.NextLevelId);
        }

        private static string FormatBuildingInfo(Fantasy.BattleBuildingStateInfo selected, BuildingConfig config, bool includeEffect)
        {
            var shopId = GetShopId(selected);
            var shop = shopId <= 0 ? null : ConfigSystem.Instance.Tables.TbBattleShop.GetOrDefault(shopId);
            var name = shop?.ShopName ?? config?.BuildingName ?? $"建筑 {selected.BuildingId}";
            var baseText = $"{name}  Lv.{selected.Level}  HP {selected.Hp}/{selected.MaxHp}";
            if (!includeEffect)
            {
                return baseText;
            }

            var effect = shop != null
                ? (string.IsNullOrWhiteSpace(shop.EffectDesc) ? shop.ShopType : shop.EffectDesc)
                : config == null ? string.Empty : (string.IsNullOrWhiteSpace(config.EffectDesc) ? config.BuildingType : config.EffectDesc);
            return string.IsNullOrWhiteSpace(effect) ? baseText : $"{baseText}  {effect}";
        }

        protected override void OnDestroy()
        {
            ClearSpawned(_spawnedAvatars);
            ClearSpawned(_spawnedBuildCards);
            ClearSpawned(_spawnedOperationButtons);
            ClearSpawned(_spawnedBattleShopGoods);
            _spawnedEquipmentSlots.Clear();
            Log.Info("战斗界面关闭");
        }

        private Fantasy.BattlePlayerStateInfo GetLocalPlayer()
        {
            var snapshot = BattleController.Instance.CurrentSnapshot;
            var profile = SheepNetworkService.Instance.Profile;
            return snapshot?.Players.FirstOrDefault(item => item.PlayerId == profile?.PlayerId);
        }

        private static void ClearSpawned(List<GameObject> spawned)
        {
            for (var i = spawned.Count - 1; i >= 0; i--)
            {
                if (spawned[i] != null)
                {
                    Object.Destroy(spawned[i]);
                }
            }

            spawned.Clear();
        }

        private static GameObject GetOrCreatePooled(List<GameObject> pool, int index, GameObject template, Transform parent)
        {
            while (pool.Count <= index)
            {
                var item = Object.Instantiate(template, parent, false);
                item.SetActive(false);
                pool.Add(item);
            }

            return pool[index];
        }

        private static void DeactivateFrom(List<GameObject> pool, int startIndex)
        {
            for (var i = startIndex; i < pool.Count; i++)
            {
                if (pool[i] != null)
                {
                    pool[i].SetActive(false);
                }
            }
        }

        private static bool IsElf(Fantasy.BattlePlayerStateInfo player)
        {
            return SameCamp(player?.Camp, "Elf");
        }

        private static bool IsTroll(Fantasy.BattlePlayerStateInfo player)
        {
            return SameCamp(player?.Camp, "Troll");
        }

        private static bool SameCamp(string left, string right)
        {
            return string.Equals(left, right, System.StringComparison.OrdinalIgnoreCase);
        }

        private static string FormatTime(int seconds)
        {
            return $"{seconds / 60:00}:{seconds % 60:00}";
        }

        private static string GetAvatarInitial(Fantasy.BattlePlayerStateInfo player)
        {
            if (!string.IsNullOrWhiteSpace(player?.Nickname))
            {
                return player.Nickname.Substring(0, 1);
            }

            return player == null ? "?" : (player.PlayerId % 10).ToString();
        }

        private static int GetShopId(Fantasy.BattleBuildingStateInfo building)
        {
            if (building == null || string.IsNullOrWhiteSpace(building.State))
            {
                return 0;
            }

            const string prefix = "Shop:";
            var start = building.State.IndexOf(prefix, System.StringComparison.OrdinalIgnoreCase);
            if (start < 0)
            {
                return 0;
            }

            start += prefix.Length;
            var end = building.State.IndexOf(';', start);
            var value = end < 0 ? building.State[start..] : building.State[start..end];
            return int.TryParse(value, out var shopId) ? shopId : 0;
        }

        private bool CanUseSelectedShop(Fantasy.BattlePlayerStateInfo me, out float distance, out float range)
        {
            distance = float.MaxValue;
            range = 0f;
            var building = BattleController.Instance.CurrentSnapshot?.Buildings
                .FirstOrDefault(item => GetShopId(item) == _nearbyShopId);
            if (me == null || building == null)
            {
                return false;
            }

            range = Mathf.Max(ParseShopRange(building.State), 1.5f);
            var config = ConfigSystem.Instance.Tables.TbBuilding.GetOrDefault(building.BuildingId);
            var centerX = building.GridX + Mathf.Max(config?.FootprintWidth ?? 1, 1) * 0.5f;
            var centerY = building.GridY + Mathf.Max(config?.FootprintHeight ?? 1, 1) * 0.5f;
            distance = Vector2.Distance(new Vector2(me.PosX, me.PosY), new Vector2(centerX, centerY));
            return distance <= range;
        }

        private static float ParseShopRange(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
            {
                return 0f;
            }

            const string prefix = "Range:";
            var start = state.IndexOf(prefix, System.StringComparison.OrdinalIgnoreCase);
            if (start < 0)
            {
                return 0f;
            }

            start += prefix.Length;
            var end = state.IndexOf(';', start);
            var value = end < 0 ? state[start..] : state[start..end];
            return float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var range)
                ? range
                : 0f;
        }

        private static string ShortName(string itemName, int itemId)
        {
            if (string.IsNullOrWhiteSpace(itemName))
            {
                return itemId.ToString();
            }

            return itemName.Length <= 3 ? itemName : itemName.Substring(0, 3);
        }

        private GameObject GetOrCreateGoodsButton(int index)
        {
            while (_spawnedBattleShopGoods.Count <= index)
            {
                if (_battleShopGoodsTemplate == null || _battleShopGoodsRoot == null)
                {
                    return null;
                }

                var instance = Object.Instantiate(_battleShopGoodsTemplate.gameObject, _battleShopGoodsRoot, false);
                _spawnedBattleShopGoods.Add(instance);
            }

            var item = _spawnedBattleShopGoods[index];
            item.SetActive(true);
            return item;
        }

        private GameObject GetPrefabSlot(int index)
        {
            if (index < 0 || index >= _spawnedEquipmentSlots.Count)
            {
                return null;
            }

            var item = _spawnedEquipmentSlots[index];
            item.SetActive(true);
            return item;
        }

        private Transform FindHudChild(string name)
        {
            var hud = FindChild("m_battleHud");
            return FindDescendant(hud ?? transform, name);
        }

        private T FindHudComponent<T>(string name) where T : Component
        {
            var child = FindHudChild(name);
            return child == null ? null : child.GetComponent<T>();
        }

        private Text FindResourceValueText(string rowName)
        {
            var row = FindHudChild(rowName);
            var value = FindDescendant(row, "m_txtValue");
            return value == null ? null : value.GetComponent<Text>();
        }

        private static RectTransform CreatePanel(Transform parent, string name, Color color)
        {
            var rect = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            var image = rect.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = color.a > 0f;
            return rect;
        }

        private static Text CreateText(Transform parent, string name, string value, int size, FontStyle style, TextAnchor alignment, Color color)
        {
            var rect = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            var text = rect.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            text.text = value ?? string.Empty;
            return text;
        }

        private static Button CreateButton(Transform parent, string name, string label, Color color)
        {
            var rect = CreatePanel(parent, name, color);
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = rect.GetComponent<Image>();
            var text = CreateText(rect, "m_txtLabel", label, 18, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            SetRect(text.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return button;
        }

        private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private static void SetChildText(Transform root, string childName, string value)
        {
            var child = FindDescendant(root, childName);
            var text = child == null ? null : child.GetComponent<Text>();
            if (text != null)
            {
                text.text = value ?? string.Empty;
            }
        }

        private static Transform FindDescendant(Transform root, string name)
        {
            if (root == null)
            {
                return null;
            }

            for (var i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }

                var match = FindDescendant(child, name);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }
    }
}
