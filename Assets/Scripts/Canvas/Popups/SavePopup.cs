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
        public Transform savedText;
        
        private const string SCENE_NAME = "SavePopup";

        public static async UniTask<bool> Spawn(CancellationToken ct)
        {
            SavePopup sp = await Utilities.LoadScene<SavePopup>(SCENE_NAME, LoadSceneMode.Additive, ct);
            sp.savedText.gameObject.SetActive(false);
            Utilities.PlayPopupShowAnimation(sp.gameObject);
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            bool saveButtonClicked = false;
            try
            {
                var lct = linkedCts.Token;
                var buttonTask = Utilities.SelectButton(lct, sp.saveButton, sp.cancelButton, sp.closeButton);
                var result = await buttonTask;
                linkedCts.Cancel();
                saveButtonClicked = result == sp.saveButton;
                return saveButtonClicked;
            }
            finally
            {
                linkedCts.Dispose();
                if (saveButtonClicked)
                {
                    sp.savedText.gameObject.SetActive(true);
                    await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                    sp.savedText.gameObject.SetActive(false);
                }
                await Utilities.PlayPopupCloseAnimation(sp.gameObject);
                Utilities.LoadUnloadScene(SCENE_NAME);
            }
        }
    }
}