# Rendering

## Forward vs. Deferred Rendering Path

**Decision:** Forward Rendering

**Rationale:** TODO.

Forward Rendering is the default rendering path in Unity.

## Lighting

### Realtime

By default, lights in Unity are realtime. However, using realtime lighting alone is limited, and does not produce very realistic scenes. Realtime Global Illumination (GI) can be used as well to improve realism.

### Realtime GI

#### Precomputed

**Decision:** Enabled

**Rationale:** Allows moving light sources; performance is not too much of a consideration if our target platform is PC.

Objects that will never move should be marked as "static" as a performance optimisation.

#### Baked

**Decision:** Disabled globally

**Rationale:** More performant than Precomputed GI but does not support moving light soruces.

## HDR vs. LDR

**Decision:** LDR

**Rationale:** LDR is more appropriate for a flat-shaded game.

LDR is the default setting for cameras in Unity.

**Implications:**

 - When applying Colour Grading, no tonemapping should be applied
 - When applying Colour Grading, linear trackballs should be used

## Post-Processing Effects

**Decision:** Some subtle post-processing effects have been enabled

**Rationale:** This may incur a slight performance impact but drastically improves the quality of the rendered scene.
