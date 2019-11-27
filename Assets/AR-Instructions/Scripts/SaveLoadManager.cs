using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    public void Save(Instruction instruction, bool sync = false, string path = null)
    {

        // this should be done in a more clever way. UpdateTransform is called for every gameobject even if the data not changed
        foreach (var step in instruction.Steps)
        {
            foreach (var item in step.Items)
            {
                item.UpdateTransforms();
                if (item._gameObject != null)
                {
                    item.IsActive = item._gameObject.transform.Find("Visual").gameObject.activeSelf;
                }
            }
        }

        if (sync)
        {
            WriteToDisk(instruction, path);
        }
        else
        {
            WriteToDiskAsync(instruction);
        }
    }

    /// <summary>
    /// Write the instruction to the filesystem. It is no real async method. Executeion this forwarded to the mainthread via the UnityMainThreadDispatcher
    /// </summary>
    /// <param name="save">Object to save</param>
    private void WriteToDiskAsync(Instruction save)
    {
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(Application.persistentDataPath + @"\" + save.Name +"-"+ save.DateCreated.ToString("yyyy.MM.dd-HH.mm.ss") + ".save");
        //bf.Serialize(file, save);
        //file.Close();

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            var path = Path.Combine(Application.persistentDataPath, save.Name + ".save");

            try
            {
                //Speichern
                var serializer = new XmlSerializer(typeof(Instruction));
                FileStream stream = new FileStream(path, FileMode.Create);
                serializer.Serialize(stream, save);
                stream.Flush();
                stream.Dispose();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Save Config Error " + ex.ToString());
            }
        });
    }

    private void WriteToDisk(Instruction save, string pathToSave)
    {
        var path = Path.Combine(pathToSave, save.Name + ".save"); //+ "-" + save.DateCreated.ToString("yyyy.MM.dd-HH.mm.ss") 

        try
        {
            //Speichern
            var serializer = new XmlSerializer(typeof(Instruction));
            FileStream stream = new FileStream(path, FileMode.Create);
            serializer.Serialize(stream, save);
            stream.Flush();
            stream.Dispose();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Save Config Error " + ex.ToString());
        }
    }

    /// <summary>
    /// Loads an instruction from filesystem by the given path to the save file
    /// </summary>
    /// <param name="path">Full path to the save file</param>
    /// <returns>if path is valid the instruction is return, otherwise null</returns>
    public Instruction Load(string path)
    {
        try
        {
            Debug.Log(path);
            if (File.Exists(path))
            {
                var serializer = new XmlSerializer(typeof(Instruction));
                var stream = new FileStream(path, FileMode.Open);
                var save = (Instruction)serializer.Deserialize(stream);
                //stream.Close();
                stream.Flush();
                stream.Dispose();

                return save;
            }
            else
            {
                return null;
            }
        }
        catch (System.Exception ex)
        {
            // Defaults
            Debug.LogWarning("Load Config Error " + ex.ToString());
        }

        return null;
        
    }


}
