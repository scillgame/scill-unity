using System;
using RSG;

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
    }
}