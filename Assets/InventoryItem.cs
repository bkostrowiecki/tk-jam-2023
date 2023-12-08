using System;
using UniRx;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public InventoryItemSO inventoryItemSO = null;
    public int amount = 0;
    public int blood = 0;

    public BehaviorSubject<int> amountSubject = new BehaviorSubject<int>(0);
    public BehaviorSubject<int> bloodSubject = new BehaviorSubject<int>(0);

    public IObservable<int> AmountObservable => amountSubject.AsObservable();
    public IObservable<int> BloodObservable => bloodSubject.AsObservable();

    public void Emit()
    {
        amountSubject.OnNext(amount);
        bloodSubject.OnNext(blood);
    }

    public void AddAmount(int amount)
    {
        this.amount += amount;

        this.amount = Mathf.Max(0, this.amount);

        amountSubject.OnNext(this.amount);
    }

    public void UseAmount(int amount)
    {
        this.amount -= amount;

        this.amount = Mathf.Max(0, this.amount);

        amountSubject.OnNext(this.amount);
    }

    public void AddBlood(int bloodAmount)
    {
        this.blood += bloodAmount;

        bloodSubject.OnNext(this.amount);
    }

    public bool CanSacrifice => this.blood >= inventoryItemSO.requiredBlood;
}
