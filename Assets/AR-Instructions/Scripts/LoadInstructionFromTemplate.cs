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

            InstructionLoaded?.Invoke();
        }, false);

#elif UNITY_EDITOR
        var lines = ReadFile(@"C:\Users\FlorianJasche\Desktop\Test.txt");

        GetComponentInChildren<ShowInstructionMenu>().InstructionName = InstructionManagerSingleton.Instance.Instruction.Name;
        GetComponentInChildren<ShowInstructionMenu>().EditMode = true;
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
                if (File.Exists(Path.Combine(path, entry.FullName)))
                {
                    File.Delete(Path.Combine(path, entry.FullName));
                }
                
                try
                {
                    entry.ExtractToFile(Path.Combine(path, entry.FullName), true);
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
            if (File.Exists(instructionFullPath))
            {
                var serializer = new XmlSerializer(typeof(Instruction));
                var stream = new FileStream(instructionFullPath, FileMode.Open);
                var save = (Instruction)serializer.Deserialize(stream);
                stream.Flush();
                stream.Dispose();
                InstructionManagerSingleton.Instance.Instruction = save;
            }
            else
            {
                //return null;
            }
        });
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
