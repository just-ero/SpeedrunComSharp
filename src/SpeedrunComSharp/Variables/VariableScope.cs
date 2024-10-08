﻿using System;

namespace SpeedrunComSharp;

public class VariableScope
{
    public VariableScopeType Type { get; private set; }
    public string LevelID { get; private set; }

    #region Links

    private Lazy<Level> level;

    public Level Level => level.Value;

    #endregion

    private VariableScope() { }

    private static VariableScopeType parseType(string type)
    {
        return type switch
        {
            "global" => VariableScopeType.Global,
            "full-game" => VariableScopeType.FullGame,
            "all-levels" => VariableScopeType.AllLevels,
            "single-level" => VariableScopeType.SingleLevel,
            _ => throw new ArgumentException("type"),
        };
    }

    public static VariableScope Parse(SpeedrunComClient client, dynamic scopeElement)
    {
        var scope = new VariableScope
        {
            Type = parseType(scopeElement.type as string)
        };

        if (scope.Type == VariableScopeType.SingleLevel)
        {
            scope.LevelID = scopeElement.level as string;
            scope.level = new Lazy<Level>(() => client.Levels.GetLevel(scope.LevelID));
        }
        else
        {
            scope.level = new Lazy<Level>(() => null);
        }

        return scope;
    }

    public override string ToString()
    {
        if (Type == VariableScopeType.SingleLevel)
        {
            return "Single Level: " + (Level.Name ?? "");
        }
        else
        {
            return Type.ToString();
        }
    }
}
