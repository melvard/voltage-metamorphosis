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

        private const string SCENE_NAME = "RemoveSchemePopup";
        public static async UniTask<bool> Spawn(CancellationToken ct)
        {
            RemoveSchemePopup rsp = await Utilities.LoadPopupScene<RemoveSchemePopup>(SCENE_NAME, LoadSceneMode.Additive, ct);
            Utilities.PlayPopupShowAnimation(rsp.gameObject);
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            try
            {
                var lct = linkedCts.Token;
                var buttonTask = Utilities.SelectButton(lct, rsp.removeSchemeButton, rsp.cancelButton, rsp.closeButton);
                // var cancelButtonTask = Utilities.SelectButton();
                var result = await buttonTask;
                linkedCts.Cancel();
                var removeSchemeButtonClicked = result == rsp.removeSchemeButton;
                return removeSchemeButtonClicked;
            }
            finally
            {
                linkedCts.Dispose();
                await Utilities.PlayPopupCloseAnimation(rsp.gameObject);
                Utilities.LoadUnloadScene(SCENE_NAME);
            }
        }
    }
}