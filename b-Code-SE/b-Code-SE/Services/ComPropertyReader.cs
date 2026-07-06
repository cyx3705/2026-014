using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace b_Code_SE.Services;

/// <summary>
/// 从 COM 对象动态读取常用参数属性。
/// </summary>
internal static class ComPropertyReader
{
    private static readonly Dictionary<string, string[]> KnownFeatureProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ExtrudedProtrusion"] = ["Name", "DisplayName", "Depth", "ExtentType", "ExtentSide", "ProfileSide"],
        ["ExtrudedCutout"] = ["Name", "DisplayName", "Depth", "ExtentType", "ExtentSide"],
        ["RevolvedProtrusion"] = ["Name", "DisplayName", "Angle", "ExtentType"],
        ["RevolvedCutout"] = ["Name", "DisplayName", "Angle", "ExtentType"],
        ["Hole"] = ["Name", "DisplayName", "Diameter", "Depth", "HoleType"],
        ["SimpleHole"] = ["Name", "DisplayName", "Diameter", "Depth"],
        ["Round"] = ["Name", "DisplayName", "Radius"],
        ["Chamfer"] = ["Name", "DisplayName", "Distance", "Angle"],
        ["ThinWall"] = ["Name", "DisplayName", "Thickness"],
        ["Pattern"] = ["Name", "DisplayName", "Count"],
        ["RectangularPattern"] = ["Name", "DisplayName", "Count", "XCount", "YCount"],
        ["CircularPattern"] = ["Name", "DisplayName", "Count"],
        ["Draft"] = ["Name", "DisplayName", "Angle"],
        ["Shell"] = ["Name", "DisplayName", "Thickness"],
        ["SweptProtrusion"] = ["Name", "DisplayName"],
        ["SweptCutout"] = ["Name", "DisplayName"],
        ["LoftedProtrusion"] = ["Name", "DisplayName"],
        ["LoftedCutout"] = ["Name", "DisplayName"],
    };

    private static readonly string[] GenericProperties =
        ["Name", "DisplayName", "Depth", "Distance", "Radius", "Diameter", "Angle", "Thickness", "ExtentType", "ExtentSide", "Count"];

    public static string DescribeObject(object? comObject)
    {
        if (comObject == null)
        {
            return "(null)";
        }

        string typeName = GetShortTypeName(comObject);
        List<string> parts = [$"类型={typeName}"];

        foreach (string propertyName in ResolvePropertyNames(typeName))
        {
            if (!TryReadProperty(comObject, propertyName, out object? value))
            {
                continue;
            }

            string formatted = FormatValue(propertyName, value!);
            if (!string.IsNullOrWhiteSpace(formatted))
            {
                parts.Add($"{propertyName}={formatted}");
            }
        }

        return string.Join("  ", parts);
    }

    public static string GetFeatureKey(object comObject, int index)
    {
        string typeName = GetShortTypeName(comObject);
        string name = TryReadProperty(comObject, "Name", out object? value) ? Convert.ToString(value) ?? "" : "";
        string signature = string.Join("|", ResolvePropertyNames(typeName)
            .Select(propertyName => TryReadProperty(comObject, propertyName, out object? propValue)
                ? FormatValue(propertyName, propValue!)
                : "")
            .Where(part => !string.IsNullOrWhiteSpace(part)));
        return $"{typeName}#{index}#{name}#{signature}";
    }

    private static IEnumerable<string> ResolvePropertyNames(string typeName)
    {
        if (KnownFeatureProperties.TryGetValue(typeName, out string[]? known))
        {
            return known;
        }

        return GenericProperties;
    }

    private static string GetShortTypeName(object comObject)
    {
        Type type = comObject.GetType();
        string name = type.Name;
        if (name.StartsWith("I", StringComparison.Ordinal) && name.Length > 1 && char.IsUpper(name[1]))
        {
            name = name[1..];
        }

        return name;
    }

    private static bool TryReadProperty(object comObject, string propertyName, out object? value)
    {
        value = null;
        try
        {
            value = comObject.GetType().InvokeMember(
                propertyName,
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase,
                null,
                comObject,
                null);
            return value != null && value != DBNull.Value;
        }
        catch
        {
            try
            {
                value = comObject.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)
                    ?.GetValue(comObject);
                return value != null && value != DBNull.Value;
            }
            catch
            {
                try
                {
                    dynamic item = comObject;
                    value = ComDynamic.GetProperty(item, propertyName);
                    return value != null;
                }
                catch
                {
                    return false;
                }
            }
        }
    }

    private static string FormatValue(string propertyName, object value)
    {
        if (value is string s)
        {
            return s;
        }

        if (value is bool b)
        {
            return b ? "True" : "False";
        }

        if (value is Enum e)
        {
            return $"{e} ({Convert.ToInt32(e)})";
        }

        if (value is double d)
        {
            return FormatLength(propertyName, d);
        }

        if (value is float f)
        {
            return FormatLength(propertyName, f);
        }

        if (value is int or short or long)
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";
        }

        try
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";
        }
        catch
        {
            return value.GetType().Name;
        }
    }

    private static string FormatLength(string propertyName, double meters)
    {
        if (propertyName.Contains("Angle", StringComparison.OrdinalIgnoreCase))
        {
            double degrees = meters * 180.0 / Math.PI;
            return $"{degrees:F3}° (rad={meters:F6})";
        }

        if (propertyName is "Count" or "ExtentType" or "ExtentSide" or "ProfileSide" or "HoleType")
        {
            return meters.ToString("F6", CultureInfo.InvariantCulture);
        }

        double mm = meters * 1000.0;
        return $"{mm:F3} mm (m={meters:F6})";
    }

    private static class ComDynamic
    {
        public static object? GetProperty(dynamic item, string propertyName)
        {
            switch (propertyName.ToLowerInvariant())
            {
                case "name": return item.Name;
                case "displayname": return item.DisplayName;
                case "depth": return (double)item.Depth;
                case "distance": return (double)item.Distance;
                case "radius": return (double)item.Radius;
                case "diameter": return (double)item.Diameter;
                case "angle": return (double)item.Angle;
                case "thickness": return (double)item.Thickness;
                case "extenttype": return item.ExtentType;
                case "extentside": return item.ExtentSide;
                case "profileside": return item.ProfileSide;
                case "holetype": return item.HoleType;
                case "count": return (int)item.Count;
                case "xcount": return (int)item.XCount;
                case "ycount": return (int)item.YCount;
                default: return null;
            }
        }
    }
}