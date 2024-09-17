using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingScript : MonoBehaviour
{
    public CharacterControllerScript scp;
    public PostProcessVolume PPV;

    private Vignette vignette;

    // Start is called before the first frame update
    void Start()
    {
        if (PPV != null)
        {
            PPV.profile.TryGetSettings(out vignette);
        }
    }

    // Update is called once per frame
    void Update()
    {
        AddVignetteFromHP(scp.playerHealth);
    }

    private void AddVignetteFromHP(float hp)
    {
        // Calculate vignette
        float vignetteSmoothness = -0.1f * hp + 1f;
        vignette.smoothness.value = vignetteSmoothness;
    }
}
