using UnityEngine;
using System.Collections.Generic;

namespace ClashGame.Combat
{
    public class Troop : MonoBehaviour
    {
        [Header("Troop Stats")]
        public string troopName = "Barbarian";
        public int hitPoints = 100;
        public int damage = 10;
        public float attackSpeed = 1.0f; // seconds between attacks
        public float movementSpeed = 3.0f;
        public int range = 1;
        public int housingSpace = 1;
        
        [Header("Current State")]
        public int currentHP;
        public bool isAttacking = false;
        public Transform target;
        public Building currentTargetBuilding;
        
        private float attackCooldown = 0f;
        private Rigidbody2D rb;
        private Animator animator;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            currentHP = hitPoints;
        }
        
        private void Start()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.deployedTroops.Add(this);
            }
        }
        
        private void Update()
        {
            if (currentHP <= 0)
            {
                Die();
                return;
            }
            
            if (!BattleManager.Instance.isBattleActive) return;
            
            FindTarget();
            
            if (target != null)
            {
                MoveToTarget();
                
                float distance = Vector2.Distance(transform.position, target.position);
                if (distance <= range)
                {
                    Attack();
                }
            }
            
            if (attackCooldown > 0)
            {
                attackCooldown -= Time.deltaTime;
            }
        }
        
        private void FindTarget()
        {
            if (currentTargetBuilding != null && !currentTargetBuilding.IsDestroyed)
            {
                target = currentTargetBuilding.transform;
                return;
            }
            
            // Find nearest enemy building
            Building nearestBuilding = null;
            float nearestDistance = float.MaxValue;
            
            foreach (var building in BattleManager.Instance.enemyBuildings)
            {
                if (building != null && !building.IsDestroyed)
                {
                    float distance = Vector2.Distance(transform.position, building.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestBuilding = building;
                    }
                }
            }
            
            if (nearestBuilding != null)
            {
                currentTargetBuilding = nearestBuilding;
                target = nearestBuilding.transform;
            }
        }
        
        private void MoveToTarget()
        {
            if (target == null) return;
            
            Vector2 direction = (target.position - transform.position).normalized;
            transform.position += (Vector3)(direction * movementSpeed * Time.deltaTime);
            
            // Face the target
            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        
        private void Attack()
        {
            if (attackCooldown > 0 || target == null) return;
            
            // Deal damage to building
            Building building = target.GetComponent<Building>();
            if (building != null)
            {
                building.TakeDamage(damage);
            }
            
            attackCooldown = attackSpeed;
            
            // Play attack animation
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }
        
        public void TakeDamage(int damageAmount)
        {
            currentHP -= damageAmount;
            
            if (currentHP <= 0)
            {
                Die();
            }
        }
        
        private void Die()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.deployedTroops.Remove(this);
            }
            
            // Play death animation
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }
            
            Destroy(gameObject, 1f); // Destroy after animation
        }
        
        private void OnDestroy()
        {
            if (BattleManager.Instance != null && BattleManager.Instance.deployedTroops.Contains(this))
            {
                BattleManager.Instance.deployedTroops.Remove(this);
            }
        }
    }
}
