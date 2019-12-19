using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;
using System;
using Microsoft.MixedReality.Toolkit.UI;

#if !UNITY_EDITOR
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using System.IO.Compression;
#endif

public class Export : MonoBehaviour
{
    public GameObject GameObjectToDisable;
    public GameObject WaitingDialogPrefab;
    private GameObject _waitingDialog;


    /// <summary>
    /// This event is invoked after the export is completed. Of Parameter is true => export successful, false => some erros occured
    /// </summary>
    public event EventHandler<bool> ExportCompleted;
    
    public void DoExport()
    {
        DisableUI();

        var name = InstructionManager.Instance.Instruction.Name;
        var pathToSave = Application.persistentDataPath;
        var pathToMedia = Path.Combine(Application.persistentDataPath, "media");

#if !UNITY_EDITOR
        SaveFileAsync(name, pathToSave, pathToMedia);
#else
        ShowWaitingDialog();
        ExportCompleted?.Invoke(this, true);
        ChangeWaitingDialogText();
        EnableUI();
#endif

    }

    private void ChangeWaitingDialogText()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            
            _waitingDialog.GetComponent<WaitingDialogController>().ExportCompleted();
            });
        
    }

    private void ShowWaitingDialog()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => { _waitingDialog = Instantiate(WaitingDialogPrefab);
            _waitingDialog.GetComponent<WaitingDialogController>().ObjectToReactivate = GameObjectToDisable.transform.parent.gameObject;
        });
    }

    private void DisableUI()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => { GameObjectToDisable.transform.parent.gameObject.SetActive(false); });
        
    }

    private void EnableUI()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => { GameObjectToDisable.transform.parent.gameObject.SetActive(true); });
        
    }

#if !UNITY_EDITOR
    
    private async void SaveFileAsync(string fileName, string pathToSave, string pathToMedia)
    {
        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Archieved Expert Data with Media", new List<string>() { ".zip" });            
            
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = fileName+".zip"; 

            StorageFile file = await savePicker.PickSaveFileAsync();
            
            if (file != null)
            {
                InstructionManager.Instance.Instruction.Name = file.DisplayName;
                InstructionManager.Instance.Save(true, pathToSave);
                
                ShowWaitingDialog();
                using (Stream zipFileToSave = await file.OpenStreamForWriteAsync())
                {
                    // do export async
                    await Task.Run(() =>
                    {
                        //open acces to zip file
                        //todo: check mode
                        using (ZipArchive archive = new ZipArchive(zipFileToSave, ZipArchiveMode.Update))
                        {
                            archive.CreateEntryFromFile(Path.Combine(pathToSave, InstructionManager.Instance.Instruction.Name + ".save"), InstructionManager.Instance.Instruction.Name + ".save");

                            foreach (var step in InstructionManager.Instance.Instruction.Steps)
                            {
                                foreach (var mediaFile in step.MediaFiles)
                                {
                                    archive.CreateEntryFromFile(Path.Combine(pathToMedia, mediaFile.FileName), @"media\" + mediaFile.FileName);
                                }
                            }
                        }
                    });
                }

                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    ExportCompleted?.Invoke(this, true);
                }
                else
                {
                    ExportCompleted?.Invoke(this, false);
                }

                
            }
            else
            {
                ExportCompleted?.Invoke(this, false);
            }

            ChangeWaitingDialogText();
            //EnableUI();


        }, false);
    }
#endif

}
