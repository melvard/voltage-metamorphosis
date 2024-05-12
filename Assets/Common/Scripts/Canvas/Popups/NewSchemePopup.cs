using System.Threading;
using Cysharp.Threading.Tasks;
using Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Canvas.Popups
{
    public class NewSchemePopup : MonoBehaviour
    {
        public Button newSchemeButton;
        public Button cancelButton;
        public Button closeButton;

        private const string SCENE_NAME = "NewSchemePopup";
        public static async UniTask<bool> Spawn(CancellationToken ct)
        {
            NewSchemePopup nsp = await Utilities.LoadPopupScene<NewSchemePopup>(SCENE_NAME, LoadSceneMode.Additive, ct);
            Utilities.PlayPopupShowAnimation(nsp.gameObject);
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            try
            {
                var lct = linkedCts.Token;
                var buttonTask = Utilities.SelectButton(lct, nsp.newSchemeButton, nsp.cancelButton, nsp.closeButton);
                // var cancelButtonTask = Utilities.SelectButton();
                var result = await buttonTask;
                linkedCts.Cancel();
                return result == nsp.newSchemeButton;;
            }
            finally
            {
                linkedCts.Dispose();
               
                await Utilities.PlayPopupCloseAnimation(nsp.gameObject);
                Utilities.LoadUnloadScene(SCENE_NAME);
            }
        }
    }
}