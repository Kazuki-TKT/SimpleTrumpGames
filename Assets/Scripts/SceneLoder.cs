using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;

namespace KazukiTrumpGame
{
    public class SceneLoder : Fade
    {
        public static SceneLoder Instance;

        CancellationTokenSource cts;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            cts = new CancellationTokenSource();
        }

        public void CancelLoading()
        {
            cts.Cancel();
            cts = new CancellationTokenSource(); // リセットするために新しいトークンソースを作成
        }

        public async UniTaskVoid LoadSceneAsync(string sceneName)
        {
            var token = cts.Token; // キャンセルトークンを取得

            try
            {
                
                FadeIn(1); // フェードイン
                await UniTask.Delay(TimeSpan.FromSeconds(1));

                var sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName); // シーンを非同期でロード
                await sceneLoadOperation.ToUniTask(cancellationToken: token); // シーンのロードが完了するまで待機

                FadeOut(1); // フェードアウト
                await UniTask.WaitUntil(() => endFadeOut, cancellationToken: token);
                Debug.Log("シーンロードされました");
                //GameManager.Instance.UpdateGameState(gameState); // 指定のステートにチェンジ
            }
            catch (OperationCanceledException)
            {
                Debug.Log("シーンロードがキャンセルされました");
            }
        }

    }
}
