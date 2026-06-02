using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class SheepBattleRuntimeUIPrefabBuilder
{
    private const string UiDir = "Assets/AssetRaw/UI";
    private static readonly string[] RequiredPrefabNames =
    {
        "CommonNoticeUI",
        "NicknameUI",
        "MatchQueueUI",
        "RoomListUI",
        "RoomPasswordUI",
        "RegisterUI",
        "CharacterUI",
        "BagItemWidget",
        "BagUI",
        "MailItemWidget",
        "MailUI",
        "LotteryUI",
        "RewardPopupUI",
        "SocialUI",
        "ChatUI",
        "ShopUI",
        "SplashUI",
        "VersionCheckUI",
        "LoadingUI"
    };

    [InitializeOnLoadMethod]
    private static void BuildMissingPrefabsOnEditorLoad()
    {
        EditorApplication.delayCall += () =>
        {
            if (Application.isBatchMode || HasAllRuntimePrefabs())
            {
                return;
            }

            Debug.Log("SheepBattle runtime UI prefabs are missing. Building missing fixed UI prefabs.");
            BuildMissingOnly();
        };
    }

    private static void BuildMissingOnly()
    {
        Directory.CreateDirectory(UiDir);
        if (!File.Exists($"{UiDir}/CommonNoticeUI.prefab")) BuildCommonNoticeUI();
        if (!File.Exists($"{UiDir}/NicknameUI.prefab")) BuildNicknameUI();
        if (!File.Exists($"{UiDir}/MatchQueueUI.prefab")) BuildMatchQueueUI();
        if (!File.Exists($"{UiDir}/RoomListUI.prefab")) BuildRoomListUI();
        if (!File.Exists($"{UiDir}/RoomPasswordUI.prefab")) BuildRoomPasswordUI();
        if (!File.Exists($"{UiDir}/RegisterUI.prefab")) BuildRegisterUI();
        if (!File.Exists($"{UiDir}/CharacterUI.prefab")) BuildCharacterUI();
        if (!File.Exists($"{UiDir}/BagItemWidget.prefab")) BuildBagItemWidget();
        if (!File.Exists($"{UiDir}/BagUI.prefab")) BuildBagUI();
        if (!File.Exists($"{UiDir}/MailItemWidget.prefab")) BuildMailItemWidget();
        if (!File.Exists($"{UiDir}/MailUI.prefab")) BuildMailUI();
        if (!File.Exists($"{UiDir}/LotteryUI.prefab")) BuildLotteryUI();
        if (!File.Exists($"{UiDir}/RewardPopupUI.prefab")) BuildRewardPopupUI();
        if (!File.Exists($"{UiDir}/SocialUI.prefab")) BuildSocialUI();
        if (!File.Exists($"{UiDir}/ChatUI.prefab")) BuildChatUI();
        if (!File.Exists($"{UiDir}/ShopUI.prefab")) BuildShopUI();
        if (!File.Exists($"{UiDir}/SplashUI.prefab")) BuildSplashUI();
        if (!File.Exists($"{UiDir}/VersionCheckUI.prefab")) BuildVersionCheckUI();
        if (!File.Exists($"{UiDir}/LoadingUI.prefab")) BuildLoadingUI();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("SheepBattle/Build Runtime UI Prefabs")]
    public static void BuildAll()
    {
        Directory.CreateDirectory(UiDir);
        BuildCommonNoticeUI();
        BuildNicknameUI();
        BuildMatchQueueUI();
        BuildRoomListUI();
        BuildRoomPasswordUI();
        BuildRegisterUI();
        BuildCharacterUI();
        BuildBagItemWidget();
        BuildBagUI();
        BuildMailItemWidget();
        BuildMailUI();
        BuildLotteryUI();
        BuildRewardPopupUI();
        BuildSocialUI();
        BuildChatUI();
        BuildShopUI();
        BuildSplashUI();
        BuildVersionCheckUI();
        BuildLoadingUI();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void BuildAllAndExit()
    {
        BuildAll();
        EditorApplication.Exit(0);
    }

    public static void BuildCommonNoticeUI()
    {
        var root = CreateRoot("CommonNoticeUI");
        AddMask(root.transform, 0.55f);
        var panel = CreatePanel(root.transform, 560f, 300f);
        Center(CreateText("m_txtTitle", panel, "提示", 30, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black).rectTransform, 0f, 106f, 480f, 58f);
        Center(CreateText("m_txtMessage", panel, string.Empty, 24, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.12f, 0.12f, 0.12f, 1f)).rectTransform, 0f, 16f, 464f, 118f);
        Center(CreateButton("m_btnConfirm", panel, "确定", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), 0f, -104f, 180f, 54f);
        Save(root, "CommonNoticeUI");
    }

    public static void BuildNicknameUI()
    {
        var root = CreateRoot("NicknameUI");
        AddMask(root.transform, 0.62f);
        var panel = CreatePanel(root.transform, 560f, 330f);
        Center(CreateText("m_txtTitle", panel, "取一个名字", 32, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black).rectTransform, 0f, 116f, 480f, 64f);
        Center(CreateText("m_txtTip", panel, "首次进入大厅前需要设置昵称", 22, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.2f, 0.2f, 0.2f, 1f)).rectTransform, 0f, 74f, 480f, 42f);
        Center(CreateInput("m_inputNickname", panel, "请输入昵称", string.Empty, 12, false).GetComponent<RectTransform>(), 0f, 12f, 420f, 58f);
        Center(CreateButton("m_btnConfirm", panel, "进入大厅", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), 0f, -104f, 200f, 58f);
        Save(root, "NicknameUI");
    }

    public static void BuildMatchQueueUI()
    {
        var root = CreateRoot("MatchQueueUI");
        AddMask(root.transform, 0.55f);
        var panel = CreatePanel(root.transform, 600f, 360f);
        Center(CreateText("m_txtTitle", panel, "匹配队列", 30, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black).rectTransform, 0f, 112f, 520f, 58f);
        Center(CreateText("m_txtStatus", panel, string.Empty, 24, FontStyle.Normal, TextAnchor.MiddleCenter, Color.black).rectTransform, 0f, 20f, 520f, 120f);
        Center(CreateButton("m_btnRefresh", panel, "刷新状态", new Color(0.20f, 0.46f, 0.70f, 1f)).GetComponent<RectTransform>(), -96f, -116f, 160f, 54f);
        Center(CreateButton("m_btnClose", panel, "返回大厅", new Color(0.66f, 0.22f, 0.22f, 1f)).GetComponent<RectTransform>(), 96f, -116f, 160f, 54f);
        Save(root, "MatchQueueUI");
    }

    public static void BuildRoomListUI()
    {
        var root = CreateRoot("RoomListUI");
        AddMask(root.transform, 0.55f);
        var panel = CreatePanel(root.transform, 760f, 520f);
        Anchor(CreateText("m_txtTitle", panel, "房间列表", 30, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black).rectTransform, 0f, 1f, 1f, 1f, 32f, -48f, -180f, 48f);
        Anchor(CreateButton("m_btnClose", panel, "关闭", new Color(0.66f, 0.22f, 0.22f, 1f)).GetComponent<RectTransform>(), 1f, 1f, 1f, 1f, -88f, -48f, 120f, 48f);
        Anchor(CreateButton("m_btnRefresh", panel, "刷新", new Color(0.20f, 0.46f, 0.70f, 1f)).GetComponent<RectTransform>(), 1f, 1f, 1f, 1f, -220f, -48f, 120f, 48f);

        var list = new GameObject("m_listRooms", typeof(RectTransform));
        list.layer = LayerMask.NameToLayer("UI");
        list.transform.SetParent(panel, false);
        var listRect = list.GetComponent<RectTransform>();
        listRect.anchorMin = Vector2.zero;
        listRect.anchorMax = Vector2.one;
        listRect.offsetMin = new Vector2(32f, 34f);
        listRect.offsetMax = new Vector2(-32f, -96f);

        var empty = CreateText("m_txtEmpty", list.transform, "暂无可加入房间", 24, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.18f, 0.18f, 0.18f, 1f));
        Stretch(empty.rectTransform);

        var template = CreateButton("m_btnRoomTemplate", list.transform, string.Empty, new Color(0.20f, 0.46f, 0.70f, 1f));
        var templateRect = template.GetComponent<RectTransform>();
        templateRect.anchorMin = new Vector2(0f, 1f);
        templateRect.anchorMax = new Vector2(1f, 1f);
        templateRect.pivot = new Vector2(0.5f, 1f);
        templateRect.sizeDelta = new Vector2(0f, 68f);
        templateRect.anchoredPosition = Vector2.zero;
        template.gameObject.SetActive(false);
        var label = template.GetComponentInChildren<Text>();
        label.alignment = TextAnchor.MiddleLeft;
        label.color = Color.black;
        label.fontStyle = FontStyle.Normal;
        label.rectTransform.offsetMin = new Vector2(18f, 0f);
        label.rectTransform.offsetMax = new Vector2(-18f, 0f);
        Save(root, "RoomListUI");
    }

    public static void BuildRoomPasswordUI()
    {
        var root = CreateRoot("RoomPasswordUI");
        AddMask(root.transform, 0.55f);
        var panel = CreatePanel(root.transform, 520f, 300f);
        Center(CreateText("m_txtTitle", panel, "请输入房间密码", 28, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black).rectTransform, 0f, 96f, 460f, 50f);
        Center(CreateInput("m_inputPassword", panel, "房间密码", string.Empty, 32, false).GetComponent<RectTransform>(), 0f, 24f, 380f, 54f);
        Center(CreateButton("m_btnJoin", panel, "加入", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), -86f, -84f, 150f, 52f);
        Center(CreateButton("m_btnClose", panel, "取消", new Color(0.66f, 0.22f, 0.22f, 1f)).GetComponent<RectTransform>(), 86f, -84f, 150f, 52f);
        Save(root, "RoomPasswordUI");
    }

    public static void BuildRegisterUI()
    {
        var root = CreateRoot("RegisterUI");
        AddMask(root.transform, 0.56f);
        var panel = CreatePanel(root.transform, 560f, 430f);
        Center(CreateText("m_txtTitle", panel, "注册新账号", 32, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black).rectTransform, 0f, 150f, 420f, 56f);
        Center(CreateInput("m_inputAccount", panel, "用户名（3-12位）", string.Empty, 12, false).GetComponent<RectTransform>(), 0f, 74f, 420f, 56f);
        Center(CreateInput("m_inputPassword", panel, "密码（6-20位）", string.Empty, 20, true).GetComponent<RectTransform>(), 0f, 8f, 420f, 56f);
        Center(CreateInput("m_inputConfirm", panel, "确认密码", string.Empty, 20, true).GetComponent<RectTransform>(), 0f, -58f, 420f, 56f);
        Center(CreateButton("m_btnRegister", panel, "注册", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), -96f, -146f, 160f, 54f);
        Center(CreateButton("m_btnBack", panel, "返回", new Color(0.66f, 0.22f, 0.22f, 1f)).GetComponent<RectTransform>(), 96f, -146f, 160f, 54f);
        Save(root, "RegisterUI");
    }

    public static void BuildCharacterUI()
    {
        var root = CreateRoot("CharacterUI");
        AddMask(root.transform, 0.55f);
        var panel = CreatePanel(root.transform, 860f, 560f);
        Anchor(CreateText("m_txtTitle", panel, "角色", 32, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black).rectTransform, 0f, 1f, 1f, 1f, 32f, -48f, -180f, 52f);
        Anchor(CreateButton("m_btnClose", panel, "关闭", new Color(0.66f, 0.22f, 0.22f, 1f)).GetComponent<RectTransform>(), 1f, 1f, 1f, 1f, -88f, -48f, 120f, 48f);

        var tabs = new GameObject("m_tabs", typeof(RectTransform));
        tabs.layer = LayerMask.NameToLayer("UI");
        tabs.transform.SetParent(panel, false);
        Anchor(tabs.GetComponent<RectTransform>(), 0f, 1f, 0f, 1f, 174f, -104f, 284f, 48f);
        Anchor(CreateButton("m_btnHeroTab", tabs.transform, "英雄列表", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), 0f, 0.5f, 0f, 0.5f, 70f, 0f, 132f, 44f);
        Anchor(CreateButton("m_btnGhostTab", tabs.transform, "幽灵列表", new Color(0.30f, 0.30f, 0.36f, 1f)).GetComponent<RectTransform>(), 0f, 0.5f, 0f, 0.5f, 214f, 0f, 132f, 44f);

        var list = new GameObject("m_listCharacters", typeof(RectTransform));
        list.layer = LayerMask.NameToLayer("UI");
        list.transform.SetParent(panel, false);
        Anchor(list.GetComponent<RectTransform>(), 0f, 0f, 0f, 1f, 188f, -316f, 312f, 372f);

        var template = CreateButton("m_btnCharacterTemplate", list.transform, string.Empty, new Color(0.84f, 0.87f, 0.82f, 1f));
        var templateRect = template.GetComponent<RectTransform>();
        templateRect.anchorMin = new Vector2(0f, 1f);
        templateRect.anchorMax = new Vector2(1f, 1f);
        templateRect.pivot = new Vector2(0.5f, 1f);
        templateRect.sizeDelta = new Vector2(0f, 64f);
        templateRect.anchoredPosition = Vector2.zero;
        template.gameObject.SetActive(false);
        var label = template.GetComponentInChildren<Text>();
        label.alignment = TextAnchor.MiddleLeft;
        label.color = Color.black;
        label.fontStyle = FontStyle.Normal;
        label.rectTransform.offsetMin = new Vector2(16f, 0f);
        label.rectTransform.offsetMax = new Vector2(-16f, 0f);

        var detail = CreateImage("m_detailPanel", panel, new Color(0.90f, 0.92f, 0.88f, 1f));
        Anchor(detail.rectTransform, 1f, 0.5f, 1f, 0.5f, -254f, -28f, 430f, 410f);
        Center(CreateText("m_txtName", detail.transform, "-", 30, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black).rectTransform, 0f, 154f, 360f, 54f);
        Center(CreateText("m_txtCategory", detail.transform, string.Empty, 22, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.18f, 0.18f, 0.18f, 1f)).rectTransform, 0f, 110f, 360f, 42f);
        Center(CreateText("m_txtAbility", detail.transform, string.Empty, 22, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.12f, 0.24f, 0.42f, 1f)).rectTransform, 0f, 42f, 360f, 86f);
        Center(CreateText("m_txtDescription", detail.transform, string.Empty, 20, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.12f, 0.12f, 0.12f, 1f)).rectTransform, 0f, -58f, 360f, 120f);
        Center(CreateButton("m_btnSelect", detail.transform, "选择", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), 0f, -162f, 180f, 54f);
        Save(root, "CharacterUI");
    }

    public static void BuildBagUI()
    {
        var root = CreateRoot("BagUI");
        AddMask(root.transform, 0.55f);
        var panel = CreatePanel(root.transform, 860f, 560f);
        Anchor(CreateText("m_txtTitle", panel, "背包", 32, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black).rectTransform, 0f, 1f, 1f, 1f, 32f, -48f, -260f, 52f);
        Anchor(CreateButton("m_btnRefresh", panel, "刷新", new Color(0.20f, 0.46f, 0.70f, 1f)).GetComponent<RectTransform>(), 1f, 1f, 1f, 1f, -220f, -48f, 120f, 48f);
        Anchor(CreateButton("m_btnClose", panel, "关闭", new Color(0.66f, 0.22f, 0.22f, 1f)).GetComponent<RectTransform>(), 1f, 1f, 1f, 1f, -88f, -48f, 120f, 48f);
        Center(CreateText("m_txtEmpty", panel, "背包为空", 26, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.18f, 0.18f, 0.18f, 1f)).rectTransform, 0f, -18f, 420f, 72f);

        CreateGridScrollList(panel, "m_listItems", new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(32f, 58f), new Vector2(328f, -104f), new Vector2(64f, 64f), new Vector2(8f, 8f), 4, out var listRect, out var itemContent);
        var listImage = listRect.gameObject.AddComponent<Image>();
        listImage.color = new Color(0.86f, 0.89f, 0.84f, 1f);
        CreateBagItemTemplate(itemContent);

        var detail = CreateImage("m_detailPanel", panel, new Color(0.90f, 0.92f, 0.88f, 1f));
        Anchor(detail.rectTransform, 1f, 0.5f, 1f, 0.5f, -254f, -28f, 430f, 410f);
        Center(CreateText("m_txtName", detail.transform, string.Empty, 30, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black).rectTransform, 0f, 146f, 360f, 54f);
        Center(CreateText("m_txtDescription", detail.transform, string.Empty, 22, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.12f, 0.12f, 0.12f, 1f)).rectTransform, 0f, 30f, 360f, 190f);
        Center(CreateButton("m_btnUse", detail.transform, "使用", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), 0f, -162f, 180f, 54f);
        Save(root, "BagUI");
    }

    public static void BuildBagItemWidget()
    {
        var root = CreateButton("BagItemWidget", null, string.Empty, new Color(0.78f, 0.82f, 0.76f, 1f));
        var rect = root.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(64f, 64f);
        rect.anchoredPosition = Vector2.zero;

        var label = root.transform.Find("m_txtLabel");
        if (label != null)
        {
            Object.DestroyImmediate(label.gameObject);
        }

        var icon = CreateImage("m_imgIcon", root.transform, Color.white);
        Center(icon.rectTransform, 0f, 2f, 42f, 42f);

        var count = CreateText("m_txtCount", root.transform, string.Empty, 17, FontStyle.Bold, TextAnchor.MiddleRight, Color.white);
        Anchor(count.rectTransform, 1f, 0f, 1f, 0f, -26f, 12f, 48f, 24f);
        Save(root.gameObject, "BagItemWidget");
    }

    public static void BuildMailUI()
    {
        var root = CreateRoot("MailUI");
        AddMask(root.transform, 0.55f);
        var panel = CreatePanel(root.transform, 900f, 580f);
        var titleIcon = CreateImage("m_imgTitleIcon", panel, new Color(0.95f, 0.67f, 0.18f, 1f));
        Anchor(titleIcon.rectTransform, 0f, 1f, 0f, 1f, 54f, -48f, 42f, 42f);
        Anchor(CreateText("m_txtTitle", panel, "邮件", 32, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black).rectTransform, 0f, 1f, 1f, 1f, 90f, -48f, -318f, 52f);
        Anchor(CreateButton("m_btnRefresh", panel, "刷新", new Color(0.20f, 0.46f, 0.70f, 1f)).GetComponent<RectTransform>(), 1f, 1f, 1f, 1f, -220f, -48f, 120f, 48f);
        Anchor(CreateButton("m_btnClose", panel, "关闭", new Color(0.66f, 0.22f, 0.22f, 1f)).GetComponent<RectTransform>(), 1f, 1f, 1f, 1f, -88f, -48f, 120f, 48f);
        Center(CreateText("m_txtEmpty", panel, "暂无邮件", 26, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.18f, 0.18f, 0.18f, 1f)).rectTransform, 0f, -18f, 420f, 72f);

        CreateScrollList(panel, "m_listMails", new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(32f, 58f), new Vector2(352f, -104f), out var listRect, out var mailContent);
        var listImage = listRect.gameObject.AddComponent<Image>();
        listImage.color = new Color(0.86f, 0.89f, 0.84f, 1f);
        CreateMailItemTemplate(mailContent);

        var detail = CreateImage("m_detailPanel", panel, new Color(0.90f, 0.92f, 0.88f, 1f));
        Anchor(detail.rectTransform, 1f, 0.5f, 1f, 0.5f, -266f, -26f, 452f, 430f);
        Center(CreateText("m_txtTitle", detail.transform, string.Empty, 28, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black).rectTransform, 0f, 164f, 384f, 50f);
        Center(CreateText("m_txtContent", detail.transform, string.Empty, 22, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.12f, 0.12f, 0.12f, 1f)).rectTransform, 0f, 52f, 384f, 168f);
        Center(CreateText("m_txtAttachment", detail.transform, string.Empty, 22, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.12f, 0.24f, 0.42f, 1f)).rectTransform, 0f, -48f, 384f, 36f);
        var attachmentList = CreateImage("m_listAttachments", detail.transform, new Color(0.82f, 0.86f, 0.80f, 1f));
        Center(attachmentList.rectTransform, 0f, -102f, 384f, 84f);
        var attachmentTemplate = CreateButton("m_btnAttachmentTemplate", attachmentList.transform, string.Empty, Color.white);
        var attachmentRect = attachmentTemplate.GetComponent<RectTransform>();
        attachmentRect.anchorMin = new Vector2(0f, 0.5f);
        attachmentRect.anchorMax = new Vector2(0f, 0.5f);
        attachmentRect.pivot = new Vector2(0f, 0.5f);
        attachmentRect.sizeDelta = new Vector2(74f, 74f);
        attachmentRect.anchoredPosition = Vector2.zero;
        attachmentTemplate.gameObject.SetActive(false);
        var attachmentLabel = attachmentTemplate.GetComponentInChildren<Text>();
        attachmentLabel.fontSize = 12;
        attachmentLabel.alignment = TextAnchor.MiddleCenter;
        attachmentLabel.color = Color.white;
        attachmentLabel.rectTransform.offsetMin = new Vector2(5f, 6f);
        attachmentLabel.rectTransform.offsetMax = new Vector2(-5f, -6f);
        Center(CreateButton("m_btnRead", detail.transform, "已读", new Color(0.30f, 0.30f, 0.36f, 1f)).GetComponent<RectTransform>(), -96f, -166f, 160f, 54f);
        Center(CreateButton("m_btnClaim", detail.transform, "领取", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), 96f, -166f, 160f, 54f);
        Save(root, "MailUI");
    }

    public static void BuildMailItemWidget()
    {
        var root = CreateButton("MailItemWidget", null, string.Empty, new Color(0.84f, 0.87f, 0.82f, 1f));
        var rect = root.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(0f, 72f);
        rect.anchoredPosition = Vector2.zero;

        var label = root.transform.Find("m_txtLabel");
        if (label != null)
        {
            Object.DestroyImmediate(label.gameObject);
        }

        var title = CreateText("m_txtTitle", root.transform, string.Empty, 20, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black);
        Anchor(title.rectTransform, 0f, 0.5f, 1f, 0.5f, 18f, 10f, -122f, 32f);

        var state = CreateText("m_txtState", root.transform, string.Empty, 16, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.2f, 0.6f, 0.3f, 1f));
        Anchor(state.rectTransform, 0f, 0.5f, 0f, 0.5f, 42f, -18f, 76f, 24f);

        var attachment = CreateImage("m_imgAttachment", root.transform, new Color(0.95f, 0.67f, 0.18f, 1f));
        Anchor(attachment.rectTransform, 1f, 0.5f, 1f, 0.5f, -36f, 0f, 30f, 30f);
        Save(root.gameObject, "MailItemWidget");
    }

    public static void BuildLotteryUI()
    {
        var root = CreateRoot("LotteryUI");
        AddMask(root.transform, 0.55f);
        var panel = CreatePanel(root.transform, 680f, 500f);
        Anchor(CreateText("m_txtTitle", panel, "抽奖", 32, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black).rectTransform, 0f, 1f, 1f, 1f, 32f, -48f, -180f, 52f);
        Anchor(CreateButton("m_btnClose", panel, "关闭", new Color(0.66f, 0.22f, 0.22f, 1f)).GetComponent<RectTransform>(), 1f, 1f, 1f, 1f, -88f, -48f, 120f, 48f);
        Center(CreateButton("m_btnNormalOnce", panel, "普通 1 次", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), -112f, 108f, 200f, 58f);
        Center(CreateButton("m_btnNormalTen", panel, "普通 10 次", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), 112f, 108f, 200f, 58f);
        Center(CreateButton("m_btnPremiumOnce", panel, "高级 1 次", new Color(0.42f, 0.30f, 0.68f, 1f)).GetComponent<RectTransform>(), -112f, 28f, 200f, 58f);
        Center(CreateButton("m_btnPremiumTen", panel, "高级 10 次", new Color(0.42f, 0.30f, 0.68f, 1f)).GetComponent<RectTransform>(), 112f, 28f, 200f, 58f);
        Center(CreateText("m_txtResult", panel, string.Empty, 24, FontStyle.Normal, TextAnchor.UpperCenter, new Color(0.12f, 0.12f, 0.12f, 1f)).rectTransform, 0f, -104f, 560f, 150f);
        Save(root, "LotteryUI");
    }

    public static void BuildRewardPopupUI()
    {
        var root = CreateRoot("RewardPopupUI");
        AddMask(root.transform, 0.60f);
        var panelImage = CreatePanel(root.transform, 760f, 520f);
        var panel = panelImage.gameObject;
        panel.AddComponent<CanvasGroup>();
        Anchor(CreateText("m_txtTitle", panel.transform, "获得奖励", 36, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black).rectTransform, 0f, 1f, 1f, 1f, 0f, -54f, -180f, 60f);
        Anchor(CreateButton("m_btnClose", panel.transform, "确定", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), 0.5f, 0f, 0.5f, 0f, 0f, 48f, 180f, 54f);

        var list = new GameObject("m_listRewards", typeof(RectTransform));
        list.layer = LayerMask.NameToLayer("UI");
        list.transform.SetParent(panel.transform, false);
        Anchor(list.GetComponent<RectTransform>(), 0.5f, 0.5f, 0.5f, 0.5f, 0f, 12f, 620f, 300f);

        var template = CreateButton("m_btnRewardTemplate", list.transform, string.Empty, Color.white);
        var rect = template.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(104f, 124f);
        rect.anchoredPosition = Vector2.zero;
        template.gameObject.SetActive(false);

        var label = template.GetComponentInChildren<Text>();
        label.name = "m_txtLabel";
        label.fontSize = 17;
        label.alignment = TextAnchor.LowerCenter;
        label.color = Color.white;
        label.rectTransform.offsetMin = new Vector2(8f, 10f);
        label.rectTransform.offsetMax = new Vector2(-8f, -44f);

        var quality = CreateText("m_txtQuality", template.transform, string.Empty, 16, FontStyle.Bold, TextAnchor.UpperCenter, Color.white);
        Anchor(quality.rectTransform, 0f, 1f, 1f, 1f, 0f, -12f, 0f, 26f);
        Save(root, "RewardPopupUI");
    }

    public static void BuildSocialUI()
    {
        var root = CreateRoot("SocialUI");
        AddMask(root.transform, 0.55f);
        var panel = CreatePanel(root.transform, 860f, 560f);
        Anchor(CreateText("m_txtTitle", panel, "关注 0  粉丝 0", 32, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black).rectTransform, 0f, 1f, 1f, 1f, 32f, -48f, -250f, 52f);
        Anchor(CreateButton("m_btnClose", panel, "关闭", new Color(0.66f, 0.22f, 0.22f, 1f)).GetComponent<RectTransform>(), 1f, 1f, 1f, 1f, -88f, -48f, 120f, 48f);

        var tabs = new GameObject("m_tabs", typeof(RectTransform));
        tabs.layer = LayerMask.NameToLayer("UI");
        tabs.transform.SetParent(panel, false);
        Anchor(tabs.GetComponent<RectTransform>(), 0f, 1f, 0f, 1f, 188f, -110f, 324f, 48f);
        Anchor(CreateButton("m_btnFollowing", tabs.transform, "关注", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), 0f, 0.5f, 0f, 0.5f, 72f, 0f, 132f, 44f);
        Anchor(CreateButton("m_btnFans", tabs.transform, "粉丝", new Color(0.30f, 0.30f, 0.36f, 1f)).GetComponent<RectTransform>(), 0f, 0.5f, 0f, 0.5f, 216f, 0f, 132f, 44f);

        var search = new GameObject("m_search", typeof(RectTransform));
        search.layer = LayerMask.NameToLayer("UI");
        search.transform.SetParent(panel, false);
        Anchor(search.GetComponent<RectTransform>(), 1f, 1f, 1f, 1f, -270f, -110f, 390f, 48f);
        Anchor(CreateInput("m_inputSearch", search.transform, "玩家ID/昵称/账号", string.Empty, 24, false).GetComponent<RectTransform>(), 0f, 0.5f, 0f, 0.5f, 122f, 0f, 236f, 44f);
        Anchor(CreateButton("m_btnSearch", search.transform, "搜索", new Color(0.20f, 0.46f, 0.70f, 1f)).GetComponent<RectTransform>(), 1f, 0.5f, 1f, 0.5f, -58f, 0f, 112f, 44f);

        var list = new GameObject("m_listPlayers", typeof(RectTransform));
        list.layer = LayerMask.NameToLayer("UI");
        list.transform.SetParent(panel, false);
        Anchor(list.GetComponent<RectTransform>(), 0f, 0f, 1f, 1f, 0f, -320f, -64f, 350f);
        Stretch(CreateText("m_txtEmpty", list.transform, "暂无玩家", 24, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.18f, 0.18f, 0.18f, 1f)).rectTransform);
        CreateListButtonTemplate(list.transform, "m_btnPlayerTemplate");
        Save(root, "SocialUI");
    }

    public static void BuildChatUI()
    {
        var root = CreateRoot("ChatUI");
        var panel = CreateImage("m_drawerPanel", root.transform, new Color(0.94f, 0.96f, 0.92f, 0.98f));
        var panelRect = panel.rectTransform;
        panelRect.anchorMin = new Vector2(0f, 0f);
        panelRect.anchorMax = new Vector2(0.34f, 1f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        var tabs = new GameObject("m_tabs", typeof(RectTransform));
        tabs.layer = LayerMask.NameToLayer("UI");
        tabs.transform.SetParent(panel.transform, false);
        Anchor(CreateText("m_txtTitle", panel.transform, "聊天", 26, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black).rectTransform, 0f, 1f, 1f, 1f, 16f, -30f, -86f, 44f);
        Anchor(CreateButton("m_btnCollapse", panel.transform, "收", new Color(0.12f, 0.18f, 0.22f, 0.92f)).GetComponent<RectTransform>(), 1f, 0.5f, 1f, 0.5f, 24f, 0f, 48f, 92f);

        Anchor(tabs.GetComponent<RectTransform>(), 0f, 1f, 1f, 1f, 16f, -78f, -16f, 42f);
        Anchor(CreateButton("m_btnComposite", tabs.transform, "综合", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), 0f, 0.5f, 0.5f, 0.5f, 0f, 0f, -4f, 38f);
        Anchor(CreateButton("m_btnPrivate", tabs.transform, "私聊", new Color(0.30f, 0.30f, 0.36f, 1f)).GetComponent<RectTransform>(), 0.5f, 0.5f, 1f, 0.5f, 4f, 0f, -4f, 38f);

        CreateScrollList(panel.transform, "m_listContacts", new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(16f, 92f), new Vector2(136f, -112f), out _, out var contactContent);
        var contactTemplate = CreateText("m_txtContactTemplate", contactContent, string.Empty, 18, FontStyle.Normal, TextAnchor.MiddleLeft, Color.black);
        contactTemplate.gameObject.AddComponent<LayoutElement>().preferredHeight = 34f;
        contactTemplate.gameObject.SetActive(false);

        var hint = CreateText("m_txtPrivateHint", panel.transform, "暂无私聊对象", 18, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.32f, 0.32f, 0.32f, 1f));
        Anchor(hint.rectTransform, 0f, 0.5f, 0f, 0.5f, 76f, 0f, 112f, 60f);

        CreateScrollList(panel.transform, "m_listMessages", new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(16f, 92f), new Vector2(-16f, -112f), out var list, out var content);
        var template = CreateText("m_txtMessageTemplate", content, string.Empty, 18, FontStyle.Normal, TextAnchor.UpperLeft, Color.black);
        template.horizontalOverflow = HorizontalWrapMode.Wrap;
        template.verticalOverflow = VerticalWrapMode.Overflow;
        template.gameObject.AddComponent<LayoutElement>().preferredHeight = 34f;
        template.gameObject.SetActive(false);
        Stretch(CreateText("m_txtEmpty", list, "暂无聊天消息", 20, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.18f, 0.18f, 0.18f, 1f)).rectTransform);

        Anchor(CreateText("m_txtTargetLabel", panel.transform, "目标ID", 18, FontStyle.Normal, TextAnchor.MiddleLeft, Color.black).rectTransform, 0f, 0f, 0f, 0f, 56f, 62f, 76f, 34f);
        Anchor(CreateInput("m_inputTarget", panel.transform, "玩家ID", string.Empty, 18, false).GetComponent<RectTransform>(), 0f, 0f, 0f, 0f, 160f, 62f, 176f, 36f);
        Anchor(CreateInput("m_inputMessage", panel.transform, "输入聊天内容", string.Empty, 120, false).GetComponent<RectTransform>(), 0f, 0f, 1f, 0f, -82f, 24f, -148f, 40f);
        Anchor(CreateButton("m_btnSend", panel.transform, "发送", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), 1f, 0f, 1f, 0f, -72f, 24f, 104f, 40f);
        Save(root, "ChatUI");
    }

    public static void BuildShopUI()
    {
        var root = CreateRoot("ShopUI");
        AddMask(root.transform, 0.55f);
        var panel = CreatePanel(root.transform, 920f, 600f);
        Anchor(CreateText("m_txtTitle", panel, "商店与任务", 32, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black).rectTransform, 0f, 1f, 1f, 1f, 32f, -48f, -300f, 52f);
        Anchor(CreateButton("m_btnRefresh", panel, "刷新", new Color(0.20f, 0.46f, 0.70f, 1f)).GetComponent<RectTransform>(), 1f, 1f, 1f, 1f, -220f, -48f, 120f, 48f);
        Anchor(CreateButton("m_btnClose", panel, "关闭", new Color(0.66f, 0.22f, 0.22f, 1f)).GetComponent<RectTransform>(), 1f, 1f, 1f, 1f, -88f, -48f, 120f, 48f);

        var tabs = new GameObject("m_tabs", typeof(RectTransform));
        tabs.layer = LayerMask.NameToLayer("UI");
        tabs.transform.SetParent(panel, false);
        Anchor(tabs.GetComponent<RectTransform>(), 0f, 1f, 0f, 1f, 188f, -108f, 304f, 48f);
        Anchor(CreateButton("m_btnShopTab", tabs.transform, "商店", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), 0f, 0.5f, 0f, 0.5f, 70f, 0f, 132f, 44f);
        Anchor(CreateButton("m_btnTaskTab", tabs.transform, "任务", new Color(0.30f, 0.30f, 0.36f, 1f)).GetComponent<RectTransform>(), 0f, 0.5f, 0f, 0.5f, 214f, 0f, 132f, 44f);

        var shopPanel = CreateModePanel(panel, "m_shopPanel");
        var goodsList = CreatePlainList(shopPanel, "m_listGoods", "暂无商品", new Vector2(28f, 28f), new Vector2(352f, -136f));
        CreateListButtonTemplate(goodsList, "m_btnGoodsTemplate");
        var goodsDetail = CreateDetailPanel(shopPanel, "m_detailPanel");
        Center(CreateText("m_txtName", goodsDetail, string.Empty, 30, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black).rectTransform, 0f, 164f, 384f, 50f);
        Center(CreateText("m_txtDescription", goodsDetail, string.Empty, 22, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.12f, 0.12f, 0.12f, 1f)).rectTransform, 0f, 78f, 384f, 104f);
        Center(CreateText("m_txtReward", goodsDetail, string.Empty, 21, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.12f, 0.24f, 0.42f, 1f)).rectTransform, 0f, -24f, 384f, 92f);
        Center(CreateText("m_txtPrice", goodsDetail, string.Empty, 22, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.30f, 0.22f, 0.08f, 1f)).rectTransform, 0f, -104f, 384f, 70f);
        Center(CreateButton("m_btnBuy", goodsDetail, "购买", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), 0f, -176f, 180f, 54f);

        var taskPanel = CreateModePanel(panel, "m_taskPanel");
        var taskList = CreatePlainList(taskPanel, "m_listTasks", "暂无任务", new Vector2(28f, 28f), new Vector2(352f, -136f));
        CreateListButtonTemplate(taskList, "m_btnTaskTemplate");
        var taskDetail = CreateDetailPanel(taskPanel, "m_detailPanel");
        Center(CreateText("m_txtTitle", taskDetail, string.Empty, 30, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black).rectTransform, 0f, 164f, 384f, 50f);
        Center(CreateText("m_txtDescription", taskDetail, string.Empty, 22, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.12f, 0.12f, 0.12f, 1f)).rectTransform, 0f, 82f, 384f, 98f);
        Center(CreateText("m_txtProgress", taskDetail, string.Empty, 22, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.30f, 0.22f, 0.08f, 1f)).rectTransform, 0f, 0f, 384f, 58f);
        Center(CreateText("m_txtReward", taskDetail, string.Empty, 21, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.12f, 0.24f, 0.42f, 1f)).rectTransform, 0f, -84f, 384f, 104f);
        Center(CreateButton("m_btnClaim", taskDetail, "领取", new Color(0.22f, 0.44f, 0.78f, 1f)).GetComponent<RectTransform>(), 0f, -176f, 180f, 54f);
        Save(root, "ShopUI");
    }

    public static void BuildSplashUI()
    {
        var root = CreateRoot("SplashUI");
        Stretch(CreateImage("m_imgBg", root.transform, new Color(0.08f, 0.12f, 0.13f, 1f)).rectTransform);
        Center(CreateText("m_txtLogo", root.transform, "SheepBattle", 56, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white).rectTransform, 0f, 48f, 560f, 90f);
        Center(CreateText("m_txtVersion", root.transform, "v0.1.0", 22, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.74f, 0.82f, 0.78f, 1f)).rectTransform, 0f, -48f, 260f, 40f);
        Save(root, "SplashUI");
    }

    public static void BuildVersionCheckUI()
    {
        var root = CreateRoot("VersionCheckUI");
        Stretch(CreateImage("m_imgBg", root.transform, new Color(0.09f, 0.12f, 0.13f, 1f)).rectTransform);
        Center(CreateText("m_txtTitle", root.transform, "版本检查", 36, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white).rectTransform, 0f, 84f, 360f, 64f);
        Center(CreateText("m_txtStatus", root.transform, "正在检查资源版本...", 24, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.78f, 0.86f, 0.80f, 1f)).rectTransform, 0f, 20f, 620f, 56f);
        Center(CreateText("m_txtVersion", root.transform, "当前版本：v0.1.0", 20, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.60f, 0.68f, 0.64f, 1f)).rectTransform, 0f, -34f, 360f, 40f);
        var update = CreateButton("m_btnUpdate", root.transform, "前往更新", new Color(0.20f, 0.48f, 0.36f, 1f));
        Center(update.GetComponent<RectTransform>(), 0f, -108f, 260f, 56f);
        update.gameObject.SetActive(false);
        Save(root, "VersionCheckUI");
    }

    public static void BuildLoadingUI()
    {
        var root = CreateRoot("LoadingUI");
        Stretch(CreateImage("m_imgBg", root.transform, new Color(0.10f, 0.13f, 0.12f, 1f)).rectTransform);
        Center(CreateText("m_txtTitle", root.transform, "加载中", 34, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white).rectTransform, 0f, 72f, 360f, 60f);
        Center(CreateText("m_txtTip", root.transform, string.Empty, 22, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.77f, 0.84f, 0.78f, 1f)).rectTransform, 0f, 14f, 620f, 48f);
        var track = CreateImage("m_imgTrack", root.transform, new Color(0.24f, 0.28f, 0.27f, 1f));
        Center(track.rectTransform, 0f, -48f, 520f, 22f);
        var bar = CreateImage("m_imgBar", track.transform, new Color(0.18f, 0.62f, 0.42f, 1f));
        bar.rectTransform.anchorMin = new Vector2(0f, 0f);
        bar.rectTransform.anchorMax = new Vector2(0f, 1f);
        bar.rectTransform.pivot = new Vector2(0f, 0.5f);
        bar.rectTransform.sizeDelta = new Vector2(0f, 0f);
        bar.rectTransform.anchoredPosition = Vector2.zero;
        Center(CreateText("m_txtProgress", root.transform, "0%", 20, FontStyle.Normal, TextAnchor.MiddleCenter, Color.white).rectTransform, 0f, -84f, 160f, 32f);
        Save(root, "LoadingUI");
    }

    private static void AddMask(Transform parent, float alpha)
    {
        Stretch(CreateImage("m_imgMask", parent, new Color(0f, 0f, 0f, alpha)).rectTransform);
    }

    private static Transform CreatePanel(Transform parent, float width, float height)
    {
        var image = CreateImage("m_imgPanel", parent, new Color(0.94f, 0.96f, 0.94f, 1f));
        Center(image.rectTransform, 0f, 0f, width, height);
        return image.transform;
    }

    private static GameObject CreateRoot(string name)
    {
        var root = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster));
        root.layer = LayerMask.NameToLayer("UI");
        var rect = root.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        return root;
    }

    private static Image CreateImage(string name, Transform parent, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.layer = LayerMask.NameToLayer("UI");
        if (parent != null)
        {
            go.transform.SetParent(parent, false);
        }
        var image = go.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private static Text CreateText(string name, Transform parent, string value, int size, FontStyle style, TextAnchor anchor, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);
        var text = go.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = anchor;
        text.color = color;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.text = value;
        return text;
    }

    private static InputField CreateInput(string name, Transform parent, string placeholder, string value, int limit, bool password)
    {
        var image = CreateImage(name, parent, Color.white);
        var input = image.gameObject.AddComponent<InputField>();
        input.targetGraphic = image;
        input.characterLimit = limit;
        input.contentType = password ? InputField.ContentType.Password : InputField.ContentType.Standard;
        var holder = CreateText("m_txtPlaceholder", image.transform, placeholder, 22, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.45f, 0.45f, 0.45f, 1f));
        StretchInset(holder.rectTransform, 16f, 0f);
        var text = CreateText("m_txtText", image.transform, value, 22, FontStyle.Normal, TextAnchor.MiddleLeft, Color.black);
        StretchInset(text.rectTransform, 16f, 0f);
        input.placeholder = holder;
        input.textComponent = text;
        input.text = value;
        return input;
    }

    private static Button CreateButton(string name, Transform parent, string label, Color color)
    {
        var image = CreateImage(name, parent, color);
        var button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        var text = CreateText("m_txtLabel", image.transform, label, 22, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        Stretch(text.rectTransform);
        return button;
    }

    private static Button CreateListButtonTemplate(Transform parent, string name)
    {
        return CreateListButtonTemplate(parent, name, new Color(0.84f, 0.87f, 0.82f, 1f));
    }

    private static Button CreateListButtonTemplate(Transform parent, string name, Color color)
    {
        var template = CreateButton(name, parent, string.Empty, color);
        var rect = template.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(0f, 64f);
        rect.anchoredPosition = Vector2.zero;
        template.gameObject.SetActive(false);

        var label = template.GetComponentInChildren<Text>();
        label.alignment = TextAnchor.MiddleLeft;
        label.color = Color.black;
        label.fontStyle = FontStyle.Normal;
        label.rectTransform.offsetMin = new Vector2(16f, 0f);
        label.rectTransform.offsetMax = new Vector2(-16f, 0f);
        return template;
    }

    private static Button CreateBagItemTemplate(Transform parent)
    {
        var template = CreateButton("m_item_BagItemTemplate", parent, string.Empty, new Color(0.78f, 0.82f, 0.76f, 1f));
        var rect = template.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(64f, 64f);
        rect.anchoredPosition = Vector2.zero;
        template.gameObject.SetActive(false);

        var label = template.transform.Find("m_txtLabel");
        if (label != null)
        {
            Object.DestroyImmediate(label.gameObject);
        }

        var icon = CreateImage("m_imgIcon", template.transform, Color.white);
        Center(icon.rectTransform, 0f, 2f, 42f, 42f);

        var count = CreateText("m_txtCount", template.transform, string.Empty, 17, FontStyle.Bold, TextAnchor.MiddleRight, Color.white);
        Anchor(count.rectTransform, 1f, 0f, 1f, 0f, -26f, 12f, 48f, 24f);
        return template;
    }

    private static Button CreateMailItemTemplate(Transform parent)
    {
        var template = CreateButton("m_item_MailTemplate", parent, string.Empty, new Color(0.84f, 0.87f, 0.82f, 1f));
        var rect = template.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(0f, 72f);
        rect.anchoredPosition = Vector2.zero;
        template.gameObject.SetActive(false);

        var label = template.transform.Find("m_txtLabel");
        if (label != null)
        {
            Object.DestroyImmediate(label.gameObject);
        }

        var title = CreateText("m_txtTitle", template.transform, string.Empty, 20, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black);
        Anchor(title.rectTransform, 0f, 0.5f, 1f, 0.5f, 18f, 10f, -122f, 32f);

        var state = CreateText("m_txtState", template.transform, string.Empty, 16, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.2f, 0.6f, 0.3f, 1f));
        Anchor(state.rectTransform, 0f, 0.5f, 0f, 0.5f, 42f, -18f, 76f, 24f);

        var attachment = CreateImage("m_imgAttachment", template.transform, new Color(0.95f, 0.67f, 0.18f, 1f));
        Anchor(attachment.rectTransform, 1f, 0.5f, 1f, 0.5f, -36f, 0f, 30f, 30f);
        return template;
    }

    private static Transform CreateModePanel(Transform parent, string name)
    {
        var panel = new GameObject(name, typeof(RectTransform));
        panel.layer = LayerMask.NameToLayer("UI");
        panel.transform.SetParent(parent, false);
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(0f, 0f);
        rect.offsetMax = new Vector2(0f, -118f);
        return panel.transform;
    }

    private static Transform CreatePlainList(Transform parent, string name, string emptyText, Vector2 offsetMin, Vector2 offsetMax)
    {
        var list = new GameObject(name, typeof(RectTransform));
        list.layer = LayerMask.NameToLayer("UI");
        list.transform.SetParent(parent, false);
        var rect = list.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        Stretch(CreateText("m_txtEmpty", list.transform, emptyText, 24, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.18f, 0.18f, 0.18f, 1f)).rectTransform);
        return list.transform;
    }

    private static Transform CreateDetailPanel(Transform parent, string name)
    {
        var detail = CreateImage(name, parent, new Color(0.90f, 0.92f, 0.88f, 1f));
        Anchor(detail.rectTransform, 1f, 0.5f, 1f, 0.5f, -270f, -28f, 460f, 442f);
        return detail.transform;
    }

    private static void CreateScrollList(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, out RectTransform listRect, out RectTransform contentRect)
    {
        var list = new GameObject(name, typeof(RectTransform), typeof(ScrollRect));
        list.layer = LayerMask.NameToLayer("UI");
        list.transform.SetParent(parent, false);
        listRect = list.GetComponent<RectTransform>();
        listRect.anchorMin = anchorMin;
        listRect.anchorMax = anchorMax;
        listRect.offsetMin = offsetMin;
        listRect.offsetMax = offsetMax;

        var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D));
        viewport.layer = LayerMask.NameToLayer("UI");
        viewport.transform.SetParent(list.transform, false);
        Stretch(viewport.GetComponent<RectTransform>());

        var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        content.layer = LayerMask.NameToLayer("UI");
        content.transform.SetParent(viewport.transform, false);
        contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        var layout = content.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 6f;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        content.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var scroll = list.GetComponent<ScrollRect>();
        scroll.viewport = viewport.GetComponent<RectTransform>();
        scroll.content = contentRect;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
    }

    private static void CreateGridScrollList(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, Vector2 cellSize, Vector2 spacing, int columns, out RectTransform listRect, out RectTransform contentRect)
    {
        var list = new GameObject(name, typeof(RectTransform), typeof(ScrollRect));
        list.layer = LayerMask.NameToLayer("UI");
        list.transform.SetParent(parent, false);
        listRect = list.GetComponent<RectTransform>();
        listRect.anchorMin = anchorMin;
        listRect.anchorMax = anchorMax;
        listRect.offsetMin = offsetMin;
        listRect.offsetMax = offsetMax;

        var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D));
        viewport.layer = LayerMask.NameToLayer("UI");
        viewport.transform.SetParent(list.transform, false);
        Stretch(viewport.GetComponent<RectTransform>());

        var content = new GameObject("Content", typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter));
        content.layer = LayerMask.NameToLayer("UI");
        content.transform.SetParent(viewport.transform, false);
        contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        var layout = content.GetComponent<GridLayoutGroup>();
        layout.cellSize = cellSize;
        layout.spacing = spacing;
        layout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        layout.startAxis = GridLayoutGroup.Axis.Horizontal;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = Mathf.Max(1, columns);
        content.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var scroll = list.GetComponent<ScrollRect>();
        scroll.viewport = viewport.GetComponent<RectTransform>();
        scroll.content = contentRect;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
    }

    private static void Center(RectTransform rect, float x, float y, float width, float height)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);
    }

    private static void Anchor(RectTransform rect, float minX, float minY, float maxX, float maxY, float x, float y, float width, float height)
    {
        rect.anchorMin = new Vector2(minX, minY);
        rect.anchorMax = new Vector2(maxX, maxY);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void StretchInset(RectTransform rect, float horizontal, float vertical)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(horizontal, vertical);
        rect.offsetMax = new Vector2(-horizontal, -vertical);
    }

    private static void Save(GameObject root, string name)
    {
        PrefabUtility.SaveAsPrefabAsset(root, $"{UiDir}/{name}.prefab");
        Object.DestroyImmediate(root);
    }

    private static bool HasAllRuntimePrefabs()
    {
        for (var i = 0; i < RequiredPrefabNames.Length; i++)
        {
            if (!File.Exists($"{UiDir}/{RequiredPrefabNames[i]}.prefab"))
            {
                return false;
            }
        }

        return true;
    }
}
