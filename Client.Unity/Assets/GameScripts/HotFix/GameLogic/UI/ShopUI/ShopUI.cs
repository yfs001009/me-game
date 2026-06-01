using System.Collections.Generic;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Reward;
using GameLogic.SheepBattle.Shop;
using GameLogic.SheepBattle.Task;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "ShopUI")]
    internal sealed class ShopUI : UIWindow
    {
        private Button _btnClose;
        private Button _btnRefresh;
        private Button _btnShopTab;
        private Button _btnTaskTab;
        private RectTransform _shopPanel;
        private RectTransform _taskPanel;
        private RectTransform _goodsRoot;
        private Button _goodsTemplate;
        private Text _txtGoodsEmpty;
        private Text _txtGoodsName;
        private Text _txtGoodsDesc;
        private Text _txtGoodsReward;
        private Text _txtGoodsPrice;
        private Button _btnBuy;
        private RectTransform _taskRoot;
        private Button _taskTemplate;
        private Text _txtTaskEmpty;
        private Text _txtTaskTitle;
        private Text _txtTaskDesc;
        private Text _txtTaskProgress;
        private Text _txtTaskReward;
        private Button _btnClaim;

        private readonly List<Button> _goodsItems = new();
        private readonly List<Button> _taskItems = new();
        private ShopGoodsEntryViewModel _currentGoods;
        private TaskEntryViewModel _currentTask;
        private bool _showTask;

        protected override void ScriptGenerator()
        {
            _btnClose = FindChildComponent<Button>("m_imgPanel/m_btnClose");
            _btnRefresh = FindChildComponent<Button>("m_imgPanel/m_btnRefresh");
            _btnShopTab = FindChildComponent<Button>("m_imgPanel/m_tabs/m_btnShopTab");
            _btnTaskTab = FindChildComponent<Button>("m_imgPanel/m_tabs/m_btnTaskTab");
            _shopPanel = FindChild("m_imgPanel/m_shopPanel") as RectTransform;
            _taskPanel = FindChild("m_imgPanel/m_taskPanel") as RectTransform;
            _goodsRoot = FindListContent("m_imgPanel/m_shopPanel/m_listGoods");
            _goodsTemplate = FindListComponent<Button>("m_imgPanel/m_shopPanel/m_listGoods", "m_btnGoodsTemplate");
            _txtGoodsEmpty = FindChildComponent<Text>("m_imgPanel/m_shopPanel/m_listGoods/m_txtEmpty");
            _txtGoodsName = FindChildComponent<Text>("m_imgPanel/m_shopPanel/m_detailPanel/m_txtName");
            _txtGoodsDesc = FindChildComponent<Text>("m_imgPanel/m_shopPanel/m_detailPanel/m_txtDescription");
            _txtGoodsReward = FindChildComponent<Text>("m_imgPanel/m_shopPanel/m_detailPanel/m_txtReward");
            _txtGoodsPrice = FindChildComponent<Text>("m_imgPanel/m_shopPanel/m_detailPanel/m_txtPrice");
            _btnBuy = FindChildComponent<Button>("m_imgPanel/m_shopPanel/m_detailPanel/m_btnBuy");
            _taskRoot = FindListContent("m_imgPanel/m_taskPanel/m_listTasks");
            _taskTemplate = FindListComponent<Button>("m_imgPanel/m_taskPanel/m_listTasks", "m_btnTaskTemplate");
            _txtTaskEmpty = FindChildComponent<Text>("m_imgPanel/m_taskPanel/m_listTasks/m_txtEmpty");
            _txtTaskTitle = FindChildComponent<Text>("m_imgPanel/m_taskPanel/m_detailPanel/m_txtTitle");
            _txtTaskDesc = FindChildComponent<Text>("m_imgPanel/m_taskPanel/m_detailPanel/m_txtDescription");
            _txtTaskProgress = FindChildComponent<Text>("m_imgPanel/m_taskPanel/m_detailPanel/m_txtProgress");
            _txtTaskReward = FindChildComponent<Text>("m_imgPanel/m_taskPanel/m_detailPanel/m_txtReward");
            _btnClaim = FindChildComponent<Button>("m_imgPanel/m_taskPanel/m_detailPanel/m_btnClaim");
        }

        protected override void RegisterEvent()
        {
            AddUIEvent<ShopViewChangedEvent>(OnShopViewChanged);
            AddUIEvent<TaskViewChangedEvent>(OnTaskViewChanged);
        }

        protected override void OnCreate()
        {
            SetActive(_goodsTemplate, false);
            SetActive(_taskTemplate, false);
            _btnClose?.onClick.AddListener(() => GameModule.UI.CloseUI<ShopUI>());
            _btnRefresh?.onClick.AddListener(RefreshCurrent);
            _btnShopTab?.onClick.AddListener(() => SwitchTab(false));
            _btnTaskTab?.onClick.AddListener(() => SwitchTab(true));
            _btnBuy?.onClick.AddListener(OnClickBuy);
            _btnClaim?.onClick.AddListener(OnClickClaim);
        }

        protected override void OnRefresh()
        {
            SwitchTab(false);
            ShopController.Instance.RefreshAsync().Coroutine();
            TaskController.Instance.RefreshAsync().Coroutine();
            RefreshGoodsList();
            RefreshTaskList();
        }

        private void OnShopViewChanged(ShopViewChangedEvent eventData)
        {
            RefreshGoodsList();
        }

        private void OnTaskViewChanged(TaskViewChangedEvent eventData)
        {
            RefreshTaskList();
        }

        private void SwitchTab(bool showTask)
        {
            _showTask = showTask;
            SetActive(_shopPanel, !showTask);
            SetActive(_taskPanel, showTask);
            SetTabColor(_btnShopTab, !showTask);
            SetTabColor(_btnTaskTab, showTask);
        }

        private void RefreshCurrent()
        {
            if (_showTask)
            {
                TaskController.Instance.RefreshAsync(TaskController.Instance.Model.TaskType, TaskController.Instance.Model.ActivityId, TaskController.Instance.Model.FeatureId).Coroutine();
                return;
            }

            ShopController.Instance.RefreshAsync(ShopController.Instance.Model.ShopType, ShopController.Instance.Model.ActivityId, ShopController.Instance.Model.FeatureId).Coroutine();
        }

        private void RefreshGoodsList()
        {
            var goods = ShopController.Instance.Model.Goods;
            SetActive(_txtGoodsEmpty, goods.Count == 0);
            for (var i = 0; i < goods.Count; i++)
            {
                RefreshGoodsItem(i, goods[i]);
            }

            for (var i = goods.Count; i < _goodsItems.Count; i++)
            {
                SetActive(_goodsItems[i], false);
            }

            _currentGoods = goods.Count > 0 ? goods[0] : null;
            RefreshGoodsDetail();
        }

        private void RefreshGoodsItem(int index, ShopGoodsEntryViewModel goods)
        {
            var button = GetOrCreateGoodsItem(index);
            if (button == null)
            {
                return;
            }

            SetActive(button, true);
            button.name = $"m_btnGoods_{goods.GoodsId}";
            var label = button.GetComponentInChildren<Text>();
            SetText(label, $"{goods.Name}\n{CurrencyName(goods.PriceCurrencyId)} x{goods.PriceAmount}  限购 {goods.LimitText}");
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                _currentGoods = goods;
                RefreshGoodsDetail();
            });
        }

        private void RefreshGoodsDetail()
        {
            SetText(_txtGoodsName, _currentGoods?.Name ?? string.Empty);
            SetText(_txtGoodsDesc, _currentGoods == null ? string.Empty : $"{_currentGoods.ShopName}\n{_currentGoods.Description}");
            SetText(_txtGoodsReward, RewardText(_currentGoods?.Reward));
            SetText(_txtGoodsPrice, _currentGoods == null ? string.Empty : $"价格：{CurrencyName(_currentGoods.PriceCurrencyId)} x{_currentGoods.PriceAmount}\n限购：{_currentGoods.LimitText}");
            if (_btnBuy != null)
            {
                _btnBuy.interactable = _currentGoods != null && _currentGoods.IsAvailable && !_currentGoods.SoldOut;
            }
        }

        private Button GetOrCreateGoodsItem(int index)
        {
            if (_goodsTemplate == null || _goodsRoot == null)
            {
                return null;
            }

            while (_goodsItems.Count <= index)
            {
                var instance = Object.Instantiate(_goodsTemplate.gameObject, _goodsRoot, false);
                _goodsItems.Add(instance.GetComponent<Button>());
            }

            return _goodsItems[index];
        }

        private void RefreshTaskList()
        {
            var tasks = TaskController.Instance.Model.Tasks;
            SetActive(_txtTaskEmpty, tasks.Count == 0);
            for (var i = 0; i < tasks.Count; i++)
            {
                RefreshTaskItem(i, tasks[i]);
            }

            for (var i = tasks.Count; i < _taskItems.Count; i++)
            {
                SetActive(_taskItems[i], false);
            }

            _currentTask = tasks.Count > 0 ? tasks[0] : null;
            RefreshTaskDetail();
        }

        private void RefreshTaskItem(int index, TaskEntryViewModel task)
        {
            var button = GetOrCreateTaskItem(index);
            if (button == null)
            {
                return;
            }

            SetActive(button, true);
            button.name = $"m_btnTask_{task.TaskId}";
            var state = task.IsClaimed ? "已领取" : task.IsComplete ? "可领取" : "进行中";
            SetText(button.GetComponentInChildren<Text>(), $"{task.Title}\n{task.ProgressText}  {state}");
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                _currentTask = task;
                RefreshTaskDetail();
            });
        }

        private void RefreshTaskDetail()
        {
            SetText(_txtTaskTitle, _currentTask?.Title ?? string.Empty);
            SetText(_txtTaskDesc, _currentTask?.Description ?? string.Empty);
            SetText(_txtTaskProgress, _currentTask == null ? string.Empty : $"进度：{_currentTask.ProgressText}\n类型：{_currentTask.TaskType}");
            SetText(_txtTaskReward, RewardText(_currentTask?.Reward));
            if (_btnClaim != null)
            {
                _btnClaim.interactable = _currentTask != null && _currentTask.IsComplete && !_currentTask.IsClaimed;
            }
        }

        private Button GetOrCreateTaskItem(int index)
        {
            if (_taskTemplate == null || _taskRoot == null)
            {
                return null;
            }

            while (_taskItems.Count <= index)
            {
                var instance = Object.Instantiate(_taskTemplate.gameObject, _taskRoot, false);
                _taskItems.Add(instance.GetComponent<Button>());
            }

            return _taskItems[index];
        }

        private void OnClickBuy()
        {
            if (_currentGoods != null)
            {
                ShopController.Instance.BuyAsync(_currentGoods.GoodsId).Coroutine();
            }
        }

        private void OnClickClaim()
        {
            if (_currentTask != null)
            {
                TaskController.Instance.ClaimAsync(_currentTask.TaskId).Coroutine();
            }
        }

        private static string RewardText(Fantasy.RewardInfo reward)
        {
            var items = RewardDisplayService.FromReward("奖励", reward).Items;
            if (items.Count == 0)
            {
                return "奖励：无";
            }

            var lines = new List<string>();
            for (var i = 0; i < items.Count; i++)
            {
                lines.Add($"{items[i].Name} x{items[i].Count}");
            }

            return $"奖励：\n{string.Join("\n", lines)}";
        }

        private static string CurrencyName(int currencyId)
        {
            return currencyId switch
            {
                2 => "钻石",
                3 => "活动币",
                _ => "金币"
            };
        }

        private static void SetTabColor(Button button, bool selected)
        {
            var image = button?.GetComponent<Image>();
            if (image != null)
            {
                image.color = selected ? new Color(0.22f, 0.44f, 0.78f, 1f) : new Color(0.30f, 0.30f, 0.36f, 1f);
            }
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private static void SetActive(Component component, bool active)
        {
            if (component != null)
            {
                component.gameObject.SetActive(active);
            }
        }

        private RectTransform FindListContent(string listPath)
        {
            return FindChild($"{listPath}/Viewport/Content") as RectTransform
                   ?? FindChild(listPath) as RectTransform;
        }

        private T FindListComponent<T>(string listPath, string name) where T : Component
        {
            return FindChildComponent<T>($"{listPath}/Viewport/Content/{name}")
                   ?? FindChildComponent<T>($"{listPath}/{name}");
        }
    }
}
