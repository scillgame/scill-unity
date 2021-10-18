# SCILL.Model.BattlePass
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**battle_pass_id** | **string** | The unique id of this battle pass. | [optional] 
**app_id** | **string** | The unique id of the app | [optional] 
**battle_pass_name** | **string** | The name of the battle bass. You can set that in the Admin Panel. The language is set with the query parameter language. See documentation for more info on that. | [optional] 
**battle_pass_description** | **string** | The description of the battle bass. You can set that in the Admin Panel and it can also be HTML. The language is set with the query parameter language. See documentation for more info on that. | [optional] 
**battle_pass_short_description** | **string** | A short description of the battle bass. You can set that in the Admin Panel and it can also be HTML. The language is set with the query parameter language. See documentation for more info on that. | [optional] 
**battle_pass_disclaimer** | **string** | Use this to provide some terms and conditions following along this battle passes purchase. | [optional] 
**battle_pass_priority** | **int?** | The priority of the battle pass. I.e. if multiple are available you can use this field to sort them. | [optional] 
**package_sku_ios** | **string** | If you want to sell Battle Passes you can use this field to trigger in-app purchase products within your mobile app. You can set this value in the Admin Panel. This one is for iOS. | [optional] 
**package_sku_android** | **string** | If you want to sell Battle Passes you can use this field to trigger in-app purchase products within your mobile app. You can set this value in the Admin Panel. Use this to set the package string for Android. | [optional] 
**image_xs** | **string** | The xs sized image name or url. You can determine the best size distribution yourself and depends on your application or UI | [optional] 
**image_s** | **string** | The s sized image name or url. You can determine the best size distribution yourself and depends on your application or UI | [optional] 
**image_m** | **string** | The m sized image name or url. You can determine the best size distribution yourself and depends on your application or UI | [optional] 
**image_l** | **string** | The l sized image name or url. You can determine the best size distribution yourself and depends on your application or UI | [optional] 
**image_xl** | **string** | The xl sized image name or url. You can determine the best size distribution yourself and depends on your application or UI | [optional] 
**start_date** | **string** | The date (in iso format) when the Battle Pass starts. Tracking begins once this date is passed. | [optional] 
**end_date** | **string** | The date (in iso format) when the Battle Pass ends. Tracking stops once the end is reached and users will not be able to progress further than what they have achieved up to that point. | [optional] 
**read_more_link** | **string** | If the Battle Pass costs “money” you may want to route the user to a web site/page, where they can learn more about this battle pass. You can also use this field to route the user inside your application by providing a path or whatever works for you. | [optional] 
**is_unlocked_incrementally** | **bool?** | Indicates if one level after the other must be activated or if users can activate whichever level they want. Typically battle passes are unlocked level by level, but if battle passes are used for other applications (like user referal programs) it can be useful to set this to false. | [optional] 
**is_active** | **bool?** | Indicated if this battle pass is active. | [optional] 
**unlocked_at** | **string** | The date in iso format when the user unlocked this Battle Pass. | [optional] 
**can_purchase_with_money** | **bool?** | Indicates that this Battle Pass can be purchased via in-app purchase. This can be set in the Admin Panel. | [optional] 
**can_purchase_with_coins** | **bool?** | Indicates that this Battle Pass can be purchased with SCILL Coins. This can be set in the Admin Panel. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

