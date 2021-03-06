using System;
using System.Threading;
using System.Threading.Tasks;
using BusV.Telegram.Models;
using BusV.Telegram.Models.Cache;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Caching.Distributed
{
    public static class CacheExtensions
    {
        [Obsolete]
        public static Task RemoveAsync(
            this IDistributedCache cache,
            UserChat userchat,
            CancellationToken cancellationToken = default
        ) =>
            cache.RemoveAsync(GetKey(userchat, ""), cancellationToken);

        public static Task<UserProfileContext> GetProfileAsync(
            this IDistributedCache cache,
            UserChat userchat,
            CancellationToken cancellationToken = default
        ) =>
            cache.GetStringAsync(GetKey(userchat, "profile"), cancellationToken)
                .ContinueWith(t =>
                        t.Result == null
                            ? null
                            : JsonConvert.DeserializeObject<UserProfileContext>(t.Result),
                    TaskContinuationOptions.OnlyOnRanToCompletion
                );

        public static Task SetProfileAsync(
            this IDistributedCache cache,
            UserChat userchat,
            UserProfileContext context,
            CancellationToken cancellationToken = default
        ) =>
            cache.SetStringAsync(
                GetKey(userchat, "profile"),
                JsonConvert.SerializeObject(context),
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(1)
                },
                cancellationToken
            );

        public static Task RemoveProfileAsync(
            this IDistributedCache cache,
            UserChat userchat,
            CancellationToken cancellationToken = default
        ) =>
            cache.RemoveAsync(GetKey(userchat, "profile"), cancellationToken);

        public static Task<BusPredictionsContext> GetBusPredictionAsync(
            this IDistributedCache cache,
            UserChat userchat,
            CancellationToken cancellationToken = default
        ) =>
            cache.GetStringAsync(GetKey(userchat, "bus"), cancellationToken)
                .ContinueWith(t =>
                        t.Result == null
                            ? null
                            : JsonConvert.DeserializeObject<BusPredictionsContext>(t.Result),
                    TaskContinuationOptions.OnlyOnRanToCompletion
                );

        public static Task SetBusPredictionAsync(
            this IDistributedCache cache,
            UserChat userchat,
            BusPredictionsContext context,
            CancellationToken cancellationToken = default
        ) =>
            cache.SetStringAsync(
                GetKey(userchat, "bus"),
                JsonConvert.SerializeObject(context),
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(20)
                },
                cancellationToken
            );

        public static Task RemoveBusPredictionAsync(
            this IDistributedCache cache,
            UserChat userchat,
            CancellationToken cancellationToken = default
        ) =>
            cache.RemoveAsync(GetKey(userchat, "bus"), cancellationToken);

        public static Task<UserLocationContext> GetLocationAsync(
            this IDistributedCache cache,
            UserChat userchat,
            CancellationToken cancellationToken = default
        ) =>
            cache.GetStringAsync(GetKey(userchat, "location"), cancellationToken)
                .ContinueWith(t =>
                        t.Result == null
                            ? null
                            : JsonConvert.DeserializeObject<UserLocationContext>(t.Result),
                    TaskContinuationOptions.OnlyOnRanToCompletion
                );

        public static Task SetLocationAsync(
            this IDistributedCache cache,
            UserChat userchat,
            UserLocationContext context,
            CancellationToken cancellationToken = default
        ) =>
            cache.SetStringAsync(
                GetKey(userchat, "location"),
                JsonConvert.SerializeObject(context),
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(15)
                },
                cancellationToken
            );

        public static Task RemoveLocationAsync(
            this IDistributedCache cache,
            UserChat userchat,
            CancellationToken cancellationToken = default
        ) =>
            cache.RemoveAsync(GetKey(userchat, "location"), cancellationToken);

        private static string GetKey(UserChat userchat, string kind) =>
            $@"{{""u"":{userchat.UserId},""c"":{userchat.ChatId},""k"":""{kind}""}}";
    }
}
