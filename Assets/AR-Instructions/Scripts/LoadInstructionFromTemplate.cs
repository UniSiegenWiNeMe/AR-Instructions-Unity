using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Events;
using System;

#if WINDOWS_UWP
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using System.IO.Compression;
using System.Xml.Serialization;
#endif

public class LoadInstructionFromTemplate : MonoBehaviour
{
#if WINDOWS_UWP
    private FileOpenPicker openPicker;
#endif

    public UnityEvent InstructionLoaded;

    public void OnLoadInstruction()
    {
        OpenFileAsync(Application.persistentDataPath);
    }

    private async void OpenFileAsync(string path)
    {
#if WINDOWS_UWP

        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
        {
        try
        {
            Debug.Log("path:" + path);
            openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".zip");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if(file!= null)
            {
                await file.CopyAsync(Windows.Storage.ApplicationData.Current.LocalCacheFolder, file.Name, NameCollisionOption.ReplaceExisting);
                await load(file, path);
            }        
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error in Import: " + ex.ToString());
        }

        Debug.Log("instruction loaded");
        //GetComponentInChildren<ShowInstructionMenu>().InstructionName = InstructionManagerSingleton.Instance.Instruction.Name;
        //GetComponentInChildren<ShowInstructionMenu>().EditMode = true;
        InstructionLoaded?.Invoke();
        Debug.Log("Event invoked");


        }, false);

#elif UNITY_EDITOR
        var lines = ReadFile(@"C:\Users\FlorianJasche\Desktop\Test.txt");

        GetComponentInChildren<ShowInstructionMenu>().InstructionName = InstructionManagerSingleton.Instance.Instruction.Name;
        GetComponentInChildren<ShowInstructionMenu>().EditMode = true;
        //InstructionManagerSingleton.Instance.LoadFromTemplate(lines, "test");
        InstructionLoaded?.Invoke();
#endif

    }

#if WINDOWS_UWP

    private async Task load(StorageFile file, string path)
    {
            if(!Directory.Exists(Path.Combine(path, "media")))
            {
                Directory.CreateDirectory(Path.Combine(path, "media"));
            }

            using (ZipArchive archive = ZipFile.OpenRead(Path.Combine(Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path, file.Name)))
            {
                foreach (var entry in archive.Entries)
                {
                    Debug.Log(Path.Combine(path, entry.FullName));
                    if (File.Exists(Path.Combine(path, entry.FullName)))
                    {
                        File.Delete(Path.Combine(path, entry.FullName));
                    }
                
                    try
                    {
                        entry.ExtractToFile(Path.Combine(path, entry.FullName), true);
                        //archive.ExtractToDirectory(path1);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.ToString());
                    }

                }

                
            }
    UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
    var instructionFullPath = Path.Combine(path,file.DisplayName + ".save");
    Debug.Log(instructionFullPath);
    if (File.Exists(instructionFullPath))
            {
    Debug.Log("file is da");
                var serializer = new XmlSerializer(typeof(Instruction));
                var stream = new FileStream(instructionFullPath, FileMode.Open);
                var save = (Instruction)serializer.Deserialize(stream);

    Debug.Log(save != null? "save is da": "kein save :(");
                //stream.Close();
                stream.Flush();
                stream.Dispose();

                InstructionManagerSingleton.Instance.Instruction = save;
            }
            else
            {
                //return null;
            }
    });
        
        //XmlSerializer serializer = new XmlSerializer(typeof(Instruction));
        //using (Stream reader = new FileStream(instructionFullPath, FileMode.Open))
        //{
        //    // Call the Deserialize method to restore the object's state.
        //    var Instruction = (Instruction)serializer.Deserialize(reader);
        //    InstructionManagerSingleton.Instance.Instruction = Instruction;
        //}

        //UnityMainThreadDispatcher.Instance().Enqueue(() =>
        //{
        //    if (file != null)
        //    {
        //        // Application now has read/write access to the picked file
        //        var lines = ReadFile(Path.Combine(Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path, file.Name));

        //        GetComponentInChildren<ShowInstructionMenu>().InstructionName = file.DisplayName;
        //        GetComponentInChildren<ShowInstructionMenu>().EditMode = true;
        //        InstructionManagerSingleton.Instance.LoadFromTemplate(lines, file.DisplayName);
        //        InstructionLoaded?.Invoke();

        //    }
        //    else
        //    {
        //        // The picker was dismissed with no selected file
        //        Debug.Log("File picker operation cancelled");
        //    }
        //});
    }

#endif
    private List<string> ReadFile(string path)
    {
        List<string> lines = new List<string>();

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);

        while (reader.Peek() >= 0)
        {
            var line = reader.ReadLine();
            lines.Add(line);
        }

        reader.Close();

        return lines;
    }
}
