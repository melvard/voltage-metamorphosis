using System.Threading;
using Cysharp.Threading.Tasks;
using Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Canvas.Popups
{
    public class EditSchemePopup : MonoBehaviour
    {
        public Button editSchemeButton;
        public Button cancelButton;
        public Button closeButton;

        private const string SCENE_NAME = "EditSchemePopup";
        public static async UniTask<bool> Spawn(CancellationToken ct)
        {
            EditSchemePopup esp = await Utilities.LoadPopupScene<EditSchemePopup>(SCENE_NAME, LoadSceneMode.Additive, ct);
            Utilities.PlayPopupShowAnimation(esp.gameObject);
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            try
            {
                var lct = linkedCts.Token;
                var buttonTask = Utilities.SelectButton(lct, esp.editSchemeButton, esp.cancelButton, esp.closeButton);
                // var cancelButtonTask = Utilities.SelectButton();
                var result = await buttonTask;
                linkedCts.Cancel();
                return result == esp.editSchemeButton;;
            }
            finally
            {
                linkedCts.Dispose();
               
                await Utilities.PlayPopupCloseAnimation(esp.gameObject);
                Utilities.LoadUnloadScene(SCENE_NAME);
            }
        }
    }
}