# SCILL.Model.Challenge
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**challenge_id** | **string** | The unique id of this challenge. Every challenge is linked to a product. | [optional] 
**challenge_name** | **string** | The name of the challenge in the language set by the language parameter. | [optional] 
**challenge_description** | **string** | An optional multi-language description that can be set in the Admin Panel. Used to describe exactly what the user has to do. | [optional] 
**challenge_duration_time** | [**decimal?**](BigDecimal.md) | The duration of the challenge in seconds. Challenges auto lock after time-out and need to be unlocked again. | [optional] 
**live_date** | **string** | The date this challenge should start. Use that field to create challenges that start in the future. | [optional] 
**challenge_goal** | **int?** | Indicates how many “tasks” must be completed or done to complete this challenge. | [optional] 
**user_challenge_current_score** | **int?** | Indicates how many tasks the user already has completed. Use this in combination with challenge_goal to render a nice progress bar. | [optional] 
**challenge_icon** | **string** | In the admin panel you can set a string representing an image. This can be a URL, but it can also be an image or texture that you have in your games asset database. | [optional] 
**challenge_icon_hd** | **string** | This is the HD variant of the challenge icon image. If you have a game, that runs on multiple platforms that could come in handy. Otherwise just leave blank. | [optional] 
**challenge_price** | **int?** | If you purchase the challenge, you can set a price. | [optional] 
**challenge_reward** | **string** | Set a reward for this challenge. This is a string value that you can map to anything in your code. Use in combination with challenge_reward_type. | [optional] 
**challenge_reward_type** | **string** | The reward type can be set to various different settings. Use it to implement different reward types on your side and use challenge_reward to set the value or amount of this reward. | [optional] 
**challenge_goal_condition** | **int?** | With this you can set the way how the SCILL system approaches the challenges state. 0 means, that the counter of the challenge must be brought above the goal. If this is 1, then the counter must be kept below the goal. This is often useful for challenges that include times, like: Manage the level in under 50 seconds. | [optional] 
**challenge_xp** | **int?** | If you have experience, player rankings whatever, you can use this field to set the gain in that when this challenge is rewarded. | [optional] 
**repeatable** | **bool?** | If this challenge can be only activated once per user this will be false. Otherwise this challenge will always be added to list of available challenges (see personal or alliance challenges). | [optional] 
**type** | **string** | Indicates the status of the challenge. This can be one of the following unlock: Challenge does not track anything. in-progress: Challenge is active and tracking. overtime: User did not manage to finish the challenge in time. unclaimed: The challenge has been completed but the reward has not yet been claimed. finished: The challenge has been successfully be completed and the reward has been claimed | [optional] 
**challenge_auto_activated** | **bool?** | Indicates if the challenges lifecycle is handled automatically by the SCILL backend. Use this flag to decide when to show action buttons for unlocking, activating, claiming or canceling challenges. Hide the buttons if this flag is true, and let the user manage challenges manually if this flag is false. | [optional] 
**is_claimed** | **bool?** | If the challenge reward has been claimed this is true otherwise its false. | [optional] 
**user_challenge_unlocked_at** | **string** | This is the timestamp the challenge has been unlocked. | [optional] 
**user_challenge_activated_at** | **string** | This is the timestamp the challenge has been activated. | [optional] 
**user_challenge_is_claimed** | **bool?** | Indicates if this challenge has been claimed. | [optional] 
**user_challenge_status** | **int?** | Gives indication in what state the challenge is. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

