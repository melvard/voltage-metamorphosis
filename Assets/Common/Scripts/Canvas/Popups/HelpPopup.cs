using System.Threading;
using Cysharp.Threading.Tasks;
using Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Common.Scripts.Common.Canvas.Popups
{
    public class HelpPopup : MonoBehaviour
    {
        public Button gotItButton;
        public Button closeButton;
        private const string SCENE_NAME = "HelpPopup";

        public static async UniTask Spawn(CancellationToken ct)
        {
            HelpPopup hp = await Utilities.LoadPopupScene<HelpPopup>(SCENE_NAME, LoadSceneMode.Additive, ct);
            Utilities.PlayPopupShowAnimation(hp.gameObject);
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            try
            {
                var lct = linkedCts.Token;
                var buttonTask = Utilities.SelectButton(lct, hp.gotItButton, hp.closeButton);
                var result = await buttonTask;
                linkedCts.Cancel();
                // var saveButtonClicked = result == hp.gotItButton;
                // return saveButtonClicked;
            }
            finally
            {
                linkedCts.Dispose();
                await Utilities.PlayPopupCloseAnimation(hp.gameObject);
                Utilities.LoadUnloadScene(SCENE_NAME);
            }
            
        } 
    }
}