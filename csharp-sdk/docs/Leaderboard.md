# SCILL.Model.Leaderboard
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**leaderboard_id** | **string** | The id of the leaderboard | [optional] 
**name** | **string** | The name of the leaderboard | [optional] 
**grouped_by_users** | [**List&lt;LeaderboardRanking&gt;**](LeaderboardRanking.md) | An array of LeaderboardRanking Items for individual users | [optional] 
**grouped_by_teams** | [**List&lt;LeaderboardRanking&gt;**](LeaderboardRanking.md) | An array of LeaderboardRanking Items for teams. Provide a team_id in the event payload to also create leaderboards for teams | [optional] 
**num_teams** | [**decimal?**](BigDecimal.md) | The total number of team rankings available in the leaderboard | [optional] 
**num_users** | [**decimal?**](BigDecimal.md) | The total number of user rankings available in the leaderboard | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

