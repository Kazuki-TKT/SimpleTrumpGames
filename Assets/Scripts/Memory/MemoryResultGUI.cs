using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
namespace KazukiTrumpGame.Memory
{
    public class MemoryResultGUI : ResultGUIBase
    {
        MemorySceneDirector sceneDirector;

        void Start()
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

        void HandleTurnTypeChanged(TurnType obj)
        {
            HandleTurnTypeChangedTask(obj).Forget();
        }

        async UniTaskVoid HandleTurnTypeChangedTask(TurnType obj)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                switch (obj)
                {
                    case TurnType.PLAYER:
                        break;
                    case TurnType.RESULT:
                        SetObject(setObjects, false);
                        await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: cts.Token);
                        StartCoroutine(MovePanel(-50));
                        break;
                    case TurnType.INITIAL:
                        break;
                }
            }

        }
        protected override void OnRestartGame()
        {
            sceneDirector.ReStartGame();
        }

    }


}
