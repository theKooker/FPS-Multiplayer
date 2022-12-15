using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering;

public class NetworkShadowMesh : MonoBehaviourPunCallbacks
{
    [SerializeField] private PhotonView pView;
    [SerializeField] private SkinnedMeshRenderer[] meshRenderers;
#if UNITY_EDITOR
    [SerializeField] private ShadowCastingMode shadowCastingMode = ShadowCastingMode.ShadowsOnly;
    private ShadowCastingMode _oldShadowCastingMode = ShadowCastingMode.ShadowsOnly;
#endif

    private void Start() {
        if (!pView.IsMine)
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
