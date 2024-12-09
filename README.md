This mod (RAHI) is still in Beta and may be unstable. But since it only adds a hediff, it wont break your safe. You can safely add or remove it at any time.

# WARNING: 

This is a difficulty mod, unrecommended for new players. 

But if you love extreme climate challenge, this mod is for you. It will also be more funny to combine it with some others like Yayo's Planet，Climate Cycle++，Adjustable Temperature Events, or simply make heat wave event appear more often in VE Events config.

# What this mod does:

It will make high temperature biomes MUCH harder. No longer it will be possible to simply wear a bunch of legendary hyperweave or devilstrand apparels with alot of armor values then permanently forget the temperature. 

In Vanilla, the more clothings a pawn wear, the higher their max comfortable temperature becomes. There is no "negative" impact of wearing too much in summer, even a parkas doesn't give you any penalty in a heat wave of 50 celcius. This is unrealistic and immersion breaking. 
	
Besides, as we all know, even the same clothing doesn't give you the same heat insulation in different weather, especially humidity. A duster in 40 celcius doesn't result in the same heat insulation in arid desert with clear sky and in rainy jungle. However Vanilla game ignore this factor completely. 
	
This mod focus on dealing with this issue by making apparel heat insulation more realistic and challenging. Some previously useless traits like Nudist will also have their place in certain gameplay styles.

# How this mod works:

From ambiance temperature being above 30C, this hediff will trigger. If temperature drops below it will disappear and return to vanilla calculation.

The hediff simply adjusts vanilla Maximum Comfortable Temperature (MaxCT) for all player human pawns.

The value of new MaxCT takes into account all these elements:

1. Humidity penalty:
Now in wet biomes and weathers, pawns will suffer more maxHC penalty for each clothing piece worn. 
Tropical non-desert biomes are supposed to have more humidity penalty (Adjustable in config)

2. Apparels
Now there are two kinds of apparels: Heat Insulation Apparels(HIA) and Non Heat Insulation Apparels (HIA).


For all non-HIA:
- Most apparels are non-HIA
- Instead of increasing max comfortable temperature(MaxCT) by most clothings, it will mostly reduce it instead.
- The more vanilla increase MaxCT, now the more it will decrease. 
- Some apparels have doubled MaxCT penalty: Parka, Kid Parka
- For heavier apparels between 5-10kg, each extra kg above 5 will reduce MaxCT by 0.2C.
- For heavier apparels more than 10kg, each extra kg above 10 will reduce MaxCT by 0.5C.
- Some basic apparels have MaxCT reduction permanently 0: Basic Shirt, Pants, Corset, Tribal, KidTribal.
- Basic apparels may still bring humidity penalties on wet biomes or weathers.
- Phoenix Armor permanently keeps its vanilla heat insulation bonus.
- Same for all clothings with EVA decompression resistance from SOS2. 
SOS2 doesn't have to be installed to get this feature applied. RAHI will simply check if "SaveOurShip2.CompEVA" compClass exists in ThingDef XML.
Example of modded apparels that keep their original MaxCT bonus (list not exhaustive): 
Eccentric Tech - Angel Apparel Ancient Mech Armor (only those with EVA compatibility)
Ancient Mech Armors

For HIA:
Currently there are 4 apparels: duster, cowboy hat, shade cone, hat hood.
They will largely increase MaxCT when no humidity penalty applies (must be in dry biomes like desert or arid shrubland, and it't not rainy). Otherwise they will still act like non-HIA.


3. Carrying weight
If the sum of apparels worn on a pawn reaches a certain percentage of his/her carrying capacity, it will furthermore reduce MaxCT


4. Gene
Heat tolerance/super-tolerance gene from Biotech can have bonus of +5C/+10C besides what vanilla offers.


5. Body part exposure
If some body parts are not covered by any layer of any apparel, it will increase MaxCT. 
- Nudist trait will now have some advantage.
- Some apparels that dont cover some body parts like arms or shoulders are now useful in hot weather, just as real life.
There is a cap for this bonus to not let nudist trait become overpowered.


# How this mod impacts game play:

- You now need to choose wisely apparel in hot weather or biome to balance between the other stats and MaxCT.
- Think twice before spamming legendary hyperweave apparels for summer, because better quality and material will now increase MaxCT penalty.
- Phoenix Armor now becomes extremely useful for combat in hot weather.
- Heat tolerance genes are much more important than vanilla. 