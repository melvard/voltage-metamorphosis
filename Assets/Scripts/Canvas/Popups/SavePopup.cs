using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Canvas.Popups
{
    public class SavePopup : MonoBehaviour
    {
        public Button saveButton;
        public Button cancelButton;
        public Button closeButton;
        private const string SCENE_NAME = "SavePopup";

        public static async UniTask<bool> Spawn(CancellationToken ct)
        {
            SavePopup sp = await Utilities.LoadPopupScene<SavePopup>(SCENE_NAME, LoadSceneMode.Additive, ct);
            Utilities.PlayPopupShowAnimation(sp.gameObject);
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            try
            {
                var lct = linkedCts.Token;
                var buttonTask = Utilities.SelectButton(lct, sp.saveButton, sp.cancelButton, sp.closeButton);
                var result = await buttonTask;
                linkedCts.Cancel();
                var saveButtonClicked = result == sp.saveButton;
                return saveButtonClicked;
            }
            finally
            {
                linkedCts.Dispose();
                await Utilities.PlayPopupCloseAnimation(sp.gameObject);
                Utilities.LoadUnloadScene(SCENE_NAME);
            }
        }
    }
}