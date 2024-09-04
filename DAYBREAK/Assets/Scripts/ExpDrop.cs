using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpDrop : MonoBehaviour
{
    [SerializeField] int expAmount;

    public int ExpAmount { get => expAmount; set => expAmount = value; }
}
