# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.1.0] - 2021-01-12

This version adds a full working nice looking sample for personal challenges and battle passes and many UI assets that you can use for your own game

### Added
- Start and end date Text properties to SCILLBattlePass to render the start and end dates of the battle pass
- Added support for challenge icons/images in SCILLBattlePassChallengeItem
- Added class SCILLBattlePassNextLevel which renders the next level in a local Text object
- Added some text formats that can be adjusted with a string, giving more flexibility without requiring overriding classes
- Made hiding completed challenges in battle pass an option (can be also shown which was not possible before)
- Added class SCILLPagination to handle all pagination for battle pass levels which also allows to jump to specific pages
- Added support for selected and deselected battle pass levels to highlight current reward preview.

### Changed
- Added an overridable method in SCILLBattlePassCurrentLevel 
- Added tooltips and some structure to inspector
- Refactored pagination code from SCILLBattlePassLevels to SCILLPagination which is more flexible
- Refactored selected battle pass level from SCILLBattlePassLevels to SCILLBattlePassManager
- Reworked SCILLRewardPreview to use the new global selected battle pass level
- Claim button in reward preview is automatically linked to `OnClaimButtonPassRewardButtonClicked` if not set in Unity

### Fixed
- Fixed a bug in SCILLBattlePass that happened if no `unlockGroup` was provided
- Fixed a bug with wrongly placed UI items in some cases

## [1.0.0] - 2020-12-18

Initial release of the Unity SDK for SCILL.

Some important info about this release:

* SCILL C# SDK Version 1.2.1 is embedded in the `SDK` folder and will be kept up-to-date in every release
* Extensive documentation can be found here: [https://developers.scillgame.com/sdks/unity.html](https://developers.scillgame.com/sdks/unity.html)
