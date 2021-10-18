# SCILL.Model.BattlePassLevel
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**level_id** | **string** | Unique id of this BattlePassLevel object. | [optional] 
**app_id** | **string** | The app id | [optional] 
**battle_pass_id** | **string** | The id of the battle pass this level belongs to | [optional] 
**reward_amount** | **string** | In the Admin Panel you can set different types of rewards. You can also set an identifier of an in-game-item or anything you like. Use this to include the reward into your own business logic. | [optional] 
**reward_type_name** | **string** | There are different types of rewards available. Possible values are Coins, Voucher, Money and Experience. This is deprecated in favor of level_reward_type which uses a slug instead of a human readable expression | [optional] 
**level_reward_type** | **string** | The reward type in a machine readable slug. Available values are nothing, coin, experience, item | [optional] 
**level_completed** | **bool?** | Indicates if this level is completed, i.e. all challenges have been completed. | [optional] 
**level_priority** | [**decimal?**](BigDecimal.md) | Indicates the position of the level. | [optional] 
**reward_claimed** | **bool?** | Indicates if this level has already be claimed. | [optional] 
**activated_at** | **string** | The date when this level has been activated or null if it&#x27;s not activated. | [optional] 
**challenges** | [**List&lt;BattlePassLevelChallenge&gt;**](BattlePassLevelChallenge.md) | An array of BattlePassLevelChallenge objects. Please note, not all values are available from the challenge object, as battle passes handle the lifecycle of challenges. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

