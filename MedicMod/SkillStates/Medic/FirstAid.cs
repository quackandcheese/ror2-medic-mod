using EntityStates;
using MedicMod.Content;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MedicMod.SkillStates
{
    public class FirstAid : BaseSkillState
    {
        public static float duration = 0.5f;
        public static float healCoefficient = Modules.StaticValues.healCoefficient;

        private MedicTracker tracker;
        private bool exitPending;
        private float exitCountdown;
        private float entryCountdown;

        private float healAmount;

        public override void OnEnter()
        {
            base.OnEnter();
            // Hold pill bottle anim

            // Replace with custom tracker component that targets teammates instead of enemies
            tracker = base.gameObject.GetComponent<MedicTracker>();
            tracker.enabled = true;
            this.entryCountdown = 0.1f;
            this.exitCountdown = 0.25f;
            exitPending = false;
        }

        public override void OnExit()
        {
            tracker.enabled = false;

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!isAuthority)
                return;

            this.entryCountdown -= Time.fixedDeltaTime;
            if (this.exitPending)
            {
                this.exitCountdown -= Time.fixedDeltaTime;
                if (this.exitCountdown <= 0f)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
            else if (base.inputBank && this.entryCountdown <= 0f)
            {
                if (inputBank.skill4.justPressed)
                {
                    this.exitPending = true;
                }
                // Maybe if eventually made it heal gradually and not in an instant, make it so you can heal while full health
                else if (inputBank.skill1.down)
                {
                    HealthComponent allyHealth = tracker.GetTrackingTarget().healthComponent;
                    if (allyHealth.health >= allyHealth.fullHealth)
                        return;

                    healAmount = allyHealth.fullHealth * healCoefficient;
                    allyHealth.Heal(healAmount, default(ProcChainMask), false);
                    // Throw pills anim + projectiles?
                    this.exitPending = true;
                    StartCooldown();
                }
                else if (inputBank.skill2.down)
                {
                    if (healthComponent.health >= healthComponent.fullHealth)
                        return;

                    healAmount = healthComponent.fullHealth * healCoefficient;
                    healthComponent.Heal(healAmount, default(ProcChainMask), false);
                    // Downing pill bottle anim
                    this.exitPending = true;
                    StartCooldown();
                }
            }
        }

        private void StartCooldown()
        {
            GenericSkill skill = base.skillLocator.GetSkill(SkillSlot.Special);
            if (skill)
            {
                skill.DeductStock(1);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
