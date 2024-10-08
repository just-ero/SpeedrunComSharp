﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SpeedrunComSharp;

public class VariableValue : IElementWithID
{
    public string ID { get; private set; }

    public string VariableID { get; private set; }

    #region Links

    internal Lazy<Variable> variable;
    internal Lazy<string> value;

    public Variable Variable => variable.Value;
    public string Value => value.Value;
    public string Name => Variable.Name;

    public bool IsCustomValue => string.IsNullOrEmpty(ID);

    #endregion

    private VariableValue() { }

    public static VariableValue CreateCustomValue(SpeedrunComClient client, string variableId, string customValue)
    {
        var value = new VariableValue
        {
            VariableID = variableId
        };

        value.variable = new Lazy<Variable>(() => client.Variables.GetVariable(value.VariableID));
        value.value = new Lazy<string>(() => customValue);

        return value;
    }

    public static VariableValue ParseValueDescriptor(SpeedrunComClient client, KeyValuePair<string, dynamic> valueElement)
    {
        var value = new VariableValue
        {
            VariableID = valueElement.Key,
            ID = valueElement.Value as string
        };

        //Parse Links

        value.variable = new Lazy<Variable>(() => client.Variables.GetVariable(value.VariableID));
        value.value = new Lazy<string>(() => value.Variable.Values.FirstOrDefault(x => x.ID == value.ID).Value);

        return value;
    }

    public static VariableValue ParseIDPair(SpeedrunComClient client, Variable variable, KeyValuePair<string, dynamic> valueElement)
    {
        var value = new VariableValue
        {
            VariableID = variable.ID,
            ID = valueElement.Key,

            //Parse Links

            variable = new Lazy<Variable>(() => variable)
        };

        string valueName = valueElement.Value as string;
        value.value = new Lazy<string>(() => valueName);

        return value;
    }

    public override int GetHashCode()
    {
        return (ID ?? string.Empty).GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj is not VariableValue other)
        {
            return false;
        }

        return ID == other.ID;
    }

    public override string ToString()
    {
        return Value;
    }
}
