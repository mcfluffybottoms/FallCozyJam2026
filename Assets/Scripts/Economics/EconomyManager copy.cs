// using System;
// using UnityEngine;

// public class EconomyManager : MonoBehaviour
// {
//     [SerializeField] private LevelItemsCollector collector;
//     public MoneySaver MoneySaver = new(100);

//     private void OnEnable()
//     {
//         if (collector != null)
//             collector.OnWeedCollected += HandleWeedCollected;
//     }

//     private void OnDisable()
//     {
//         if (collector != null)
//             collector.OnWeedCollected -= HandleWeedCollected;
//     }

//     private void HandleWeedCollected(LevelItems.Weed weed)
//     {
//         if (weed == null) return;

//         int reward = weed.Reward;
//         MoneySaver.IncreaseMoney(reward);
//     }
// }