using System;
using System.Collections.Generic;
using RSG;
using SCILL.Model;

namespace SCILL.Client
{
    public static class PromiseExtensions
    {
        public static IPromise<T> ExtractResponseData<T>(this IPromise<ApiResponse<T>> promiseWithHttpInfo)
        {
            var promise = new Promise<T>((resolve, reject) =>
            {
                promiseWithHttpInfo.Then(apiResponse => { resolve(apiResponse.Data); }).Catch(reject);
            });
            return promise;
        }

        public static IPromise<ApiResponse<Leaderboard>> ToLeaderboardPromise(
            this IPromise<ApiResponse<LeaderboardResults>> leaderboardResultsPromise)
        {
            // We're converting the Api Response with a LeaderboardResults struct into a response with a Leaderboard struct
            // This is done to ensure backwards compatibility.
            // TODO: Remove this, once we drop the new Unity SDK, because this is just unnecessarily weird.
            var promise = new Promise<ApiResponse<Leaderboard>>((resolve, reject) =>
                {
                    leaderboardResultsPromise.Then(
                        apiResponseWithLeaderboardResults =>
                        {
                            ApiResponse<Leaderboard> convertedResponse = CopyResponse<LeaderboardResults, Leaderboard>(apiResponseWithLeaderboardResults);
                            convertedResponse.Data = apiResponseWithLeaderboardResults.Data.ToLeaderboard();
                            resolve(convertedResponse);
                        }
                    ).Catch(reject);
                }
            );
            return promise;
        }
        
        public static IPromise<ApiResponse<List<Leaderboard>>> ToLeaderboardsPromise(
            this IPromise<ApiResponse<List<LeaderboardResults>>> leaderboardResultsPromise)
        {
            // We're converting the Api Response with a LeaderboardResults struct into a response with a Leaderboard struct
            // This is done to ensure backwards compatibility.
            // TODO: Remove this, once we drop the new Unity SDK, because this is just unnecessarily weird.
            var promise = new Promise<ApiResponse<List<Leaderboard>>>((resolve, reject) =>
                {
                    leaderboardResultsPromise.Then(
                        apiResponseWithLeaderboardResults =>
                        {
                            ApiResponse<List<Leaderboard>> convertedResponse = CopyResponse<List<LeaderboardResults>, List<Leaderboard>>(apiResponseWithLeaderboardResults);
                            convertedResponse.Data = apiResponseWithLeaderboardResults.Data.ToLeaderboards();
                            resolve(convertedResponse);
                        }
                    ).Catch(reject);
                }
            );
            return promise;
        }

        private static ApiResponse<T1> CopyResponse<T0, T1>(ApiResponse<T0> apiResponseWithLeaderboardResults)
        {
            return new ApiResponse<T1>(
                apiResponseWithLeaderboardResults.StatusCode,
                apiResponseWithLeaderboardResults.Headers,
                apiResponseWithLeaderboardResults.RawData,
                apiResponseWithLeaderboardResults.Content,
                apiResponseWithLeaderboardResults.Error
            );
        }
    }
}