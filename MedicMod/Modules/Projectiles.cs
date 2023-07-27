using MedicMod.Content;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace MedicMod.Modules
{
    internal static class Projectiles
    {
        internal static GameObject bombPrefab;
        internal static GameObject gauzePrefab;

        internal static void RegisterProjectiles()
        {
            CreateBomb();
            CreateGauze();

            AddProjectile(bombPrefab);
            AddProjectile(gauzePrefab);
        }

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Content.AddProjectilePrefab(projectileToAdd);
        }

        private static void CreateBomb()
        {
            bombPrefab = CloneProjectilePrefab("CommandoGrenadeProjectile", "HenryBombProjectile");

            ProjectileImpactExplosion bombImpactExplosion = bombPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(bombImpactExplosion);

            bombImpactExplosion.blastRadius = 16f;
            bombImpactExplosion.destroyOnEnemy = true;
            bombImpactExplosion.lifetime = 12f;
            bombImpactExplosion.impactEffect = Modules.Assets.bombExplosionEffect;
            //bombImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("HenryBombExplosion");
            bombImpactExplosion.timerAfterImpact = true;
            bombImpactExplosion.lifetimeAfterImpact = 0.1f;

            ProjectileController bombController = bombPrefab.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("HenryBombGhost") != null) bombController.ghostPrefab = CreateGhostPrefab("HenryBombGhost");
            bombController.startSound = "";
        }

        private static void CreateGauze()
        {
            gauzePrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Gravekeeper/GravekeeperHookProjectileSimple.prefab").WaitForCompletion(), "GauzeProjectile", true);
            Object.Destroy(gauzePrefab.GetComponent<ProjectileSingleTargetImpact>());
            gauzePrefab.AddComponent<ProjectileGauzeController>();
            ProjectileController projectileController = gauzePrefab.GetComponent<ProjectileController>();
            projectileController.allowPrediction = false;
            //component.ghostPrefab = gameObject;
            gauzePrefab.GetComponent<ProjectileSimple>().desiredForwardSpeed = 120f;
            gauzePrefab.GetComponent<ProjectileSimple>().lifetime = 0.6f;
            gauzePrefab.GetComponent<ProjectileStickOnImpact>().ignoreCharacters = false;
            //ContentAddition.AddProjectile(gauzePrefab);

            /* -------- OLD VERSION BELOW -------- */

            /*gauzePrefab = CloneProjectilePrefab("loaderhook", "GauzeProjectile");

            ProjectileGauzeController grappleController = gauzePrefab.AddComponent<ProjectileGauzeController>();
            ProjectileGrappleController oldGrapController = gauzePrefab.GetComponent<ProjectileGrappleController>();

            grappleController.acceleration = oldGrapController.acceleration;
            grappleController.escapeForceMultiplier = oldGrapController.escapeForceMultiplier;
            grappleController.initiallMoveImpulse = oldGrapController.initiallMoveImpulse;
            grappleController.initialLookImpulse = oldGrapController.initialLookImpulse;
            grappleController.lookAcceleration = oldGrapController.lookAcceleration;
            grappleController.lookAccelerationRampUpCurve = oldGrapController.lookAccelerationRampUpCurve;
            grappleController.lookAccelerationRampUpDuration = oldGrapController.lookAccelerationRampUpDuration;
            grappleController.maxHookDistancePitchModifier = oldGrapController.maxHookDistancePitchModifier;
            grappleController.minHookDistancePitchModifier = oldGrapController.minHookDistancePitchModifier;
            grappleController.maxTravelDistance = oldGrapController.maxTravelDistance;
            grappleController.moveAcceleration = oldGrapController.moveAcceleration;
            grappleController.nearBreakDistance = oldGrapController.nearBreakDistance;
            grappleController.normalOffset = oldGrapController.normalOffset;
            grappleController.hookDistanceRTPCstring = oldGrapController.hookDistanceRTPCstring;
            grappleController.enterSoundString = oldGrapController.enterSoundString;
            grappleController.exitSoundString = oldGrapController.exitSoundString;
            grappleController.ropeEndTransform = grappleController.gameObject.transform.Find("RopeEnd");
            grappleController.ownerHookStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.GauzeGrapple));

            gauzePrefab.GetComponent<EntityStateMachine>().initialStateType = new EntityStates.SerializableEntityStateType("MedicMod.Content.ProjectileGauzeController+FlyState, MedicMod, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null");//new EntityStates.SerializableEntityStateType(typeof(ProjectileGauzeController));
            gauzePrefab.GetComponent<EntityStateMachine>().mainStateType = new EntityStates.SerializableEntityStateType("MedicMod.Content.ProjectileGauzeController+FlyState, MedicMod, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null");


            GameObject.Destroy(gauzePrefab.GetComponent<ProjectileGrappleController>());*/

            //ProjectileController projectileController = gauzePrefab.GetComponent<ProjectileController>();
            //if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("HenryBombGhost") != null) projectileController.ghostPrefab = CreateGhostPrefab("HenryBombGhost");
            //projectileController.startSound = "";

        }

        private static void InitializeImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = 1f;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.falloffModel = RoR2.BlastAttack.FalloffModel.None;
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeAfterImpact = 0f;
            projectileImpactExplosion.lifetimeRandomOffset = 0f;
            projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
            projectileImpactExplosion.timerAfterImpact = false;

            projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}