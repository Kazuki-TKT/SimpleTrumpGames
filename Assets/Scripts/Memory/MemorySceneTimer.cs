using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace KazukiTrumpGame.Memory
{
    public class MemorySceneTimer : MonoBehaviour
    {
        MemorySceneDirector sceneDirector;

        [SerializeField] TextMeshProUGUI[] timerText;

        CancellationTokenSource cts;

        public int totalSeconds;

        private void Start()
        {
            //ìoò^
            sceneDirector = GameObject.FindGameObjectWithTag("GameDirector").GetComponent<MemorySceneDirector>();
            sceneDirector.onCurrentTurnChanged += HandleTurnTypeChanged;
        }

        private void OnDestroy()
        {
            //âèú
            sceneDirector.onCurrentTurnChanged -= HandleTurnTypeChanged;
        }

        private void HandleTurnTypeChanged(TurnType obj)
        {
            switch (obj)
            {
                case TurnType.PLAYER:
                    StartTimer().Forget();
                    break;
                case TurnType.RESULT:
                    StopTimer();
                    break;
                case TurnType.INITIAL:
                    timerText[0].text = "00:00";
                    timerText[1].text = "00:00";
                    totalSeconds = 0;
                    break;
            }
        }

        public async UniTaskVoid StartTimer()
        {
            using (cts = new CancellationTokenSource())
            {
                totalSeconds = 0;

                try
                {
                    while (true)
                    {
                        TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
                        timerText[0].text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
                        timerText[1].text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
                        await UniTask.Delay(1000, cancellationToken: cts.Token);
                        totalSeconds++;
                    }
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("Timer was cancelled.");
                }
            }
                
        }

        public void StopTimer()
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }
    }
}
