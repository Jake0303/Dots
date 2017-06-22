#if !UNITY_WEBGL && !UNITY_STANDALONE_WIN
    using UnityEngine;
    using UnityEngine.Advertisements;
    using System.Collections;



    public class ADManager : MonoBehaviour
    {
        public void StartAd()
        {
            int showAd = Random.Range(0, 2);
            if (showAd == 1)
                StartCoroutine("ShowAd");
        }

        IEnumerator ShowAd()
        {
            while (!Advertisement.IsReady())
            {
                yield return null;
            }
            Advertisement.Show();
            yield break;
        }
    }
#endif