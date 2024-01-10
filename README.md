# (A little tiny bit less) WIP!!
Some things might be broken and some events aren't implemented yet, please report any errors you encounter on the mod github page (link above) with the following information:
1) What was the error?
2) What other mods you have installed?
3) What are the steps to reproduce the error?
4) Add log files.
- When testing out the mod, please enable logging debug messages to disk in your BepInEx config file.

# Lucky Dice
This mod adds a bunch of different dice, each with it's own event pool.
When a dice is used (left mouse) it "rolls" an event from it's pool.

The dice items (that I made) are not usable in the ship phase or on the company moon to prevent cheesing.

# Example mod that extends Lucky Dice:
https://github.com/OE100/ExampleEventExtensionMod

# Todo:
- Guide on extending the event/item system of the mod (the api is already in there, just needs documentation)
- Implement some events that I didn't have time to make just yet
- Write what each event does

# Releases:

# 0.3.2 - API & Fixes
- Fix: Bug where if a client was activating the dice he would get stuck.
- API: Added utility function to get all living, controller players.
- API: Added utility function to get the closest player to a position.

# 0.3.1 - API
- API: Added IsOneTimeUse() to DiceItem to be able to set if it should destroy itself after use or not.
- API: Added OnRPC virtual functions to make modifying the inherent behaviour a bit easier.

# 0.3.0 - Game version v47 & Fixes
- Fix: Enemies spawned outside should now behave correctly even if they aren't meant to be spawned outside.
- Transpiler: Special fix for jesters that are spawned outside.
- Fix: TTT event now cancels correctly when there's 1 player instead of crashing the game. (for the single players)
- Fix: Bug where the light would remain after a dice was used.

# 0.2.1 - Fixes
- Fix: Trying to solve a bug causing non-host players to not be able to activate the dice.
- Fix: Trying to make the "drop \[item\]" prompt

# 0.2.0 - API & Fixes
- Fix: Flying to moons no longer breaks clients. (lol, my bad)
- Fix: Spawn enemy events should work even if the enemy isn't spawnable on the moon in vanilla.
- API: Added an enemy registry that maps the EnemyAI type to its prefab for easier access. (for an example of how to use it see my enemy spawn event)
- API: Changed my enemy spawn events to use generics for ease of use. (not backwards compatible)

# 0.1.1 - API & Examples
- Add: An example for how to make a mod that extends lucky dice.
- API: Some minor variable visibility changes.

# 0.1.0 - Changes & Additions
- Add: config options to set dice spawn chance, if dice can spawn at all, if dice should be activatable or just scrap.
- Change: event api to use custom attributes. (not backwards compatible)

# 0.0.2 - Fixes
- Fix: Bug that caused items spawned by events to not show on scan.

# 0.0.1 - Initial Beta
- Early Beta
- Added extendable dice items (with regular solids already implemented)
- Added extendable event system (with some base event types and events already implemented)
