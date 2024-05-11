using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Canvas.Popups
{
    public class ClearDashboardPopup : MonoBehaviour
    {
        public Button clearDashboardButton;
        public Button cancelButton;
        public Button closeButton; 
        public Transform dashboardClearedText;

        private const string SCENE_NAME = "ClearDashboardPopup";
        public static async UniTask<bool> Spawn(CancellationToken ct)
        {
            ClearDashboardPopup cdp = await Utilities.LoadPopupScene<ClearDashboardPopup>(SCENE_NAME, LoadSceneMode.Additive, ct);
            Utilities.PlayPopupShowAnimation(cdp.gameObject);
            cdp.dashboardClearedText.gameObject.SetActive(false);
            bool clearButtonClicked = false;
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            try
            {
                var lct = linkedCts.Token;
                var buttonTask = Utilities.SelectButton(lct, cdp.clearDashboardButton, cdp.cancelButton, cdp.closeButton);
                // var cancelButtonTask = Utilities.SelectButton();
                var result = await buttonTask;
                linkedCts.Cancel();
                clearButtonClicked = result == cdp.clearDashboardButton;
                return clearButtonClicked;
            }
            finally
            {
                linkedCts.Dispose();
                if (clearButtonClicked)
                {
                    cdp.dashboardClearedText.gameObject.SetActive(true);
                    await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                    cdp.dashboardClearedText.gameObject.SetActive(false);
                }
                await Utilities.PlayPopupCloseAnimation(cdp.gameObject);
                Utilities.LoadUnloadScene(SCENE_NAME);
            }
        }
    }
}