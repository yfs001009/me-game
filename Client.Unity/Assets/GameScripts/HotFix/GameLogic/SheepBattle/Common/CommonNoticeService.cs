using TEngine;

namespace GameLogic.SheepBattle.Common
{
    public static class CommonNoticeService
    {
        public static void Show(string message, string title = "提示")
        {
            GameModule.UI.ShowUIAsync<CommonNoticeUI>(new CommonNoticeData(title, message));
        }
    }

    public sealed class CommonNoticeData
    {
        public CommonNoticeData(string title, string message)
        {
            Title = title ?? "提示";
            Message = message ?? string.Empty;
        }

        public string Title { get; }
        public string Message { get; }
    }
}
