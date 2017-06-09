using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyTTCBot.Models;
using NetTelegram.Bot.Framework;
using NetTelegram.Bot.Framework.Abstractions;
using NetTelegramBotApi.Types;

namespace MyTTCBot.Commands
{
    public class LocationHanlder : UpdateHandlerBase
    {
        public const string OsmAndLocationRegex = @"geo:([+|-]?\d+(?:.\d+)?),([+|-]?\d+(?:.\d+)?)";

        private readonly MyTtcDbContext _dbContext;

        private readonly IMemoryCache _cache;

        public LocationHanlder(IMemoryCache cache, MyTtcDbContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
        }

        public override bool CanHandleUpdate(IBot bot, Update update)
        {
            return update.Message.Location != null ||
                   update.Message.Text?.ToLower().Contains("geo:") == true;
        }

        public override async Task<UpdateHandlingResult> HandleUpdateAsync(IBot bot, Update update)
        {
            var userChat = new UserChat(update.Message.From.Id, update.Message.Chat.Id);
            if (!_cache.TryGetValue(userChat, out UserContext context))
            {
                context = new UserContext
                {
                    Location = new UserLocation()
                };
            }

            if (update.Message.Location != null)
            {
                context.Location = (UserLocation)update.Message.Location;
            }
            else
            {
                var match = Regex.Match(update.Message.Text, OsmAndLocationRegex, RegexOptions.IgnoreCase);
                context.Location = new UserLocation
                {
                    Latitude = double.Parse(match.Groups[1].Value),
                    Longitude = double.Parse(match.Groups[2].Value),
                };
            }
            var slidingExpiryOption = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(1) };
            _cache.Set(userChat, context, slidingExpiryOption);
            await AddLocation(update);
            return await Task.FromResult(UpdateHandlingResult.Handled);
        }

        private async Task AddLocation(Update update)
        {
            try
            {
                var uc = await _dbContext.UserChatContexts.FirstOrDefaultAsync(
                    x => x.UserId == update.Message.From.Id && x.ChatId == update.Message.Chat.Id);

                if (uc == null)
                {
                    uc = (UserChatContext)update;

                    var location = new FrequentLocation
                    {
                        Latitude = update.Message.Location.Latitude,
                        Longitude = update.Message.Location.Longitude,
                        Name = "Some name",
                    };

                    uc.FrequentLocations.Add(location);
                    await _dbContext.AddAsync(uc);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
