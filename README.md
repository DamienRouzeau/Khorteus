Khorteus est un jeu solo PvE à la première personne, développé dans le cadre d’un projet étudiant.
Le joueur doit défendre un générateur central de nombreuses vagues d'ennemis variés tout en explorant des mines pour récolter des cristaux et améliorer ses chances de survie.

Itch.io : https://damienrzu.itch.io/khorteus 

# Technologies utilisées
## Moteur : Unity

## Version : 2022.3.37f1 (LTS)

## Langage : C#

## Conventions de code
### Nommage des variables et fonctions
public **premiereVariable**

private **deuxiemeVariable**

local **_troisiemeVariable**

void **Fonction()**

### Nommage des scripts
**ScriptBehaviour** → Pour un script standard

**ScriptManager** → Pour un manager global

# Workflow Git
## Pushs
Chaque push suit la convention suivante :

**fix :** détail des modifications → Correction de bugs mineurs

**feat :** détail des ajouts → Ajout de fonctionnalités

Les pushs sont effectués après chaque modification ou ajout, si le projet reste stable.

## Merges
Les merges sont réalisés uniquement si la stabilité du jeu est garantie.

Les merges incluent une description claire des modifications.

Lorsqu’ils contiennent un ajout majeur ou une correction d’un bug bloquant


## Création d’une nouvelle version
Les modifications sont mergées sur la branche main.

Un build est généré et testé.

Si aucun bug bloquant n’est détecté :

Le build est publié sur Itch.io

Un journal de mise à jour est rédigé avec :

Les nouvelles fonctionnalités

Les correctifs appliqués

Le lien vers le nouveau build

## Inputs
WASD/ZQSD : Déplacements

E : Intéraction

LMB : tire/coup/placement de tourelle

RMB : Viser

Espace : Saut

Shift : Sprint

R : Recharge

