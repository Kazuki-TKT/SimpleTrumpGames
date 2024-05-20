using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KazukiTrumpGame
{
    public class ScenLoadButton : MonoBehaviour
    {
        public string sceneName;
        void Start()
        {
            CustomButton customButton = gameObject.GetComponent<CustomButton>();
            customButton.onClickCallback = () => SceneLoder.Instance.LoadSceneAsync(sceneName).Forget();
        }
    }
}
