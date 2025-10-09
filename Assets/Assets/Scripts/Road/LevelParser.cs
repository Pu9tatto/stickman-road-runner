using UnityEngine;
using System.Collections.Generic;

public static class LevelParser
{
    public static List<PrefabType> ParseLevelFile(string fileName)
    {
        List<PrefabType> prefabTypes = new List<PrefabType>();

        // Загружаем текстовый файл из Resources
        TextAsset textAsset = Resources.Load<TextAsset>($"Levels/{fileName}");
        if (textAsset == null)
        {
            Debug.LogError($"Level file not found: {fileName}");
            return prefabTypes;
        }

        string[] lines = textAsset.text.Split('\n');

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            // Пропускаем пустые строки и комментарии
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                continue;

            prefabTypes.Add(ParsePrefabType(trimmedLine));
        }

        Debug.Log($"Parsed {prefabTypes.Count} segments from {fileName}");
        return prefabTypes;
    }

    private static PrefabType ParsePrefabType(string line)
    {
        string typeString = line.Trim().ToUpper();

        switch (typeString)
        {
            case "STRAIGHT": return PrefabType.STRAIGHT;
            case "CURVE_R": return PrefabType.CURVE_R;
            case "CURVE_L": return PrefabType.CURVE_L;
            case "START": return PrefabType.START;
            case "FINISH": return PrefabType.FINISH;
            default:
                Debug.LogWarning($"Unknown prefab type: {typeString}");
                return PrefabType.STRAIGHT;
        }
    }

}