using System.Collections.Generic;
using UnityEngine;

namespace Adastra
{
    public class RandomMusicLoader : MonoBehaviour
    {
        [SerializeField] private List<GameObject> musicTracks = new List<GameObject>();

        private void Awake()
        {
            foreach (var go in musicTracks)
            {
                go.SetActive(false);
            }
        }

        private void Start()
        {
            int temp = Random.Range(0, musicTracks.Count);
        
            switch (temp)
            {
                case 0:
                    musicTracks[0].SetActive(true);
                    break;
                case 1:
                    musicTracks[1].SetActive(true);
                    break;
            }
        }
    }
}