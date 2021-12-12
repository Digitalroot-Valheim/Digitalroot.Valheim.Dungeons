# Digitalroot.Valheim.Dungeon.OdinsHollow
[![Build](https://github.com/Digitalroot-Valheim/Digitalroot.Valheim.DungeonsDemo/actions/workflows/builder.yml/badge.svg)](https://github.com/Digitalroot-Valheim/Digitalroot.Valheim.DungeonsDemo/actions/workflows/builder.yml)  


## Dungeons Odin's Hollow
<!--
//www.plantuml.com/plantuml/png/XP31IiGm48RlUOfuSbVOI_7afOjLH13n81usv11tHpUGp2pJgRPlRrias4gpn_p_bs7cszRYM9eOrEM57262hazk3y6zA_D4UoUUy8MfxLPL88IrrLURZLvLt9taUgrlT7NquQ1ICBQFAC2U-1w_F1D6GDaHmZbygOTsrkTMCw_iiFuWZZQBxPv1tZnyPwbfPTNoPIFTfyFBJlTFGwnIflmL-71xr-0d4FuvEcLvZSNmfLp6Yyn8QFynffoBAsmAawQKpQNqnHRmqCJm0m00
-->
<img src="https://www.plantuml.com/plantuml/svg/XP31IiGm48RlUOfuSbVOI_7afOjLH13n81usv11tHpUGp2pJgRPlRrias4gpn_p_bs7cszRYM9eOrEM57262hazk3y6zA_D4UoUUy8MfxLPL88IrrLURZLvLt9taUgrlT7NquQ1ICBQFAC2U-1w_F1D6GDaHmZbygOTsrkTMCw_iiFuWZZQBxPv1tZnyPwbfPTNoPIFTfyFBJlTFGwnIflmL-71xr-0d4FuvEcLvZSNmfLp6Yyn8QFynffoBAsmAawQKpQNqnHRmqCJm0m00">

## Trap Spawners
<!--
@startuml
!theme spacelab

node "Spawn Trigger" <<GameObject>> as SpawnTrigger1 
[Trap Trigger] <<TrapTrigger>> as TrapTrigger1
SpawnTrigger1 *-- [TrapTrigger1]
rectangle "Spawn Pool" as SP1 #green

[TrapTrigger1] *-left- SP1 
node "Spawn Point" <<GameObject>> as SpawnPoint1
node "Spawn Point" <<GameObject>> as SpawnPoint2
node "Spawn Point" <<GameObject>> as SpawnPoint3
[Spawner] <<TrapSpawner>> as Spawner1
[Spawner] <<TrapSpawner>> as Spawner2
[Spawner] <<TrapSpawner>> as Spawner3

rectangle "Spawn Pool" as SP3 #green
rectangle "Spawn Pool" as SP4 #green
rectangle "Spawn Pool" as SP5 #green

SpawnPoint1 *-- [Spawner1]
SpawnPoint2 *-- [Spawner2]
SpawnPoint3 *-- [Spawner3]

[Spawner1] *-down- SP3
[Spawner2] *-down- SP4
[Spawner3] *-down- SP5

[TrapTrigger1] o-- SpawnPoint1
[TrapTrigger1] o-- SpawnPoint2
[TrapTrigger1] o-- SpawnPoint3

node "Spawn Trigger" <<GameObject>> as SpawnTrigger2
[Trap Trigger] <<TrapTrigger>> as TrapTrigger2
SpawnTrigger2 *-- [TrapTrigger2]
rectangle "Spawn Pool" as SP2 #green

[TrapTrigger2] *-right- SP2 

node "Spawn Point" <<GameObject>> as SpawnPoint4 
node "Spawn Point" <<GameObject>> as SpawnPoint5
[Spawner] <<TrapSpawner>> as Spawner4
[Spawner] <<TrapSpawner>> as Spawner5
rectangle "Spawn Pool" as SP6 #green
rectangle "Spawn Pool" as SP7 #green
SpawnPoint4 *-- [Spawner4]
SpawnPoint5 *-- [Spawner5]
[Spawner4] *-down- SP6
[Spawner5] *-down- SP7
[TrapTrigger2] o-- SpawnPoint4
[TrapTrigger2] o-- SpawnPoint5
node "Global Spawn Pool" <<GameObject>> as GlobalSpawnPool 
[Spawn Pool] <<TrapSpawnPool>> as SpawnPool #green
GlobalSpawnPool *-- [SpawnPool]
GlobalSpawnPool -left-o [TrapTrigger1]
GlobalSpawnPool -right-o [TrapTrigger2]

@enduml
-->

<img src="https://www.plantuml.com/plantuml/svg/dPGnRy8m48Nt-nKktIfbY1F3WX0ROv7A5cKmu0OgnqR1YB_Vu3ZrcRHMqWtxVUTmp--ylXhUc5ijuSMSH2t8zS8FGl8zWD97GMOV5tvNPDUSgqeqCx9SRdWjtlTVuc1MAyAln09EJmaKkuPVUhvi-SVQBRi2j952MFsM9AJ0UWbD-o6kAldpLxPQovdzy3ObhrKZX088YzfpfFWqYKM2FhRwhCoVNLWrdLf0fnPaKDY5zySj4TYsCOgYewWCeZvcpiSeb8-1M0-1RzTTQzzMYIGQI1HBMI1b9RX6tVqUzLqzxjTxIV5s3huIRRD1KdIIuBkFopGkP_-Q6JfjP2W4nOEHeV6HeR-CZFEk_NaoYMLWQgJpoMF6HWKt7qMnQClpCS5Tz13k2ISmn-5aWSHAy1HAsnmyWRONeVNFAShZCdD6RwJUSqbm8qExEyWLQqcWy3msrAfTYOUT8KzdeCxjAGEzUtfr-8uFCHSt_HnUM0jrlDNo6m00">
