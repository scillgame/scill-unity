# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.4.0] - 2021-03-26
### Added
- Added language parameter to `SCILLClient`
- Added Leaderboards API

### Changed
- Changed to official MQTTnet library instead of a fork of it.
- Switched to Websocket MQTT connection (as a preparation for WebGL support coming soon)

## [1.3.0] - 2021-02-10
### Added
- Added `GetUnresolvedPersonalChallenges` function to the `ChallengesApi` which is a duplicate of getPersonalChallenges right now.
- Added `GetAllPersonalChallenges` function to the `ChallengesApi` which also returns finished challenges.

## [1.2.1] - 2020-12-08
### Added
- Added `challenge_goal_condition` to the `BattlePassLevel` object.
- Added `level_reward_type` to the `BattlePassLevel` object.

### Deprecated
- `challenge_reward_type` should not be used anymore as it has been replaced by `level_reward_type` in `BattlePassLevel`

## [1.2.0] - 2020-12-07
### Added
- Added `challenge_goal_condition` to the `Challenge` object.
- Added `challenge_reward_type` to the `Challenge` object.
- Added `image_xs`, `image_s`, `image_m`, `image_l` and `image_xl` to `BattlePass` object

### Changed
- Changed type of `challenge_reward` in the `Challenge` object from integer to string

## [1.1.0] - 2020-12-04
### Added
- Added Realtime Notification System for Challenges and Battle Passes

### Changed
- Reworked Battle Pass System
