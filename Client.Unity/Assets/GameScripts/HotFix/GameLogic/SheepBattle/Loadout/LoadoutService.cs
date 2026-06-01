using System.Collections.Generic;
using System.Linq;
using GameLogic.SheepBattle.Asset;
using GameLogic.SheepBattle.Config;

namespace GameLogic.SheepBattle.Loadout
{
    /// <summary>
    /// MVP 卡组服务。当前优先使用已解锁建筑卡，后续 LoadoutUI 只需要替换这里的选择结果。
    /// </summary>
    public sealed class LoadoutService
    {
        private const int MaxBuildingCards = 6;

        public static LoadoutService Instance { get; } = new LoadoutService();

        private readonly List<int> selectedBuildingCardIds = new();

        private LoadoutService()
        {
        }

        public IReadOnlyList<int> GetSelectedBuildingCardIds()
        {
            EnsureSelectedBuildingCards();
            return selectedBuildingCardIds;
        }

        private void EnsureSelectedBuildingCards()
        {
            var unlockedCardIds = AssetController.Instance.Model.UnlockedBuildingCardIds;
            if (unlockedCardIds.Count <= 0 && selectedBuildingCardIds.Count > 0)
            {
                return;
            }

            selectedBuildingCardIds.Clear();
            selectedBuildingCardIds.AddRange(ConfigSystem.Instance.Tables.TbBuildingCard.DataList
                .Where(card => unlockedCardIds.Count <= 0 || unlockedCardIds.Contains(card.CardId))
                .OrderBy(card => card.SortOrder)
                .ThenBy(card => card.CardId)
                .Take(MaxBuildingCards)
                .Select(card => card.CardId));
        }
    }
}
