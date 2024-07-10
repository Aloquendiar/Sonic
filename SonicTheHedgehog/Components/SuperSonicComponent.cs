﻿using EntityStates;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Skills;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Survivors;
using SonicTheHedgehog.Modules.Forms;
using SonicTheHedgehog.SkillStates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using HarmonyLib;
using System.Linq;
using System.Collections.Generic;

namespace SonicTheHedgehog.Components
{
    public class SuperSonicComponent : NetworkBehaviour
    {
        public EntityStateMachine superSonicState;

        [Tooltip("The form you have selected. Not necessarily the form you are currently in, but the one that you're focused on. Attempting to transform will transform you into this form.")]
        public FormDef targetedForm;

        [Tooltip("The form you're currently in. If not transformed into anything, this will be null.")]
        public FormDef activeForm;

        public Material formMaterial;
        public Material defaultMaterial;

        public Mesh formMesh;
        public Mesh defaultModel;

        private CharacterBody body;
        private CharacterModel model;
        private Inventory inventory;
        private Animator modelAnimator;

        private TemporaryOverlay flashOverlay;
        private static Material flashMaterial;

        public Dictionary<FormDef, ItemTracker> formToItemTracker = new Dictionary<FormDef, ItemTracker>();

        private void Start()
        {
            targetedForm = Forms.superSonicDef;
            body = base.GetComponent<CharacterBody>();
            if (!body.isPlayerControlled)
            {
                Destroy(this);
            }
            model = body.modelLocator.modelTransform.gameObject.GetComponent<CharacterModel>();
            modelAnimator = model.transform.GetComponent<Animator>();
            superSonicState = EntityStateMachine.FindByCustomName(base.gameObject, "SonicForms");
            flashMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressFlashBright.mat").WaitForCompletion();

            CreateUnsyncItemTrackers();
        }

        public void CreateUnsyncItemTrackers()
        {
            foreach (FormDef form in Forms.formsCatalog)
            {
                if (form.requiresItems && !form.shareItems)
                {
                    CreateTrackerForForm(form);
                }
            }
        }

        public virtual void CreateTrackerForForm(FormDef form)
        {
            ItemTracker itemTracker = body.gameObject.AddComponent<ItemTracker>();
            itemTracker.form = form;
            itemTracker.body = body;
            formToItemTracker.Add(form, itemTracker);
        }

        public void FixedUpdate()
        {
            if (body.hasAuthority && body.isPlayerControlled)
            {
                if (activeForm != targetedForm) // Adding isPlayerControlled I guess fixed super transforming all Sonics
                {
                    if (Config.SuperTransformKey().Value.IsPressed())
                    {
                        if (Forms.formToHandlerObject.TryGetValue(targetedForm, out GameObject handlerObject))
                        {
                            FormHandler handler = handlerObject.GetComponent(typeof(FormHandler)) as FormHandler;
                            if (handler.CanTransform(this))
                            {
                                Debug.Log("Attempt Transform");
                                Transform();
                            }
                        }
                    }
                }
            }
        }

        public void Transform()
        {
            EntityStateMachine bodyState = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
            if (!bodyState) { return; }
            if (!Forms.formToHandlerObject.TryGetValue(targetedForm, out GameObject handlerObject)) { return; }
            FormHandler handler = handlerObject.GetComponent(typeof(FormHandler)) as FormHandler;
            if (bodyState.SetInterruptState(new SuperSonicTransformation { emeraldAnimation = !handler.NetworkteamSuper }, InterruptPriority.Frozen))
            {
                if (NetworkServer.active)
                {
                    //FormHandler.instance.OnTransform();
                    handler.OnTransform(base.gameObject);
                }
                else
                {
                    new SuperSonicTransform(GetComponent<NetworkIdentity>().netId, targetedForm.formIndex).Send(NetworkDestination.Server);
                }
            }
        }

        public void OnTransform(FormDef form)
        {
            this.activeForm = form;
            if (!form) { return; }
            ModelSkinController skin = model.GetComponentInChildren<ModelSkinController>();
            if (!skin) { return; }
            GetSuperModel(model.GetComponentInChildren<ModelSkinController>().skins[body.skinIndex].nameToken);
            SuperModel();
        }

        public void TransformEnd()
        {
            this.activeForm = null;
            ResetModel();
        }

        // Thank you DxsSucuk
        public void SuperModel()
        {
            defaultMaterial = model.baseRendererInfos[0].defaultMaterial; // Textures
            if (formMaterial)
            {
                model.baseRendererInfos[0].defaultMaterial = formMaterial;
            }
            
            if (modelAnimator && activeForm.superAnimations) // Animations
            {
                modelAnimator.SetFloat("isSuperFloat", 1f);
            }

            if (formMesh) // Model
            {
                defaultModel = model.mainSkinnedMeshRenderer.sharedMesh;
                model.mainSkinnedMeshRenderer.sharedMesh = formMesh;
            }

            if (model)
            {
                flashOverlay = model.gameObject.AddComponent<TemporaryOverlay>(); // Flash
                flashOverlay.duration = 1;
                flashOverlay.animateShaderAlpha = true;
                flashOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 0.7f, 1f, 0f);
                flashOverlay.originalMaterial = flashMaterial;
                flashOverlay.destroyComponentOnEnd = true;
                flashOverlay.AddToCharacerModel(model);
            }
        }

        public void ResetModel()
        {
            model.baseRendererInfos[0].defaultMaterial = defaultMaterial; // Textures

            if (modelAnimator) // Animations
            {
                modelAnimator.SetFloat("isSuperFloat", 0f);
            }

            if (formMesh) // Model
            {
                model.mainSkinnedMeshRenderer.sharedMesh = defaultModel;
            }

            if (model) // Flash
            {
                flashOverlay = model.gameObject.AddComponent<TemporaryOverlay>();
                flashOverlay.duration = 0.35f;
                flashOverlay.animateShaderAlpha = true;
                flashOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                flashOverlay.originalMaterial = flashMaterial;
                flashOverlay.destroyComponentOnEnd = true;
                flashOverlay.AddToCharacerModel(model);
            }
        }

        public virtual void GetSuperModel(string skinName)
        {
            if (activeForm.renderDictionary.TryGetValue(skinName, out RenderReplacements replacements))
            {
                formMesh = replacements.mesh;
                formMaterial = replacements.material;
            }


            /*switch (skinName)
            {
                case SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "DEFAULT_SKIN_NAME":
                    formMaterial = Materials.CreateHopooMaterial("matSuperSonic");
                    formMesh = Assets.mainAssetBundle.LoadAsset<GameObject>("SuperSonicMesh")
                        .GetComponent<SkinnedMeshRenderer>().sharedMesh;
                    break;
                case SonicTheHedgehogCharacter.SONIC_THE_HEDGEHOG_PREFIX + "MASTERY_SKIN_NAME":
                    formMaterial = Materials.CreateHopooMaterial("matSuperMetalSonic");
                    formMesh = null;
                    break;
            }*/
        }
    }

    public class ItemTracker : MonoBehaviour
    {
        public FormDef form;

        public CharacterBody body;

        public Inventory inventory;

        public bool allItems;

        private bool eventsSubscribed;

        // HOW DO I GET THE INVENTORY?!?!?
        private void OnDisable()
        {
            SubscribeEvents(false);
        }

        private void FixedUpdate()
        {
            if (!eventsSubscribed)
            {
                if (body)
                {
                    if (body.inventory)
                    {
                        Debug.Log("inventory found");
                        inventory = body.inventory;
                        SubscribeEvents(true);
                    }
                }
            }
        }

        public void SubscribeEvents(bool subscribe)
        {
            if (inventory)
            {
                if (eventsSubscribed ^ subscribe)
                {
                    if (subscribe)
                    {
                        inventory.onInventoryChanged += CheckItems;
                        Debug.Log("subscribe");
                        eventsSubscribed = true;
                        CheckItems();
                    }
                    else
                    {
                        inventory.onInventoryChanged -= CheckItems;
                        eventsSubscribed = false;
                    }
                }
            }
        }

        public void CheckItems()
        {
            if (!form) { Debug.LogError("No form??"); allItems= false; return; }
            if (!inventory) { Debug.LogError("No inventory????????"); allItems = false; return; }
            foreach (NeededItem item in form.neededItems)
            {
                if (!item.item) { Debug.LogError("No item????????"); return; }
                if (inventory.GetItemCount(item) < item.count)
                {
                    allItems = false;
                    Debug.Log("Missing items for " + form.ToString() + ": \n" + (new NeededItem { item = item.item, count = (uint)(item.count - inventory.GetItemCount(item))}).ToString());
                    return;
                }
            }
            Debug.Log("All items needed for " + form.ToString());
            allItems = true;
        }
    }
}