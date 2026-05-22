using System.Collections.Generic;
using System.Reflection;
using GameLogic;
using GameLogic.SheepBattle.App;
#if ENABLE_OBFUZ
using Obfuz;
#endif
using TEngine;
#pragma warning disable CS0436

/// <summary>
/// TEngine hotfix entry. Framework startup remains in Launcher/Procedure; this class only starts SheepBattle business logic.
/// </summary>
#if ENABLE_OBFUZ
[ObfuzIgnore(ObfuzScope.TypeName | ObfuzScope.MethodName)]
#endif
public partial class GameApp
{
    private static List<Assembly> _hotfixAssembly;

    /// <summary>
    /// Hotfix main entry called by TEngine after HybridCLR assemblies are loaded.
    /// </summary>
    public static void Entrance(object[] objects)
    {
        GameEventHelper.Init();
        _hotfixAssembly = (List<Assembly>)objects[0];
        Log.Warning("======= Entrance GameApp =======");
        Utility.Unity.AddDestroyListener(Release);
        StartGameLogic();
    }

    private static void StartGameLogic()
    {
        SheepBattleApp.Start();
    }

    private static void Release()
    {
        SheepBattleApp.Release();
        SingletonSystem.Release();
        Log.Warning("======= Release GameApp =======");
    }
}

