using UnityEngine;
using Random = UnityEngine.Random;

namespace Adastra
{
    public class RandomMusicLoader : MonoBehaviour
    {
        [SerializeField] private GameObject musicTrack1;
        [SerializeField] private GameObject musicTrack2;

        private void Awake()
        {
            musicTrack1.SetActive(false);
            musicTrack2.SetActive(false);
        }

        private void Start()
        {
            int temp = Random.Range(0, 1);
        
            switch (temp)
            {
                case 0:
                    musicTrack1.SetActive(true);
                    break;
                case 1:
                    musicTrack2.SetActive(true);
                    break;
            }
        }
    }
}