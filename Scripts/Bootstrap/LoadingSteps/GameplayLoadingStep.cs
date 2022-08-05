using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bootstrap.LoadingSteps
{
    [CreateAssetMenu(menuName = "Game/Loading/Gameplay Step")]
    public class GameplayLoadingStep : GameLoadingStep
    {
        [SerializeField] private int gameplaySceneBuildIndex;
    
        protected override async UniTask LoadStepInternal()
        {
            await SceneManager.LoadSceneAsync(gameplaySceneBuildIndex, LoadSceneMode.Additive);
        }
    }
}
