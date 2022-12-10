using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering;

public class NetworkShadowMesh : NetworkBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] meshRenderers;
#if UNITY_EDITOR
    [SerializeField] private ShadowCastingMode shadowCastingMode = ShadowCastingMode.ShadowsOnly;
    private ShadowCastingMode _oldShadowCastingMode = ShadowCastingMode.ShadowsOnly;
#endif

    public override void OnNetworkSpawn() {
        if (!IsOwner)
            return;

        foreach (var renderer in meshRenderers)
            renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if (shadowCastingMode != _oldShadowCastingMode) {
            foreach (var renderer in meshRenderers)
                renderer.shadowCastingMode = shadowCastingMode;
            _oldShadowCastingMode = shadowCastingMode;
        }
    }
#endif
}
