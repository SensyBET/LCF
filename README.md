# Application de Configuration de Port COM et Communication avec Multimètre Keithley 2700

Cette application est conçue pour configurer un port COM pour la communication série, en particulier avec un multimètre Keithley 2700 équipé d'une carte multiplexeur 7700. Elle implémente des commandes et trames de base pour établir cette communication. Ce document explique comment utiliser l'application, ses fonctionnalités et sa configuration.

## Table des Matières
1. [Fonctionnalités](#fonctionnalités)
2. [Installation](#installation)
3. [Utilisation](#utilisation)
   - [Configuration des Ports](#configuration-des-ports)
   - [Communication avec Keithley 2700](#communication-avec-keithley-2700)
4. [Architecture du Code](#architecture-du-code)
5. [Contributions](#contributions)
6. [Licence](#licence)

## Fonctionnalités

- Scanne et affiche les ports COM disponibles.
- Configure les paramètres des ports COM (baudrate, parité, taille des données, bits d'arrêt, handshake).
- Envoie des commandes spécifiques au multimètre Keithley 2700 et gère les réponses.
- Historique des commandes envoyées.
- Interface utilisateur intuitive pour sélectionner et configurer les ports.

## Installation

1. Clonez le dépôt sur votre machine locale.
2. Ouvrez le projet dans votre environnement de développement préféré (par exemple, Visual Studio).
3. Restaurez les packages NuGet nécessaires (System.IO.Ports, ...).
4. Compilez le projet.

## Utilisation

### Configuration des Ports

1. Lancez l'application.
2. Cliquez sur le bouton "Scan" pour détecter les ports COM disponibles.
3. Sélectionnez un port dans la liste déroulante.
4. Configurez les paramètres du port selon vos besoins:
   - **Baudrate**: Sélectionnez la vitesse de communication (par exemple, 9600, 19200).
   - **Parité**: Choisissez le type de parité (None, Odd, Even, Mark, Space).
   - **Taille des données**: Sélectionnez la taille des données (5, 6, 7, 8 bits).
   - **Bits d'arrêt**: Choisissez le nombre de bits d'arrêt (One, OnePointFive, Two).
   - **Handshake**: Sélectionnez le type de handshake (None, RequestToSend, RequestToSendXOnXOff, XOnXOff).

### Communication avec Keithley 2700

0. (Après avoir configuré le port)
1. Utilisez le champ de texte pour entrer des commandes spécifiques au Keithley 2700.
2. Cliquez sur "Envoyer" pour transmettre la commande au multimètre.
3. Les réponses du multimètre s'afficheront dans la console de l'application.

**Exemples de commandes pour Keithley 2700**:
- `SYST:ERR?` : Renvoie la dernière erreur présente dans le buffer et l'enlève de celui-ci.
- `TRAC:CLE` : Efface le buffer des mesures.
- `SYST:PRES` : Réinitialise le système.
- `ROUT:OPEN:ALL` : Ouvre tous les relais du multiplexeur.

## Architecture du Code

### Fichiers Principaux

- **MainViewModel.cs**: Gère la logique principale de l'application, y compris les commandes de scan et d'envoi.
- **IO_Ports.cs**: Contient les méthodes pour scanner et afficher les ports COM disponibles.
- **Port.cs**: Définit les propriétés et méthodes pour configurer et utiliser un port série.

### Classes Principales

- `MainViewModel` : Implémente l'interface utilisateur et la gestion des ports.
- `IO_Ports` : Fournit des fonctions pour détecter les ports disponibles.
- `Port` : Représente un port série avec ses paramètres et méthodes pour envoyer/recevoir des données.

## Contributions

Les contributions sont les bienvenues ! Pour contribuer, veuillez suivre ces étapes:

1. Fork le dépôt.
2. Créez une branche pour votre fonctionnalité (`git checkout -b feature/ma-fonctionnalite`).
3. Commitez vos modifications (`git commit -m 'Ajouter une nouvelle fonctionnalité'`).
4. Poussez la branche (`git push origin feature/ma-fonctionnalite`).
5. Ouvrez une Pull Request.

## Licence

Ce projet est sous licence MIT. Voir le fichier [LICENSE](LICENSE.md) pour plus de détails.