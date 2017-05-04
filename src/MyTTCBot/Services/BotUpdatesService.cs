﻿using System;
using System.Threading.Tasks;
using MyTTCBot.Extensions;
using MyTTCBot.Managers;
using NetTelegramBotApi;
using NetTelegramBotApi.Requests;

namespace MyTTCBot.Services
{
    public class BotUpdatesService : IBotUpdatesService
    {
        public bool ShouldStop { get; set; }

        public bool IsRunning { get; private set; }

        private readonly TelegramBot _bot;

        private readonly IBotManager _botManager;

        private long? _offset;

        public BotUpdatesService(TelegramBot bot, IBotManager botManager)
        {
            _bot = bot;
            _botManager = botManager;
        }

        public void Start()
        {
            if (IsRunning)
                throw new InvalidOperationException("Service is already started");
            else
                IsRunning = true;

            Task.Factory.StartNew(GetUpdates)
                .ContinueWith(task => throw task.Exception.InnerException, TaskContinuationOptions.OnlyOnFaulted)
                .ConfigureAwait(false);
        }

        private async Task GetUpdates()
        {
            do
            {
                var updates = await _bot.MakeRequestAsync(new GetUpdates { Offset = _offset });
                foreach (var update in updates)
                {
                    if (!update.IsValid())
                    {
                        continue;
                    }

                    _offset = update.UpdateId + 1;
                    await _botManager.ProcessMessage(update.Message);
                }
                await Task.Delay(3000);
            } while (!ShouldStop);
        }
    }
}