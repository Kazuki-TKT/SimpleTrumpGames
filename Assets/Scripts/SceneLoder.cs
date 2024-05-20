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
            cts = new CancellationTokenSource(); // ���Z�b�g���邽�߂ɐV�����g�[�N���\�[�X���쐬
        }

        public async UniTaskVoid LoadSceneAsync(string sceneName)
        {
            var token = cts.Token; // �L�����Z���g�[�N�����擾

            try
            {
                
                FadeIn(1); // �t�F�[�h�C��
                await UniTask.Delay(TimeSpan.FromSeconds(1));

                var sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName); // �V�[����񓯊��Ń��[�h
                await sceneLoadOperation.ToUniTask(cancellationToken: token); // �V�[���̃��[�h����������܂őҋ@

                FadeOut(1); // �t�F�[�h�A�E�g
                await UniTask.WaitUntil(() => endFadeOut, cancellationToken: token);
                Debug.Log("�V�[�����[�h����܂���");
                //GameManager.Instance.UpdateGameState(gameState); // �w��̃X�e�[�g�Ƀ`�F���W
            }
            catch (OperationCanceledException)
            {
                Debug.Log("�V�[�����[�h���L�����Z������܂���");
            }
        }

    }
}
