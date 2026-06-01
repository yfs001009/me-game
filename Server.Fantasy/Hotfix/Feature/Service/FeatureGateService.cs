using Fantasy;
using GameConfig.open;
using Hotfix.Config;

namespace Hotfix.Feature.Service;

public sealed class FeatureGateService
{
    private readonly Dictionary<string, FeatureGateEntity> gates = new(StringComparer.OrdinalIgnoreCase);
    private bool loadedFromConfig;

    public FeatureGateService()
    {
    }

    public bool IsOpen(string featureId)
    {
        EnsureLoaded();
        if (string.IsNullOrWhiteSpace(featureId))
        {
            return true;
        }

        if (!gates.TryGetValue(featureId.Trim(), out var gate))
        {
            return false;
        }

        var now = DateTimeOffset.UtcNow;
        if (!gate.IsOpen)
        {
            return false;
        }

        if (gate.OpensAtUtc != default && now < gate.OpensAtUtc)
        {
            return false;
        }

        return gate.ClosesAtUtc == default || now <= gate.ClosesAtUtc;
    }

    public FeatureGateEntity? Get(string featureId)
    {
        EnsureLoaded();
        return string.IsNullOrWhiteSpace(featureId) ? null : gates.GetValueOrDefault(featureId.Trim());
    }

    private void EnsureLoaded()
    {
        if (loadedFromConfig)
        {
            return;
        }

        foreach (var config in ConfigSystem.Instance.Tables.TbOpenFeature.DataList)
        {
            RegisterFromConfig(config);
        }

        loadedFromConfig = true;
    }

    private void RegisterFromConfig(OpenFeatureConfig config)
    {
        Register(
            config.FeatureId,
            config.Name,
            config.Category,
            string.IsNullOrWhiteSpace(config.OpenConditionType) ? "Always" : config.OpenConditionType,
            config.IsEnabled,
            ParseUtc(config.StartTime),
            ParseUtc(config.EndTime));
    }

    public FeatureGateEntity RegisterAlwaysOpen(string featureId, string name, string category)
    {
        return Register(featureId, name, category, "Always", true, default, default);
    }

    public FeatureGateEntity Register(
        string featureId,
        string name,
        string category,
        string openCondition,
        bool isOpen,
        DateTimeOffset opensAtUtc,
        DateTimeOffset closesAtUtc)
    {
        featureId = Normalize(featureId);
        if (gates.TryGetValue(featureId, out var gate))
        {
            gate.Name = name;
            gate.Category = category;
            gate.OpenCondition = openCondition;
            gate.IsOpen = isOpen;
            gate.OpensAtUtc = opensAtUtc;
            gate.ClosesAtUtc = closesAtUtc;
            return gate;
        }

        gate = new FeatureGateEntity
        {
            FeatureId = featureId,
            Name = name,
            Category = category,
            OpenCondition = openCondition,
            IsOpen = isOpen,
            OpensAtUtc = opensAtUtc,
            ClosesAtUtc = closesAtUtc
        };
        gates.Add(featureId, gate);
        return gate;
    }

    private static string Normalize(string featureId)
    {
        return string.IsNullOrWhiteSpace(featureId) ? string.Empty : featureId.Trim();
    }

    private static DateTimeOffset ParseUtc(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return default;
        }

        return DateTimeOffset.TryParse(value.Trim(), out var parsed) ? parsed.ToUniversalTime() : default;
    }
}
