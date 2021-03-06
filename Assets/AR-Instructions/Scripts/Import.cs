﻿using System.Collections;
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
using System.Xml.Linq;
using System.Text;
#endif

public class Import
{
#if WINDOWS_UWP
    private FileOpenPicker openPicker;
#endif

    public event EventHandler<string> ImportCompleted;

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
        string tmpName = string.Empty;
        if (!Directory.Exists(Path.Combine(path, "media")))
        {
            Directory.CreateDirectory(Path.Combine(path, "media"));
        }

        using (ZipArchive archive = ZipFile.OpenRead(Path.Combine(Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path, file.Name)))
        {
            foreach (var entry in archive.Entries)
            {
                tmpName = entry.FullName;

                if (tmpName.EndsWith(".save"))
                {
                    int i = 1;
                    while (File.Exists(Path.Combine(path, tmpName)))
                    {
                        
                        tmpName = entry.FullName.Substring(0, entry.FullName.LastIndexOf('.')) + " (" + i + ").save";
                        i++;
                    }
                }
                else
                {
                    if (File.Exists(Path.Combine(path, tmpName)))
                    {
                        File.Delete(Path.Combine(path, tmpName));
                    }
                }
                try
                {
                    entry.ExtractToFile(Path.Combine(path, tmpName), true);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.ToString());
                }
            }
        }
    
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            var instructionFullPath = Path.Combine(path, tmpName);
            if (File.Exists(instructionFullPath))
            {
                ImportCompleted?.Invoke(this,instructionFullPath);
            }
            else
            {
            }
    });
        
    }

#endif
}