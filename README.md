中文版说明在后面

# WARNING: 

1. This mod (RAHI) only adds a hediff. It wont break your save or cause obvious lag, and you can safely add or remove it at any time.

2. This is a difficulty mod, unrecommended for new players. But if you love extremely hot weather challenge, RAHI is for you. 

# What this mod does:

In short:

It will make hot weather and high temperature more realistic.

- Your pawns will now need to wear less to be comfortable in summer 
- Some specific apparels supposed to be worn in summer or high temperature are now literally useful (Phnenix Armor, T shirt, Corset, short/skirt from VE Apparel, etc.)
- Some other factors like humidity (from biome and weather) and exposed body part will also impact heat resistance.

In Vanilla, the more clothings a pawn wear, the higher their max comfortable temperature (MaxCT) becomes. There is no "negative" impact of wearing too much in summer, even a parkas doesn't give you any penalty in a heat wave of 50 celcius. This is unrealistic and immersion breaking. 
	
Besides, as we all know, even the same clothing doesn't give you the same heat insulation in different weather, especially humidity. A duster in 40 celcius doesn't result in the same heat insulation in arid desert with clear sky and in rainy jungle. However Vanilla game ignore this factor completely. 
	
This mod focus on dealing with this issue by making apparel heat insulation more realistic and challenging. Some previously useless traits like Nudist will also have their place in certain gameplay styles.

# How this mod works:

It will add a hediff adjusting vanilla MaxCT for all player pawns. 

From ambiance temperature being above 30C, this hediff will trigger. If temperature drops below it will disappear and return to vanilla calculation.

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
- If an apparel has vanilla heat insulation inferior than cold one, the latter will be used instead for the calculation of penalty.
- Some apparels have doubled MaxCT penalty: Parka, Kid Parka
- Gloves and boots/shoes in VE apparels have half MaxCT penalty.
- For heavier apparels between 5-10kg, each extra kg above 5 will reduce MaxCT by 0.2C.
- For heavier apparels more than 10kg, each extra kg above 10 will reduce MaxCT by 0.5C.
- Some basic apparels have MaxCT reduction permanently 0: Basic Shirt, Pants, Corset, Tribal, KidTribal.
- Same for some apparels from VE Apparels: Casual T Shirt, Short, Skirt
- Basic apparels may still bring humidity penalties on wet biomes or weathers.
- Phoenix Armor permanently keeps its vanilla heat insulation bonus.
- Same for all clothings with EVA decompression resistance from SOS2. 
SOS2 doesn't have to be installed to get this feature applied. RAHI will simply check if "SaveOurShip2.CompEVA" compClass exists in ThingDef XML.
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
- Legs are considered exposed if a pawn wears Short or Skirt from VE Apparel, without other apparel covering legs.
There is a cap for this bonus to not let nudist trait become overpowered.


# How this mod impacts game play:

- You now need to choose wisely apparel in hot weather or biome to balance between the other stats and MaxCT.
- Just like real life, do not let your pawn wear too much in summer. 
- Think twice before spamming legendary hyperweave apparels for summer, because better quality and material will now increase MaxCT penalty.
- Phoenix Armor now becomes extremely useful for combat in hot weather.
- Heat tolerance genes are much more important than vanilla.
- Personal shields are also more important now in summer for pawns can't wear too much to stack armor. 


# DLC requirement:

- Biotech: Not a must have, but without it you will never get heat resistant genes bonus
- Royalty: Not a must have either, but without it you wont have the most important Phoenix Armor.


# Compatibility:

Should be compatible with most mods including apparel ones, since RAHI adds nothing but a hediff.
But for modded apparels, they will be dynamically classified by RAHI:
- In no case they will be considerred as HIA
- If they have SOS2 "Decompression Resistance" trait, no matter if SOS2 is installed or not, their MaxCT will not be touched by RAHI.
- All other rules about MaxCT penalty calculation apply.

Besides, Mods adding new biomes will never be considered as "dry" biome so will never trigger HIA bonus. 


# Gameplay balance:

Default parameters of RAHI are just my own personal preference, it may not be balanced for everyone. But most parameters of RAHI are customizable in mod config so you can easily balance the gameplay yourself.


# Performance:

Since RAHI script dynamically changes MaxCT of all your pawns, I know people may have concern about lagging. To minimize this issue, I well optimized the script with professional software engineering knowledge in mind (I was software engineer IRL for years) 

- RAHI doesn't serialize anything into your save file. In no case it can break your save. Safe to be added or removed mid-game.

- The script runs only for human player pawns, so it wont suddenly lag your game when a bunch of raiders or other NPC fill your map, or when you have alot of non-human pawns(mechs, animals,)

- The script runs only on pawns staying in ambience temperature > 30C. Otherwise the hediff will be removed and vanilla MaxCT will apply. This optimization is because the minimum MaxCT for a pawn is 21C, so no chance of getting heat strike when temperature is below 30C, in this case running the script is a waste of your CPU resource.

- The script tries to run only once per in-game hour (unlike some mods trying to run on each tick)

- Most codes are simply doing elemental calculations to reduce unnecessary complexity.

If your save involves alot of pawns and experience lag because of this mod, please report and I'll see what I can do furthermore, for I myself never play with more than 20 pawns.


# Recommended to play with for ultimate challenge:
- Yayo's Planet
- Climate Cycle++
- Adjustable Temperature Events
- VE Events (make heat wave and global warming events appear more often in config)


# 警告：

本模组（RAHI）只增加了一个健康状况（hediff）。它不会破坏存档，也不会导致明显的卡顿，您可以随时安全地添加或移除它。

这是一个增加难度的模组，不推荐新玩家使用。但如果您热衷于极端高温挑战，那么 RAHI 非常适合您。


# 主要功能：

简而言之：

增加炎热气候和高温环境下小人抗热计算的真实性。小人们现在在夏天需要穿更少的衣物以保持舒适，和真实世界一样。一些特定的服装（如凤凰甲、T恤、胸衣以及VE服饰拓展的短裤/短裙等）现在也更有用了。

其他一些因素（如湿度、生物群落天气的影响、裸露身体部位）也会对耐热性产生影响。

在原版游戏中，角色穿的衣物越多，最高舒适温度（MaxCT）就越高。即使在 50°C 的热浪中穿大衣，也没有任何惩罚。这显然不现实，并且破坏了沉浸感。

此外，即使是相同的衣物，在不同的天气条件下，其隔热效果也不同，尤其是湿度的影响。但原版游戏完全忽略这一点。

本模组旨在解决这些问题，使服装的隔热效果更加真实且具有挑战性。一些之前无用的特性（如裸体主义）也会在某些玩法中发挥作用。


# 本模组的工作原理：

它会为所有玩家角色添加一个调整原版的小人抗热上限（ MaxCT ）的健康状况（hediff）。

当环境温度超过 30°C 时，该健康状况就会触发。如果温度低于此值则会消失，并返回原版的计算方式。

新MaxCT的值考虑了以下因素：

1. 湿度惩罚：

在潮湿的生物群落和天气中，每件衣物会带来更多的 MaxCT 惩罚。
热带非沙漠生物群落的湿度惩罚更高（可在配置中调整）。

2. 服装：

服装被分为两类：隔热服装和非隔热服装。

对于非隔热服装：

- 大多数服装属于非隔热服装。
- 原版中增加 MaxCT 的服装，现在大多会减少 MaxCT。
- 原版中 MaxCT 提升越大的服装，现在减少得越多。
- 如果服装的原版热隔热值低于冷隔热值，则使用冷隔热值进行计算。
- 一些服装有双倍 MaxCT 惩罚，例如大衣和儿童大衣。
- VE服饰拓展中的手套和鞋子/靴子只有一半的 MaxCT 惩罚。
- 对于 5-10 公斤的衣物，每超出 5 公斤的部分使 MaxCT 减少 0.2°C。
- 对于超过 10 公斤的衣物，每超出 10 公斤的部分使 MaxCT 减少 0.5°C。
- 一些基础服装的 MaxCT 减少值永久为 0，例如基础衬衫、裤子、胸衣、部落服、儿童部落服。
- VE服饰拓展的一些基础服装也一样，例如休闲T恤、短裤、短裙。
- 基础服装在潮湿的生物群落或天气中依然有湿度惩罚。
- 凤凰甲保留其原版隔热加成。
- 拥有SOS2的EVA抗泄压属性的服装同样不受影响。例如：Eccentric Tech的天使服、古代机甲护甲。注意本功能无需安装SOS2，仅取决于模组作者是否在ThingDef中定义了SOS2抗泄压属性。

对于隔热服装：

当前有 4 种服装：防尘衣、牛仔帽、遮阳帽、连帽帽。
在干燥生物群落中（如沙漠、干燥灌木林）且无雨时，MaxCT大幅增加。否则，其效果与非隔热服装相同。
服装重量：
如果角色穿戴的服装总重量达到其携带能力的某一比例，则会进一步减少 MaxCT。

3. 基因：
Biotech DLC 中的耐热基因/超级耐热基因额外提供 +5°C/+10°C 抗热加成。

4. 暴露的身体部位：

- 某些身体部位未被任何衣物覆盖时，会增加 MaxCT。
- 裸体主义者特性现在会有一定优势。
- 一些不覆盖手臂或肩膀的衣物就和现实中的短袖一样，在炎热天气中更有用。
- 小人如果穿着VE服饰拓展中的短裤或短裙，在没有其它衣物覆盖腿部时也会被视为裸露腿部，有抗热加成奖励。
- 该加成有上限，以防止裸体主义者特性过于强大。


# 本模组对游戏玩法的影响：

- 在炎热气候和天气时，如今需要谨慎选择小人的服饰，以平衡抗热和护甲之类的其它属性，再也无法无脑堆衣服两者兼得了。
- 不要让小人在夏天穿得太多，就像现实生活中一样。
- 夏季衣服不要无脑堆传奇级和高等材料，因为更好的材料如今反而会增加抗热惩罚。
- 凤凰甲在炎热天气中的战斗中更重要了。
- 耐热基因也比原版更重要。
- 个人护盾在夏天作用更大，因为角色不能穿太多衣物来堆叠护甲。


# DLC 需求：

- 生物：非必需，但少了它就没有耐热基因了。
- 皇家：非必需，但少了它就没凤凰甲了。


# 兼容性：

本模组应兼容大多数模组，包括服装模组。但对于模组新增的服饰，本模组会动态地决定其部分新属性：
- 它们不会被算为类似防尘衣的“抗热衣物”
- 如果模组作者在XML文件中加了SOS2的“抗泄压”属性（Decompression Resistance），无论SOS2有没有安装，本模组都不会改变其抗热加成。
- 所有其它服饰都会按普通非抗热服饰来计算其抗热惩罚。

另外，本模组只认定原版的沙漠，极端沙漠和干旱灌木丛为“干燥”气候。对于增加新气候的模组，新气候不会被本模组判定为“干燥”气候，因此防尘衣这类抗热服饰的特殊加成不会触发。


# 游戏平衡：

默认参数只是我个人的偏好，可能并不适合所有人。多数参数都可以在模组配置中自行调整，选择您自己想要的游戏平衡。


# 性能优化：

由于本模组的脚本会动态更改所有玩家小人的抗热上限，许多人可能会担心卡顿问题。为了最大程度地减少这一问题，我根据多年的软件工程师经验对脚本进行了优化。

- 本模组没有任何序列化操作，不会对存档文件进行任何修改。因此绝无可能破坏您的存档，并且模组可以安全地在游戏中途添加或移除。

- 脚本仅对玩家小人生效，因此当大量袭击者或其他 NPC 出现在地图上，或者有许多非人类角色（机械族、动物等）时，不会突然导致游戏卡顿。

- 脚本仅在角色处于环境温度 >30°C 时运行。否则小人的该健康状况会被移除，并恢复原版的抗热计算。该优化所基于的考量是，原版小人的抗热上限最低是21°C，因此当温度低于30°C时，按照游戏机制不可能出现中暑，此时运行模组脚本纯属浪费CPU。

- 脚本每游戏小时仅运行一次（不像某些优化较差的模组每个tick都会运行一次）。

- 大多数代码都只是基本的算数计算，以减少不必要的程序复杂度。

- 我自己的存档从未玩过超过20个小人。如果存档涉及大量角色，并因本模组而出现卡顿问题，请向我报告，我会试着进一步寻找更多优化方案。


# 推荐同时一起开的模组（喜欢极限炎热气候挑战的话）：

- Yayo's Planet
- Climate Cycle++
- Adjustable Temperature Events
- VE事件拓展（可以提高热浪和全球变暖事件的概率）