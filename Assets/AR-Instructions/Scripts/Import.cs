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

public class Import
{
#if WINDOWS_UWP
    private FileOpenPicker openPicker;
#endif

    public event EventHandler ImportCompleted;

    public Import()
    {

    }

    public void DoImport()
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
                    await extract(file, path);
                }        
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error in Import: " + ex.ToString());
            }

            
        }, false);
#endif

    }

#if WINDOWS_UWP

    private async Task extract(StorageFile file, string path)
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
                InstructionManager.Instance.Instruction = save;
                ImportCompleted?.Invoke(this,null);
            }
            else
            {
            }
        });
    }

#endif
}