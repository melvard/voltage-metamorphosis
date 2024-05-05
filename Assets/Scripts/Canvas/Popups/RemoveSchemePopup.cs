using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Canvas.Popups
{
    public class RemoveSchemePopup : MonoBehaviour
    {
        public Button removeSchemeButton;
        public Button cancelButton;
        public Button closeButton; 
        public Transform dashboardClearedText;

        private const string SCENE_NAME = "RemoveSchemePopup";
        public static async UniTask<bool> Spawn(CancellationToken ct)
        {
            RemoveSchemePopup rsp = await Utilities.LoadScene<RemoveSchemePopup>(SCENE_NAME, LoadSceneMode.Additive, ct);
            Utilities.PlayPopupShowAnimation(rsp.gameObject);
            rsp.dashboardClearedText.gameObject.SetActive(false);
            bool removeSchemeButtonClicked = false;
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            try
            {
                var lct = linkedCts.Token;
                var buttonTask = Utilities.SelectButton(lct, rsp.removeSchemeButton, rsp.cancelButton, rsp.closeButton);
                // var cancelButtonTask = Utilities.SelectButton();
                var result = await buttonTask;
                linkedCts.Cancel();
                removeSchemeButtonClicked = result == rsp.removeSchemeButton;
                return removeSchemeButtonClicked;
            }
            finally
            {
                linkedCts.Dispose();
                if (removeSchemeButtonClicked)
                {
                    rsp.dashboardClearedText.gameObject.SetActive(true);
                    await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                    rsp.dashboardClearedText.gameObject.SetActive(false);
                }
                await Utilities.PlayPopupCloseAnimation(rsp.gameObject);
                Utilities.LoadUnloadScene(SCENE_NAME);
            }
        }
    }
}