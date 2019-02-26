# Post Processing Stack

## Overview

The new (v2 onwards) Post Processing Stack is designed to be applied to 'volumes', or layers of rendering. If you only want a single stack of effects, there are now a few extra hoops to jump through.

## Usage

Once installed, you can make a default Post Processing Profile asset, which can be configured with post-processing effects.

Next, the camera of your choice (e.g. the Main Camera prefab) needs to have two components added:
- Post Process Layer, with the Layer selected and Anti-aliasing configured
- Post Process Volume - this can just be set to global, weight 1, and the profile designated

The Volume can have overrides provided against the profile.
