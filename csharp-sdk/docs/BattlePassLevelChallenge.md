# SCILL.Model.BattlePassLevelChallenge
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**challenge_id** | **string** | The unique id of this challenge. Every challenge is linked to a product. | [optional] 
**challenge_name** | **string** | The name of the challenge in the language set by the language parameter. | [optional] 
**challenge_goal** | **int?** | Indicates how many “tasks” must be completed or done to complete this challenge. | [optional] 
**challenge_goal_condition** | **int?** | With this you can set the way how the SCILL system approaches the challenges state. 0 means, that the counter of the challenge must be brought above the goal. If this is 1, then the counter must be kept below the goal. This is often useful for challenges that include times, like: Manage the level in under 50 seconds. | [optional] 
**user_challenge_current_score** | **int?** | Indicates how many tasks the user already has completed. Use this in combination with challenge_goal to render a nice progress bar. | [optional] 
**challenge_xp** | **int?** | If you have experience, player rankings whatever, you can use this field to set the gain in that when this challenge is rewarded. | [optional] 
**challenge_icon** | **string** | In the admin panel you can set a string representing an image. This can be a URL, but it can also be an image or texture that you have in your games asset database. | [optional] 
**challenge_icon_hd** | **string** | This is the HD variant of the challenge icon image. If you have a game, that runs on multiple platforms that could come in handy. Otherwise just leave blank. | [optional] 
**type** | **string** | Indicates the status of the challenge. This can be one of the following unlock: Challenge does not track anything. in-progress: Challenge is active and tracking. overtime: User did not manage to finish the challenge in time. unclaimed: The challenge has been completed but the reward has not yet been claimed. finished: The challenge has been successfully be completed and the reward has been claimed | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

