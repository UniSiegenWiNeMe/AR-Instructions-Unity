using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.MiniWizard.Scripts
{
    public class AppBarWithDeletConfirmationDialog: AppBar
    {
        public GameObject DeleteConfirmationDialogPrefab;

        private GameObject deleteConfirmationDialog;
        private Transform buttonTransform;
        public new void OnButtonPressed(AppBarButton button)
        {
            buttonTransform = button.gameObject.transform;
            base.OnButtonPressed(button);
        }

        protected override void OnClickRemove()
        {
            deleteConfirmationDialog = Instantiate(DeleteConfirmationDialogPrefab, buttonTransform.position, buttonTransform.rotation);
            deleteConfirmationDialog.GetComponent<DeleteConfirmationController>().OnCancle += AppBarWithDeletConfirmationDialog_OnCancle;
            deleteConfirmationDialog.GetComponent<DeleteConfirmationController>().OnDelete += AppBarWithDeletConfirmationDialog_OnDelete;
            gameObject.SetActive(false);

        }

        private void AppBarWithDeletConfirmationDialog_OnDelete(object sender, EventArgs e)
        {
            base.OnClickRemove();
            Destroy(deleteConfirmationDialog);
        }

        private void AppBarWithDeletConfirmationDialog_OnCancle(object sender, EventArgs e)
        {
            gameObject.SetActive(true);
            Destroy(deleteConfirmationDialog);

        }


    }
}
