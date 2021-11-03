# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.2.1] - 2021-11-03

### Changed
- Removed unnecessary Debug Logs

## [2.2.0] - 2021-10-25

Added the capability to access SCILL Api v2 endpoints for leaderboards. Leaderboard can be set to v2 on 
creation in the 4Players Admin Panel. Leaderboards created for the Api v2 can only be accessed using Api v2
requests. Leaderboards created for the Api v1 can only be accessed using Api v1 requests.

**Important:** the SDK will automatically try to 
access the v2 endpoints. If you're still using leaderboards created for the v1 endpoints, you will have to do the following:
- Create a file called: `SCILLConfig.json` in a `Resources` folder under your `Assets` directory.
- Set the file's content to: ``` {
  "ApiVersion": "v1"
  }```

The SDK will then use the v1 endpoints of the SCILL Api. 

### Added

- Can now call `ResetLeaderboardRankings` to reset all ranking data of a leaderboard, using the `SCILLBackend`. This request is not available on Client Side Code.
- Can now add a `SCILLConfig.json` file in a `Resources` folder under the `Assets` directory to adjust SCILL-SDK settings, i.e. the api version, the Api Domain and the domain prefixes for the Authentication, Events, Challenges, BattlePasses and Leaderboards Apis. The default configuration is set to:

``` json 
{
  "ApiVersion": "v2",
  "Domain" : "scill.4players.io",
  "DomainPrefixAuthentication" : "us",
  "DomainPrefixEvents" : "ep",
  "DomainPrefixChallenges" : "pcs",
  "DomainPrefixBattlePasses" : "es",
  "DomainPrefixLeaderboards" : "ls"
}
```

### Changed

- The Leaderboard Api requests ``GetLeaderboardAsync``, ``GetLeaderboardsAsync``, `GetLeaderboardRankingAsync` and `GetLeaderboardRankingsAsync` now provide additional, optional parameters. These parameters can only be used for requests sent using Api Version `v2`.


## [2.1.1] - 2021-09-10

### Fixed
- Fixed an issue with Personal Challenge Category names not being displayed correctly.

## [2.1.0] - 2021-09-08

We've added visual and audio feedback effects to the samples - they will play for certain triggers like "Personal Challenge Completed" or "Battle Pass Level Changed".

The `SCILLAudio` prefab can simply be dropped into your scene and will automatically connect to all relevant systems. Using a new `SCILLAudioSettings` Scriptable Object or adjusting the default you can switch out our sample clips with your own files. Samples of how to play particle effects or fire off UI animations can be seen in the sample scenes.

### Added
- Added Scripts for playing audio clips on certain triggers, e.g. Personal Challenge Completed or Battle Pass Level Changed
- Audio feedback clips can be changed using the new `SCILLAudioSettings` Scriptable Object
- Added sample particle systems and audio clips for visual and audio feedback
- Added UnityEvents to Challenge items

### Changed
- Improved update behavior of `SCILLChallengeItem` and `SCILLBattlePassChallengeItem`

### Fixed
- Fixed issue with Battle Pass Level not being updated correctly under some conditions

## [2.0.3] - 2021-07-19

In this release we've added the Unity specific C# SDK project to the repository for easy access to the source code. The DLL produced by this project will only work with Unity, it will not work when trying to use it in a standalone C# project. For standalone C# projects, please use [the standalone SCILL C# SDK.](https://github.com/scillgame/scill-csharp)

### Added
- The C# SDK project is now part of this repository

### Changed
- Leaderboard Ranking Item: relative resource path for loading avatar images can now be adjusted in the editor

### Fixed
- Fixed issues with personal challenges not displaying progress bar correctly in some cases
- Fixed issues with the battlepass UI not displaying in cases where a reward with a display prefab was loaded, but no photobox was selected in the RewardPreview


## [2.0.2] - 2021-07-19

### Fixed
- Fixed an issue with leaderboards not updating on realtime update notifications
- Fixed an issue with the header display of a users ranking not being updated correctly when receiving an update notification
- Fixed an issue with leaderboards not loading pages correctly

## [2.0.1] - 2021-07-19

Removed Newtonsoft from the supplied plugins, making the plugin less prone to assembly clashes. If the SDK is added via a .unitypackage, or added directly into the Asset folder, you will need to  get the "com.unity.nuget.newtonsoft-json" package from the Unity Package Manger to resolve the dependencies. The Newtonsoft package is automatically supplied with the newest Unity versions.

### Changed

- Removed Newtonsoft DLL, instead opting for adding "com.unity.nuget.newtonsoft-json" version 2.0.0 as a package dependency. This will avoid assembly clashes, if  Newtonsoft is already used in a project.
- Changed SCILL.dll Newtonsoft dependency from 13.0.1 to 12.0.2, for better backwards support

### Fixed
- Issue with GetAccessTokenAsync not being called in ScillManager startup if the callback version of the function is overriden

### Added
- ScillBackend documentation

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
