# SCILL.Model.BattlePassLevelReward
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**app_id** | **string** | The unique id of the app | [optional] 
**battle_pass_id** | **string** | The unique id of this battle pass. | [optional] 
**level_id** | **string** | Unique id of this BattlePassLevel object. | [optional] 
**user_id** | **string** | This is your user id. You can set this to whatever you like, either your real user id or an obfuscated user id. However you need to be consistent here. Events linked to this user id only track if challenges or battle passes are unlocked with the same user id. | [optional] 
**level_position_index** | **int?** | Typical usage pattern is to load battle pass levels with getBattlePassLevels operation and store them for rendering. Using this value you can quickly identify the index of the level that changed. | [optional] 
**reward_amount** | **string** | In the Admin Panel you can set different types of rewards. You can also set an identifier of an in-game-item or anything you like. Use this to include the reward into your own business logic. | [optional] 
**reward_type_name** | **string** | There are different types of rewards available. Possible values are Coins, Voucher, Money and Experience. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

