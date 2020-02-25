using BS_Utils.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomFloorPlugin {


    /// <summary>
    /// Instantiable wrapper class for <see cref="RotationEventEffect"/>s, that handles registering and de-registering
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA1812:Avoid unistantiated internal classes", Justification = "Instantiated by Unity")]
    internal class RotationEventEffectManager:MonoBehaviour {


        /// <summary>
        /// To be filled with all <see cref="RotationEventEffect"/>s on the local <see cref="CustomPlatform"/>
        /// </summary>
        private RotationEventEffect[] effectDescriptors;


        /// <summary>
        /// To be filled with spawned <see cref="LightRotationEventEffect"/>s
        /// </summary>
        private List<LightRotationEventEffect> lightRotationEffects;


        private MultiRotationEventEffect[] multiEffects;

        /// <summary>
        /// Registers all currently known <see cref="LightRotationEventEffect"/>s for Events.
        /// </summary>
        internal void RegisterForEvents() {
            foreach(LightRotationEventEffect rotEffect in lightRotationEffects) {
                BSEvents.beatmapEvent += rotEffect.HandleBeatmapObjectCallbackControllerBeatmapEventDidTrigger;
            }
            foreach(MultiRotationEventEffect effect in multiEffects) {
                BSEvents.beatmapEvent += effect.EventCallback;
            }
        }


        /// <summary>
        /// De-Registers from lighting events<br/>
        /// [Unity calls this when the <see cref="MonoBehaviour"/> becomes inactive in the hierachy]
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Called by Unity")]
        private void OnDisable() {
            foreach(LightRotationEventEffect rotEffect in lightRotationEffects) {
                BSEvents.beatmapEvent -= rotEffect.HandleBeatmapObjectCallbackControllerBeatmapEventDidTrigger;
            }
            foreach(MultiRotationEventEffect effect in multiEffects) {
                BSEvents.beatmapEvent -= effect.EventCallback;
            }
        }


        /// <summary>
        /// Creates all <see cref="LightRotationEventEffect"/>s for the <paramref name="currentPlatform"/><br/>
        /// (From <see cref="RotationEventEffect"/>s present on the <paramref name="currentPlatform"/>)
        /// </summary>
        /// <param name="currentPlatform">Current <see cref="CustomPlatform"/>s <see cref="GameObject"/></param>
        internal void CreateEffects(GameObject currentPlatform) {
            lightRotationEffects = new List<LightRotationEventEffect>();

            effectDescriptors = currentPlatform.GetComponentsInChildren<RotationEventEffect>(true);

            foreach(RotationEventEffect effectDescriptor in effectDescriptors) {
                LightRotationEventEffect rotEvent = effectDescriptor.gameObject.AddComponent<LightRotationEventEffect>();
                PlatformManager.SpawnedComponents.Add(rotEvent);
                ReflectionUtil.SetPrivateField(rotEvent, "_event", (BeatmapEventType)effectDescriptor.eventType);
                ReflectionUtil.SetPrivateField(rotEvent, "_rotationVector", effectDescriptor.rotationVector);
                ReflectionUtil.SetPrivateField(rotEvent, "_transform", rotEvent.transform);
                ReflectionUtil.SetPrivateField(rotEvent, "_startRotation", rotEvent.transform.rotation);
                lightRotationEffects.Add(rotEvent);
            }

            multiEffects = currentPlatform.GetComponentsInChildren<MultiRotationEventEffect>();
        }
    }
}
