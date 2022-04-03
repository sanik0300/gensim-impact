using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Симулятор_генетики_4
{
    //Да знаю я про newtonsoft.json, просто захотелось самостоятельно serializer написать
    //No hate, I know about newtonsoft.json, but this time I wanted to write a serializer myself
    public class DifferentGenesConverter : JsonConverter<Gene>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Gene);
        }
        public override Gene Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string property = "Type";
            bool in_array = false;
            Gene result = null;
            PropertyInfo[] pis = null;
            int num_of_props = -2;
            List<object> arr_vals = new List<object>(51);
            object what = null;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.EndObject:
                        return result;
                        break;
                    case JsonTokenType.PropertyName:
                        property = reader.GetString();
                        do
                        {
                            if (++num_of_props < 0)
                                break;
                        }
                        while (property != pis[num_of_props].Name);
                        break;
                    case JsonTokenType.StartArray:
                        in_array = true;
                        break;
                    case JsonTokenType.EndArray:
                        IList val = Array.CreateInstance(arr_vals[0].GetType(), arr_vals.Count);
                        for(int i =0; i<arr_vals.Count; i++)
                        {
                            val[i] = arr_vals[i];
                        }
                        arr_vals.Clear();
                        pis[num_of_props].SetValue(result, val);                       
                        in_array = false;
                        break;
                    default:
                        switch (reader.TokenType)
                        {
                            case JsonTokenType.String:
                                what = reader.GetString();
                                if (property == "Type")
                                {
                                    switch (what)
                                    {
                                        case "Binary":
                                            result = new Binary();
                                            break;
                                        case "Activer":
                                            result = new Activer();
                                            break;
                                        case "Raspred":
                                            result = new Raspred();
                                            break;
                                        default:
                                            result = new Quaternal();
                                            break;
                                    }
                                    pis = result.GetType().GetProperties();
                                }
                                else if(property=="lethal")
                                {
                                    what = new LethalComponent((string)what);
                                }
                                break;
                            case JsonTokenType.Number:
                                if (pis[num_of_props].PropertyType == typeof(int))
                                    what = reader.GetInt32();
                                else if (pis[num_of_props].PropertyType == typeof(float))
                                    what = reader.GetSingle();
                                else
                                    what = reader.GetDouble();
                                break;
                            case JsonTokenType.Null:
                                what = null;
                                break;
                            case JsonTokenType.True:
                                what = true;
                                break;
                            case JsonTokenType.False:
                                what = false;
                                break;
                        }
                        if (property == "Type")
                            break;
                        if (in_array)
                            arr_vals.Add(pis[num_of_props].PropertyType == typeof(int[])? Convert.ToInt32(what) : what);
                        else
                            pis[num_of_props].SetValue(result, what);                                            
                        break;
                }
            }
            return result;
        }

        public override void Write(Utf8JsonWriter writer, Gene value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Type", value.GetType().Name);
            foreach (PropertyInfo pi in value.GetType().GetProperties())
            {
                if (pi.GetValue(value) == null || Attribute.GetCustomAttribute(pi, typeof(JsonIgnoreAttribute))!=null)
                    continue;
                writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(pi.Name) ?? pi.Name);

                if (pi.PropertyType.IsArray)
                {
                    var something = Convert.ChangeType(pi.GetValue(value), pi.PropertyType);
                    writer.WriteStartArray();
                    foreach (var x in something as Array)
                    {
                        choosing_types(x, ref writer);
                    }
                    writer.WriteEndArray();
                }
                else
                {
                    var obj = pi.GetValue(value);
                    choosing_types(obj, ref writer);
                }
            }
            writer.WriteEndObject();
        }
        private void choosing_types(object le_what, ref Utf8JsonWriter writer)
        {
            if (le_what is bool)
                writer.WriteBooleanValue((bool)le_what);
            else if (le_what is string)
                writer.WriteStringValue((string)le_what);
            else if (le_what is null)
                writer.WriteNullValue();
            else if (le_what is LethalComponent)
                writer.WriteStringValue((le_what as LethalComponent).ToString());
            else
                writer.WriteNumberValue(Convert.ToDouble(le_what));
        }
    }
}
