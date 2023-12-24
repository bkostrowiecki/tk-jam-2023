using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;
using MoreMountains.Feedbacks;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ReactDistance
{
    public PlayerReactArea area;
    public float distance;
}

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    public bool isDebugging = false;

    public GameObject statesContainer;
    [Header("Saves")]
    public ItemsFeederSO itemsFeederSO;

    [Header("Input")]
    public Transform playerRotationBaseTransform;
    [Header("Internals")]
    public Killable killable;
    public Animator animator;
    public WeaponStates weaponStates;

    [Header("Gravity")]
    public float gravity = 10f;
    public float maxGravityVelocity = 100f;
    float gravityVelocity;
    private BasePlayerState[] states;
    [Header("Hell")]
    public float hellHeight = -20f;
    public float snapshotTime = 1f;
    float lastSnapshotTimer = 0f;
    public List<Vector3> snapshots = new();
    public LayerMask hellDetectionLayers;

    [Header("Collectable")]
    public LayerMask collectablesLayerMask;
    float collectablesDetectionRadius = 0.5f;

    [Header("Reactions")]
    public List<ReactDistance> playerReactAreas = new();

    [Header("Immortality")]
    public MMF_Player becomeImmortalFeedbacks = new();
    public float hitTimeImmortality = 0.5f;

    [Header("Stamina")]
    public int maxStamina = 100;
    BehaviorSubject<int> maxStaminaSubject;
    public IObservable<int> MaxStaminaObservable => maxStaminaSubject.AsObservable();
    int currentStamina = 0;
    BehaviorSubject<int> currentStaminaSubject;
    public IObservable<int> CurrentStaminaObservable => currentStaminaSubject.AsObservable();
    public int staminaJumpUsage = 10;
    public float fullRegenerationTime = 2.5f;
    public float regenerationDelay = 0.5f;
    private float lastStaminaUsageTimer;

    [Header("Bloodlust")]
    public int bloodIncrease = 10;

    [Header("Inventory")]
    public Inventory inventory;
    public PotionUsages potionUsages;
    public float potionUseBreakTime = 3f;
    float potionUseTimer;
    public InventoryItemSO selectedPotionSO;
    public InventoryItemSO selectedWeaponSO;

    BehaviorSubject<InventoryItemSO> selectedPotionSOSubject;
    BehaviorSubject<int> selectedPotionAmountSubject;
    BehaviorSubject<InventoryItemSO> selectedWeaponSOSubject;
    public IObservable<InventoryItemSO> SelectedPotionSOObservable => selectedPotionSOSubject.AsObservable();
    public IObservable<int> SelectedPotionAmountObservable => selectedPotionAmountSubject.AsObservable();
    public IObservable<InventoryItemSO> SelectedWeaponSOObservable => selectedWeaponSOSubject.AsObservable();
    public IObservable<List<InventoryItem>> InventoryItemsObservable => inventory.InventoryItemsObservable;

    [Header("Death")]
    public float dieReactionDelay = 2.4f;
    public UnityEvent onDied = new UnityEvent();
    private bool isMovementHolded;
    private float cachedAnimatorSpeed;
    bool canAttack;
    private InventoryItem selectedPotion;

    public bool shouldStartOver = false;

    void Awake()
    {
        states = statesContainer.GetComponentsInChildren<BasePlayerState>(true);

        foreach (var child in states)
        {
            child.playerController = this;
            child.isDebugging = isDebugging;
            child.gameObject.SetActive(false);
        }

        states[0].gameObject.SetActive(true);

        lastSnapshotTimer = Time.time;
        MakeSnapshot();

        currentStamina = maxStamina;
        currentStaminaSubject = new BehaviorSubject<int>(currentStamina);
        maxStaminaSubject = new BehaviorSubject<int>(maxStamina);

        selectedPotionSOSubject = new BehaviorSubject<InventoryItemSO>(selectedPotionSO);
        selectedPotionAmountSubject = new BehaviorSubject<int>(inventory.GetInventoryItemAmount(selectedPotionSO));
        selectedWeaponSOSubject = new BehaviorSubject<InventoryItemSO>(selectedWeaponSO);

        if (!shouldStartOver)
        {
            var path = Application.persistentDataPath + "/rogal";

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            InventorySaveDto data = formatter.Deserialize(stream) as InventorySaveDto;
            stream.Close();

            inventory.ApplySave(data, itemsFeederSO);

            SetWeapon(itemsFeederSO.FindByName(data.selectedWeapon));
            SetPotion(itemsFeederSO.FindByName(data.selectedPotion));
        }
        else
        {
            SetWeapon(selectedWeaponSO);
            SetPotion(selectedPotionSO);
        }

        inventory.Emit();
    }

    void OnDisable()
    {
        var path = Application.persistentDataPath + "/rogal";

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.OpenOrCreate);

        var saveDto = inventory.InventorySaveDto;
        saveDto.selectedWeapon = selectedWeaponSO?.itemName;
        saveDto.selectedPotion = selectedPotionSO?.itemName;

        formatter.Serialize(stream, saveDto);
        stream.Close();
    }

    public Vector3 AddGravity(Vector3 movement)
    {
        gravityVelocity += gravity * Time.deltaTime;

        gravityVelocity = Mathf.Min(maxGravityVelocity, gravityVelocity);

        if (characterController.isGrounded)
        {
            gravityVelocity = -0.2f;
        }

        return movement + Vector3.down * gravityVelocity;
    }

    public IEnumerator ResetTriggerAsync(string trigger, float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.ResetTrigger(trigger);
    }

    public IEnumerator SetTriggerAsync(string trigger, float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetTrigger(trigger);
    }

    public Vector3 AddDiminishedGravity(Vector3 movement)
    {
        gravityVelocity += gravity * 0.1f * Time.deltaTime;

        gravityVelocity = Mathf.Min(maxGravityVelocity, gravityVelocity);

        if (characterController.isGrounded)
        {
            gravityVelocity = -0.2f;
        }

        return movement + Vector3.down * gravityVelocity;
    }

    public void ZeroGravity()
    {
        gravityVelocity = 0f;
    }

    public void ActivateState(BasePlayerState basePlayerState)
    {
        foreach (var child in states)
        {
            child.gameObject.SetActive(child == basePlayerState);
        }
    }

    void Update()
    {
        if (transform.position.y < hellHeight)
        {
            TakeHellDamage();
            RecoverPosition();
        }

        if (!InputExtensions.MouseOverUI() && Input.GetButtonDown("Fire2"))
        {
            TryUsePotion();
        }

        if (lastSnapshotTimer + snapshotTime < Time.time)
        {
            Ray ray = new Ray(transform.position + characterController.height * Vector3.down * 0.5f, Vector3.down);

            if (Physics.Raycast(ray, 5f, hellDetectionLayers))
            {
                MakeSnapshot();
                lastSnapshotTimer = Time.time;
            }
        }

        DetectCollectable();

        if (lastStaminaUsageTimer + regenerationDelay < Time.time)
        {
            var step = (maxStamina / fullRegenerationTime) * Time.deltaTime;
            IncreaseStamina(step);
        }
    }

    void IncreaseStamina(float step)
    {
        currentStamina = Mathf.Clamp(Mathf.CeilToInt((float)currentStamina + step), 0, maxStamina);
        currentStaminaSubject.OnNext(currentStamina);
    }

    void LateUpdate()
    {
        ResetPossibleReactions();
    }

    void ResetPossibleReactions()
    {
        playerReactAreas.Clear();
    }

    void DetectCollectable()
    {
        var collectablesOverlapping = Physics.OverlapSphere(transform.position, collectablesDetectionRadius, collectablesLayerMask);

        foreach (var collectableOverlapping in collectablesOverlapping)
        {
            var collectable = collectableOverlapping.GetComponent<Collectable>();

            collectable.Collect();
        }
    }

    #if UNITY_EDITOR
    void OnGUI()
    {
        GUI.Label(new Rect(40, 40, 120, 120), new GUIContent(playerReactAreas.ToArray().ToString()));
    }
    #endif

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectablesDetectionRadius);
    }

    void RecoverPosition()
    {
        characterController.enabled = false;
        characterController.transform.SetPositionAndRotation(snapshots[1] + Vector3.up, characterController.transform.rotation);

        StartCoroutine(RecoverPositionAsync());
    }

    IEnumerator RecoverPositionAsync()
    {
        yield return new WaitForEndOfFrame();
        characterController.enabled = true;
    }

    void MakeSnapshot()
    {
        if (snapshots.Count > 2)
        {
            snapshots.RemoveAt(0);
        }
        snapshots.Add(transform.position);
    }

    void TakeHellDamage()
    {
        killable.TakeDamage(new HellDamage(killable.maxHealthPoints));

        MakeImmortalForTime(hitTimeImmortality, 0.1f);
    }

    public void MakeImmortal()
    {
        MakeImmortalForTime(hitTimeImmortality, 0.1f);
    }

    public void MakeReactionPossible(PlayerReactArea playerReactArea)
    {
        var found = playerReactAreas.Find((item) => item.area == playerReactArea);

        if (found != null)
        {
            found.distance = CalculateDistance(playerReactArea.transform.position);
        }
        else
        {
            var newEntry = new ReactDistance
            {
                distance = CalculateDistance(playerReactArea.transform.position),
                area = playerReactArea
            };

            playerReactAreas.Add(newEntry);
        }
    }

    public void MakeImmortalForTime(float time, float feedbackDelay)
    {
        StartCoroutine(MakeImmortalForTimeAsync(time, feedbackDelay));
    }

    public IEnumerator MakeImmortalForTimeAsync(float time, float feedbackDelay)
    {
        killable.canTakeDamage = false;

        yield return new WaitForSeconds(feedbackDelay);
        becomeImmortalFeedbacks?.PlayFeedbacks();

        yield return new WaitForSeconds(time - feedbackDelay);

        becomeImmortalFeedbacks?.StopFeedbacks();
        killable.canTakeDamage = true;
    }

    float CalculateDistance(Vector3 position)
    {
        return (transform.position - position).sqrMagnitude;
    }

    public void UseStamina()
    {
        currentStamina = Mathf.Clamp(currentStamina - staminaJumpUsage, 0, maxStamina);
        currentStaminaSubject.OnNext(currentStamina);

        lastStaminaUsageTimer = Time.time;
    }

    public bool CanUseStamina => currentStamina > 0;

    public bool CanSacrifice
    {
        get
        {
            var inventoryItem = inventory.FindInventoryItemBySOWithHighestBlood(selectedWeaponSO);

            return inventoryItem != null ? inventoryItem.CanSacrifice : false;
        }
    }

    public void TryUsePotion()
    {
        if (potionUseTimer + potionUseBreakTime >= Time.time)
        {
            return;
        }

        if (selectedPotionSO == null)
        {
            return;
        }

        if (inventory.GetInventoryItemAmount(selectedPotionSO) > 0)
        {
            inventory.UseInventoryItem(selectedPotionSO, 1);
            potionUsages.UsePotion(selectedPotionSO);

            potionUseTimer = Time.time;

            var amount = inventory.GetInventoryItemAmount(selectedPotionSO);
            selectedPotionAmountSubject.OnNext(amount);

            if (inventory.GetInventoryItemAmount(selectedPotionSO) == 0)
            {
                ClearSelectedPotion();
            }
        }
    }

    void ClearSelectedPotion()
    {
        selectedPotionSO = null;
        selectedPotionAmountSubject.OnNext(0);
        selectedPotionSOSubject.OnNext(null);
    }

    public void SetPotion(InventoryItemSO inventoryItemSO)
    {
        if (inventoryItemSO == null)
        {
            ClearSelectedPotion();

            return;
        }

        if (!inventoryItemSO.IsPotion)
        {
            throw new System.Exception("Selected inventory item is not a potion " + inventoryItemSO.name);
        }

        selectedPotionSO = inventoryItemSO;
        selectedPotionSOSubject.OnNext(selectedPotionSO);
        
        var amount = inventory.GetInventoryItemAmount(inventoryItemSO);
        selectedPotionAmountSubject.OnNext(amount);
    }

    public void SetWeapon(InventoryItemSO inventoryItemSO)
    {
        if (inventoryItemSO == null)
        {
            ClearSelectedWeapon();

            return;
        }

        if (!inventoryItemSO.IsWeapon)
        {
            throw new System.Exception("Selected inventory item is not a weapon " + inventoryItemSO.name);
        }

        selectedWeaponSO = inventoryItemSO;
        weaponStates.SetCurrentWeapon(inventoryItemSO);
        selectedWeaponSOSubject.OnNext(selectedWeaponSO);
    }

    void ClearSelectedWeapon()
    {
        selectedWeaponSO = null;
        weaponStates.SetCurrentWeapon(null);
        selectedWeaponSOSubject.OnNext(selectedWeaponSO);
    }

    public void Die()
    {
        foreach (var state in states)
        {
            state.gameObject.SetActive(false);
        }

        animator.SetTrigger("die");
        killable.canTakeDamage = false;

        StartCoroutine(ShowGameOverAsync());
    }

    IEnumerator ShowGameOverAsync()
    {
        yield return new WaitForSecondsRealtime(dieReactionDelay);

        onDied?.Invoke();
    }

    public void HoldMovement(float modifiedSpeed)
    {
        if (!isMovementHolded)
        {
            cachedAnimatorSpeed = animator.speed;
        }
        animator.speed = modifiedSpeed;
        statesContainer.gameObject.SetActive(false);
        isMovementHolded = true;
    }

    public void RestoreMovement()
    {
        animator.speed = 1;
        statesContainer.gameObject.SetActive(true);
    }

    public void HoldAttacks()
    {
        canAttack = false;
        weaponStates.gameObject.SetActive(false);
    }

    public void RestoreAttacks()
    {
        canAttack = true;
        weaponStates.gameObject.SetActive(true);
    }

    public void UseWeapon()
    {
        inventory.UseInventoryItem(selectedWeaponSO, 1);
        var amount = inventory.GetInventoryItemAmount(selectedWeaponSO);

        if (amount == 0)
        {
            ClearSelectedWeapon();
        }
        else
        {
            selectedWeaponSOSubject.OnNext(selectedWeaponSO);
        }
    }

    public void GainBlood()
    {
        var inventoryItem = inventory.FindInventoryItemBySOWithHighestBlood(selectedWeaponSO);
        inventoryItem.AddBlood(bloodIncrease);
    }

    public Vector3 CalculateMouseDirection()
    {
        Ray screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.up, transform.position);
        plane.Raycast(screenRay, out var distance);

        Vector3 worldPoint = screenRay.GetPoint(distance);

        var direction = (worldPoint - transform.position).normalized;

        return direction;
    }

    public void React()
    {
        playerReactAreas.Sort(SortAreas);

        if (playerReactAreas.Count > 0)
        {
            var first = playerReactAreas[0];

            first.area.React();
        }
    }

    int SortAreas(ReactDistance x, ReactDistance y)
    {
        float sort = y.distance - x.distance;
        return sort < 0 ? -1 : 1;
    }

    public void TakeItems(Inventory inventory)
    {
        this.inventory.TakeFromOtherInventory(inventory);
    }
}

public class HellDamage : BaseDamage
{
    int playerMaxHealth;

    public HellDamage(int playerMaxHealth)
    {
        this.playerMaxHealth = playerMaxHealth;
    }

    public int CalculateDamage()
    {
        return playerMaxHealth / 5;
    }
}