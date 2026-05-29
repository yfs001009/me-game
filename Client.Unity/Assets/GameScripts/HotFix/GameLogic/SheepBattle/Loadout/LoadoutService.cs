using System.Collections.Generic;
using System.Linq;
using GameLogic.SheepBattle.Config;

namespace GameLogic.SheepBattle.Loadout
{
    /// <summary>
    /// MVP 卡组服务。当前使用默认前 6 张建筑卡，后续 LoadoutUI 只需要替换这里的选择结果。
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
            EnsureDefaultBuildingCards();
            return selectedBuildingCardIds;
        }

        private void EnsureDefaultBuildingCards()
        {
            if (selectedBuildingCardIds.Count > 0)
            {
                return;
            }

            selectedBuildingCardIds.AddRange(ConfigSystem.Instance.Tables.TbBuildingCard.DataList
                .OrderBy(card => card.SortOrder)
                .ThenBy(card => card.CardId)
                .Take(MaxBuildingCards)
                .Select(card => card.CardId));
        }
    }
}
