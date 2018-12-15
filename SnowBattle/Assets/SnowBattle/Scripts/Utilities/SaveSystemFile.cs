using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Audio;

public class SaveSystemFile {
    public static void SaveConfiguration(float musicVolume, float effectsVolume)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/config.fun";

        FileStream stream = new FileStream(path, FileMode.Create);

        ConfigurationData cd = new ConfigurationData(musicVolume, effectsVolume);

        formatter.Serialize(stream, cd);

        stream.Close();
    }

    public static ConfigurationData LoadConfiguration()
    {
        string path = Application.persistentDataPath + "/config.fun";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path,FileMode.Open);

            ConfigurationData cd = formatter.Deserialize(stream) as ConfigurationData;

            stream.Close();

            return cd;
        }
        else
        {
            Debug.LogError("Save file not found in"+path);

            return null;
        }
    }
}
