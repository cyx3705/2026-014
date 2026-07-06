using SolidEdgeFramework;

namespace b_Code_SE.Services;

/// <summary>
/// 命令执行前后对比零件特征，解析新增或修改特征的参数（如拉伸 Depth）。
/// ApplicationEvents 不携带命令参数，需在 AfterCommandRun 后读取模型对象。
/// </summary>
internal sealed class FeatureParameterInspector
{
    private List<FeatureSnapshot> _beforeSnapshot = [];

    public void Snapshot(Application application)
    {
        _beforeSnapshot = CaptureFeatures(application);
    }

    public IReadOnlyList<string> InspectChanges(Application application, bool commitSnapshot = true)
    {
        List<string> results = [];
        List<FeatureSnapshot> current = CaptureFeatures(application);

        if (current.Count > _beforeSnapshot.Count)
        {
            for (int i = _beforeSnapshot.Count; i < current.Count; i++)
            {
                results.Add($"新增特征: {current[i].Description}");
            }
        }
        else
        {
            foreach (string line in InspectSelection(application))
            {
                results.Add(line);
            }

            FeatureSnapshot? changed = FindChangedFeature(_beforeSnapshot, current);
            if (changed != null)
            {
                results.Add($"变更特征: {changed.Description}");
            }
            else if (current.Count > 0)
            {
                results.Add($"当前最后特征: {current[^1].Description}");
            }
        }

        if (commitSnapshot)
        {
            _beforeSnapshot = current;
        }

        if (results.Count == 0)
        {
            results.Add("未检测到特征变化（零件环境外或命令尚未提交特征）");
        }

        return results;
    }

    private static FeatureSnapshot? FindChangedFeature(
        IReadOnlyList<FeatureSnapshot> before,
        IReadOnlyList<FeatureSnapshot> after)
    {
        int compareCount = Math.Min(before.Count, after.Count);
        for (int i = compareCount - 1; i >= 0; i--)
        {
            if (!string.Equals(before[i].Signature, after[i].Signature, StringComparison.Ordinal))
            {
                return after[i];
            }
        }

        return null;
    }

    private static List<FeatureSnapshot> CaptureFeatures(Application application)
    {
        List<FeatureSnapshot> snapshots = [];
        int index = 0;
        foreach (object feature in EnumeratePartFeatures(application))
        {
            index++;
            snapshots.Add(new FeatureSnapshot(
                ComPropertyReader.GetFeatureKey(feature, index),
                ComPropertyReader.DescribeObject(feature)));
        }

        return snapshots;
    }

    private static IEnumerable<string> InspectSelection(Application application)
    {
        List<string> lines = [];
        try
        {
            SelectSet selectSet = application.ActiveSelectSet;
            for (int i = 1; i <= selectSet.Count; i++)
            {
                object item = selectSet.Item(i);
                lines.Add($"选中对象: {ComPropertyReader.DescribeObject(item)}");
            }
        }
        catch
        {
            // 忽略
        }

        return lines;
    }

    private static IEnumerable<object> EnumeratePartFeatures(Application application)
    {
        object? document = application.ActiveDocument;
        if (document == null)
        {
            yield break;
        }

        dynamic partDocument = document;
        object? models;
        try
        {
            models = partDocument.Models;
        }
        catch
        {
            yield break;
        }

        if (models == null)
        {
            yield break;
        }

        dynamic modelsCollection = models;
        int modelCount = modelsCollection.Count;
        for (int modelIndex = 1; modelIndex <= modelCount; modelIndex++)
        {
            dynamic model = modelsCollection.Item(modelIndex);
            object? features;
            try
            {
                features = model.Features;
            }
            catch
            {
                continue;
            }

            if (features == null)
            {
                continue;
            }

            dynamic featuresCollection = features;
            int featureCount = featuresCollection.Count;
            for (int featureIndex = 1; featureIndex <= featureCount; featureIndex++)
            {
                yield return featuresCollection.Item(featureIndex);
            }
        }
    }

    private sealed record FeatureSnapshot(string Signature, string Description);
}