using System.Collections;
using TMPro;
using UnityEngine;

namespace GameLogic
{
    public class LoadingAnimation : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI loadingText;

        private IEnumerator Start()
        {
            while (true)
            {
                loadingText.text = "Loading";
                yield return new WaitForSeconds(0.1f);
                loadingText.text = "Loading.";
                yield return new WaitForSeconds(0.1f);
                loadingText.text = "Loading..";
                yield return new WaitForSeconds(0.1f);
                loadingText.text = "Loading...";
            }
        }
    }
}