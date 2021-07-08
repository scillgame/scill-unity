# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2021-07-08

This release adds support for WebGL builds to the SCILL Unity SDK.

### Changed
* Reworked API to use UnityWebRequest, adding support for WebGL
* Changed API from using async/await to promise/callback based requests
* Removed synchronous API calls
* Moved the Start and Stop Update Notification methods (i.e. `StartChallengeUpdateNotifications`, `StartBattlePassUpdateNotifications` and `StartLeaderboardUpdateNotifications`) from `SCILLClient` to `SCILLManager`
* SCILL API has been updated to the latest version
* Removed `SCILLThreadSafety` class


### Added
* Realtime Leaderboard Updates
* SCILL Notifications, allowing simple notifications to be displayed on the screen
* `SCILLPersonalChallengesManager`, giving centralized access to personal challenge categories, challenges and update notifications
* Added namespaces

### Fixed
* Issues with IL2CPP managed code stripping

### How to upgrade

 This release adds breaking changes to projects using a 1.x.y version of the SDK. In order to upgrade your project to this version, you will need to do the following:
* Switch direct API requests from using to async to using callbacks. Example: 

Old v1.x.y:
``` csharp 
try{
   var categories = await SCILLManager.Instance.SCILLClient.GetAllPersonalChallengesAsync();
   UpdateCategories(categories);
}
catch(APIException e)
{
   Handle(e);
}
```
New v2.0.0:
``` csharp 
SCILLManager.Instance.SCILLClient.GetAllPersonalChallengesAsync(
   categories =>
   {
      UpdateCategories(categories);
   },
   e =>
   {
      if(e is APIException apiException){
         Handle(apiException);
      }
   });
```

* Remove all synchronous API calls and replace them with async calls
* Ensure that all "Start Update Notifications" calls are directly called in the `SCILLManager.Instance` instead of `SCILLManager.Instance.SCILLClient`, e.g. use `SCILLManager.Instance.StartChallengeUpdateNotifications(OnChallengeWebhookMessage);` instead of `SCILLManager.Instance.SCILLClient.StartChallengeUpdateNotifications(OnChallengeWebhookMessage);`
* Ensure that the SCILL namespace is used


## [1.4.0] - 2021-03-26

This release primary focus has been on adding leaderboards to SCILL Unity SDK. We added ready-to-use leaderboard prefabs. More Info on the leaderboards can be found in our [developer documentation](https://developers.scillgame.com/sdks/unity/classes/scillleaderboard.html)

### Added
- Support for different languages in SCILL backend has been added
- Added `SetUserInfo` and `GetUserInfo` to `SCILLManager` to set usernames and avatars
- Added Leaderboards (Classes, Prefabs, etc)

### Changed
- SCILL C# SDK has been updated to the latest version (1.3.0)
- Changed some UI parameters like spacing for challenge lists
- Errors when sending events is now caught and printed in the console
- Reworked `SCILLManager` inspector property order and tooltips
- Use `GetAllPersonalChallenges` as `GetPersonalChallenges` has been deprecated
- Changed execution order of `SCILLManager` to `-1` so that it runs before everything else

### Fixed
- Fixed a couple of minor issues in the Battle Pass system

## [1.3.0] - 2021-02-11

### Changed
- Added `*.meta` tags to support Package Manager

## [1.2.0] - 2021-02-10

### Changed
- Updated SCILL C# SDK to 1.3.0 (added a couple of more functions and changed `challenge_duration` type from integer to decimal)

## [1.1.1] - 2021-01-13

### Added
- Added script `SCILLCameraOffset` which can be attached to a camera to offset the perspective center to place items without perspective distortion to the left or right side.

### Changed
- Changed Battle Pass Style 2 scene to offset reward item to the right without perspective distortion.

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
