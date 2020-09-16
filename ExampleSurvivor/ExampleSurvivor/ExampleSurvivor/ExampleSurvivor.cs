using System;
using System.Reflection;
using System.Collections.Generic;
using BepInEx;
using R2API;
using R2API.Utils;
using EntityStates;
using EntityStates.ExampleSurvivorStates;
using RoR2;
using RoR2.Skills;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using KinematicCharacterController;

namespace ExampleSurvivor
{

    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin(MODUID, "Guts", "1.0.0")] // put your own name and version here
    [R2APISubmoduleDependency(nameof(PrefabAPI), nameof(SurvivorAPI), nameof(LoadoutAPI), nameof(ItemAPI), nameof(DifficultyAPI), nameof(BuffAPI))] // need these dependencies for the mod to work properly


    public class ExampleSurvivor : BaseUnityPlugin
    {
        public const string MODUID = "com.SirHamburger.Guts"; // put your own names here

        public static GameObject characterPrefab; // the survivor body prefab
        public GameObject characterDisplay; // the prefab used for character select
        public GameObject doppelganger; // umbra shit

        public static ChildLocator childLocator;

        public static GameObject arrowProjectile; // prefab for our survivor's primary attack projectile

        private static readonly Color characterColor = new Color(0.55f, 0.55f, 0.55f); // color used for the survivor

        private void Awake()
        {
            Assets.PopulateAssets(); // first we load the assets from our assetbundle
            CreatePrefab(); // then we create our character's body prefab
            RegisterStates(); // register our skill entitystates for networking
            RegisterCharacter(); // and finally put our new survivor in the game
            CreateDoppelganger(); // not really mandatory, but it's simple and not having an umbra is just kinda lame
        }

        private static GameObject CreateModel(GameObject main)
        {
            Destroy(main.transform.Find("ModelBase").gameObject);
            Destroy(main.transform.Find("CameraPivot").gameObject);
            Destroy(main.transform.Find("AimOrigin").gameObject);

            // make sure it's set up right in the unity project
            GameObject model = Assets.MainAssetBundle.LoadAsset<GameObject>("GutsBerserker Variant");
            //@ArtifactOfDoom:Assets/Import/artifactofdoom_icon/ArtifactDoomEnabled.png
            return model;
        }
        public static GameObject gutsModel;
        internal static void CreatePrefab()
        {
            Debug.LogWarning("In CreatePrefab");
            // first clone the commando prefab so we can turn that into our own survivor
            characterPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody"), "ExampleSurvivorBody", true, "/home/sirhamburger/Git/Risk-of-Rain-2-Guts-Code/ExampleSurvivor/ExampleSurvivor/ExampleSurvivor/ExampleSurvivor/ExampleSurvivor.cs", "CreatePrefab", 151);
            if(characterPrefab ==null)
                Debug.LogError("characterPrefab == null");
            characterPrefab.GetComponent<NetworkIdentity>().localPlayerAuthority = true;

            // create the model here, we're gonna replace commando's model with our own
            GameObject model = CreateModel(characterPrefab);
            gutsModel = model;
                        Debug.LogWarning("before get ModelBase");

            GameObject gameObject = new GameObject("ModelBase");
            gameObject.transform.parent = characterPrefab.transform;
            gameObject.transform.localPosition = new Vector3(0f, -0.81f, 0f);
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                        Debug.LogWarning("before get CameraPivot");

            GameObject gameObject2 = new GameObject("CameraPivot");
            gameObject2.transform.parent = gameObject.transform;
            gameObject2.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            gameObject2.transform.localRotation = Quaternion.identity;
            gameObject2.transform.localScale = Vector3.one;
                        Debug.LogWarning("before get AimOrigin");
            GameObject gameObject3 = new GameObject("AimOrigin");
            if(gameObject ==null)
                Debug.LogError("GameIbject ==null")
;
            gameObject3.transform.parent = gameObject.transform;
            gameObject3.transform.localPosition = new Vector3(0f, 1.4f, 0f);
            gameObject3.transform.localRotation = Quaternion.identity;
            gameObject3.transform.localScale = Vector3.one;

            if(model ==null)
                Debug.LogError("model ==null");
            Transform transform = model.transform;
            transform.parent = gameObject.transform;
            transform.localPosition = Vector3.zero;
            transform.localScale = new Vector3(1.01f, 1.01f, 1.01f);
            transform.localRotation = Quaternion.identity;

                        Debug.LogWarning("before get CharacterDirection");

            CharacterDirection characterDirection = characterPrefab.GetComponent<CharacterDirection>();
            characterDirection.moveVector = Vector3.zero;
            characterDirection.targetTransform = gameObject.transform;
            characterDirection.overrideAnimatorForwardTransform = null;
            characterDirection.rootMotionAccumulator = null;
            characterDirection.modelAnimator = model.GetComponentInChildren<Animator>();
            characterDirection.driveFromRootRotation = false;
            characterDirection.turnSpeed = 720f;

            // set up the character body here
            CharacterBody bodyComponent = characterPrefab.GetComponent<CharacterBody>();
            bodyComponent.bodyIndex = -1;
            bodyComponent.baseNameToken = "EXAMPLESURVIVOR_NAME"; // name token
            bodyComponent.subtitleNameToken = "EXAMPLESURVIVOR_SUBTITLE"; // subtitle token- used for umbras
            bodyComponent.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes;
            bodyComponent.rootMotionInMainState = false;
            bodyComponent.mainRootSpeed = 0;
            bodyComponent.baseMaxHealth = 90;
            bodyComponent.levelMaxHealth = 24;
            bodyComponent.baseRegen = 0.5f;
            bodyComponent.levelRegen = 0.25f;
            bodyComponent.baseMaxShield = 0;
            bodyComponent.levelMaxShield = 0;
            bodyComponent.baseMoveSpeed = 7;
            bodyComponent.levelMoveSpeed = 0;
            bodyComponent.baseAcceleration = 80;
            bodyComponent.baseJumpPower = 15;
            bodyComponent.levelJumpPower = 0;
            bodyComponent.baseDamage = 15;
            bodyComponent.levelDamage = 3f;
            bodyComponent.baseAttackSpeed = 1;
            bodyComponent.levelAttackSpeed = 0;
            bodyComponent.baseCrit = 1;
            bodyComponent.levelCrit = 0;
            bodyComponent.baseArmor = 0;
            bodyComponent.levelArmor = 0;
            bodyComponent.baseJumpCount = 1;
            bodyComponent.sprintingSpeedMultiplier = 1.45f;
            bodyComponent.wasLucky = false;
            bodyComponent.hideCrosshair = false;
            bodyComponent.aimOriginTransform = gameObject3.transform;
            bodyComponent.hullClassification = HullClassification.Human;
            bodyComponent.portraitIcon = Assets.charPortrait;
            bodyComponent.isChampion = false;
            bodyComponent.currentVehicle = null;
            bodyComponent.skinIndex = 0U;
            Debug.LogWarning("before get CharacterMotor");


            // the charactermotor controls the survivor's movement and stuff
            CharacterMotor characterMotor = characterPrefab.GetComponent<CharacterMotor>();
            characterMotor.walkSpeedPenaltyCoefficient = 1f;
            characterMotor.characterDirection = characterDirection;
            characterMotor.muteWalkMotion = false;
            characterMotor.mass = 100f;
            characterMotor.airControl = 0.25f;
            characterMotor.disableAirControlUntilCollision = false;
            characterMotor.generateParametersOnAwake = true;


            InputBankTest inputBankTest = characterPrefab.GetComponent<InputBankTest>();
            inputBankTest.moveVector = Vector3.zero;

            Debug.LogWarning("before get CameraTargetParams");

            CameraTargetParams cameraTargetParams = characterPrefab.GetComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<CameraTargetParams>().cameraParams;
            cameraTargetParams.cameraPivotTransform = null;
            cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
            cameraTargetParams.recoil = Vector2.zero;
            cameraTargetParams.idealLocalCameraPos = Vector3.zero;
            cameraTargetParams.dontRaycastToPivot = false;

            // this component is used to locate the character model(duh), important to set this up here
            Debug.LogWarning("before get ModelLocator");
            ModelLocator modelLocator = characterPrefab.GetComponent<ModelLocator>();
            modelLocator.modelTransform = transform;
            modelLocator.modelBaseTransform = gameObject.transform;
            modelLocator.dontReleaseModelOnDeath = false;
            modelLocator.autoUpdateModelTransform = true;
            modelLocator.dontDetatchFromParent = false;
            modelLocator.noCorpse = false;
            modelLocator.normalizeToFloor = false; // set true if you want your character to rotate on terrain like acrid does
            modelLocator.preserveModel = false;

            // childlocator is something that must be set up in the unity project, it's used to find any child objects for things like footsteps or muzzle flashes
            // also important to set up if you want quality
                        Debug.LogWarning("before get ChildLocator");

            ChildLocator childLocator = model.GetComponent<ChildLocator>();
            ExampleSurvivor.childLocator= childLocator;

            // this component is used to handle all overlays and whatever on your character, without setting this up you won't get any cool effects like burning or freeze on the character
            // it goes on the model object of course
                                    Debug.LogWarning("before add CharacterModel");

            CharacterModel characterModel = model.AddComponent<CharacterModel>();
            characterModel.body = bodyComponent;
            //Debug.LogError("model.GetComponentInChildren<SkinnedMeshRenderer>().name: " + model.GetComponentInChildren<SkinnedMeshRenderer>().material.name);
//
            // foreach(var element in characterModel.baseRendererInfos)
            //    Debug.LogError(element.defaultMaterial.name);
            //characterModel.baseRendererInfos = new CharacterModel.RendererInfo[model.GetComponentInChildren<SkinnedMeshRenderer>().materials.Length];
            //int pos = 0;
            //foreach(var material in model.GetComponentInChildren<SkinnedMeshRenderer>().materials)
            //{
            //    characterModel.baseRendererInfos[pos] =
            //            new CharacterModel.RendererInfo
            //            {
            //                defaultMaterial = material,
            //                renderer =model.GetComponentInChildren<SkinnedMeshRenderer>(),
            //                defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
            //                ignoreOverlays = false
            //        
            //    };
            //    pos++;
            //    Debug.LogWarning(material.name);
            //}
            //foreach(var element in characterModel.baseRendererInfos)
            //    Debug.LogError(element.defaultMaterial.name);
            //

            characterModel.autoPopulateLightInfos = true;
            characterModel.invisibilityCount = 0;
            characterModel.temporaryOverlays = new List<TemporaryOverlay>();

                                    Debug.LogWarning("before add TeamComponent");

            TeamComponent teamComponent = null;
            if (characterPrefab.GetComponent<TeamComponent>() != null) teamComponent = characterPrefab.GetComponent<TeamComponent>();
            else teamComponent = characterPrefab.GetComponent<TeamComponent>();
            teamComponent.hideAllyCardDisplay = false;
            teamComponent.teamIndex = TeamIndex.None;

            HealthComponent healthComponent = characterPrefab.GetComponent<HealthComponent>();
            healthComponent.health = 90f;
            healthComponent.shield = 0f;
            healthComponent.barrier = 0f;
            healthComponent.magnetiCharge = 0f;
            healthComponent.body = null;
            healthComponent.dontShowHealthbar = false;
            healthComponent.globalDeathEventChanceCoefficient = 1f;

            characterPrefab.GetComponent<Interactor>().maxInteractionDistance = 3f;
            characterPrefab.GetComponent<InteractionDriver>().highlightInteractor = true;

            // this disables ragdoll since the character's not set up for it, and instead plays a death animation
            CharacterDeathBehavior characterDeathBehavior = characterPrefab.GetComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathStateMachine = characterPrefab.GetComponent<EntityStateMachine>();
            characterDeathBehavior.deathState = new SerializableEntityStateType(typeof(GenericCharacterDeath));

                                    Debug.LogWarning("before add SfxLocator");

            // edit the sfxlocator if you want different sounds
            SfxLocator sfxLocator = characterPrefab.GetComponent<SfxLocator>();
            sfxLocator.deathSound = "Play_ui_player_death";
            sfxLocator.barkSound = "";
            sfxLocator.openSound = "";
            sfxLocator.landingSound = "Play_char_land";
            sfxLocator.fallDamageSound = "Play_char_land_fall_damage";
            sfxLocator.aliveLoopStart = "";
            sfxLocator.aliveLoopStop = "";

                                    Debug.LogWarning("before get Rigidbody");

            Rigidbody rigidbody = characterPrefab.GetComponent<Rigidbody>();
            rigidbody.mass = 100f;
            rigidbody.drag = 0f;
            rigidbody.angularDrag = 0f;
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            rigidbody.interpolation = RigidbodyInterpolation.None;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rigidbody.constraints = RigidbodyConstraints.None;

                                    Debug.LogWarning("before get CapsuleCollider");

            CapsuleCollider capsuleCollider = characterPrefab.GetComponent<CapsuleCollider>();
            capsuleCollider.isTrigger = false;
            capsuleCollider.material = null;
            capsuleCollider.center = new Vector3(0f, 0f, 0f);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 1.82f;
            capsuleCollider.direction = 1;

                                    Debug.LogWarning("before get KinematicCharacterMotor");

            KinematicCharacterMotor kinematicCharacterMotor = characterPrefab.GetComponent<KinematicCharacterMotor>();
            kinematicCharacterMotor.CharacterController = characterMotor;
            kinematicCharacterMotor.Capsule = capsuleCollider;
            kinematicCharacterMotor.Rigidbody = rigidbody;

            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 1.82f;
            capsuleCollider.center = new Vector3(0, 0, 0);
            capsuleCollider.material = null;

            kinematicCharacterMotor.DetectDiscreteCollisions = false;
            kinematicCharacterMotor.GroundDetectionExtraDistance = 0f;
            kinematicCharacterMotor.MaxStepHeight = 0.2f;
            kinematicCharacterMotor.MinRequiredStepDepth = 0.1f;
            kinematicCharacterMotor.MaxStableSlopeAngle = 55f;
            kinematicCharacterMotor.MaxStableDistanceFromLedge = 0.5f;
            kinematicCharacterMotor.PreventSnappingOnLedges = false;
            kinematicCharacterMotor.MaxStableDenivelationAngle = 55f;
            kinematicCharacterMotor.RigidbodyInteractionType = RigidbodyInteractionType.None;
            kinematicCharacterMotor.PreserveAttachedRigidbodyMomentum = true;
            kinematicCharacterMotor.HasPlanarConstraint = false;
            kinematicCharacterMotor.PlanarConstraintAxis = Vector3.up;
            kinematicCharacterMotor.StepHandling = StepHandlingMethod.None;
            kinematicCharacterMotor.LedgeHandling = true;
            kinematicCharacterMotor.InteractiveRigidbodyHandling = true;
            kinematicCharacterMotor.SafeMovement = false;

            // this sets up the character's hurtbox, kinda confusing, but should be fine as long as it's set up in unity right
                                                Debug.LogWarning("before get HurtBoxGroup");

            HurtBoxGroup hurtBoxGroup = model.AddComponent<HurtBoxGroup>();

            HurtBox componentInChildren = model.GetComponentInChildren<CapsuleCollider>().gameObject.AddComponent<HurtBox>();
            componentInChildren.gameObject.layer = LayerIndex.entityPrecise.intVal;
            componentInChildren.healthComponent = healthComponent;
            componentInChildren.isBullseye = true;
            componentInChildren.damageModifier = HurtBox.DamageModifier.Normal;
            componentInChildren.hurtBoxGroup = hurtBoxGroup;
            componentInChildren.indexInGroup = 0;

            hurtBoxGroup.hurtBoxes = new HurtBox[]
            {
                componentInChildren
            };

            hurtBoxGroup.mainHurtBox = componentInChildren;
            hurtBoxGroup.bullseyeCount = 1;

            // this is for handling footsteps, not needed but polish is always good
                                                            Debug.LogWarning("before get FootstepHandler");

            FootstepHandler footstepHandler = model.AddComponent<FootstepHandler>();
            footstepHandler.baseFootstepString = "Play_player_footstep";
            footstepHandler.sprintFootstepOverrideString = "";
            footstepHandler.enableFootstepDust = true;
            footstepHandler.footstepDustPrefab = Resources.Load<GameObject>("Prefabs/GenericFootstepDust");

            // ragdoll controller is a pain to set up so we won't be doing that here..
            RagdollController ragdollController = model.AddComponent<RagdollController>();
            ragdollController.bones = null;
            ragdollController.componentsToDisableOnRagdoll = null;

            // this handles the pitch and yaw animations, but honestly they are nasty and a huge pain to set up so i didn't bother
            Debug.LogWarning("before add AimAnimator");
            AimAnimator aimAnimator = model.AddComponent<AimAnimator>();
            aimAnimator.inputBank = inputBankTest;
            aimAnimator.directionComponent = characterDirection;
            aimAnimator.pitchRangeMax = 55f;
            aimAnimator.pitchRangeMin = -50f;
            aimAnimator.yawRangeMin = -44f;
            aimAnimator.yawRangeMax = 44f;
            aimAnimator.pitchGiveupRange = 30f;
            aimAnimator.yawGiveupRange = 10f;
            aimAnimator.giveupDuration = 8f;
            Debug.LogWarning("finished CreatePrefab");
        }

        private void RegisterCharacter()
        {
            Debug.LogWarning("In Register Character");
            // now that the body prefab's set up, clone it here to make the display prefab
            characterDisplay = PrefabAPI.InstantiateClone(characterPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject, "ExampleSurvivorDisplay", true, "/home/sirhamburger/Git/Risk-of-Rain-2-Guts-Code/ExampleSurvivor/ExampleSurvivor/ExampleSurvivor/ExampleSurvivor/ExampleSurvivor.cs", "RegisterCharacter", 153);
            characterDisplay.AddComponent<NetworkIdentity>();

            // clone rex's syringe projectile prefab here to use as our own projectile
            arrowProjectile = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/SyringeProjectile"), "Prefabs/Projectiles/ExampleArrowProjectile", true, "/home/sirhamburger/Git/Risk-of-Rain-2-Guts-Code/ExampleSurvivor/ExampleSurvivor/ExampleSurvivor/ExampleSurvivor/ExampleSurvivor.cs", "RegisterCharacter", 155);

            // just setting the numbers to 1 as the entitystate will take care of those
            arrowProjectile.GetComponent<ProjectileController>().procCoefficient = 1f;
            arrowProjectile.GetComponent<ProjectileDamage>().damage = 1f;
            arrowProjectile.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;

            // register it for networking
            if (arrowProjectile) PrefabAPI.RegisterNetworkPrefab(arrowProjectile);

            // add it to the projectile catalog or it won't work in multiplayer
            ProjectileCatalog.getAdditionalEntries += list =>
            {
                list.Add(arrowProjectile);
            };


            // write a clean survivor description here!
            string desc = "Example Survivor something something.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Sample text 1." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Sample text 2." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Sample Text 3." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Sample Text 4.</color>" + Environment.NewLine + Environment.NewLine;

            // add the language tokens
            LanguageAPI.Add("EXAMPLESURVIVOR_NAME", "Example Survivor");
            LanguageAPI.Add("EXAMPLESURVIVOR_DESCRIPTION", desc);
            LanguageAPI.Add("EXAMPLESURVIVOR_SUBTITLE", "Template for Custom Survivors");

            // add our new survivor to the game~
            SurvivorDef survivorDef = new SurvivorDef
            {
                name = "EXAMPLESURVIVOR_NAME",
                unlockableName = "",
                descriptionToken = "EXAMPLESURVIVOR_DESCRIPTION",
                primaryColor = characterColor,
                bodyPrefab = characterPrefab,
                displayPrefab = characterDisplay
            };


            SurvivorAPI.AddSurvivor(survivorDef);

            // set up the survivor's skills here
            SkillSetup();

            // gotta add it to the body catalog too
            BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(characterPrefab);
            };
        }

        void SkillSetup()
        {
            // get rid of the original skills first, otherwise we'll have commando's loadout and we don't want that
            foreach (GenericSkill obj in characterPrefab.GetComponentsInChildren<GenericSkill>())
            {
                BaseUnityPlugin.DestroyImmediate(obj);
            }

            PassiveSetup();
            PrimarySetup();
            secondarySetup();
            
        }

        void RegisterStates()
        {
            // register the entitystates for networking reasons
            LoadoutAPI.AddSkill(typeof(ExampleSurvivorFireArrow));
            LoadoutAPI.AddSkill(typeof(GutsSwordAttack));
        }

        void PassiveSetup()
        {
            Debug.LogWarning("In PassiveSetup");
            // set up the passive skill here if you want
            SkillLocator component = characterPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("EXAMPLESURVIVOR_PASSIVE_NAME", "Passive");
            LanguageAPI.Add("EXAMPLESURVIVOR_PASSIVE_DESCRIPTION", "<style=cIsUtility>Doot</style> <style=cIsHealing>doot</style>.");

            component.passiveSkill.enabled = true;
            component.passiveSkill.skillNameToken = "EXAMPLESURVIVOR_PASSIVE_NAME";
            component.passiveSkill.skillDescriptionToken = "EXAMPLESURVIVOR_PASSIVE_DESCRIPTION";
            component.passiveSkill.icon = Assets.iconP;
            Debug.LogWarning("finished PassiveSetup");
        }

        void PrimarySetup()
        {
            SkillLocator component = characterPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("EXAMPLESURVIVOR_PRIMARY_CROSSBOW_NAME", "Crossbow");
            LanguageAPI.Add("EXAMPLESURVIVOR_PRIMARY_CROSSBOW_DESCRIPTION", "Fire an arrow, dealing <style=cIsDamage>200% damage</style>.");

            // set up your primary skill def here!

            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(ExampleSurvivorFireArrow));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 0f;
            mySkillDef.beginSkillCooldownOnSkillEnd = false;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Any;
            mySkillDef.isBullets = false;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = false;
            mySkillDef.noSprint = true;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.shootDelay = 0f;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Assets.icon1;
            mySkillDef.skillDescriptionToken = "EXAMPLESURVIVOR_PRIMARY_CROSSBOW_DESCRIPTION";
            mySkillDef.skillName = "EXAMPLESURVIVOR_PRIMARY_CROSSBOW_NAME";
            mySkillDef.skillNameToken = "EXAMPLESURVIVOR_PRIMARY_CROSSBOW_NAME";

            LoadoutAPI.AddSkillDef(mySkillDef);

            component.primary = characterPrefab.AddComponent<GenericSkill>();
            

            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            component.primary.SetFieldValue("_skillFamily", newFamily);
            SkillFamily skillFamily = component.primary.skillFamily;

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };

            




            // add this code after defining a new skilldef if you're adding an alternate skill

            /*Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = newSkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(newSkillDef.skillNameToken, false, null)
            };*/
            Debug.LogWarning("finished PrimarySetup");
        }
        private void secondarySetup()
        {
            SkillLocator component = characterPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("EXAMPLESURVIVOR_PRIMARY_CROSSBOW_NAME", "Crossbow");
            LanguageAPI.Add("EXAMPLESURVIVOR_PRIMARY_CROSSBOW_DESCRIPTION", "Fire an arrow, dealing <style=cIsDamage>200% damage</style>.");
            var skillGutsSwordAttack = ScriptableObject.CreateInstance<SkillDef>();
            skillGutsSwordAttack.activationState = new SerializableEntityStateType(typeof(GutsSwordAttack));
            skillGutsSwordAttack.activationStateMachineName = "Weapon";
            skillGutsSwordAttack.baseMaxStock = 1;
            skillGutsSwordAttack.baseRechargeInterval = 0f;
            skillGutsSwordAttack.beginSkillCooldownOnSkillEnd = false;
            skillGutsSwordAttack.canceledFromSprinting = false;
            skillGutsSwordAttack.fullRestockOnAssign = true;
            skillGutsSwordAttack.interruptPriority = InterruptPriority.Any;
            skillGutsSwordAttack.isBullets = false;
            skillGutsSwordAttack.isCombatSkill = true;
            skillGutsSwordAttack.mustKeyPress = false;
            skillGutsSwordAttack.noSprint = true;
            skillGutsSwordAttack.rechargeStock = 1;
            skillGutsSwordAttack.requiredStock = 1;
            skillGutsSwordAttack.shootDelay = 0f;
            skillGutsSwordAttack.stockToConsume = 1;
            skillGutsSwordAttack.icon = Assets.icon1;
            //skillGutsSwordAttack.skillDescriptionToken = "EXAMPLESURVIVOR_PRIMARY_CROSSBOW_DESCRIPTION";
            //skillGutsSwordAttack.skillName = "EXAMPLESURVIVOR_PRIMARY_CROSSBOW_NAME";
            //skillGutsSwordAttack.skillNameToken = "EXAMPLESURVIVOR_PRIMARY_CROSSBOW_NAME";

            LoadoutAPI.AddSkillDef(skillGutsSwordAttack);
            component.secondary = characterPrefab.AddComponent<GenericSkill>();
             SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            component.secondary.SetFieldValue("_skillFamily", newFamily);
            SkillFamily skillFamily = component.secondary.skillFamily;
            component.secondary.SetFieldValue("_skillFamily", newFamily);


            skillFamily = component.secondary.skillFamily;

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = skillGutsSwordAttack,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(skillGutsSwordAttack.skillNameToken, false, null)
            };
        }
        private void CreateDoppelganger()
        {
            Debug.LogWarning("In CreateDoppelganger");
            // set up the doppelganger for artifact of vengeance here
            // quite simple, gets a bit more complex if you're adding your own ai, but commando ai will do

            doppelganger = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), "ExampleSurvivorMonsterMaster", true, "/home/sirhamburger/Git/Risk-of-Rain-2-Guts-Code/ExampleSurvivor/ExampleSurvivor/ExampleSurvivor/ExampleSurvivor/ExampleSurvivor.cs", "CreateDoppelganger", 159);

            MasterCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(doppelganger);
            };

            CharacterMaster component = doppelganger.GetComponent<CharacterMaster>();
            component.bodyPrefab = characterPrefab;
            Debug.LogWarning("finished CreateDoppelganger");
        }
    }



    // get the assets from your assetbundle here
    // if it's returning null, check and make sure you have the build action set to "Embedded Resource" and the file names are right because it's not gonna work otherwise
    public static class Assets
    {
        public static AssetBundle MainAssetBundle = null;
        public static AssetBundleResourcesProvider Provider;

        public static Texture charPortrait;

        public static Sprite iconP;
        public static Sprite icon1;
        public static Sprite icon2;
        public static Sprite icon3;
        public static Sprite icon4;

        public static void PopulateAssets()
        {
             Debug.LogWarning("try to load Example Survivor");

            if (MainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ExampleSurvivor.berserk"))
                {
                    MainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                    Provider = new AssetBundleResourcesProvider("@ExampleSurvivor", MainAssetBundle);
                }
            }
            Debug.LogWarning("loaded Example Survivor");

            // include this if you're using a custom soundbank
            /*using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("ExampleSurvivor.ExampleSurvivor.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }*/

            // and now we gather the assets
            try{
            charPortrait = MainAssetBundle.LoadAsset<Sprite>("PassiveIcon").texture;
            
            iconP = MainAssetBundle.LoadAsset<Sprite>("PassiveIcon");
            icon1 = MainAssetBundle.LoadAsset<Sprite>("PassiveIcon");
            icon2 = MainAssetBundle.LoadAsset<Sprite>("PassiveIcon");
            icon3 = MainAssetBundle.LoadAsset<Sprite>("PassiveIcon");
            icon4 = MainAssetBundle.LoadAsset<Sprite>("PassiveIcon");
            }
            catch
            {
                Debug.LogError("Error while loading ths skill spites.");
            }
            Debug.LogWarning("leaving populateAssets");
        }
    }
}



// the entitystates namespace is used to make the skills, i'm not gonna go into detail here but it's easy to learn through trial and error
namespace EntityStates.ExampleSurvivorStates
{
    public class ExampleSurvivorFireArrow : BaseSkillState
    {
        public float damageCoefficient = 2f;
        public float baseDuration = 0.75f;
        public float recoil = 1f;
        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerToolbotRebar");

        private float duration;
        private float fireDuration;
        private bool hasFired;
        private Animator animator;
        private string muzzleString;
        private OverlapAttack overlapAttack;
        private HitBox hitBox;
        public override void OnEnter()
        {
            Debug.LogWarning("In PrimarySkill");
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.fireDuration = 0.25f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();
            this.muzzleString = "Muzzle";
            HitBox hitbox=null;


            var gameObject = GameObject.Find("spine upper weapon_end");
            Transform transform = gameObject.transform;

            //ChildLocator childLocator = ExampleSurvivor.ExampleSurvivor.childLocator;

            //Transform transform = childLocator.FindChild("spine upper weapon_end");
            //Transform transform = ExampleSurvivor.ExampleSurvivor.characterPrefab.transform.Find("Armature/Hips/Spine/Chest/arm right wrist weapon_end/spine upper weapon/spine upper weapon_end");
            if(transform == null)
            Debug.LogError("transform == null");
            CapsuleCollider collider = transform.GetComponent<CapsuleCollider>();
            if(collider==null)
            Debug.LogError("collider == null");



            try{
            hitbox = transform.GetComponent<CapsuleCollider>().gameObject.AddComponent<HitBox>();
            this.hitBox = hitbox;
            if(hitbox == null)
                Debug.LogError("hitbox == null");
            }
            catch
            {Debug.LogError("error while adding Hitbox");}
            
            overlapAttack=base.InitMeleeOverlap(1000,tracerEffectPrefab,base.GetModelTransform(),hitbox.name);
            overlapAttack.damageType=DamageType.BleedOnHit;

            base.PlayAnimation("Gesture, Override", "Slash", "FireArrow.playbackRate", this.duration);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void FireArrow()
        {
            //BasicMeleeAttack basicMeleeAttack = new BasicMeleeAttack();
            //OverlapAttack overlap= new OverlapAttack();


            //overlap.Fire()
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.FireMeleeOverlap(overlapAttack,this.animator,hitBox.name,0f,false);
            //overlapAttack.Fire(null);
            if (base.fixedAge >= this.fireDuration)
            {
                FireArrow();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}

namespace EntityStates.ExampleSurvivorStates
{
    public class GutsSwordAttack : BaseSkillState
    {
        public float damageCoefficient = 2f;
        public float baseDuration = 3.75f;
        public float recoil = 1f;
        //public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerToolbotRebar");

        private float duration;
        private float fireDuration;
        private bool hasFired;
        private Animator animator;
        private string muzzleString;

        public override void OnEnter()
        {
            Debug.LogWarning("In SecondaySkill");
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.fireDuration = 0.25f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();
            this.muzzleString = "Muzzle";


            base.PlayAnimation("Gesture, Override", "GutsSwordAttack", "GutsSwordAttack.playbackRate", 3.7f);
                        Debug.LogWarning("Playing animation");

        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void FireArrow()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;

                base.characterBody.AddSpreadBloom(0.75f);
                Ray aimRay = base.GetAimRay();
                EffectManager.SimpleMuzzleFlash(Commando.CommandoWeapon.FirePistol.effectPrefab, base.gameObject, this.muzzleString, false);

                if (base.isAuthority)
                {
                    ProjectileManager.instance.FireProjectile(ExampleSurvivor.ExampleSurvivor.arrowProjectile, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageCoefficient * this.damageStat, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireDuration)
            {
                FireArrow();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}