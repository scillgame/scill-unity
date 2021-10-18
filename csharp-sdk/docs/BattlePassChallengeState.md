# SCILL.Model.BattlePassChallengeState
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**app_id** | **string** | The unique id of the app | [optional] 
**battle_pass_id** | **string** | The unique id of this battle pass. | [optional] 
**level_id** | **string** | Unique id of this BattlePassLevel object. | [optional] 
**user_id** | **string** | This is your user id. You can set this to whatever you like, either your real user id or an obfuscated user id. However you need to be consistent here. Events linked to this user id only track if challenges or battle passes are unlocked with the same user id. | [optional] 
**level_position_index** | **int?** | Typical usage pattern is to load battle pass levels with getBattlePassLevels operation and store them for rendering. Using this value you can quickly identify the index of the level that changed. | [optional] 
**challenge_id** | **string** | The unique id of this challenge. Every challenge is linked to a product. | [optional] 
**challenge_position_index** | **int?** | Same as level_position_index. Use this index to identify the challenge that changed within the levels challenges array. Typical usage pattern is to update the previously stored score and type. | [optional] 
**challenge_goal** | **int?** | Indicates how many “tasks” must be completed or done to complete this challenge. | [optional] 
**user_challenge_current_score** | **int?** | Indicates how many tasks the user already has completed. Use this in combination with challenge_goal to render a nice progress bar. | [optional] 
**type** | **string** | Indicates the status of the challenge. This can be one of the following unlock: Challenge does not track anything. in-progress: Challenge is active and tracking. overtime: User did not manage to finish the challenge in time. unclaimed: The challenge has been completed but the reward has not yet been claimed. finished: The challenge has been successfully be completed and the reward has been claimed | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

