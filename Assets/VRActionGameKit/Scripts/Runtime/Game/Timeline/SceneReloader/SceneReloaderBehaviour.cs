using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace _3DGamekitLite.Scripts.Runtime.Game.Timeline.SceneReloader
{
    [Serializable]
    public class SceneReloaderBehaviour : PlayableBehaviour
    {
        public void ReloadScene (GameObject sceneGameObject)
        {
            SceneManager.LoadSceneAsync (sceneGameObject.scene.buildIndex);
        }
    }
}
