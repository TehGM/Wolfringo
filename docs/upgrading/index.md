---
uid: Upgrading.Introduction
---

# Upgrading Wolfringo in Your Project
Libraries evolve over time, and Wolfringo is no different. Sometimes the changes are so big, so you might need to alter code after upgrading Wolfringo version in your bot.

To make this task easier, I provide some upgrade guides.

## Upgrade Impact
How much will need changing? Let's take a look at the versions.

***Patch Version Change*** *(1.0.0 => 1.0.1)* usually contains only small fixes, so your code should be fine in vast majority of situations - unless you tweak some niche Wolfringo internals.

***Minor Version Change*** *(1.0.x => 1.1.x)* usually contains new features, and has little or no breaking changes. Sometimes you might need to update some code if you customize Wolfringo behaviours, but this should be rare.

***Major Version Change*** *(1.x.x => 2.x.x)* means significant changes in Wolfringo. Breaking changes are almost guaranteed, so there's a high chance you'll need to make changes.

## Upgrade Guides
- [1.x => 2.x](xref:Upgrading.1_x-to-2_x)
- [2.0 => 2.1](xref:Upgrading.2_0-to-2_1)