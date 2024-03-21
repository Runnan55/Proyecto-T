using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public void SaveData<T>(string key, T value)
    {
        if (typeof(T) == typeof(int))
        {
            PlayerPrefs.SetInt(key, (int)(object)value);
        }

        else if (typeof(T) == typeof(float))
        {
            PlayerPrefs.SetFloat(key, (float)(object)value);
        }

        else if (typeof(T) == typeof(string))
        {
            PlayerPrefs.SetString(key, (string)(object)value);
        }

        else if (typeof(T) == typeof(bool))
        {
            PlayerPrefs.SetInt(key, (bool)(object)value ? 1 : 0);
        }

        PlayerPrefs.Save();
    }

    public T Load<T>(string key, T defaultValue)
    {
        if (typeof(T) == typeof(int))
        {
            return (T)(object)PlayerPrefs.GetInt(key, (int)(object)defaultValue);
        }
        else if (typeof(T) == typeof(float))
        {
            return (T)(object)PlayerPrefs.GetFloat(key, (float)(object)defaultValue);
        }
        else if (typeof(T) == typeof(string))
        {
            return (T)(object)PlayerPrefs.GetString(key, (string)(object)defaultValue);
        }
        else if (typeof(T) == typeof(bool))
        {
            return (T)(object)(PlayerPrefs.GetInt(key, (bool)(object)defaultValue ? 1 : 0) != 0);
        }

        return defaultValue;
    }
}
